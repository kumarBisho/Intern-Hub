using System;
using System.Net;
using System.Net.Mail;
using InternMS.Api.Services.Email;

namespace InternMS.Api.Services.Email
{
    public class EmailService : IEmailService
    {
          public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST");
            var smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
            var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER");
            Console.WriteLine($"SMTP Config - Host: {smtpHost}, Port: {smtpPort}, User: {smtpUser}");
            var smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS");

            if (smtpHost == null || smtpUser == null || smtpPass == null)
            {
                throw new Exception("SMTP configuration is missing in environment variables.");
            }

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}