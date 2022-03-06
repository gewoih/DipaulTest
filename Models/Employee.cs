namespace DipaulTest.Models
{
	public class Employee
	{
		public Company Company { get; set; }
		public Role Role { get; set; }
		public string Name { get; set; }

		public Employee(Company company, Role role, string name)
		{
			this.Company = company;
			this.Role = role;
			this.Name = name;
		}
	}
}
