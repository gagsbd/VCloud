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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="xmlWorkload"></param>
        /// <param name="method"></param>
        /// <param name="preSend">This  the action (built deleagte with no return) that taks HttpRequestMessage as input. Caller of this methos can use this action to 
        /// do anythign that is needed before request is submitted to url. e.g addig a authrization header.</param>
        /// <param name="postSend">This  the action (built deleagte with no return) that taks HttpResponseHeaders as input. Caller of this method can use this action to 
        /// do anythign that is needed after request is completed e.g reading   response header.</param>
        /// <returns></returns>
        public static XmlDocument InvokeApi(string url, string xmlWorkload, HttpMethod method, Action<HttpRequestMessage> preSend = null, Action<HttpResponseHeaders> postSend = null)
        {
            var result = new XmlDocument();
          
            using (var httpClient = new HttpClient())
            {

                //Setup a reauest
                var request = new HttpRequestMessage()
                { 
                    RequestUri = new Uri(url),
                    Method = method

                };


                //If it is a GET method ignore that content head else assign it
                if (method != HttpMethod.Get)
                {
                    request.Content = new StringContent(xmlWorkload, Encoding.UTF8);// "application/vnd.vmware.admin.organization+xml; charset=ISO-8859-1");
                }

                //Invoke the presend action if ther is one "?" make sure that is calles Invoke mthos if preSend is not null
                preSend?.Invoke(request);

                var response = httpClient.SendAsync(request).Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpException(response.ReasonPhrase);
                }
                else
                {
                    //Invoke the postSend action if ther is one "?" make sure that is calles Invoke mthos if postSend is not null
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