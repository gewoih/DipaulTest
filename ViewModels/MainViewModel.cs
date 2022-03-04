using DipaulTest.Commands;
using DipaulTest.Models;
using DipaulTest.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace DipaulTest.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		#region Constructor
		public MainViewModel()
		{
			this.Companies = new ObservableCollection<Company>();
			this.Employees = new ObservableCollection<Employee>();
			this.Roles = new ObservableCollection<Role>();

			this.LoadXmlCommand = new RelayCommand(OnLoadXmlCommandExecuted, CanLoadXmlCommandExecute);
			this.UpdateXmlCommand = new RelayCommand(OnUpdateXmlCommandExecuted, CanUpdateXmlCommandExecute);
		}
		#endregion

		#region Properties
		private ObservableCollection<Company> _Companies;
		public ObservableCollection<Company> Companies
		{
			get => _Companies;
			set => Set(ref _Companies, value);
		}

		private ObservableCollection<Employee> _Employees;
		public ObservableCollection<Employee> Employees
		{
			get => _Employees;
			set => Set(ref _Employees, value);
		}

		private ObservableCollection<Role> _Roles;
		public ObservableCollection<Role> Roles
		{
			get => _Roles;
			set => Set(ref _Roles, value);
		}

		private Company _SelectedCompany;
		public Company SelectedCompany
		{
			get => _SelectedCompany;
			set
			{
				Set(ref _SelectedCompany, value);
				this.Employees = new ObservableCollection<Employee>(this.SelectedCompany.Employees);
			}
		}

		private Employee _SelectedEmployee;
		public Employee SelectedEmployee
		{
			get => _SelectedEmployee;
			set => Set(ref _SelectedEmployee, value);
		}
		#endregion

		#region Commands
		public ICommand LoadXmlCommand { get; }
		private bool CanLoadXmlCommandExecute(object p) => true;
		private void OnLoadXmlCommandExecuted(object p)
		{
			XmlDocument xDoc = new XmlDocument();
			xDoc.Load("Companies.xml");

			XmlElement xRoot = xDoc.DocumentElement;
			if (xRoot != null)
			{
				foreach (XmlElement xNode in xRoot)
				{
					if (xNode.Name == "companies")
					{
						foreach (XmlNode companyNode in xNode.ChildNodes)
						{
							if (companyNode.Name == "company")
							{
								Company newCompany = new Company(companyNode.Attributes.GetNamedItem("name").Value);

								foreach (XmlNode employeeNode in companyNode.ChildNodes)
								{
									if (employeeNode.Name == "employee")
										newCompany.Employees.Add(new Employee(newCompany, new Role(employeeNode.InnerText), employeeNode.Attributes.GetNamedItem("name").Value));
								}

								this.Companies.Add(newCompany);
							}
						}
					}
				}
			}
		}

		public ICommand UpdateXmlCommand { get; }
		private bool CanUpdateXmlCommandExecute(object p) => this.Companies != null;
		private void OnUpdateXmlCommandExecuted(object p)
		{

		}
		#endregion
	}
}
