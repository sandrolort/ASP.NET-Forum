using System.Text.Json;

namespace Common.DTOs;

public class ErrorDetails
{
    public ErrorDetails(int statusCode, string? message)
    {
        StatusCode = statusCode;
        Message = message;
    }

    public int StatusCode { get; init; }
    public string? Message { get; init; }

    public override string ToString() => JsonSerializer.Serialize(this);
}