Imports System.Text
Imports DevExpress.ExpressApp.Security.ClientServer
Imports System.Configuration
Imports DevExpress.Persistent.Base
Imports DevExpress.ExpressApp.Security
Imports DevExpress.ExpressApp.Security.Strategy
Imports DevExpress.ExpressApp
Imports System.Collections
Imports DevExpress.ExpressApp.Xpo
Imports System.ServiceModel
Imports System.Runtime.Remoting.Channels
Imports DevExpress.ExpressApp.Security.ClientServer.Wcf


Namespace ConsoleApplicationServer
	Friend Class Program
		Shared Sub Main(ByVal args() As String)
			Try
				ValueManager.ValueManagerType = GetType(MultiThreadValueManager(Of )).GetGenericTypeDefinition()

				'string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
				InMemoryDataStoreProvider.Register()
				Dim connectionString As String = InMemoryDataStoreProvider.ConnectionString
				Console.WriteLine("Starting...")
				Dim serverApplication As New ConsoleApplicationServerServerApplication()
				serverApplication.ConnectionString = connectionString
				Console.WriteLine("Setup...")
				serverApplication.Setup()
				Console.WriteLine("CheckCompatibility...")
				serverApplication.CheckCompatibility()
				serverApplication.Dispose()
				Console.WriteLine("Starting server...")
				Dim securityProviderHandler As QueryRequestSecurityStrategyHandler = Function() New SecurityStrategyComplex(GetType(SecuritySystemUser), GetType(SecuritySystemRole), New AuthenticationStandard())
				Dim dataServer As New SecuredDataServer(connectionString, XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary, securityProviderHandler)
				Dim serviceHost As New ServiceHost(New WcfSecuredDataServer(dataServer))
				serviceHost.AddServiceEndpoint(GetType(IWcfSecuredDataServer), WcfDataServerHelper.CreateDefaultBinding(), "http://localhost:1451/DataServer")
				serviceHost.Open()
				Console.WriteLine("Server is started. Press Enter to stop.")
				Console.ReadLine()
				Console.WriteLine("Stopping...")
				serviceHost.Close()
				Console.WriteLine("Server is stopped.")
			Catch e As Exception
				Console.WriteLine("Exception occurs: " & e.Message)
				Console.WriteLine("Press Enter to close.")
				Console.ReadLine()
			End Try
		End Sub
	End Class
End Namespace
