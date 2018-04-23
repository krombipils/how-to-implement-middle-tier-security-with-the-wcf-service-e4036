using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Security.ClientServer;
using System.Configuration;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp;
using System.Collections;
using DevExpress.ExpressApp.Xpo;
using System.ServiceModel;
using System.Runtime.Remoting.Channels;
using DevExpress.ExpressApp.Security.ClientServer.Wcf;


namespace ConsoleApplicationServer {
    class Program {
        static void Main(string[] args) {
            try {
                ValueManager.ValueManagerType = typeof(MultiThreadValueManager<>).GetGenericTypeDefinition();

                //string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                InMemoryDataStoreProvider.Register();
                string connectionString = InMemoryDataStoreProvider.ConnectionString;
                Console.WriteLine("Starting...");
                ConsoleApplicationServerServerApplication serverApplication = new ConsoleApplicationServerServerApplication();
                serverApplication.ConnectionString = connectionString;
                Console.WriteLine("Setup...");
                serverApplication.Setup();
                Console.WriteLine("CheckCompatibility...");
                serverApplication.CheckCompatibility();
                serverApplication.Dispose();
                Console.WriteLine("Starting server...");
                QueryRequestSecurityStrategyHandler securityProviderHandler = delegate() {
                    return new SecurityStrategyComplex(typeof(SecuritySystemUser), typeof(SecuritySystemRole), new AuthenticationStandard());
                };
                SecuredDataServer dataServer = new SecuredDataServer(
                    connectionString, XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary,
                            securityProviderHandler);
                ServiceHost serviceHost = new ServiceHost(new WcfSecuredDataServer(dataServer));
                serviceHost.AddServiceEndpoint(typeof(IWcfSecuredDataServer), WcfDataServerHelper.CreateDefaultBinding(), "http://localhost:1451/DataServer");
                serviceHost.Open();
                Console.WriteLine("Server is started. Press Enter to stop.");
                Console.ReadLine();
                Console.WriteLine("Stopping...");
                serviceHost.Close();
                Console.WriteLine("Server is stopped.");
            }
            catch (Exception e) {
                Console.WriteLine("Exception occurs: " + e.Message);
                Console.WriteLine("Press Enter to close.");
                Console.ReadLine();
            }
        }
    }
}
