using Microsoft.EntityFrameworkCore;
using TelerikBlazorEF.Data;

namespace TelerikBlazorEF.Services
{
    public class EmployeeService
    {
        private readonly IDbContextFactory<DbContextEF> _contextFactory;

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            return dbContext.Employees.ToList();
        }

        public async Task<int> CreateEmployeeAsync(Employee newEmployee)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            dbContext.Employees.Add(newEmployee);
            await dbContext.SaveChangesAsync();

            return newEmployee.Id;
        }

        public async Task UpdateEmployeeAsync(Employee updatedEmployee)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            Employee? originalEmployee = await dbContext.FindAsync<Employee>(updatedEmployee.Id);

            if (originalEmployee != null)
            {
                dbContext.Entry(originalEmployee).State = EntityState.Detached;
                dbContext.Update(updatedEmployee);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteEmployeeAsync(Employee employee)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            Employee? employeeToDelete = dbContext.Employees.FirstOrDefault(x => x.Id == employee.Id);

            if (employeeToDelete != null)
            {
                dbContext.Employees.Remove(employeeToDelete);
                await dbContext.SaveChangesAsync();
            }
        }

        public EmployeeService(IDbContextFactory<DbContextEF> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task GenerateData(int employeeCount = 50)
        {
            var wordGenerator = new NameGenerator();

            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            if (!dbContext.Employees.Any())
            {
                for (int i = 1; i <= employeeCount; i++)
                {
                    dbContext.Employees.Add(new Employee()
                    {
                        Id = i,
                        FirstName = wordGenerator.Word(5),
                        LastName = wordGenerator.Word(5),
                        BirthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-18 - Random.Shared.Next(1, 30)).AddDays(Random.Shared.Next(0, 365))),
                        Gender = (Gender)Random.Shared.Next(0, typeof(Gender).GetEnumValues().Length),
                        HasDriversLicense = i % 5 != 0,
                        Salary = Random.Shared.Next(1000, 10000),
                        ManagerId = i <= employeeCount / 10 ? null : Random.Shared.Next(1, employeeCount / 10 + 1),
                        HasEmployess = i <= employeeCount / 10 ? true : false
                    });
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task ClearData()
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            dbContext.Employees.RemoveRange(dbContext.Employees);

            await dbContext.SaveChangesAsync();
        }
    }
}
