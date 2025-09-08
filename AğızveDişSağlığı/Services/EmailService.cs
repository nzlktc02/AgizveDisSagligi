using MailKit.Net.Smtp;
using MimeKit;
using System.Text;

namespace AğızveDişSağlığı.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string fullName);
        Task SendPasswordResetEmailAsync(string toEmail, string fullName);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string fullName)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Ağız ve Diş Sağlığı Takip", "noreply@agizdis.com"));
                message.To.Add(new MailboxAddress(fullName, toEmail));
                message.Subject = "Hoş Geldiniz - Ağız ve Diş Sağlığı Takip Sistemi";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = GenerateWelcomeEmailHtml(fullName);

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("your-email@gmail.com", "your-app-password");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Welcome email sent to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send welcome email to {toEmail}");
                // Email gönderimi başarısız olsa bile kullanıcı kaydı devam etsin
            }
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string fullName)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Ağız ve Diş Sağlığı Takip", "noreply@agizdis.com"));
                message.To.Add(new MailboxAddress(fullName, toEmail));
                message.Subject = "Parola Sıfırlama - Ağız ve Diş Sağlığı Takip Sistemi";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = GeneratePasswordResetEmailHtml(fullName);

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("your-email@gmail.com", "your-app-password");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Password reset email sent to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send password reset email to {toEmail}");
            }
        }

        private string GenerateWelcomeEmailHtml(string fullName)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='utf-8'>");
            html.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            html.AppendLine("<title>Hoş Geldiniz</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; background-color: #f8f9fa; }");
            html.AppendLine(".container { max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }");
            html.AppendLine(".header { text-align: center; margin-bottom: 30px; }");
            html.AppendLine(".logo { font-size: 24px; font-weight: bold; color: #ec4899; }");
            html.AppendLine(".content { margin-bottom: 30px; }");
            html.AppendLine(".footer { text-align: center; color: #666; font-size: 14px; }");
            html.AppendLine(".button { display: inline-block; background-color: #ec4899; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin: 20px 0; }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("<div class='container'>");
            html.AppendLine("<div class='header'>");
            html.AppendLine("<div class='logo'>🦷 Ağız ve Diş Sağlığı Takip</div>");
            html.AppendLine("</div>");
            html.AppendLine("<div class='content'>");
            html.AppendLine($"<h2>Merhaba {fullName}!</h2>");
            html.AppendLine("<p>Ağız ve Diş Sağlığı Takip sistemimize hoş geldiniz!</p>");
            html.AppendLine("<p>Artık aşağıdaki özellikleri kullanabilirsiniz:</p>");
            html.AppendLine("<ul>");
            html.AppendLine("<li>📊 Günlük ağız ve diş sağlığı durumunuzu takip edin</li>");
            html.AppendLine("<li>🎯 Kişisel hedefler belirleyin ve ilerlemenizi izleyin</li>");
            html.AppendLine("<li>💡 Günlük sağlık önerileri alın</li>");
            html.AppendLine("<li>📈 Son 7 günlük verilerinizi görüntüleyin</li>");
            html.AppendLine("</ul>");
            html.AppendLine("<p>Hesabınız başarıyla oluşturuldu ve giriş yapabilirsiniz.</p>");
            html.AppendLine("</div>");
            html.AppendLine("<div class='footer'>");
            html.AppendLine("<p>Bu e-posta otomatik olarak gönderilmiştir. Lütfen yanıtlamayın.</p>");
            html.AppendLine("<p>© 2024 Ağız ve Diş Sağlığı Takip Sistemi</p>");
            html.AppendLine("</div>");
            html.AppendLine("</div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        private string GeneratePasswordResetEmailHtml(string fullName)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='utf-8'>");
            html.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            html.AppendLine("<title>Parola Sıfırlama</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; background-color: #f8f9fa; }");
            html.AppendLine(".container { max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }");
            html.AppendLine(".header { text-align: center; margin-bottom: 30px; }");
            html.AppendLine(".logo { font-size: 24px; font-weight: bold; color: #ec4899; }");
            html.AppendLine(".content { margin-bottom: 30px; }");
            html.AppendLine(".footer { text-align: center; color: #666; font-size: 14px; }");
            html.AppendLine(".alert { background-color: #d1ecf1; border: 1px solid #bee5eb; color: #0c5460; padding: 15px; border-radius: 5px; margin: 20px 0; }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("<div class='container'>");
            html.AppendLine("<div class='header'>");
            html.AppendLine("<div class='logo'>🦷 Ağız ve Diş Sağlığı Takip</div>");
            html.AppendLine("</div>");
            html.AppendLine("<div class='content'>");
            html.AppendLine($"<h2>Merhaba {fullName}!</h2>");
            html.AppendLine("<p>Parolanız başarıyla sıfırlandı.</p>");
            html.AppendLine("<div class='alert'>");
            html.AppendLine("<strong>Güvenlik Uyarısı:</strong> Eğer bu işlemi siz yapmadıysanız, lütfen hemen bizimle iletişime geçin.");
            html.AppendLine("</div>");
            html.AppendLine("<p>Artık yeni parolanızla sisteme giriş yapabilirsiniz.</p>");
            html.AppendLine("</div>");
            html.AppendLine("<div class='footer'>");
            html.AppendLine("<p>Bu e-posta otomatik olarak gönderilmiştir. Lütfen yanıtlamayın.</p>");
            html.AppendLine("<p>© 2024 Ağız ve Diş Sağlığı Takip Sistemi</p>");
            html.AppendLine("</div>");
            html.AppendLine("</div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }
    }
}
