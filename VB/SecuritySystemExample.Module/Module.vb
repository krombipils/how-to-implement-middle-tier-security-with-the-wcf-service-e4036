Imports System
Imports System.Collections.Generic

Imports DevExpress.ExpressApp
Imports System.Reflection
Imports DevExpress.ExpressApp.Security.Strategy
Imports DevExpress.Persistent.BaseImpl.PermissionPolicy

Namespace PermissionPolicyExample.Module
    Public NotInheritable Partial Class PermissionPolicyExampleModule
        Inherits ModuleBase

        Public Sub New()
            InitializeComponent()
        End Sub
        Protected Overrides Function GetDeclaredExportedTypes() As IEnumerable(Of Type)
            Dim result As New List(Of Type)(MyBase.GetDeclaredExportedTypes())
            result.AddRange(New Type() { GetType(PermissionPolicyUser), GetType(PermissionPolicyRole) })
            Return result
        End Function
    End Class
End Namespace
