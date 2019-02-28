using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using InfraManagement.Models;
using static System.Diagnostics.Trace;
using static InfraManagement.Services.HttpHelper;

namespace InfraManagement.Services
{
    public class VCloudService : ICloudService
    {
        private string EndPoint { get; set; }
        private string UserName { get; set; }
        private string Password { get; set; }
        private string Version { get; set; }
        private string  VdcTemplateId { get; set; }

        private string _currentToken = "";

        public VCloudService(string endPoint, string apiVersion,string vdcTemplateId, string userName, string password)
        {
            this.EndPoint = endPoint;
            this.Version = apiVersion;
            this.UserName = userName;
            this.Password = password;
            this.VdcTemplateId = vdcTemplateId;
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
                //HttpWebRequest authRequest = (HttpWebRequest)HttpWebRequest.Create(EndPoint + "/api/sessions");
                //authRequest.Method = "POST";
                //authRequest.Accept = Version;

                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(UserName + "@System:" + Password));
                //authRequest.Headers.Add("Authorization:" + "Basic " + credentials);
                //// So we setup the request, now we need to execute the task

                //HttpWebResponse authresponse = (HttpWebResponse)authRequest.GetResponse();

                //Stream receivestream = authresponse.GetResponseStream();

                //StreamReader readstream = new StreamReader(receivestream, Encoding.UTF8);

                //result = authresponse.GetResponseHeader("x-vcloud-authorization");
                _currentToken = result;

                InvokeApi(EndPoint + "/api/sessions", "", HttpMethod.Post,r=> {
                    r.Headers.Clear(); //Make sure tere is nothing in the header
                    r.Headers.Authorization =  new System.Net.Http.Headers.AuthenticationHeaderValue( "Basic",credentials);

                },
                headers => {
                    var authHeader = headers.GetValues("x-vcloud-authorization")?.ToList();
                    if (authHeader != null && authHeader.Count > 0)
                    {
                        result = authHeader[0]; 
                    }
                }
                );
                return result;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public string CreateOrg(OrgInfo newOrg)
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

               var xmlDoc= InvokeApi(EndPoint + "/api/admin/orgs", xmldata, HttpMethod.Post, r => { AddHeaders(r, "application/vnd.vmware.admin.organization+xml"); });

                var links = xmlDoc.GetElementsByTagName("Link");

                foreach (XmlNode link in links)
                {
                    if (link.Attributes["type"]?.Value == "application/vnd.vmware.vcloud.org+xml")
                    {
                        result = link.Attributes["href"].Value;
                        break;
                    }
                }
                //result = GetOrgHref(newOrg.CompanyShortName);
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
            xmldata = String.Format( @"<?xml version='1.0' encoding='UTF-8'?>
                                      <vcloud:User  xmlns:vcloud='http://www.vmware.com/vcloud/v1.5'  name='Admin' >
                                      <vcloud:IsEnabled>true</vcloud:IsEnabled>
                                      <vcloud:IsLocked>false</vcloud:IsLocked>
                                      <vcloud:IsExternal>false</vcloud:IsExternal>
                                      <vcloud:ProviderType>INTEGRATED</vcloud:ProviderType>
                                      <vcloud:StoredVmQuota>400</vcloud:StoredVmQuota>
                                      <vcloud:DeployedVmQuota>200</vcloud:DeployedVmQuota>
                                      <vcloud:Role
                                          href='{0}'
                                          name='Organization Administrator'
                                          type='application/vnd.vmware.admin.role+xml'/>
                                      <vcloud:Password>{1}</vcloud:Password>
                                      </vcloud:User>",adminRole,this.Password);

            //This creates a admin user
            var xmlDoc = InvokeApi(EndPoint + "/api/admin/org/" + GetOrgId(orgHref) + "/users", xmldata, HttpMethod.Post, r => { AddHeaders(r, "application/vnd.vmware.admin.user+xml"); });//application/vnd.vmware.admin.user+xml

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
                                      </vcloud:User> ", emailAddress, adminRole);
            InvokeApi(result, xmldata, HttpMethod.Put, null);
            return result;
        }

