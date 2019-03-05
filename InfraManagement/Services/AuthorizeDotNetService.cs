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
using static InfraManagement.Services.HttpHelper;

namespace InfraManagement.Services
{
    public class AuthorizeDotNetService : IPaymentGateway
    {
        private string UserName { get; set; }
        private string Password { get; set; }
        private string EndPoint { get; set; }

        /// <summary>
        /// These construcctor paramters are read from web.config injected in Unitconfig.cs
        /// </summary>
        /// <param name="login"></param>
        /// <param name="transactionKey"></param>
        /// <param name="endPoint"></param>
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
               
                string expirydate;
                expirydate = card.CCExpYear + "-" + card.CCExpMonth.ToString("00");


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
                                          <cardNumber>" + card.CCnumber+ @"</cardNumber>
                                          <expirationDate>" + expirydate + @"</expirationDate>
                                          <cardCode>" + card.CCCVS + @"</cardCode>
                                          </creditCard>
                                          </payment>
                                          </paymentProfiles>
                                          </profile>
                                          <validationMode>testMode</validationMode>
                                          </createCustomerProfileRequest>";
       

                var xmlDoc = InvokeApi(this.EndPoint, authXML, HttpMethod.Post);

                XmlNodeList xmlnode;
                xmlnode = xmlDoc.GetElementsByTagName("resultCode");
                if ((xmlnode[0].InnerText) == "Ok")
                {
                    xmlnode = xmlDoc.GetElementsByTagName("customerProfileId");
                    if (xmlnode?.Count > 0)
                    {
                        result.ProfileId = xmlnode[0].InnerText;
                    }
                    xmlnode = xmlDoc.GetElementsByTagName("numericString");

                    if (xmlnode?.Count > 0)
                    {
                        result.PaymentProfileId = xmlnode[0].InnerText;
                    }
                    result.IsAuthorized = true;
                }
                else
                {
                    xmlnode = xmlDoc.GetElementsByTagName("text");
                    if (xmlnode?.Count > 0)
                    {
                        result.IsError = true;
                        result.Error = xmlnode[0].InnerText;
                    }
                    result.IsAuthorized = false;
                }

                
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Error = ex.Message;
                
            }

            return result;
        }
    }
}