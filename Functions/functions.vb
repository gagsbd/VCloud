Imports Microsoft.VisualBasic
Imports System.Net.Http
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Net
Imports System.Data
Imports System.Data.SqlClient



Namespace LiveIT

    Public Class vCloudFunctions


        Dim connectionstring As String = My.Settings.DatabaseStore
        Dim username = My.Settings.vCloudUserName
        Dim adminPass = My.Settings.vCloudUserPass
        Dim apiVersion As String = My.Settings.apiVersion
        Public Function Authenticate(objorg As Org)
            Dim urlvcloud As String
            urlvcloud = objorg.url
            Dim authRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(urlvcloud & "/api/sessions"), HttpWebRequest)
            authRequest.Method = "POST"
            authRequest.Accept = apiVersion

            Dim credentials As String = Convert.ToBase64String(Encoding.ASCII.GetBytes(username & "@System:" & adminPass))
            authRequest.Headers.Add("Authorization:" & "Basic " & credentials)
            'So we setup the request, now we need to execute the task

            Dim authresponse As HttpWebResponse = CType(authRequest.GetResponse(), HttpWebResponse)

            Dim receivestream As Stream = authresponse.GetResponseStream

            Dim readstream As New StreamReader(receivestream, Encoding.UTF8)

            objorg.authtoken = authresponse.GetResponseHeader("x-vcloud-authorization")
            authRequest = Nothing
            authresponse = Nothing
            receivestream = Nothing
            readstream = Nothing
            Return (objorg)


        End Function
        Public Function verifyOrgNotExist(objorg As Org)
            Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.url & "/api/org"), HttpWebRequest)
            orgRequest.Method = "GET"
            orgRequest.Accept = apiVersion

            orgRequest.Headers.Add("x-vcloud-authorization", objorg.authtoken)

            Dim orgResponse As HttpWebResponse = CType(orgRequest.GetResponse(), HttpWebResponse)
            Dim receivestream As Stream = orgResponse.GetResponseStream

            Dim readstream As New StreamReader(receivestream, Encoding.UTF8)

            Dim orgXML As String = readstream.ReadToEnd()
            readstream.Close()
            readstream = Nothing
            receivestream.Close()
            receivestream = Nothing
            Dim xmlDoc As New System.Xml.XmlDocument()

            xmlDoc.LoadXml(orgXML)

            Dim xmlnode As XmlNodeList

            Dim i As Integer
            Dim compare As String
            xmlnode = xmlDoc.GetElementsByTagName("Org")


            For i = 0 To xmlnode.Count - 1
                compare = (xmlnode(i).Attributes("name").Value)
                If objorg.companyshortname = compare Then
                    objorg.alreadyexists = True

                End If
            Next

            orgRequest = Nothing
            orgResponse = Nothing
            receivestream = Nothing
            orgXML = Nothing
            xmlDoc = Nothing
            xmlnode = Nothing
            i = Nothing

            Return (objorg)

        End Function
        Public Function getOrgHref(objorg As Org)

            Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.url & "/api/org"), HttpWebRequest)
            orgRequest.Method = "GET"
            orgRequest.Accept = apiVersion

            orgRequest.Headers.Add("x-vcloud-authorization", objorg.authtoken)

            Dim orgResponse As HttpWebResponse = CType(orgRequest.GetResponse(), HttpWebResponse)
            Dim receivestream As Stream = orgResponse.GetResponseStream

            Dim readstream As New StreamReader(receivestream, Encoding.UTF8)

            Dim orgXML As String = readstream.ReadToEnd()
            readstream.Close()
            readstream = Nothing
            receivestream.Close()
            receivestream = Nothing
            Dim xmlDoc As New System.Xml.XmlDocument()

            xmlDoc.LoadXml(orgXML)

            Dim xmlnode As XmlNodeList

            Dim i As Integer
            Dim compare As String
            xmlnode = xmlDoc.GetElementsByTagName("Org")

            For i = 0 To xmlnode.Count - 1
                compare = (xmlnode(i).Attributes("name").Value)
                If objorg.companyshortname = compare Then
                    objorg.href = xmlnode(i).Attributes("href").Value
                End If
            Next

            orgRequest = Nothing
            orgResponse = Nothing
            receivestream = Nothing
            orgXML = Nothing
            xmlDoc = Nothing
            xmlnode = Nothing
            i = Nothing

            Return (objorg)

        End Function

        Public Function createOrg(objorg As Org)
            Dim urltoAdminOps As String
            urltoAdminOps = objorg.url
            objorg.adminOps = urltoAdminOps & "/api/admin/orgs"
            Dim httprequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.adminOps), HttpWebRequest)
            httprequest.Method = "POST"
            httprequest.Accept = apiVersion
            httprequest.Headers.Add("x-vcloud-authorization", objorg.authtoken)
            httprequest.ContentType = "application/vnd.vmware.admin.organization+xml; charset=ISO-8859-1"

            Dim xmldata As String
            xmldata = "<?xml version=""1.0"" encoding=""UTF-8""?><vcloud:AdminOrg
    xmlns:vcloud=""http://www.vmware.com/vcloud/v1.5""
    name=""" & objorg.companyshortname & """>
    <vcloud:FullName>" & objorg.companyfullname & "</vcloud:FullName>
    <vcloud:Settings>
        <vcloud:OrgGeneralSettings>
            <vcloud:CanPublishCatalogs>true</vcloud:CanPublishCatalogs>
            <vcloud:DeployedVMQuota>200</vcloud:DeployedVMQuota>
            <vcloud:StoredVmQuota>400</vcloud:StoredVmQuota>
            <vcloud:UseServerBootSequence>false</vcloud:UseServerBootSequence>
            <vcloud:DelayAfterPowerOnSeconds>1</vcloud:DelayAfterPowerOnSeconds>
        </vcloud:OrgGeneralSettings>
           </vcloud:Settings>
