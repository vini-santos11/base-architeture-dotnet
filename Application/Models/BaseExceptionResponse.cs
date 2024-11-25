namespace Application.Models;

public class BaseExceptionResponse
{
    public string Message { get; set; }

    public Dictionary<string, List<string>> Errors { get; set; }

    public string? Data { get; set; }
}