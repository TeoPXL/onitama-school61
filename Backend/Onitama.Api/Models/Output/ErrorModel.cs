
namespace Onitama.Api.Models.Output;

public class ErrorModel
{
    public string Message { get; set; }

    public ErrorModel()
    {
        Message = "Unknown error";
    }

    public ErrorModel(Exception exception)
    {
        Message = exception.Message;
    }

    public ErrorModel(string message)
    {
        Message = message;
    }
}