</vcloud:AdminOrg>"

            Dim enc As UTF8Encoding
            Dim postdata As Byte()
            enc = New System.Text.UTF8Encoding

            postdata = enc.GetBytes(xmldata)
            httprequest.ContentLength = postdata.Length
            Using stream = httprequest.GetRequestStream()
                stream.Write(postdata, 0, postdata.Length)
                stream.Close()
            End Using
            Dim response = httprequest.GetResponse()

            httprequest = Nothing
            postdata = Nothing
            enc = Nothing
            xmldata = Nothing

            Return (objorg)
        End Function
        Public Function enableOrg(objorg As Org)

            Dim enableRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.href.Replace("api/", "api/admin/") & "/action/enable"), HttpWebRequest)
            enableRequest.Method = "POST"
            enableRequest.Accept = apiVersion
            enableRequest.Headers.Add("x-vcloud-authorization", objorg.authtoken)
            Dim enableResponse As HttpWebResponse = CType(enableRequest.GetResponse(), HttpWebResponse)

            Dim receivestream As Stream = enableResponse.GetResponseStream
            Dim readstream As New StreamReader(receivestream, Encoding.UTF8)

            enableRequest = Nothing
            enableResponse = Nothing
            receivestream = Nothing
            readstream = Nothing

            Return (objorg)
        End Function
        Public Function createOrgVDC(objorg As Org)

            Dim httprequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.href & "/action/instantiate"), HttpWebRequest)
            httprequest.Method = "POST"
            httprequest.Accept = apiVersion
            httprequest.Headers.Add("x-vcloud-authorization", objorg.authtoken)
            httprequest.ContentType = "application/vnd.vmware.vcloud.instantiateVdcTemplateParams+xml"

            Dim xmldata As String
            xmldata = "<InstantiateVdcTemplateParams xmlns=""http://www.vmware.com/vcloud/v1.5"" name=""Dallas"">
    <Source href=""https://vcp1.liveitcloud.com/api/vdcTemplate/90354940-511a-428a-b442-69bc8a8e179e"" name=""Dallas-Primary"" type=""application/vnd.vmware.admin.vdcTemplate+xml""/>
    <Description>Dallas</Description>
