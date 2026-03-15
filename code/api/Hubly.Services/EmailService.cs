using Hubly.api.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

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
            _port = int.Parse(_configuration["EmailSettings:Port"]);
            _enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]);
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
                <h1>Welcome to Hubly, {username}!</h1>
                <p>Thank you for registering. To confirm your email, use the code below:</p>
                <h2 style='color: #0066cc; font-size: 24px; padding: 10px; background-color: #f0f0f0; text-align: center; border-radius: 5px;'>{confirmationCode}</h2>
                <p>This code will expire in 24 hours.</p>
                <p>If you did not request this email, please ignore it.</p>",


                TextBody = $@"
                Welcome to Hubly, {username}!
                
                Thank you for registering. To confirm your email, use the code below:
                
                {confirmationCode}
                
                This code will expire in 24 hours.
                
                If you did not request this email, please ignore it."
            };


            message.Body = bodyBuilder.ToMessageBody();
            using var client = new SmtpClient();
            await client.ConnectAsync(
                _host,
                _port,
                _enableSsl
            );
            await client.AuthenticateAsync(
                _userName,
                _password
            );
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}

