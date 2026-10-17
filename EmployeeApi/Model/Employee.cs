using System.ComponentModel.DataAnnotations;

namespace EmployeeApi.Model
{
    public class Employee
    {
        public int Id { get; private set; }

        [Required, StringLength(100)]
        public string Name { get; private set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Salary { get; private set; }
        private Employee()
        {

        }
        public static Employee Create(string name, decimal salary)
        {
            Validate(name, salary);
            return new Employee { Name = name.Trim(), Salary = salary };
        }
        public void Update(string name, decimal salary)
        {
            Validate(name, salary);
            Name = name.Trim();
            Salary = salary;
        }

        private static void Validate(string name, decimal salary)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));
            if (salary <= 0)
                throw new ArgumentException("Salary must be greater than zero.", nameof(salary));
        }
    }
}
