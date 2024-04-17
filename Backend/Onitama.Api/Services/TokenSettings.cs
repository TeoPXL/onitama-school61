namespace Onitama.Api.Services;
//DO NOT TOUCH THIS FILE!!

public class TokenSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationTimeInMinutes { get; set; }
}