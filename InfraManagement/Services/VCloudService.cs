using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Xml;
using InfraManagement.Models;
using static System.Diagnostics.Trace;

namespace InfraManagement.Services
{
    public class VCloudService : ICloudService
    {
        private string EndPoint { get; set; }
        private string UserName { get; set; }
        private string Password { get; set; }
        private string Version { get; set; }

        private string _currentToken = "";

        public VCloudService(string endPoint, string apiVersion, string userName, string password)
        {
            this.EndPoint = endPoint;
            this.Version = apiVersion;
            this.UserName = userName;
            this.Password = password;
        }

        public string Authentiate()
        {
            try
            {
                if (!String.IsNullOrEmpty(_currentToken))
                {
                    return _currentToken;
                }

                string result = "";
                HttpWebRequest authRequest = (HttpWebRequest)HttpWebRequest.Create(EndPoint + "/api/sessions");
                authRequest.Method = "POST";
                authRequest.Accept = Version;

                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(UserName + "@System:" + Password));
                authRequest.Headers.Add("Authorization:" + "Basic " + credentials);
                // So we setup the request, now we need to execute the task

                HttpWebResponse authresponse = (HttpWebResponse)authRequest.GetResponse();

                Stream receivestream = authresponse.GetResponseStream();

                StreamReader readstream = new StreamReader(receivestream, Encoding.UTF8);

                result = authresponse.GetResponseHeader("x-vcloud-authorization");
                _currentToken = result;
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public string CreateOrg(Org newOrg)
        {
            string result = "";
            try
            {

                if (!IsOrgNameAvailable(newOrg.CompanyShortName))
                {
                    return result;
                }

                string xmldata;
                xmldata = String.Format(@"<?xml version='1.0' encoding='UTF-8'?>
                            <vcloud:AdminOrg
                            xmlns:vcloud='http://www.vmware.com/vcloud/v1.5'
                            name='{0}'>
                            <vcloud:FullName>{1}</vcloud:FullName>
                            <vcloud:Settings>
                                <vcloud:OrgGeneralSettings>
                                    <vcloud:CanPublishCatalogs>true</vcloud:CanPublishCatalogs>
                                    <vcloud:DeployedVMQuota>200</vcloud:DeployedVMQuota>
                                    <vcloud:StoredVmQuota>400</vcloud:StoredVmQuota>
                                    <vcloud:UseServerBootSequence>false</vcloud:UseServerBootSequence>
                                    <vcloud:DelayAfterPowerOnSeconds>1</vcloud:DelayAfterPowerOnSeconds>
                                </vcloud:OrgGeneralSettings>
                                   </vcloud:Settings>
                        </vcloud:AdminOrg>", newOrg.CompanyShortName, newOrg.CompanyFullName);

                InvokeApi(EndPoint + "/api/admin/orgs", xmldata, HttpMethod.Post, 
                    new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Content-type", "application/vnd.vmware.admin.organization+xml; charset=ISO-8859-1" )});
                              
                result = GetOrgHref(newOrg.CompanyShortName);
                EnableOrg(result);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public string CreateAdminUser(string orgHref,string emailAddress)
        {

            // Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.orgEdgeGateway & "/action/convertToAdvancedGateway"), HttpWebRequest)
            // Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.href.Replace("api/", "api/admin/")), HttpWebRequest)

            var result = "";

            var adminRole = GetAdminRoles(orgHref);

            string xmldata;
            xmldata = String.Format( @"<?xml version='1.0' encoding='UTF-8'?><vcloud:User
                                                            xmlns:vcloud='http://www.vmware.com/vcloud/v1.5'
                                                            name='Admin'
                                                            operationKey='operationKey'>
                  <vcloud:IsEnabled>true</vcloud:IsEnabled>
                  <vcloud:IsLocked>false</vcloud:IsLocked>
                  <vcloud:IsExternal>false</vcloud:IsExternal>
                  <vcloud:ProviderType>INTEGRATED</vcloud:ProviderType>
                  <vcloud:StoredVmQuota>400</vcloud:StoredVmQuota>
                  <vcloud:DeployedVmQuota>200</vcloud:DeployedVmQuota>
                  <vcloud:Role
                      href='{0}'
                      name='Organization Administrator'
                      type=""application/vnd.vmware.admin.role+xml""/>
                  <vcloud:Password>{1}</vcloud:Password>
                  </vcloud:User>",adminRole,this.Password);

            //This creates a admin user
            var xmlDoc = InvokeApi(orgHref.Replace("api/", "api/admin/") + "/users", xmldata, HttpMethod.Post, null);
                       
            XmlNodeList xmlnode;
            
            xmlnode = xmlDoc.GetElementsByTagName("User");
            if (xmlnode != null && xmlnode.Count > 0)
            {
                //This will have admin url
                result = (xmlnode[0].Attributes["href"].Value);
            }

            //update the admin password

            xmldata = String.Format(@"<?xml version='1.0' encoding='UTF-8'?>
                                      <vcloud:User xmlns:vcloud='http://www.vmware.com/vcloud/v1.5'
                                        name = 'Admin' operationKey = 'operationKey' >
                                         <vcloud:EmailAddress >{0}</ vcloud:EmailAddress >
                                         <vcloud:Role href ='{1}' name='Organization Administrator' type='application/vnd.vmware.admin.role + xml' />
                                      </ vcloud:User > ", emailAddress, adminRole);
            InvokeApi(result, xmldata, HttpMethod.Put, null);
            return result;
        }

        public bool IsOrgNameAvailable(string orgName)
        {
            bool result = false;
            try
            {
                HttpWebRequest orgRequest = (HttpWebRequest)HttpWebRequest.Create(EndPoint + "/api/org");

                var xmlDoc = InvokeApi(EndPoint + "/api/org", "", HttpMethod.Get, null);

                var xmlnode = xmlDoc.SelectSingleNode("//Org[@name='" + orgName + "']");
                result = (xmlnode.Attributes["name"].Value == orgName);

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string CreatedVDC(string orgHref)
        {
            string result = "";

            try
            {

                var xmldata = @"<InstantiateVdcTemplateParams xmlns='http://www.vmware.com/vcloud/v1.5' name='Dallas'>
                              <Source href='https://vcp1.liveitcloud.com/api/vdcTemplate/90354940-511a-428a-b442-69bc8a8e179e' name='Dallas-Primary' type='application/vnd.vmware.admin.vdcTemplate+xml'/>
                              <Description>Dallas</Description>
                            </InstantiateVdcTemplateParams>";

                var xmlDoc = InvokeApi(orgHref + "/action/instantiate",xmldata,HttpMethod.Post,null);

                result = xmlDoc.SelectSingleNode("/Task")?.Attributes["href"]?.Value;

            }
            catch (Exception ex)
            {

                throw;
            }

            return result;
        }

        public string GetVDC(string orgHref)
        {
            string result = "";

            try
            {
                var xmlDoc = InvokeApi(orgHref, "", HttpMethod.Get, null);
                result = xmlDoc.SelectSingleNode("//Task[@type='application/vnd.vmware.vcloud.vdc+xml']")?.Attributes["href"]?.Value;
            }
            catch (Exception ex)
            {

                throw;
            }

            return result;
        }

        public string CreateCatalog(string orgHref)
        {
            var result = "";

            try
            {
                var xmldata = @"<?xml version='1.0' encoding='UTF-8'?>
                                <vcloud:AdminCatalog xmlns:vcloud='http://www.vmware.com/vcloud/v1.5' name='Catalog'>
                                    <vcloud:Description>Catalog</vcloud:Description>
                                </vcloud:AdminCatalog>";

                var xmlDoc = InvokeApi(orgHref + "/admin/catalogs", xmldata, HttpMethod.Post, new List<KeyValuePair<string, string>> {
                                                                                                new KeyValuePair<string, string>("Content-type","application/vnd.vmware.admin.catalog+xml; charset=ISO-8859-1")
                                                                                                });

            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public string UpdateEdgeGateWayToAdvanced(string edgeGatewayEndPoint)
        {
            string result = "";
            try
            {
                var xmlDoc=InvokeApi(edgeGatewayEndPoint + "/action/convertToAdvancedGateway", "", HttpMethod.Post);
                result = xmlDoc.SelectSingleNode("//Task")?.Attributes["href"]?.Value;
            }
            catch (Exception ex)
            {
                Write(ex, "Error");
    
            }

            return result;
        }
         
        public string GetTaskStatus(string taskEndPoint)
        {
            var result = "";
            try
            {
                var xmlDoc = InvokeApi(taskEndPoint, "", HttpMethod.Get, null);
                result = xmlDoc?.SelectSingleNode("//Task")?.Attributes["status"]?.Value;
            }
            catch (Exception ex)
            {

                throw;
            }

            return result;
        }

        private string GetEdgeGateway(string orgVDCEndPoint)
        {
            var result = "";

            try
            {
                var xmlDoc = InvokeApi(orgVDCEndPoint + "/admin/edgeGateways", "", HttpMethod.Get);
                result = xmlDoc?.SelectSingleNode("//EdgeGatewayRecord").Attributes["href"].Value;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.Write(ex, "ERROR");
                throw;
            }

            return result;
        }
        private string GetAdminRoles(string orgUrl)
        {
            string result = "";
            //HttpWebRequest orgRequest = (HttpWebRequest)HttpWebRequest.Create(objorg.href.Replace("api/", "api/admin/"));

            var xmlDoc = InvokeApi(orgUrl + "/admin/", "", HttpMethod.Get, null);
            int i;
            
            var xmlnode = xmlDoc.SelectSingleNode("//RoleReference[@name='Organization Administrator']");
            result = xmlnode.Attributes["href"].Value;

            return result;

        }

        private string GetOrgHref(string orgName)
        {
            string result = "";
            var xmlDoc = InvokeApi(EndPoint + "/api/org", "", HttpMethod.Get, null);

            XmlNodeList xmlnode;
                     
            xmlnode = xmlDoc.GetElementsByTagName("Org");
            var node = xmlDoc.SelectSingleNode("//Org[@name='" + orgName + "']");

            result = node.Attributes["href"].Value;

           return result;

        }

        private void EnableOrg(string orgHref)
        {
            
            InvokeApi(orgHref.Replace("api/", "api/admin/") + "/action/enable", "", HttpMethod.Post, null);
                       
        }

        private XmlDocument InvokeApi(string url, string xmlWorkload,HttpMethod method, List<KeyValuePair<string,string>> requestHeaders=null)
        {
            var result = new XmlDocument();
            // POST /rest/issue/{issue}/timetracking/workitem
            using (var httpClient = new HttpClient())
            {

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = method,
                };

                request.Headers.Add("x-vcloud-authorization", Authentiate());
                request.Headers.Add("Accept", Version);
                if (requestHeaders != null)
                {
                    foreach (var header in requestHeaders)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }

                }

                request.Content = new StringContent(xmlWorkload, Encoding.UTF8, "application/xml");
                var response = httpClient.SendAsync(request).Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
                else
                {
                    result.LoadXml(response.Content.ReadAsStringAsync().Result);
                }
            }
            return result;

        }
    }
}