using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using InfraManagement.Models;

namespace InfraManagement.Services
{
    public class AuthorizeDotNetService : IPaymentGateway
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EndPoint { get; set; }
        public AuthorizeDotNetService(string login, string transactionKey, string endPoint)
        {
            this.UserName = login;
            this.Password = transactionKey;
            this.EndPoint = endPoint;
        }

        public AuthResult Authorize(PaymentCard card)
        {
            AuthResult result = new AuthResult() { IsAuthorized = false };
            try
            {
                string authapilogin = "76xfD5Gua";
                string transactionkey = "5h4LwUEb8nY88a76";
                string URL = "https://apitest.authorize.net/xml/v1/request.api";
                if (objcc.production)
                {
                    authapilogin = "76xfD5Gua";
                    transactionkey = "5h4LwUEb8nY88a76";
                    URL = "https://apitest.authorize.net/xml/v1/request.api";
                }
                string expirydate;
                expirydate = card.CCExpYear + "-" + card.CCExpMonth;


                string authXML;

                authXML = @"<createCustomerProfileRequest xmlns=""AnetApi/xml/v1/schema/AnetApiSchema.xsd"">
                                      <merchantAuthentication>
                                          <name>" + this.UserName + @"</name>
                                          <transactionKey>" + this.Password + @"</transactionKey>
                                      </merchantAuthentication>
                                       <profile>
                                          <merchantCustomerId>" + card.FirstName + " " + card.LastName + @"</merchantCustomerId>
                                          <description></description>
                                          <email>" + card.EmailAddress + @"</email>
                                          <paymentProfiles>
                                            <customerType>individual</customerType>
                                          <payment>
                                          <creditCard>
                                          <cardNumber>" + card. + @"</cardNumber>
                                          <expirationDate>" + expirydate + @"</expirationDate>
                                          <cardCode>" + card.CCCVS + @"</cardCode>
                                          </creditCard>
                                          </payment>
                                          </paymentProfiles>
                                          </profile>
                                          <validationMode>testMode</validationMode>
                                          </createCustomerProfileRequest>";





                HttpWebRequest httprequest = (HttpWebRequest)HttpWebRequest.Create(this.EndPoint);
                httprequest.Method = "POST";
                UTF8Encoding enc;
                enc = new System.Text.UTF8Encoding();
                byte[] postdata;
                postdata = enc.GetBytes(authXML);
                httprequest.ContentLength = postdata.Length;
                using (var stream = httprequest.GetRequestStream())
                {
                    stream.Write(postdata, 0, postdata.Length);
                    stream.Close();
                }
                var response = httprequest.GetResponse();
                Stream receivestream = response.GetResponseStream();
                StreamReader readstream = new StreamReader(receivestream, Encoding.UTF8);

                string responsexml = readstream.ReadToEnd();
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.LoadXml(responsexml);
                XmlNodeList xmlnode;
                xmlnode = xmlDoc.GetElementsByTagName("resultCode");
                if ((xmlnode[0].InnerText) == "Ok")
                {
                    xmlnode = xmlDoc.GetElementsByTagName("customerProfileId");
                    result.ProfileId = xmlnode[0].InnerText;
                    xmlnode = xmlDoc.GetElementsByTagName("numericString");
                    result.IsAuthorized = true;
                }
                else
                {
                    result.IsAuthorized = false;
                }

                
            }
            catch (Exception ex)
            {

                //TODO: Log error
            }

            return result;
        }
    }
}