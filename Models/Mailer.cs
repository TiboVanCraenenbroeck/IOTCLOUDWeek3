using System;
using System.Collections.Generic;
using System.Text;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit;
using SendGrid.Helpers.Mail;

namespace oef1.Models
{
    public class Mailer
    {
        public void SendMail(Registration registration)
        {
            // Sendgrid toevoegen in NuGet
            /*var apiKey = Environment.GetEnvironmentVariable("azure_f821da223c329a6e7deb33cf53076647");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("tibo.van.craenenbroeck@student.howest.be");
            var subject = "Test Registration";
            var to = new EmailAddress(strTo);
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);*/
            // https://github.com/jstedfast/MailKit
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(Environment.GetEnvironmentVariable("FromMail")));
            message.To.Add(new MailboxAddress(registration.Email));
            message.Subject = "Nieuwe registratie";

            message.Body = new TextPart("plain")
            {
                Text = $"Beste Bedankt voor de registratie {registration.LastName}"
            };

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect("smtp.sendgrid.net", 587, false);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("azure_f821da223c329a6e7deb33cf53076647@azure.com", "admin.123");

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
