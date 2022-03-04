namespace DipaulTest.Models
{
	public class Employee
	{
		public Company Company { get; }
		public Role Role { get; }
		public string Name { get; }

		public Employee(Company company, Role role, string name)
		{
			this.Company = company;
			this.Role = role;
			this.Name = name;
		}
	}
}
