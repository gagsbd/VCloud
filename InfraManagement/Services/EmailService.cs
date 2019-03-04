using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace InfraManagement.Services
{
    public class EmailService : INotificationService
    {
        public void Send(string subject, string message, string sendTo)
        {
            MailMessage msg = new MailMessage("do_not_reply@liveitcg.com", sendTo);
            msg.Body = message;
            msg.BodyEncoding = Encoding.UTF8;
            msg.IsBodyHtml = true;
            msg.Subject = subject;

            using (SmtpClient smtp = new SmtpClient())
            {

                //smtp.SendAsync(msg,null);
                //smtp.Timeout = 60 * 1000;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("gdeepadiah@gmail.com", "gags@12345");
                smtp.Send(msg);
            }
        }
    }
}