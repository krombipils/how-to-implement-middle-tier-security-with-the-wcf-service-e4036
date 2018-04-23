Imports System.Text
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.Persistent.Base
Imports DevExpress.Xpo

Namespace SecuritySystemExample.Module.BusinessObjects
	<DefaultClassOptions, ImageName("BO_Person")> _
	Public Class Contact
		Inherits BaseObject
		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub
'INSTANT VB NOTE: The variable name was renamed since Visual Basic does not allow class members with the same name:
		Private name_Renamed As String
		Public Property Name() As String
			Get
				Return name_Renamed
			End Get
			Set(ByVal value As String)
				SetPropertyValue("Name", name_Renamed, value)
			End Set
		End Property
'INSTANT VB NOTE: The variable email was renamed since Visual Basic does not allow class members with the same name:
		Private email_Renamed As String
		Public Property Email() As String
			Get
				Return email_Renamed
			End Get
			Set(ByVal value As String)
				SetPropertyValue("Email", email_Renamed, value)
			End Set
		End Property
	End Class
End Namespace
