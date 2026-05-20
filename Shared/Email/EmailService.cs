using MailKit.Net.Smtp;
using MimeKit;

namespace UserApi.Shared.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config) => _config = config;

    public async Task SendLoginNotificationAsync(string name, string username, string email)
    {
        try
        {
            var templatePath = Path.Combine(AppContext.BaseDirectory, "Shared", "Email", "Templates", "login-email.html");
            var html = (await File.ReadAllTextAsync(templatePath))
                .Replace("{{name}}", name)
                .Replace("{{username}}", username)
                .Replace("{{email}}", email)
                .Replace("{{loginTime}}", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_config["Mail:Username"]));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Login Notification";
            message.Body = new TextPart("html") { Text = html };

            using var client = new SmtpClient();
            await client.ConnectAsync(_config["Mail:Host"], int.Parse(_config["Mail:Port"]!), MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_config["Mail:Username"], _config["Mail:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to send login email: {ex.Message}");
        }
    }
}
