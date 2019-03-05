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
//using static InfraManagement.Services.HttpHelper;

namespace InfraManagement.Services
{
    /// <summary>
    /// This is the implementation for Vcloud directoe apis
    /// </summary>
    public class VCloudService : ICloudService
    {
        private string EndPoint { get; set; }
        private string UserName { get; set; }
        private string Password { get; set; }
        private string Version { get; set; }
        private string VdcTemplateId { get; set; }
        private string VdcTemplateName { get; set; }

        private string _currentToken = "";

        public VCloudService(string endPoint, string apiVersion, string vdcTemplateId, string vdcTemplateName, string userName, string password)
        {
            this.EndPoint = endPoint;
            this.Version = apiVersion;
            this.UserName = userName;
            this.Password = password;
            this.VdcTemplateId = vdcTemplateId;
            this.VdcTemplateName = vdcTemplateName;
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

                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(UserName + "@System:" + Password));

                _currentToken = result;

                HttpHelper.InvokeApi(EndPoint + "/api/sessions", "", HttpMethod.Post,
                //This anonymous method servers as presend parameter of InvokeApi, use this to add the auth headers
                r =>
                {
                    r.Headers.Clear(); //Make sure there is nothing in the header
                    r.Headers.Add("Accept", Version);
                    r.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                },
                //This anonymous method servers as postsend parameter of InvokeApi, use this to read the response headers
                 headers =>
                 {
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

        public bool IsAdminUserAvaialbe(OrgInfo newOrg)
        {
            return IsAdminUserAvaialbe(newOrg);
        }
        public bool IsAdminUserAvaialbe(string adminUsername)
        {
            var result = true;
            try
            {
                var users = InvokeProtectedApi(EndPoint + "/api/admin/users/query", "", HttpMethod.Get);
                var existingUsers = users.GetElementsByTagName("UserRecord");
                foreach (XmlElement user in existingUsers)
                {
                    if (user.Attributes["name"].Value.ToLower() == adminUsername)
                    {
                        result = false;
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                result = true;
                throw;
            }
            return result;
        }

        public string CreateOrg(OrgInfo newOrg)
        {

            string result = "";
            try
            {
                if (!IsOrgNameAvailable(newOrg.CompanyShortName))
                {
                    throw new ApplicationException($"{newOrg.CompanyShortName} is already in uase.");
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

                var xmlDoc = InvokeProtectedApi(EndPoint + "/api/admin/orgs", xmldata, HttpMethod.Post, "application/vnd.vmware.admin.organization+xml");

                var links = xmlDoc.GetElementsByTagName("Link");

                foreach (XmlNode link in links)
                {
                    if (link.Attributes["type"]?.Value == "application/vnd.vmware.vcloud.org+xml")
                    {
                        result = link.Attributes["href"].Value;
                        result = GetOrgId(result);
                        EnableOrg(result);
                        return result;
                    }
                }
                //result = GetOrgHref(newOrg.CompanyShortName);

                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public string CreateAdminUser(string cloudTenantId, string emailAddress, string userName, string password)
        {

            // Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.orgEdgeGateway & "/action/convertToAdvancedGateway"), HttpWebRequest)
            // Dim orgRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(objorg.href.Replace("api/", "api/admin/")), HttpWebRequest)

            var result = "";


            var adminRole = GetAdminRoles(cloudTenantId);




            string xmldata;
            xmldata = String.Format(@"<?xml version='1.0' encoding='UTF-8'?>
                                      <vcloud:User xmlns:vcloud='http://www.vmware.com/vcloud/v1.5' name='{0}' >
                                      <vcloud:IsEnabled>true</vcloud:IsEnabled>
                                      <vcloud:IsLocked>false</vcloud:IsLocked>
                                      <vcloud:IsExternal>false</vcloud:IsExternal>
                                      <vcloud:ProviderType>INTEGRATED</vcloud:ProviderType>
                                      <vcloud:StoredVmQuota>400</vcloud:StoredVmQuota>
                                      <vcloud:DeployedVmQuota>200</vcloud:DeployedVmQuota>
                                      <vcloud:Role
                                          href='{1}'
                                          name='Organization Administrator'
                                          type='application/vnd.vmware.admin.role+xml'/>
                                      <vcloud:Password>{2}</vcloud:Password>
                                      </vcloud:User>", userName, adminRole, password);

            //This creates a admin user
            var xmlDoc = InvokeProtectedApi(EndPoint + "/api/admin/org/" + cloudTenantId + "/users", xmldata, HttpMethod.Post, "application/vnd.vmware.admin.user+xml");//application/vnd.vmware.admin.user+xml

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
                                        name='{0}' operationKey='operationKey' >
                                         <vcloud:EmailAddress>{1}</vcloud:EmailAddress>
                                         <vcloud:Role href='{2}' name='Organization Administrator' type='application/vnd.vmware.admin.role + xml' />
                                      </vcloud:User>", userName, emailAddress, adminRole);
            InvokeProtectedApi(result, xmldata, HttpMethod.Put, "application/vnd.vmware.admin.user+xml");
            return result;
        }

        public bool IsOrgNameAvailable(string orgName)
        {
            bool result = true;
            try
            {

                var xmlDoc = InvokeProtectedApi(EndPoint + "/api/org", "", HttpMethod.Get);

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

        public string CreateVDC(string cloudTenantId)
        {
            string result = "";

            try
            {

                var xmldata = String.Format(@"<InstantiateVdcTemplateParams xmlns='http://www.vmware.com/vcloud/v1.5' name='Dallas'>
                              <Source href='{0}/api/vdcTemplate/{1}' name='{2}'  type='application/vnd.vmware.admin.vdcTemplate+xml'/>
                              <Description>Dallas</Description>
                            </InstantiateVdcTemplateParams>", EndPoint, VdcTemplateId, this.VdcTemplateName);
 
                var xmlDoc = InvokeProtectedApi(EndPoint + "/api/org/" + cloudTenantId + "/action/instantiate", xmldata, HttpMethod.Post, "application/vnd.vmware.vcloud.instantiateVdcTemplateParams+xml");
                var xmlNode = xmlDoc.GetElementsByTagName("Task");

                if (xmlNode != null && xmlNode.Count > 0)
                {
                    result = (xmlNode[0].Attributes["href"].Value);
                }

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }

            return result;
        }

        public string GetVDC(string cloudTenantId)
        {
            string result = "";

            try
            {
                var xmlDoc = InvokeProtectedApi(this.EndPoint + "/api/org/" + cloudTenantId, "", HttpMethod.Get);


                var xmlNode = xmlDoc.GetElementsByTagName("Link");

                foreach (XmlElement item in xmlNode)
                {
                    if (item.Attributes["type"]?.Value == "application/vnd.vmware.vcloud.vdc+xml")
                    {
                        result = item.Attributes["href"]?.Value;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return result;
        }

        public string CreateCatalog(string cloudTenantId)
        {
            var result = "";

            try
            {
                var xmldata = @"<?xml version='1.0' encoding='UTF-8'?>
                                <vcloud:AdminCatalog xmlns:vcloud='http://www.vmware.com/vcloud/v1.5' name='Catalog" + Guid.NewGuid().ToString().Replace("-", "") + @"' >
                                    <vcloud:Description>Catalog</vcloud:Description>
                                </vcloud:AdminCatalog>";

                var xmlDoc = InvokeProtectedApi(EndPoint + "/api/admin/org/" + cloudTenantId + "/catalogs", xmldata, HttpMethod.Post, "application/vnd.vmware.admin.catalog+xml");


            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public async Task<string> UpdateEdgeGateWayToAdvanced(string cloudTenantId)
        {
            string result = "";
            try
            {
                string orgVDCEndPoint = GetVDC(cloudTenantId);
                string edgeGatewayEndPoint = GetEdgeGateway(orgVDCEndPoint);
                var xmlDoc = InvokeProtectedApi(edgeGatewayEndPoint + "/action/convertToAdvancedGateway", "", HttpMethod.Post);

                var xmlNode = xmlDoc.GetElementsByTagName("Task");

                if (xmlNode != null && xmlNode.Count > 0)
                {
                    result = (xmlNode[0].Attributes["href"].Value);
                }
            }
            catch (Exception ex)
            {
                Write(ex, "Error");
                throw;
            }

            return await Task.FromResult(result);
        }

        public string GetTaskStatus(string taskEndPoint)
        {
            var result = "";
            try
            {
                var xmlDoc = InvokeProtectedApi(taskEndPoint, "", HttpMethod.Get);
                var xmlNode = xmlDoc.GetElementsByTagName("Task");
                if (xmlNode != null && xmlNode.Count > 0)
                {
                    result = xmlNode[0].Attributes["status"]?.Value;
                }

            }
            catch (Exception ex)
            {

                throw;
            }

            return result;
        }

        private string GetEdgeGateway(string orgVdcEndpoint)
        {
            var result = "";

            try
            {
                var xmlDoc = InvokeProtectedApi(orgVdcEndpoint.Replace("api/", "api/admin/") + "/edgeGateways", "", HttpMethod.Get);

                var xmlNode = xmlDoc.GetElementsByTagName("EdgeGatewayRecord");
                if (xmlNode != null && xmlNode.Count > 0)
                {
                    result = xmlNode[0].Attributes["href"].Value;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.Write(ex, "ERROR");
                throw;
            }

            return result;
        }
        private string GetAdminRoles(string cloudTenantId)
        {
            string result = "";
            //HttpWebRequest orgRequest = (HttpWebRequest)HttpWebRequest.Create(objorg.href.Replace("api/", "api/admin/"));

            var xmlDoc = InvokeProtectedApi(EndPoint + "/api/admin/org/" + cloudTenantId + "/", "", HttpMethod.Get);


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
            var xmlDoc = InvokeProtectedApi(EndPoint + "/api/org", "", HttpMethod.Get, null);

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

        public void EnableOrg(string cloudTenantId)
        {

            InvokeProtectedApi(EndPoint + "/api/admin/org/" + cloudTenantId + "/action/enable", "", HttpMethod.Post, null);

        }



        private string GetOrgId(string orgHref)
        {
            int lastIndex = orgHref.LastIndexOf('/');
            var resutl = orgHref.Substring(lastIndex + 1);
            return resutl;
        }

        private string InvokeAnonymousApi(string url, string xmlLoad, HttpMethod method, string contentType = null)
        {
            string result = "";
            HttpHelper.InvokeApi(url, xmlLoad, method, r =>
            {
                r.Headers.Add("Accept", Version);
                if (!String.IsNullOrEmpty(contentType))
                {
                    r.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                }
            }, headers =>
            {
                var authHeader = headers.GetValues("x-vcloud-authorization")?.ToList();
                if (authHeader != null && authHeader.Count > 0)
                {
                    result = authHeader[0];
                }
            });

            return result;
        }

        /// <summary>
        /// This method adds authorization and accept headers before making a request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="xmlLoad"></param>
        /// <param name="method"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private XmlDocument InvokeProtectedApi(string url, string xmlLoad, HttpMethod method, string contentType = null)
        {
            //AddHeaders method is passes as presend action, it gets called inside InvokeApi before sendign request
            return HttpHelper.InvokeApi(url, xmlLoad, method, r => AddHeaders(r, contentType));
        }

        private void AddHeaders(HttpRequestMessage request, string contentType = null)
        {
            request.Headers.Add("x-vcloud-authorization", Authentiate());
            request.Headers.Add("Accept", Version);
            if (!String.IsNullOrEmpty(contentType))
            {
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            }
        }
    }


}