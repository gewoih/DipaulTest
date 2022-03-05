using DipaulTest.Commands;
using DipaulTest.Models;
using DipaulTest.ViewModels.Base;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
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

				XmlDocument xDoc = new XmlDocument();
				xDoc.Load(this.OpenedXmlPath);

				XmlElement xRoot = xDoc.DocumentElement;
				if (xRoot == null)
					return;
				
				foreach (XmlElement xNode in xRoot)
				{
					if (xNode.Name != "companies")
						continue;

					foreach (XmlNode companyNode in xNode.ChildNodes)
					{
						if (companyNode.Name != "company")
							continue;
						
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

		public ICommand UpdateXmlCommand { get; }
		private bool CanUpdateXmlCommandExecute(object p) => !String.IsNullOrEmpty(this.OpenedXmlPath);
		private void OnUpdateXmlCommandExecuted(object p)
		{
			SaveFileDialog fileDialog = new SaveFileDialog();
			fileDialog.Filter = "XML files (.xml)|*.xml";

			if ((bool)fileDialog.ShowDialog())
			{
				XmlDocument xDoc = new XmlDocument();
				xDoc.AppendChild(xDoc.CreateProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\""));
				XmlElement xRoot = xDoc.CreateElement("root");
				xDoc.AppendChild(xRoot);

				foreach (var company in this.Companies)
				{
					XmlElement companyElement = xDoc.CreateElement("company");
					companyElement.Attributes.Append(xDoc.CreateAttribute("name"));
					companyElement.Attributes.GetNamedItem("name").InnerText = company.Name;

					foreach (var employee in company.Employees)
					{
						XmlElement employeeElement = xDoc.CreateElement("employee");
						employeeElement.Attributes.Append(xDoc.CreateAttribute("name"));
						employeeElement.Attributes.GetNamedItem("name").InnerText = employee.Name;
						employeeElement.InnerText = employee.Role.Name;

						companyElement.AppendChild(employeeElement);
					}
					xDoc.DocumentElement.AppendChild(companyElement);
				}
				
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
