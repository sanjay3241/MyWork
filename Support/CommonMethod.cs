using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace MyWork.Support
{
    public class CommonFunction 
    { 
        public static bool SendMail(string toEmail, string Header, string Subject)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("msanjay6013@gmail.com");
            mail.To.Add(new MailAddress(toEmail));
            mail.Subject = Header;
            mail.Body = Subject;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential("msanjay6013@gmail.com", "plqjelcxrdymkvpa");
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mail);
                return true;
            }
            catch (SmtpException ex)
            {
                return false;
            }
        }

        

    }
}