        public bool IsOrgNameAvailable(string orgName)
        {
            bool result = true;
            try
            {
                HttpWebRequest orgRequest = (HttpWebRequest)HttpWebRequest.Create(EndPoint + "/api/org");

                var xmlDoc = InvokeApi(EndPoint + "/api/org", "", HttpMethod.Get, null);

                var xmlnode = xmlDoc.GetElementsByTagName("Org");
                if (xmlnode != null)
                {
                    foreach (XmlElement item in xmlnode)
                    {
                        if (item.Attributes["name"]?.Value == orgName)
                        {
                            result = false;
                            break;
                        }
                    }
                }

               
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string CreateVDC(string orgHref)
        {
            string result = "";

            try
            {

                var xmldata = String.Format( @"<InstantiateVdcTemplateParams xmlns='http://www.vmware.com/vcloud/v1.5' name='Dallas'>
                              <Source href='{0}/api/vdcTemplate/{1}' name='Dallas-Primary' type='application/vnd.vmware.admin.vdcTemplate+xml'/>
                              <Description>Dallas</Description>
                            </InstantiateVdcTemplateParams>",EndPoint,VdcTemplateId);

                var xmlDoc =  InvokeApi(EndPoint + "/api/org/" + GetOrgId(orgHref) + "/action/instantiate",xmldata,HttpMethod.Post, r => { AddHeaders(r, "application/vnd.vmware.vcloud.instantiateVdcTemplateParams+xml"); });

                result = xmlDoc.SelectSingleNode("/Task")?.Attributes["href"]?.Value;

                return result;
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

                var xmlDoc = InvokeApi(EndPoint + "/api/admin/org/" + GetOrgId(orgHref) + "/catalogs", xmldata, HttpMethod.Post,r => { AddHeaders(r, "application/vnd.vmware.admin.catalog+xml"); });
                                                                                                

            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public async Task<string> UpdateEdgeGateWayToAdvanced(string orgHref)
        {
            string result = "";
            try
            {
                string orgVDCEndPoint = GetVDC(orgHref);
                string edgeGatewayEndPoint = GetEdgeGateway(orgVDCEndPoint);
                var xmlDoc=InvokeApi(edgeGatewayEndPoint + "/action/convertToAdvancedGateway", "", HttpMethod.Post);
                result = xmlDoc.SelectSingleNode("//Task")?.Attributes["href"]?.Value;
            }
            catch (Exception ex)
            {
                Write(ex, "Error");
    
            }

            return await Task.FromResult(result);
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

            var xmlDoc = InvokeApi(EndPoint + "/api/admin/org/" + GetOrgId(orgUrl) + "/", "", HttpMethod.Get, null);
            
            
            var xmlnode = xmlDoc.GetElementsByTagName("RoleReference");

            foreach (XmlNode item in xmlnode)
            {
                if (item.Attributes["name"].Value == "Organization Administrator")
                {
                    result = item.Attributes["href"].Value;
                    break;
                }
            }

            
            return result;

        }

        private string GetOrgHref(string orgName)
        {
            string result = "";
            var xmlDoc = InvokeApi(EndPoint + "/api/org", "", HttpMethod.Get, null);

            XmlNodeList xmlnode;
            
            xmlnode = xmlDoc.GetElementsByTagName("Org");
            if (xmlnode != null)
            {
                foreach (XmlElement item in xmlnode)
                {
                    if (item.Attributes["name"]?.Value == orgName)
                    {
                        result = item.Attributes["href"].Value;
                        break;
                    }
                }
            }
           
           return result;

        }

        private void EnableOrg(string orgHref)
        {
            
            InvokeApi(  EndPoint + "/api/admin/org/" + GetOrgId(orgHref) + "/action/enable", "", HttpMethod.Post, null);
                       
        }

        

        private string GetOrgId(string orgHref)
        {
           int lastIndex = orgHref.LastIndexOf('/');
            var resutl = orgHref.Substring(lastIndex + 1);
            return resutl;
        }

        private void AddHeaders(HttpRequestMessage request,string contentType)
        {
            request.Headers.Add("x-vcloud-authorization", Authentiate());
            request.Headers.Add("Accept", Version);
            if (!String.IsNullOrEmpty(contentType))
            {
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            }
        }
    }

    //Types for xml parsing
    public class OrgList
    {
        [XmlElement("Org")]
        [Newtonsoft.Json.JsonProperty("Org") ]
        public List<Org> Orgs { get; set; }
    }
    public class Org
    {
        public string href { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }
}