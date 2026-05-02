using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace FilscusBySky.BusinessLogic;

public class EmailService
{
	private readonly IConfiguration _config;

	public EmailService(IConfiguration config)
	{
		_config = config;
	}

	public async Task StuurWachtwoordResetAsync(string email, string resetLink)
	{
		var message = new MimeMessage();
		message.From.Add(new MailboxAddress("FilscusBySky", _config["Email:Van"]));
		message.To.Add(new MailboxAddress("", email));
		message.Subject = "Wachtwoord resetten - FilscusBySky";

		message.Body = new TextPart("html")
		{
			Text = $"""
                <div style="font-family: Arial, sans-serif; max-width: 600px;">
                    <h2 style="color: #154273;">Wachtwoord resetten</h2>
                    <p>Je hebt een aanvraag ingediend om je wachtwoord te resetten.</p>
                    <p>Klik op de knop hieronder om je wachtwoord te resetten:</p>
                    <a href="{resetLink}"
                       style="background-color: #154273; color: white;
                              padding: 12px 24px; text-decoration: none;
                              display: inline-block; margin: 16px 0;">
                        Wachtwoord resetten
                    </a>
                    <p>Deze link is 1 uur geldig.</p>
                    <p>Als je geen aanvraag hebt ingediend, kan je deze e-mail negeren.</p>
                </div>
                """
		};

		using var client = new SmtpClient();
		await client.ConnectAsync(_config["Email:Host"], 587, SecureSocketOptions.StartTls);
		await client.AuthenticateAsync(_config["Email:Gebruiker"], _config["Email:Wachtwoord"]);
		await client.SendAsync(message);
		await client.DisconnectAsync(true);
	}
}