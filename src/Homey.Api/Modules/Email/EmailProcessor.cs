using System.Threading.Channels;
using MailKit.Net.Smtp;
using MimeKit;

namespace Homey.Api.Modules.Email;

public class EmailProcessor(ILogger<EmailProcessor> logger, IOptionsMonitor<SmtpOptions> smtpOptions, Channel<EmailMessage> notificationChannel) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await notificationChannel.Reader.WaitToReadAsync(stoppingToken))
        {
            var notification = await notificationChannel.Reader.ReadAsync(stoppingToken);
            await SendEmail(notification);
        }
    }

    private async Task SendEmail(EmailMessage notification)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(smtpOptions.CurrentValue.FromName, smtpOptions.CurrentValue.FromEmail));
        email.To.Add(new MailboxAddress(notification.Email, notification.Email));
        email.Subject = notification.Subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = notification.Message
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpOptions.CurrentValue.Host, smtpOptions.CurrentValue.Port, smtpOptions.CurrentValue.UseSsl);
        if (smtpOptions.CurrentValue.RequireAuthentication)
        {
            await client.AuthenticateAsync(smtpOptions.CurrentValue.Username, smtpOptions.CurrentValue.Password);    
        }

        try
        {
            var response = await client.SendAsync(email);
            await client.DisconnectAsync(true);
        
            logger.LogInformation($"Response: {response} -- {notification.Subject} email sent to {notification.Email}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to send {notification.Subject} email to {notification.Email}: {ex.Message}");
        }
    }
}

/// <summary>
/// DTO representing the notification to be sent to a user.
/// </summary>
/// <param name="UserId">Id of the user to send to</param>
/// <param name="Message">Message to send</param>
/// <param name="Email">Optional e-mail address</param>
public record EmailMessage(string UserId, string Subject, string Message, string? Email = null);