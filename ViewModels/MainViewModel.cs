using DipaulTest.Commands;
using DipaulTest.Models;
using DipaulTest.ViewModels.Base;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Xml.Linq;

namespace DipaulTest.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		#region Constructor
		public MainViewModel()
		{
			this.Companies = new ObservableCollection<Company>();
			this.Roles = new ObservableCollection<Role>();

			this.LoadXmlCommand = new RelayCommand(OnLoadXmlCommandExecuted, CanLoadXmlCommandExecute);
			this.UpdateXmlCommand = new RelayCommand(OnUpdateXmlCommandExecuted, CanUpdateXmlCommandExecute);
		}
		#endregion

		#region Properties
		private string OpenedXmlPath { get; set; }

		private ObservableCollection<Company> _Companies;
		public ObservableCollection<Company> Companies
		{
			get => _Companies;
			set => Set(ref _Companies, value);
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
			set => Set(ref _SelectedCompany, value);
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
			this.ClearLoadedXmlData();

			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Filter = "XML files (.xml)|*.xml";

			if ((bool)fileDialog.ShowDialog())
			{
				this.OpenedXmlPath = fileDialog.FileName;

				XDocument xDoc = XDocument.Load(this.OpenedXmlPath);
				XElement xRoot = xDoc.Element("root");
				if (xRoot == null)
					throw new Exception("В файле не найден корневой элемент 'root'!");

				XElement xCompanies = xRoot.Element("companies");
				if (xCompanies == null)
					throw new Exception("В файле не найден корневой элемент 'companies'!");

				foreach (var companyNode in xCompanies.Elements("company"))
				{
					Company newCompany = new Company(companyNode.Attribute("name").Value);
					foreach (var employeeNode in companyNode.Elements("employee"))
						newCompany.Employees.Add(new Employee(newCompany, new Role(employeeNode.Value), employeeNode.Attribute("name").Value));
					
					this.Companies.Add(newCompany);
				}
			}
		}

		public ICommand UpdateXmlCommand { get; }
		private bool CanUpdateXmlCommandExecute(object p) => !String.IsNullOrEmpty(this.OpenedXmlPath) && this.Companies.Count != 0;
		private void OnUpdateXmlCommandExecuted(object p)
		{
			SaveFileDialog fileDialog = new SaveFileDialog();
			fileDialog.Filter = "XML files (.xml)|*.xml";

			if ((bool)fileDialog.ShowDialog())
			{
				XDocument xDoc = new XDocument();
				XElement xCompanies = new XElement("companies");

				foreach (var company in this.Companies)
				{
					XElement companyElement = new XElement("company");
					companyElement.Add(new XAttribute("name", company.Name));

					foreach (var employee in company.Employees)
					{
						XElement employeeElement = new XElement("employee");
						employeeElement.Add(new XAttribute("name", employee.Name));

						if (!String.IsNullOrEmpty(employee.Role.Name))
							employeeElement.Value = employee.Role.Name;

						companyElement.Add(employeeElement);
					}
					xCompanies.Add(companyElement);
				}

				XElement xRoot = new XElement("root");
				xRoot.Add(xCompanies);

				xDoc.Add(xRoot);
				xDoc.Save(fileDialog.FileName);
			}
		}
		#endregion

		#region Methods
		private void ClearLoadedXmlData()
		{
			this.Companies.Clear();
			this.Roles.Clear();
		}
		#endregion
	}
}
