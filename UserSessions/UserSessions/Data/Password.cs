using System.ComponentModel.DataAnnotations;

namespace UserSessions.Data
{
	internal class Password
	{
		[Required]
        [Display(Name = "Current Password")]
        internal string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "New Password")]
        internal string NewPassword { get; set; } = string.Empty;

        [Required]
        [Match(nameof(NewPassword))]
        [Display(Name = "Confirm New Password")]
        internal string ConfirmedPassword { get; set; } = string.Empty;
    }
}
