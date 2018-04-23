using System;
using System.Configuration;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using System.Collections;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Security.ClientServer.Wcf;
using System.ServiceModel;

namespace SecuritySystemExample.Win {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
#if EASYTEST
			DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached;
            SecuritySystemExampleWindowsFormsApplication winApplication = 
                new SecuritySystemExampleWindowsFormsApplication();
            try {
                winApplication.ConnectionString = "http://localhost:1451/DataServer";
                WcfSecuredDataServerClient clientDataServer = new WcfSecuredDataServerClient(
                    WcfDataServerHelper.CreateDefaultBinding(), 
                    new EndpointAddress(winApplication.ConnectionString));
                ServerSecurityClient securityClient = new ServerSecurityClient(clientDataServer, new ClientInfoFactory());
                securityClient.IsSupportChangePassword = true;
                winApplication.ApplicationName = "SecuritySystemExample";
                winApplication.Security = securityClient;
                winApplication.CreateCustomObjectSpaceProvider += delegate(object sender, CreateCustomObjectSpaceProviderEventArgs e) {
                    e.ObjectSpaceProvider = new DataServerObjectSpaceProvider(clientDataServer, securityClient);
                };
                winApplication.Setup();
                winApplication.Start();
                clientDataServer.Close();
            }
            catch (Exception e) {
                winApplication.HandleException(e);
            }
        }
    }
}
