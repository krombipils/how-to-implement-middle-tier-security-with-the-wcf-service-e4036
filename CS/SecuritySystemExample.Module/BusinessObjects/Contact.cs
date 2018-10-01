using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace PermissionPolicyExample.Module.BusinessObjects {
	using DevExpress.Persistent.Validation;

	[DefaultClassOptions, ImageName("BO_Person")]
    public class Contact : BaseObject {
        public Contact(Session session) : base(session) { }
        private string name;
        public string Name {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }
        private string email;
		

		[RuleUniqueValue("Email.Unique", DefaultContexts.Save, CriteriaEvaluationBehavior = CriteriaEvaluationBehavior.InTransaction)]
        public string Email {
            get { return email; }
            set { SetPropertyValue("Email", ref email, value); }
        }
	}
}
