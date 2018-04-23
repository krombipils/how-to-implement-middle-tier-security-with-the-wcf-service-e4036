Imports System.Configuration
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Xpo
Imports DevExpress.ExpressApp.Security
Imports DevExpress.ExpressApp.Win
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.BaseImpl
Imports System.Collections
Imports System.Runtime.Remoting.Channels.Tcp
Imports System.Runtime.Remoting.Channels
Imports DevExpress.ExpressApp.Security.ClientServer
Imports DevExpress.ExpressApp.Security.ClientServer.Wcf
Imports System.ServiceModel

Namespace SecuritySystemExample.Win
	Friend NotInheritable Class Program
		''' <summary>
		''' The main entry point for the application.
		''' </summary>
		Private Sub New()
		End Sub
		<STAThread> _
		Shared Sub Main()
#If EASYTEST Then
			DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register()
#End If
			Application.EnableVisualStyles()
			Application.SetCompatibleTextRenderingDefault(False)
			EditModelPermission.AlwaysGranted = Debugger.IsAttached
			Dim winApplication As New SecuritySystemExampleWindowsFormsApplication()
			Try
				winApplication.ConnectionString = "http://localhost:1451/DataServer"
				Dim clientDataServer As New WcfSecuredDataServerClient(WcfDataServerHelper.CreateDefaultBinding(), New EndpointAddress(winApplication.ConnectionString))
				Dim securityClient As New ServerSecurityClient(clientDataServer, New ClientInfoFactory())
				securityClient.IsSupportChangePassword = True
				winApplication.ApplicationName = "SecuritySystemExample"
				winApplication.Security = securityClient
				AddHandler winApplication.CreateCustomObjectSpaceProvider, Sub(sender As Object, e As CreateCustomObjectSpaceProviderEventArgs) e.ObjectSpaceProvider = New DataServerObjectSpaceProvider(clientDataServer, securityClient)
				winApplication.Setup()
				winApplication.Start()
				clientDataServer.Close()
			Catch e As Exception
				winApplication.HandleException(e)
			End Try
		End Sub
	End Class
End Namespace
