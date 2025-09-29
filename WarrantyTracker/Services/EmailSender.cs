using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;


namespace WarrantyTracker.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;


        public EmailSender(IConfiguration config)
        {
            _config = config;
        }


        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var section = _config.GetSection("EmailSettings");
            var host = section.GetValue<string>("SmtpHost");
            var port = section.GetValue<int>("SmtpPort");
            var user = section.GetValue<string>("SmtpUser");
            var pass = section.GetValue<string>("SmtpPass");
            var fromEmail = section.GetValue<string>("FromEmail");
            var fromName = section.GetValue<string>("FromName");


            using (var client = new SmtpClient(host, port))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(user, pass);


                var mail = new MailMessage();
                mail.From = new MailAddress(fromEmail, fromName);
                mail.To.Add(email);
                mail.Subject = subject;
                mail.Body = htmlMessage;
                mail.IsBodyHtml = true;


                await client.SendMailAsync(mail);
            }
        }
    }
}