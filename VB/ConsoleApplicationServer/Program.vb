Imports System
Imports System.Collections.Generic
Imports System.Linq
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
Imports DevExpress.Persistent.BaseImpl.PermissionPolicy
Imports DevExpress.ExpressApp.Win.SystemModule
Imports DevExpress.ExpressApp.Web.SystemModule
Imports PermissionPolicyExample.Module
Imports DevExpress.ExpressApp.MiddleTier

Namespace ConsoleApplicationServer
    Friend Class Program
        Shared Sub New()
            DevExpress.Persistent.Base.PasswordCryptographer.EnableRfc2898 = True
            DevExpress.Persistent.Base.PasswordCryptographer.SupportLegacySha512 = False
        End Sub
        Private Shared Sub serverApplication_DatabaseVersionMismatch(ByVal sender As Object, ByVal e As DatabaseVersionMismatchEventArgs)
            e.Updater.Update()
            e.Handled = True
        End Sub
        Private Shared Sub serverApplication_CreateCustomObjectSpaceProvider(ByVal sender As Object, ByVal e As CreateCustomObjectSpaceProviderEventArgs)
            e.ObjectSpaceProvider = New XPObjectSpaceProvider(e.ConnectionString, e.Connection)
        End Sub
        Shared Sub Main(ByVal args() As String)
            Try
                SecurityAdapterHelper.Enable()
                ValueManager.ValueManagerType = GetType(MultiThreadValueManager(Of )).GetGenericTypeDefinition()

                'string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                Dim connectionString As String = InMemoryDataStoreProvider.ConnectionString

                Dim serverApplication As New ServerApplication()
                serverApplication.ApplicationName = "PermissionPolicyExample"
                serverApplication.CheckCompatibilityType = CheckCompatibilityType.DatabaseSchema
#If DEBUG Then
                If System.Diagnostics.Debugger.IsAttached AndAlso serverApplication.CheckCompatibilityType = CheckCompatibilityType.DatabaseSchema Then
                    serverApplication.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways
                End If
#End If

                serverApplication.Modules.BeginInit()
                serverApplication.Modules.Add(New SystemWindowsFormsModule())
                serverApplication.Modules.Add(New SystemAspNetModule())
                serverApplication.Modules.Add(New PermissionPolicyExampleModule())
                serverApplication.Modules.Add(New SecurityModule())
                serverApplication.Modules.EndInit()

                AddHandler serverApplication.DatabaseVersionMismatch, AddressOf serverApplication_DatabaseVersionMismatch
                AddHandler serverApplication.CreateCustomObjectSpaceProvider, AddressOf serverApplication_CreateCustomObjectSpaceProvider

                serverApplication.ConnectionString = connectionString
                Console.WriteLine("Setup...")
                serverApplication.Setup()
                Console.WriteLine("CheckCompatibility...")
                serverApplication.CheckCompatibility()
                serverApplication.Dispose()

                Console.WriteLine("Starting server...")
                Dim dataServerSecurityProvider As Func(Of IDataServerSecurity) = Function()
                    Dim security As New SecurityStrategyComplex(GetType(PermissionPolicyUser), GetType(PermissionPolicyRole), New AuthenticationStandard())
                    security.SupportNavigationPermissionsForTypes = False
                    Return security
                End Function

                Dim serviceHost As New WcfXafServiceHost(connectionString, dataServerSecurityProvider)
                serviceHost.AddServiceEndpoint(GetType(IWcfXafDataServer), WcfDataServerHelper.CreateNetTcpBinding(), "net.tcp://127.0.0.1:1451/DataServer")
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
