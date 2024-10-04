namespace TelerikBlazorEF.Data
{
	public class Employee
	{
		public int Id { get; set; }

		public int? ManagerId { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public DateOnly BirthDate { get; set; }

		public Gender Gender { get; set; }

		public decimal Salary { get; set; }

		public bool HasDriversLicense { get; set; }

		public Employee(string firstName, string lastName, Gender gender, DateOnly birthDate, int? managerId, decimal salary)
		{
			FirstName = firstName;
			LastName = lastName;
			Gender = gender;
			BirthDate = birthDate;
			ManagerId = managerId;
			Salary = salary;
		}
	}
}
