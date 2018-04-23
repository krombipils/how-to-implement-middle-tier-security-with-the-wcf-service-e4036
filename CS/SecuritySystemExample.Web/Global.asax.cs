using System;
using System.Configuration;
using System.Web.Configuration;
using System.Web;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Web;
using DevExpress.Web.ASPxClasses;
using System.Collections;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Security.ClientServer.Wcf;
using System.ServiceModel;

namespace SecuritySystemExample.Web {
    public class Global : System.Web.HttpApplication {
        public Global() {
            InitializeComponent();
        }
        protected void Application_Start(Object sender, EventArgs e) {
            ASPxWebControl.CallbackError += new EventHandler(Application_Error);
#if EASYTEST
			DevExpress.ExpressApp.Web.TestScripts.TestScriptsManager.EasyTestEnabled = true;
#endif
        }
        protected void Session_Start(Object sender, EventArgs e) {
            WebApplication.SetInstance(Session, new SecuritySystemExampleAspNetApplication());
            string connectionString = "http://localhost:1451/DataServer";
            WcfSecuredDataServerClient clientDataServer = new WcfSecuredDataServerClient(
                WcfDataServerHelper.CreateDefaultBinding(), new EndpointAddress(connectionString));
            Session["DataServerClient"] = clientDataServer;
            ServerSecurityClient securityClient = new ServerSecurityClient(clientDataServer, new ClientInfoFactory());
            securityClient.IsSupportChangePassword = true;
            WebApplication.Instance.ApplicationName = "SecuritySystemExample";
            WebApplication.Instance.Security = securityClient;
            WebApplication.Instance.CreateCustomObjectSpaceProvider += 
                delegate(object sender2, CreateCustomObjectSpaceProviderEventArgs e2) {
                e2.ObjectSpaceProvider = new DataServerObjectSpaceProvider(clientDataServer, securityClient);
            };
            WebApplication.Instance.Setup();
            WebApplication.Instance.Start();
        }
        protected void Application_BeginRequest(Object sender, EventArgs e) {
            string filePath = HttpContext.Current.Request.PhysicalPath;
            if (!string.IsNullOrEmpty(filePath)
                && (filePath.IndexOf("Images") >= 0) && !System.IO.File.Exists(filePath)) {
                HttpContext.Current.Response.End();
            }
        }
        protected void Application_EndRequest(Object sender, EventArgs e) {
        }
        protected void Application_AuthenticateRequest(Object sender, EventArgs e) {
        }
        protected void Application_Error(Object sender, EventArgs e) {
            ErrorHandling.Instance.ProcessApplicationError();
        }
        protected void Session_End(Object sender, EventArgs e) {
            WebApplication.LogOff(Session);
            WebApplication.DisposeInstance(Session);
            WcfSecuredDataServerClient clientDataServer = (WcfSecuredDataServerClient)Session["DataServerClient"];
            if (clientDataServer != null) {
                clientDataServer.Close();
                Session["DataServerClient"] = null;
            }
        }
        protected void Application_End(Object sender, EventArgs e) {
        }
        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
        #endregion
    }
}
