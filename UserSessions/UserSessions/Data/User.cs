using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace UserSessions.Data
{
    public abstract class UserBase
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class UserSignable : UserBase
    {
        [Display(Name = "Remember Me")]
        public bool PersistSession { get; set; } = true;
    }

    public class UserEditable : UserBase
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
    }

    public class UserNew : UserEditable
    {
        [Required]
        [Match(nameof(Password))]
        [Display(Name = "Confirm Password")]
        public string ConfirmedPassword { get; set; } = string.Empty;
    }

    public class User : UserEditable
	{
        public bool Active { get; set; }

        public Role Role { get; set; } = Role.Default;

        public string Salt { get; set; } = Guid.NewGuid().ToString();

        public string ActivationToken { get; set; } = string.Empty;

        public DateTime? ActivationTokenDeadline { get; set; }
	}

    #region Custom Validator

    public class Match : ValidationAttribute
    {
        private string DependentPropertyName { get; set; }
        private string DependentPropertyDisplayName { get; set; } = string.Empty;
        private object? DependentPropertyValue { get; set; }

        public override bool RequiresValidationContext
        {
            get { return true; }
        }

        public Match(string dependentPropertyName)
        : base("The {0} value must match the {1} value.")
        {
            DependentPropertyName = dependentPropertyName;
        }

        public override string FormatErrorMessage(string validatedPropertyName)
        {
            return string.Format(
                System.Globalization.CultureInfo.CurrentCulture,
                base.ErrorMessageString,
                validatedPropertyName,
                DependentPropertyDisplayName);
        }

        protected override ValidationResult IsValid(object? validatedValue, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException("validationContext");
            }

            PropertyInfo? dependentProperty = validationContext.ObjectType.GetProperty(DependentPropertyName);
            DependentPropertyValue = dependentProperty?.GetValue(validationContext.ObjectInstance);
            DependentPropertyDisplayName = dependentProperty?.GetCustomAttribute<DisplayAttribute>()?.Name ?? DependentPropertyName;

            if (validatedValue != null && !validatedValue.Equals(DependentPropertyValue))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new List<string> { validationContext.DisplayName });
            }

            return ValidationResult.Success!;
        }
    }

    #endregion Custom Validator
}
