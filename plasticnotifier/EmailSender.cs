using System;
using System.Net.Mail;

namespace plasticnotifier
{
    static class EmailSender
    {

        public static void Send(string toAddress, string subject, string body)
        {
            try
            {
                string SMTPUser = "yourGmailAccount@gmail.com", SMTPPassword = "thePassword";

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(SMTPUser, "Plastic notifications"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false,
                    Priority = MailPriority.Normal,
                };
                mail.To.Add(toAddress);


                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(SMTPUser, SMTPPassword),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true
                };
                smtp.Send(mail);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}