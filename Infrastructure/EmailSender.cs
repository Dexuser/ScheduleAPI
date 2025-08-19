using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;


namespace Application.Services;
public class EmailSender : IEmailSender
{
    private readonly IConfiguration _config;

    public EmailSender(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        Console.WriteLine($"Sending email to: {to}");

        //var host = _config["Smtp:Host"];
        //var port = int.Parse(_config["Smtp:Port"]);
        //var enableSsl = bool.Parse(_config["Smtp:EnableSsl"]);
        //var user = _config["Smtp:User"];
        //var password = _config["Smtp:Password"];

        //using var client = new SmtpClient(host, port)
        //{
        //    Credentials = new NetworkCredential(user, password),
        //    EnableSsl = enableSsl
        //};

        //var mail = new MailMessage
        //{
        //    From = new MailAddress(user),
        //    Subject = subject,
        //    Body = body,
        //    IsBodyHtml = true
        //};

        //mail.To.Add(to);

        //// Enviar de forma asíncrona
        //await client.SendMailAsync(mail);
    }
}