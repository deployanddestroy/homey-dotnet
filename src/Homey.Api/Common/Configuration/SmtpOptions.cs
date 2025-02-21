using System.ComponentModel.DataAnnotations;

namespace Homey.Api.Common.Configuration;

public class SmtpOptions
{
    [Required]
    public required string Host { get; init; }
    [Required]
    public int Port { get; init; } = 587;
    public string? Username { get; init; }
    public string? Password { get; init; }
    [Required]
    public required string FromEmail { get; init; }
    [Required]
    public required string FromName { get; init; }
    public bool UseSsl { get; init; }
    public bool RequireAuthentication { get; init; }
}