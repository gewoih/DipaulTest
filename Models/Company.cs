using System.Collections.Generic;

namespace DipaulTest.Models
{
	public class Company
	{
		public string Name { get; set; }
		public ICollection<Employee> Employees { get; set; }

		public Company(string name)
		{
			this.Name = name;
			this.Employees = new List<Employee>();
		}
	}
}
