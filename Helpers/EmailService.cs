using System.Net.Mail;
using System.Net;

namespace VNTour.Helpers
{
    public static class EmailService
    {
        public static async Task SendEmailAsync(string to, string subject, string body)
        {
            var fromEmail = "youremail@gmail.com";
            var password = "your_app_password"; // App password nếu dùng Gmail

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
