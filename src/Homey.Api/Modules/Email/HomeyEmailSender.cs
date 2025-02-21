using System.Threading.Channels;
using Microsoft.AspNetCore.Identity;

namespace Homey.Api.Modules.Email;

public interface IHomeyEmailSender<TUser> : IEmailSender<TUser> where TUser : AppUser, new()
{
    //TODO: Add different types of emails to send here
}

public class HomeyEmailSender<TUser>(Channel<EmailMessage> channel) : IHomeyEmailSender<TUser>
    where TUser : AppUser, new()
{
    public async Task SendConfirmationLinkAsync(TUser user, string email, string confirmationLink)
    {
        var emailMessage = @$"Please confirm your e-mail by clicking the following link: 
        <br/>
        <a href=\""{confirmationLink}\"">{confirmationLink}</a>";
        
        await channel.Writer.WriteAsync(new EmailMessage(user.Id, "Registration Confirmation", emailMessage, email));
    }

    public async Task SendPasswordResetLinkAsync(TUser user, string email, string resetLink)
    {
        var emailMessage = @$"Please click the link below to reset your password: 
        <br/>
        <a href=\""{resetLink}\"">{resetLink}</a>";
        
        await channel.Writer.WriteAsync(new EmailMessage(user.Id, "Password Reset Link", emailMessage, email));
    }

    public async Task SendPasswordResetCodeAsync(TUser user, string email, string resetCode)
    {
        var emailMessage = @$"Here is your password reset code: 
        <br/>
        <a href=\""{resetCode}\"">{resetCode}</a>";
        
        await channel.Writer.WriteAsync(new EmailMessage(user.Id, "Password Reset Code", emailMessage, email));
    }
}