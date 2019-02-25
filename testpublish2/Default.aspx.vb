
Partial Class _Default
    Inherits System.Web.UI.Page




    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim neworg As New Functions.LiveIT.vCloudFunctions.Org
        neworg.url = "https://vcp1.liveitcloud.com"
        neworg.companyshortname = txtCompanyShortName.Text
        neworg.companyfullname = txtCompanyFullName.Text
        If txtPassword.Text <> txtPasswordCheck.Text Then
            'need to tell them
            Exit Sub
        End If
        If txtOrgEmailAddress.Text.Contains("@") Then
        Else
            'need to do shit here as well
            Exit Sub
        End If
        neworg.emailaddress = txtOrgEmailAddress.Text
        neworg.adminname = txtOrgName.Text
        neworg.adminpassword = txtPassword.Text
        neworg.ccnumber = txtCCNumber.Text
        neworg.city = txtCCCity.Text
        neworg.address1 = txtCCBillingAddress.Text
        neworg.state = txtCCState.Text
        neworg.firstname = txtCCFirstName.Text
        neworg.lastname = txtCCLastName.Text
        neworg.ccexpmonth = txtCCMonth.Text
        neworg.ccexpyear4digit = txtCCYear.Text
        neworg.zip = txtCCZip.Text
        neworg.cccvc = txtCCCVC.Text


        Dim liveitfunctions As New Functions.LiveIT.vCloudFunctions
        liveitfunctions.createCustProfile(neworg)
        If neworg.authorized = False Then
            'card not good do some shit
            Exit Sub

        End If
        liveitfunctions.Authenticate(neworg)
        neworg.verified = False
        liveitfunctions.verifyOrgNotExist(neworg)
        If neworg.alreadyexists Then
            MsgBox("Org Shortname already Exists")
            Exit Sub
        End If
        liveitfunctions.createOrg(neworg)
        liveitfunctions.getOrgHref(neworg)
        liveitfunctions.enableOrg(neworg)
        liveitfunctions.getAdminRoles(neworg)
        liveitfunctions.createAdminUser(neworg)
        liveitfunctions.updateAdminUser(neworg)
        liveitfunctions.createOrgVDC(neworg)
        liveitfunctions.getVDC(neworg)
        liveitfunctions.createCatalog(neworg)
        liveitfunctions.getEdgeGateway(neworg)
        liveitfunctions.convertEdgeGateway(neworg)
        liveitfunctions.insertnew(neworg)

    End Sub
End Class
url