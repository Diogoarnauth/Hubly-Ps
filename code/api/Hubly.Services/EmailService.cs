using Hubly.api.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace Hubly.api.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _fromEmail; 
        private readonly string _fromName;
        private readonly string _host;
        private readonly int _port;
        private readonly bool _enableSsl;
        private readonly string _userName;
        private readonly string _password;


        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _fromEmail = _configuration["EmailSettings:FromEmail"];
            _fromName = _configuration["EmailSettings:FromName"];
            _port = int.Parse(_configuration["EmailSettings:Port"] ?? "587"); // TODO() ver melhor
            _enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "false"); //TODO () ver melhor
            _host = _configuration["EmailSettings:Host"];
            _userName = _configuration["EmailSettings:UserName"];
            _password = _configuration["EmailSettings:Password"];
        }


        public async Task SendConfirmationEmailAsync(string email, string username, string confirmationCode)
        {
            var message = new MimeMessage();


            message.From.Add(new MailboxAddress(_fromName, _fromEmail));
            message.To.Add(new MailboxAddress(username, email));
            message.Subject = "Confirmation Code - Hubly";
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                <div style='font-family: sans-serif; max-width: 600px; margin: auto;'>
                    <h1>Welcome to Hubly, {username}!</h1>
                    <p>Thank you for registering. To confirm your email, use the code below:</p>
                    <div style='color: #0066cc; font-size: 32px; font-weight: bold; padding: 20px; background-color: #f4f4f4; text-align: center; border-radius: 8px; letter-spacing: 5px;'>
                        {confirmationCode}
                    </div>
                    <p>This code will expire in 24 hours.</p>
                    <hr/>
                    <p style='font-size: 12px; color: #888;'>If you did not request this email, please ignore it.</p>
                </div>",

                TextBody = $"Welcome to Hubly, {username}!\n\nYour confirmation code is: {confirmationCode}\n\nThis code expires in 24 hours."

            };


            message.Body = bodyBuilder.ToMessageBody();
            using var client = new SmtpClient();
            try
            {
                // Determina a segurança correta baseada no porto
                SecureSocketOptions options = SecureSocketOptions.Auto;
                
                if (_port == 587)
                    options = SecureSocketOptions.StartTls;
                else if (_port == 465)
                    options = SecureSocketOptions.SslOnConnect;
                else if (!_enableSsl)
                    options = SecureSocketOptions.None;

                await client.ConnectAsync(_host, _port, options);
                
                // Só tenta autenticar se houver utilizador configurado
                if (!string.IsNullOrEmpty(_userName))
                {
                    await client.AuthenticateAsync(_userName, _password);
                }

                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                // Loga o erro mas permite que a transação da DB continue
                Console.WriteLine($"[EMAIL ERROR] Could not send email: {ex.Message}");
            }
            finally
            {
                if (client.IsConnected)
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}
