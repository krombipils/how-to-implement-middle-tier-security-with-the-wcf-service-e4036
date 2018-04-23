;#DropDB PermissionPolicyExampleEasyTest

#Application PermissionPolicyExample.Win
#Application PermissionPolicyExample.Web

#IfDef PermissionPolicyExample.Win

*Action About...
 
*Action OK

#EndIf

#IfDef PermissionPolicyExample.Web

*Action Choose Theme(Black Glass)

#EndIf
