Imports System
Imports System.Configuration
Imports System.Windows.Forms
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

Namespace PermissionPolicyExample.Win
    Friend NotInheritable Class Program

        Private Sub New()
        End Sub

        ''' <summary>
        ''' The main entry point for the application.
        ''' </summary>
        <STAThread> _
        Shared Sub Main()
#If EASYTEST Then
            DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register()
#End If
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached
            If Tracing.GetFileLocationFromSettings() = DevExpress.Persistent.Base.FileLocation.CurrentUserApplicationDataFolder Then
                Tracing.LocalUserAppDataPath = Application.LocalUserAppDataPath
            End If
            Tracing.Initialize()
            Dim winApplication As New PermissionPolicyExampleWindowsFormsApplication()
            ' Refer to the https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112680.aspx help article for more details on how to provide a custom splash form.
            'winApplication.SplashScreen = new DevExpress.ExpressApp.Win.Utils.DXSplashScreen("YourSplashImage.png");
            Try
                Dim connectionString As String = "net.tcp://127.0.0.1:1451/DataServer"
                Dim wcfSecuredClient As New WcfSecuredClient(WcfDataServerHelper.CreateNetTcpBinding(), New EndpointAddress(connectionString))
                Dim security As New MiddleTierClientSecurity(wcfSecuredClient)
                security.IsSupportChangePassword = True
                winApplication.Security = security
                winApplication.DatabaseUpdateMode = DatabaseUpdateMode.Never
                AddHandler winApplication.CreateCustomObjectSpaceProvider, Sub(sender As Object, e As CreateCustomObjectSpaceProviderEventArgs)
                        e.ObjectSpaceProvider = New MiddleTierServerObjectSpaceProvider(wcfSecuredClient)
                End Sub
                winApplication.Setup()
                winApplication.Start()
                wcfSecuredClient.Dispose()
            Catch e As Exception
                winApplication.HandleException(e)
            End Try
        End Sub
    End Class
End Namespace
