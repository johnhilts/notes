Dim lastLogin
Dim healthVaultDate

lastLogin = CDate("1/1/0001")
healthVaultDate = CDate("9/8/2008")

If lastLogin < healthVaultDate Then
	WScript.Echo("Last Login comes before HV Date")
Else
	WScript.Echo("Last Login comes after HV Date")
End If