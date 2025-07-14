using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Threading.Tasks;

public static class EmailService
{
    public static async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
    {
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse("your-email@gmail.com");
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = htmlContent };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync("your-email@gmail.com", "your-app-password");
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