</InstantiateVdcTemplateParams>"

            Dim enc As UTF8Encoding
            Dim postdata As Byte()
            enc = New System.Text.UTF8Encoding

            postdata = enc.GetBytes(xmldata)
            httprequest.ContentLength = postdata.Length
            Using stream = httprequest.GetRequestStream()
                stream.Write(postdata, 0, postdata.Length)
                stream.Close()
            End Using
            Dim response = httprequest.GetResponse()
            Dim receivestream As Stream = response.GetResponseStream
            Dim readstream As New StreamReader(receivestream, Encoding.UTF8)

            Dim orgXML As String = readstream.ReadToEnd()
            readstream.Close()
            readstream = Nothing
            receivestream.Close()
            receivestream = Nothing
            Dim xmlDoc As New System.Xml.XmlDocument()

            ' Let's get the task ID so we can pass this to check status
            xmlDoc.LoadXml(orgXML)

            Dim xmlnode As XmlNodeList

            'Dim i As Integer
            xmlnode = xmlDoc.GetElementsByTagName("Task")
            'D 'im compareVDC As String
            'Dim compare As String
            ' compareVDC = "application/vnd.vmware.vcloud.vdc+xml"

            objorg.taskhref = (xmlnode(0).Attributes("href").Value)


            '  For i = 0 To xmlnode.Count - 1
            ' compare = (xmlnode(i).Attributes("type").Value)
            'If compare = compareVDC Then
            'objorg.orgvdc = xmlnode(i).Attributes("href").Value
            'End If
            'Next
            Dim progress As Integer
            progress = 0
            'OpeningForm.barDatacenter.Value = 0

            Do While objorg.task <> "success"
                Call checkTaskStatus(objorg)
                'OpeningForm.BringToFront()
                'OpeningForm.barDatacenter.Value = progress
                progress = progress + 10
                Threading.Thread.Sleep(15000)
            Loop
            'OpeningForm.barDatacenter.Value = 100



            httprequest = Nothing
            postdata = Nothing
            enc = Nothing
            xmldata = Nothing




            Return (objorg)
        End Function
        Public Function getVDC(objorg As Org)
            'MsgBox("dude")
            Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.href), HttpWebRequest)
            orgRequest.Method = "GET"
            orgRequest.Accept = apiVersion

            orgRequest.Headers.Add("x-vcloud-authorization", objorg.authtoken)

            Dim orgResponse As HttpWebResponse = CType(orgRequest.GetResponse(), HttpWebResponse)
            Dim receivestream As Stream = orgResponse.GetResponseStream

            Dim readstream As New StreamReader(receivestream, Encoding.UTF8)

            Dim orgXML As String = readstream.ReadToEnd()
            readstream.Close()
            readstream = Nothing
            receivestream.Close()
            receivestream = Nothing
            Dim xmlDoc As New System.Xml.XmlDocument()

            xmlDoc.LoadXml(orgXML)

            Dim xmlnode As XmlNodeList

            Dim i As Integer
            xmlnode = xmlDoc.GetElementsByTagName("Link")
            Dim compareVDC As String
            Dim compare As String
            compareVDC = "application/vnd.vmware.vcloud.vdc+xml"
            For i = 0 To xmlnode.Count - 1
                compare = (xmlnode(i).Attributes("type").Value)
                If compare = compareVDC Then
                    objorg.orgvdc = xmlnode(i).Attributes("href").Value
                End If
            Next

            'Dim review As String
            'review = objorg.orgvdc

            'MsgBox(review)



            orgRequest = Nothing
            orgResponse = Nothing
            receivestream = Nothing
            orgXML = Nothing
            xmlDoc = Nothing
            xmlnode = Nothing
            i = Nothing

            Return (objorg)

        End Function
        Public Function getEdgeGateway(objorg As Org)
            'MsgBox("dude")

            'Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create("https://vcp1.liveitcloud.com/api/admin/vdc/c90b07ab-33aa-4c9a-ba6b-aac78238d517/edgeGateways"), HttpWebRequest)
            Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.orgvdc.Replace("api/", "api/admin/") & "/edgeGateways"), HttpWebRequest)
            orgRequest.Method = "GET"
            orgRequest.Accept = apiVersion

            orgRequest.Headers.Add("x-vcloud-authorization", objorg.authtoken)

            Dim orgResponse As HttpWebResponse = CType(orgRequest.GetResponse(), HttpWebResponse)
            Dim receivestream As Stream = orgResponse.GetResponseStream

            Dim readstream As New StreamReader(receivestream, Encoding.UTF8)

            Dim orgXML As String = readstream.ReadToEnd()
            readstream.Close()
            readstream = Nothing
            receivestream.Close()
            receivestream = Nothing
            Dim xmlDoc As New System.Xml.XmlDocument()

            xmlDoc.LoadXml(orgXML)

            Dim xmlnode As XmlNodeList

            Dim i As Integer
            xmlnode = xmlDoc.GetElementsByTagName("EdgeGatewayRecord")
            'Dim compareVDC As String
            'Dim compare As String
            'compareVDC = "application/vnd.vmware.vcloud.vdc+xml"
            'For i = 0 To xmlnode.Count - 1
            'compare = (xmlnode(i).Attributes("type").Value)
            'If compare = compareVDC Then
            objorg.orgEdgeGateway = xmlnode(i).Attributes("href").Value

            'Dim review As String
            'review = objorg.orgvdc

            'MsgBox(review)



            orgRequest = Nothing
            orgResponse = Nothing
            receivestream = Nothing
            orgXML = Nothing
            xmlDoc = Nothing
            xmlnode = Nothing
            i = Nothing

            Return (objorg)

        End Function
        Public Function convertEdgeGateway(objorg As Org)
            'MsgBox("dude")

            Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.orgEdgeGateway & "/action/convertToAdvancedGateway"), HttpWebRequest)

            orgRequest.Method = "POST"
            orgRequest.Accept = apiVersion

            orgRequest.Headers.Add("x-vcloud-authorization", objorg.authtoken)

            Dim orgResponse As HttpWebResponse = CType(orgRequest.GetResponse(), HttpWebResponse)
            Dim receivestream As Stream = orgResponse.GetResponseStream

            'Dim receivestream As Stream = response.GetResponseStream
            Dim readstream As New StreamReader(receivestream, Encoding.UTF8)

            Dim orgXML As String = readstream.ReadToEnd()
            readstream.Close()
            readstream = Nothing
            receivestream.Close()
            receivestream = Nothing
            Dim xmlDoc As New System.Xml.XmlDocument()

            ' Let's get the task ID so we can pass this to check status
            xmlDoc.LoadXml(orgXML)

            Dim xmlnode As XmlNodeList


            xmlnode = xmlDoc.GetElementsByTagName("Task")
            objorg.taskhref = (xmlnode(0).Attributes("status").Value)

            Dim progress As Integer
            progress = 0
            'OpeningForm.barFirewall.Value = 0
            Do While objorg.task <> "success"
                Call checkTaskStatus(objorg)
                Threading.Thread.Sleep(15000)
                'OpeningForm.barFirewall.Value = progress
                progress = progress + 10
            Loop
            'OpeningForm.barFirewall.Value = 100


            orgRequest = Nothing
            orgResponse = Nothing
            receivestream = Nothing


            Return (objorg)

        End Function
        Public Function createAdminUser(objorg As Org)

            'Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.orgEdgeGateway & "/action/convertToAdvancedGateway"), HttpWebRequest)
            'Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.href.Replace("api/", "api/admin/")), HttpWebRequest)
            Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.href.Replace("api/", "api/admin/") & "/users"), HttpWebRequest)

            orgRequest.Method = "POST"
            orgRequest.Accept = apiVersion

            orgRequest.Headers.Add("x-vcloud-authorization", objorg.authtoken)



            Dim xmldata As String
            xmldata = "<?xml version=""1.0"" encoding=""UTF-8""?><vcloud:User
                                                            xmlns:vcloud=""http://www.vmware.com/vcloud/v1.5""
                                                            name=""Admin""
                                                            operationKey=""operationKey"">
                  <vcloud:IsEnabled>true</vcloud:IsEnabled>
                  <vcloud:IsLocked>false</vcloud:IsLocked>
                  <vcloud:IsExternal>false</vcloud:IsExternal>
                  <vcloud:ProviderType>INTEGRATED</vcloud:ProviderType>
                  <vcloud:StoredVmQuota>400</vcloud:StoredVmQuota>
                  <vcloud:DeployedVmQuota>200</vcloud:DeployedVmQuota>
                  <vcloud:Role
                      href=""" & objorg.orgAdminRole & """
                      name=""Organization Administrator""
                      type=""application/vnd.vmware.admin.role+xml""/>
                  <vcloud:Password>" & objorg.adminpassword & "</vcloud:Password>
                  </vcloud:User>"

            Dim enc As UTF8Encoding
            Dim postdata As Byte()
            enc = New System.Text.UTF8Encoding

            postdata = enc.GetBytes(xmldata)
            orgRequest.ContentLength = postdata.Length
            Using stream = orgRequest.GetRequestStream()
                stream.Write(postdata, 0, postdata.Length)
                stream.Close()
            End Using
            Dim response = orgRequest.GetResponse()




            Dim orgResponse As HttpWebResponse = CType(orgRequest.GetResponse(), HttpWebResponse)
            Dim receivestream As Stream = orgResponse.GetResponseStream

            Dim readstream As New StreamReader(receivestream, Encoding.UTF8)

            Dim orgXML As String = readstream.ReadToEnd()
            readstream.Close()
            readstream = Nothing
            receivestream.Close()
            receivestream = Nothing
            Dim xmlDoc As New System.Xml.XmlDocument()

            ' Let's get the admin user href so we can pass this
            xmlDoc.LoadXml(orgXML)

            Dim xmlnode As XmlNodeList


            xmlnode = xmlDoc.GetElementsByTagName("User")
            objorg.adminUserHref = (xmlnode(0).Attributes("href").Value)


            orgRequest = Nothing
            postdata = Nothing
            enc = Nothing
            xmldata = Nothing
            orgRequest = Nothing
            orgResponse = Nothing
            receivestream = Nothing


            Return (objorg)

        End Function
        Public Function updateAdminUser(objorg As Org)


            Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.adminUserHref), HttpWebRequest)

            orgRequest.Method = "PUT"
            orgRequest.Accept = apiVersion

            orgRequest.Headers.Add("x-vcloud-authorization", objorg.authtoken)



            Dim xmldata As String
            xmldata = "<?xml version=""1.0"" encoding=""UTF-8""?><vcloud:User xmlns:vcloud=""http://www.vmware.com/vcloud/v1.5""
                                                            name=""Admin""
                                                            operationKey=""operationKey"">
                  
                  <vcloud:EmailAddress>" & objorg.emailaddress & "</vcloud:EmailAddress>
                  <vcloud:Role
                      href=""" & objorg.orgAdminRole & """
                      name=""Organization Administrator""
                      type=""application/vnd.vmware.admin.role+xml""/>
                  </vcloud:User>"

            Dim enc As UTF8Encoding
            Dim postdata As Byte()
            enc = New System.Text.UTF8Encoding

            postdata = enc.GetBytes(xmldata)
            orgRequest.ContentLength = postdata.Length
            Using stream = orgRequest.GetRequestStream()
                stream.Write(postdata, 0, postdata.Length)
                stream.Close()
            End Using
            Dim response = orgRequest.GetResponse()




            Dim orgResponse As HttpWebResponse = CType(orgRequest.GetResponse(), HttpWebResponse)
            Dim receivestream As Stream = orgResponse.GetResponseStream

            'orgRequest = Nothing
            'postdata = Nothing
            'enc = Nothing
            'xmldata = Nothing
            'orgRequest = Nothing
            'orgResponse = Nothing
            'receivestream = Nothing

            '------------------------------------
            '-----------------------------------------------------------------
            orgRequest = DirectCast(HttpWebRequest.Create(objorg.adminUserHref), HttpWebRequest)

            orgRequest.Method = "PUT"
            orgRequest.Accept = apiVersion

            orgRequest.Headers.Add("x-vcloud-authorization", objorg.authtoken)



            'Dim xmldata As String
            xmldata = "<?xml version=""1.0"" encoding=""UTF-8""?><vcloud:User xmlns:vcloud=""http://www.vmware.com/vcloud/v1.5""
                                                            name=""Admin""
                                                            operationKey=""operationKey"">
                  <vcloud:IsEnabled>true</vcloud:IsEnabled> 
                  <vcloud:Role
                      href=""" & objorg.orgAdminRole & """
                      name=""Organization Administrator""
                      type=""application/vnd.vmware.admin.role+xml""/>
                  </vcloud:User>"

            'Dim enc As UTF8Encoding
            'Dim postdata As Byte()
            enc = New System.Text.UTF8Encoding

            postdata = enc.GetBytes(xmldata)
            orgRequest.ContentLength = postdata.Length
            Using stream = orgRequest.GetRequestStream()
                stream.Write(postdata, 0, postdata.Length)
                stream.Close()
            End Using
            'Dim response = orgRequest.GetResponse()

            orgRequest = Nothing
            postdata = Nothing
            enc = Nothing
            xmldata = Nothing
            orgRequest = Nothing
            orgResponse = Nothing
            receivestream = Nothing


            'Dim orgResponse As HttpWebResponse = CType(orgRequest.GetResponse(), HttpWebResponse)
            'Dim receivestream As Stream = orgResponse.GetResponseStre


            Return (objorg)

        End Function
        Public Function createCatalog(objorg As Org)

            'Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.orgEdgeGateway & "/action/convertToAdvancedGateway"), HttpWebRequest)
            'Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.href.Replace("api/", "api/admin/")), HttpWebRequest)
            Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.href.Replace("api/", "api/admin/") & "/catalogs"), HttpWebRequest)

            orgRequest.Method = "POST"
            orgRequest.Accept = apiVersion

            orgRequest.Headers.Add("x-vcloud-authorization", objorg.authtoken)
            orgRequest.ContentType = "application/vnd.vmware.admin.catalog+xml; charset=ISO-8859-1"



            Dim xmldata As String
            xmldata = "<?xml version=""1.0"" encoding=""UTF-8""?><vcloud:AdminCatalog xmlns:vcloud = ""http://www.vmware.com/vcloud/v1.5"" name=""Catalog""><vcloud:Description>Catalog</vcloud:Description></vcloud:AdminCatalog>"

            Dim enc As UTF8Encoding
            Dim postdata As Byte()
            enc = New System.Text.UTF8Encoding

            postdata = enc.GetBytes(xmldata)
            orgRequest.ContentLength = postdata.Length
            Using stream = orgRequest.GetRequestStream()
                stream.Write(postdata, 0, postdata.Length)
                stream.Close()
            End Using
            Dim response = orgRequest.GetResponse()




            Dim orgResponse As HttpWebResponse = CType(orgRequest.GetResponse(), HttpWebResponse)
            Dim receivestream As Stream = orgResponse.GetResponseStream

            orgRequest = Nothing
            postdata = Nothing
            enc = Nothing
            xmldata = Nothing
            orgRequest = Nothing
            orgResponse = Nothing
            receivestream = Nothing




            orgRequest = Nothing
            postdata = Nothing
            enc = Nothing
            xmldata = Nothing
            orgRequest = Nothing
            orgResponse = Nothing
            receivestream = Nothing





            Return (objorg)

        End Function
        Public Function getAdminRoles(objorg As Org)

            Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.href.Replace("api/", "api/admin/")), HttpWebRequest)
            orgRequest.Method = "GET"
            orgRequest.Accept = apiVersion

            orgRequest.Headers.Add("x-vcloud-authorization", objorg.authtoken)

            Dim orgResponse As HttpWebResponse = CType(orgRequest.GetResponse(), HttpWebResponse)
            Dim receivestream As Stream = orgResponse.GetResponseStream

            Dim readstream As New StreamReader(receivestream, Encoding.UTF8)

            Dim orgXML As String = readstream.ReadToEnd()
            readstream.Close()
            readstream = Nothing
            receivestream.Close()
            receivestream = Nothing
            Dim xmlDoc As New System.Xml.XmlDocument()

            xmlDoc.LoadXml(orgXML)

            Dim xmlnode As XmlNodeList

            Dim i As Integer
            xmlnode = xmlDoc.GetElementsByTagName("RoleReference")
            Dim compareRole As String
            Dim compare As String
            compareRole = "Organization Administrator"
            For i = 0 To xmlnode.Count - 1
                compare = (xmlnode(i).Attributes("name").Value)
                If compareRole = compare Then
                    objorg.orgAdminRole = (xmlnode(i).Attributes("href").Value)
                End If

            Next



            orgRequest = Nothing
            orgResponse = Nothing
            receivestream = Nothing
            orgXML = Nothing
            xmlDoc = Nothing
            xmlnode = Nothing
            i = Nothing

            Return (objorg)

        End Function
        Public Function checkTaskStatus(objOrg As Org)


            Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objOrg.taskhref), HttpWebRequest)
            orgRequest.Method = "GET"
            orgRequest.Accept = apiVersion

            orgRequest.Headers.Add("x-vcloud-authorization", objOrg.authtoken)

            Dim orgResponse As HttpWebResponse = CType(orgRequest.GetResponse(), HttpWebResponse)
            Dim receivestream As Stream = orgResponse.GetResponseStream

            Dim readstream As New StreamReader(receivestream, Encoding.UTF8)

            Dim orgXML As String = readstream.ReadToEnd()
            readstream.Close()
            readstream = Nothing
            receivestream.Close()
            receivestream = Nothing
            Dim xmlDoc As New System.Xml.XmlDocument()

            xmlDoc.LoadXml(orgXML)

            Dim xmlnode As XmlNodeList

            Dim i As Integer
            xmlnode = xmlDoc.GetElementsByTagName("Task")
            Dim comparetask As String
            Dim compare As String
            comparetask = "success"

            For i = 0 To xmlnode.Count - 1
                compare = (xmlnode(i).Attributes("status").Value)
                If comparetask = compare Then
                    'msgBox("shits Done!")
                    'TaskStatus = "complete"
                    objOrg.task = "success"
                Else
                    objOrg.task = (xmlnode(i).Attributes("status").Value)
                End If

            Next




            orgRequest = Nothing
            orgResponse = Nothing
            receivestream = Nothing
            orgXML = Nothing
            xmlDoc = Nothing
            xmlnode = Nothing
            i = Nothing

            Return (objOrg)

        End Function
        Public Function AuthorizeCard(cardinfo As Org)
            Dim authapilogin As String = "76xfD5Gua"
            Dim transactionkey As String = "5h4LwUEb8nY88a76"
            Dim URL As String = "https://apitest.authorize.net/xml/v1/request.api"
            If cardinfo.production Then
                authapilogin = "76xfD5Gua"
                transactionkey = "5h4LwUEb8nY88a76"
                URL = "https://apitest.authorize.net/xml/v1/request.api"
            End If
            Dim expirydate As String
            expirydate = cardinfo.ccexpyear4digit & "-" & cardinfo.ccexpmonth


            Dim authXML As String

            authXML = "<createTransactionRequest xmlns=""AnetApi/xml/v1/schema/AnetApiSchema.xsd"">
                                      <merchantAuthentication>
                                          <name>" & authapilogin & "</name>
                                          <transactionKey>" & transactionkey & "</transactionKey>
                                      </merchantAuthentication>
                                      <refId>" & cardinfo.refid & "</refId>
                                      <transactionRequest>
                                          <transactionType>authOnlyTransaction</transactionType>
                                          <amount>1</amount>
                                          <payment>
                                              <creditCard>
                                                  <cardNumber>" & cardinfo.ccnumber & "</cardNumber>
                                                  <expirationDate>" & expirydate & "</expirationDate>
                                                  <cardCode>" & cardinfo.cccvc & "</cardCode>
                                              </creditCard>
                                          </payment>
                                          <billTo>
                                              <firstName>" & cardinfo.firstname & "</firstName>
                                              <lastName>" & cardinfo.lastname & "</lastName>
                                              <company>" & cardinfo.company & "</company>
                                              <address>" & cardinfo.address1 & "</address>
                                              <city>" & cardinfo.city & "</city>
                                              <state>" & cardinfo.state & "</state>
                                              <zip>" & cardinfo.zip & "</zip>
                                              <country>" & cardinfo.country & "</country>
                                          </billTo>
                                          <userFields>
                                              <userField>
                                                  <name>vcloudshortname</name>
                                                  <value>vcloudlongname</value>
                                              </userField>
                                          </userFields>
                                      </transactionRequest>
                                  </createTransactionRequest>"



            Dim httprequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(URL), HttpWebRequest)
            httprequest.Method = "POST"
            Dim enc As UTF8Encoding
            enc = New System.Text.UTF8Encoding
            Dim postdata As Byte()
            postdata = enc.GetBytes(authXML)
            httprequest.ContentLength = postdata.Length
            Using stream = httprequest.GetRequestStream()
                stream.Write(postdata, 0, postdata.Length)
                stream.Close()
            End Using
            Dim response = httprequest.GetResponse()
            Dim receivestream As Stream = response.GetResponseStream
            Dim readstream As New StreamReader(receivestream, Encoding.UTF8)

            Dim responsexml As String = readstream.ReadToEnd()
            Dim xmlDoc As New System.Xml.XmlDocument()
            xmlDoc.LoadXml(responsexml)
            Dim xmlnode As XmlNodeList
            xmlnode = xmlDoc.GetElementsByTagName("resultCode")
            If (xmlnode(0).InnerText) = "Ok" Then
                xmlnode = xmlDoc.GetElementsByTagName("responseCode")
                If (xmlnode(0).InnerText) = "1" Then
                    cardinfo.authorized = True
                Else
                    cardinfo.authorized = False
                End If
            Else
                cardinfo.authorized = False
            End If
            'Call createCustProfile(cardinfo)

            Return (cardinfo)
        End Function

        Function createCustProfile(objcc As Org)
            Dim authapilogin As String = "76xfD5Gua"
            Dim transactionkey As String = "5h4LwUEb8nY88a76"
            Dim URL As String = "https://apitest.authorize.net/xml/v1/request.api"
            If objcc.production Then
                authapilogin = "76xfD5Gua"
                transactionkey = "5h4LwUEb8nY88a76"
                URL = "https://apitest.authorize.net/xml/v1/request.api"
            End If
            Dim expirydate As String
            expirydate = objcc.ccexpyear4digit & "-" & objcc.ccexpmonth


            Dim authXML As String

            authXML = "<createCustomerProfileRequest xmlns=""AnetApi/xml/v1/schema/AnetApiSchema.xsd"">
                                      <merchantAuthentication>
                                          <name>" & authapilogin & "</name>
                                          <transactionKey>" & transactionkey & "</transactionKey>
                                      </merchantAuthentication>
                                       <profile>
                                          <merchantCustomerId>" & objcc.firstname & " " & objcc.lastname & "</merchantCustomerId>
                                          <description></description>
                                          <email>" & objcc.emailaddress & "</email>
                                          <paymentProfiles>
                                            <customerType>individual</customerType>
                                          <payment>
                                          <creditCard>
                                          <cardNumber>" & objcc.ccnumber & "</cardNumber>
                                          <expirationDate>" & expirydate & "</expirationDate>
                                          <cardCode>" & objcc.cccvc & "</cardCode>
                                          </creditCard>
                                          </payment>
                                          </paymentProfiles>
                                          </profile>
                                          <validationMode>testMode</validationMode>
                                          </createCustomerProfileRequest>"





            Dim httprequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(URL), HttpWebRequest)
            httprequest.Method = "POST"
            Dim enc As UTF8Encoding
            enc = New System.Text.UTF8Encoding
            Dim postdata As Byte()
            postdata = enc.GetBytes(authXML)
            httprequest.ContentLength = postdata.Length
            Using stream = httprequest.GetRequestStream()
                stream.Write(postdata, 0, postdata.Length)
                stream.Close()
            End Using
            Dim response = httprequest.GetResponse()
            Dim receivestream As Stream = response.GetResponseStream
            Dim readstream As New StreamReader(receivestream, Encoding.UTF8)

            Dim responsexml As String = readstream.ReadToEnd()
            Dim xmlDoc As New System.Xml.XmlDocument()
            xmlDoc.LoadXml(responsexml)
            Dim xmlnode As XmlNodeList
            xmlnode = xmlDoc.GetElementsByTagName("resultCode")
            If (xmlnode(0).InnerText) = "Ok" Then
                xmlnode = xmlDoc.GetElementsByTagName("customerProfileId")
                objcc.customerProfileID = xmlnode(0).InnerText
                xmlnode = xmlDoc.GetElementsByTagName("numericString")
                objcc.customerProfilePaymentID = xmlnode(0).InnerText
                objcc.authorized = True
            Else
                objcc.authorized = False

            End If


            Return (objcc)
        End Function
        Public Function insertnew(objorg As Org)

            Dim conn As SqlConnection = New SqlConnection(My.Settings.DatabaseStore)
            Dim sql As String = "insert into [orginfo] ([vcloudorgshort], [vcloudorglong], [email], [endusername], [vcloudorghref], [vcloudadminops], [edgegateway], [adminrole], [adminuserhref], [authCustProfileID], [authCustoPaymentID]) values (@vcloudorgshort, @vcloudorglong, @email, @endusername, @vcloudorghref, @vcloudadminops, @edgegateway, @adminrole, @adminuserhref, @authCustProfileID, @authCustoPaymentID)"
            Dim cmd As SqlCommand = New SqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@vcloudorgshort", objorg.companyshortname)
            cmd.Parameters.AddWithValue("@vcloudorglong", objorg.companyfullname)
            cmd.Parameters.AddWithValue("@email", objorg.emailaddress)
            cmd.Parameters.AddWithValue("@endusername", objorg.adminname)
            cmd.Parameters.AddWithValue("@vcloudorghref", objorg.href)
            cmd.Parameters.AddWithValue("@vcloudadminops", objorg.adminOps)
            cmd.Parameters.AddWithValue("@edgegateway", objorg.orgEdgeGateway)
            cmd.Parameters.AddWithValue("@adminrole", objorg.orgAdminRole)
            cmd.Parameters.AddWithValue("@adminuserhref", objorg.adminUserHref)
            cmd.Parameters.AddWithValue("@authCustProfileID", objorg.customerProfileID)
            cmd.Parameters.AddWithValue("@authCustoPaymentID", objorg.customerProfilePaymentID)


            conn.Open()
            cmd.ExecuteNonQuery()
            conn.Close()

            Return (objorg)
        End Function

        Public Class Org
            Public url As String
            Public adminOps As String
            Public companyshortname As String
            Public id As String
            Public orgvdc As String
            Public orgtype As String
            Public authstring As String
            Public companyfullname As String
            Public emailaddress As String
            Public adminpassword As String
            Public adminname As String
            Public authtoken As String
            Public href As String
            Public verified As Boolean = True
            Public orgEdgeGateway As String
            Public orgAdminRole As String
            Public task As String
            Public taskhref As String
            Public alreadyexists As Boolean
            Public adminUserHref As String
            Public customerProfileID As String
            Public customerProfilePaymentID As String
            Public ccnumber As String
            Public ccexpmonth As String
            Public ccexpyear4digit As String
            Public cccvc As String
            Public firstname As String
            Public lastname As String
            Public zip As String
            Public address1 As String
            Public address2 As String
            Public state As String
            Public country As String = "USA"
            Public vcloudShortName As String
            Public vcloudLongName As String
            Public production As Boolean = False
            Public refid As String
            Public company As String = ""
            Public city As String
            Public authorized As Boolean


        End Class
    End Class


End Namespace

