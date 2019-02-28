using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Xml;

namespace InfraManagement.Services
{
    public class HttpHelper
    {

        public static XmlDocument InvokeApi(string url, string xmlWorkload, HttpMethod method, Action<HttpRequestMessage> preSend = null, Action<HttpResponseHeaders> postSend = null)
        {
            var result = new XmlDocument();
            // POST /rest/issue/{issue}/timetracking/workitem
            using (var httpClient = new HttpClient())
            {

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = method

                };




                //if (requestHeaders != null)
                //{
                //    foreach (var header in requestHeaders)
                //    {
                //        request.Headers.Add(header.Key, header.Value);
                //    }
                //}
                if (method != HttpMethod.Get)
                {
                    request.Content = new StringContent(xmlWorkload, Encoding.UTF8);// "application/vnd.vmware.admin.organization+xml; charset=ISO-8859-1");
                    preSend?.Invoke(request);

                }

                var response = httpClient.SendAsync(request).Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
                else
                {
                    postSend?.Invoke(response.Headers);
                    var responseTxt = response.Content.ReadAsStringAsync().Result;
                    if (!String.IsNullOrEmpty(responseTxt))
                    {
                        result.LoadXml(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
            return result;

        }
    }
}