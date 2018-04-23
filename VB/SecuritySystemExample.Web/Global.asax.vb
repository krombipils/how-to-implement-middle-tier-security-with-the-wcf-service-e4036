Imports System.Configuration
Imports System.Web.Configuration

Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Xpo
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.ExpressApp.Security
Imports DevExpress.ExpressApp.Web
Imports DevExpress.Web.ASPxClasses
Imports System.Collections
Imports DevExpress.ExpressApp.Security.ClientServer
Imports DevExpress.ExpressApp.Security.ClientServer.Wcf
Imports System.ServiceModel

Namespace SecuritySystemExample.Web
	Public Class [Global]
		Inherits HttpApplication
		Public Sub New()
			InitializeComponent()
		End Sub
		Protected Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
			AddHandler ASPxWebControl.CallbackError, AddressOf Application_Error
#If EASYTEST Then
			DevExpress.ExpressApp.Web.TestScripts.TestScriptsManager.EasyTestEnabled = True
#End If
		End Sub
		Protected Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
			WebApplication.SetInstance(Session, New SecuritySystemExampleAspNetApplication())
			Dim connectionString As String = "http://localhost:1451/DataServer"
			Dim clientDataServer As New WcfSecuredDataServerClient(WcfDataServerHelper.CreateDefaultBinding(), New EndpointAddress(connectionString))
			Session("DataServerClient") = clientDataServer
			Dim securityClient As New ServerSecurityClient(clientDataServer, New ClientInfoFactory())
			securityClient.IsSupportChangePassword = True
			WebApplication.Instance.ApplicationName = "SecuritySystemExample"
			WebApplication.Instance.Security = securityClient
			AddHandler WebApplication.Instance.CreateCustomObjectSpaceProvider, Sub(sender2 As Object, e2 As CreateCustomObjectSpaceProviderEventArgs) e2.ObjectSpaceProvider = New DataServerObjectSpaceProvider(clientDataServer, securityClient)
			WebApplication.Instance.Setup()
			WebApplication.Instance.Start()
		End Sub
		Protected Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
			Dim filePath As String = HttpContext.Current.Request.PhysicalPath
			If (Not String.IsNullOrEmpty(filePath)) AndAlso (filePath.IndexOf("Images") >= 0) AndAlso (Not System.IO.File.Exists(filePath)) Then
				HttpContext.Current.Response.End()
			End If
		End Sub
		Protected Sub Application_EndRequest(ByVal sender As Object, ByVal e As EventArgs)
		End Sub
		Protected Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
		End Sub
		Protected Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
			ErrorHandling.Instance.ProcessApplicationError()
		End Sub
		Protected Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
			WebApplication.LogOff(Session)
			WebApplication.DisposeInstance(Session)
			Dim clientDataServer As WcfSecuredDataServerClient = CType(Session("DataServerClient"), WcfSecuredDataServerClient)
			If clientDataServer IsNot Nothing Then
				clientDataServer.Close()
				Session("DataServerClient") = Nothing
			End If
		End Sub
		Protected Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
		End Sub
		#Region "Web Form Designer generated code"
		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
		End Sub
		#End Region
	End Class
End Namespace
