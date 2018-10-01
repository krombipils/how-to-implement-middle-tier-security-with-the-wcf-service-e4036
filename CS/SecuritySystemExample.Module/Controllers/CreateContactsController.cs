namespace SecuritySystemExample.Module.Controllers
{
	using System;
	using System.ComponentModel;
	using DevExpress.ExpressApp;
	using DevExpress.ExpressApp.Actions;
	using DevExpress.Persistent.Base;
	using PermissionPolicyExample.Module.BusinessObjects;

	public class CreateContactsController : ViewController
	{
		private SimpleAction action;

		private IContainer components;

		public CreateContactsController()
		{
			InitializeComponent();
			RegisterActions(components);
		}

		private void InitializeComponent()
		{
			components = new Container();
			action = new SimpleAction(this, "CreateContacts", PredefinedCategory.Edit)
			{
			};

			action.Executed += ActionExecuted;
		}


		private void ActionExecuted(object sender, ActionBaseEventArgs e)
		{
			for (var i = 1; i <= 100; i++)
			{
				var mail = "dummy_email_" + Guid.NewGuid() + "@example.com";
				var name = i + "";
				var contact = ObjectSpace.CreateObject<Contact>();
				contact.Name = name;
				contact.Email = mail;
			}
			ObjectSpace.CommitChanges();
		}
	}
}