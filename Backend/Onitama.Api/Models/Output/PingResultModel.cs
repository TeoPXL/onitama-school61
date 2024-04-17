namespace Onitama.Api.Models.Output;

public class PingResultModel
{
    public bool IsAlive { get; set; }
    public string Greeting { get; set; } = string.Empty;
}