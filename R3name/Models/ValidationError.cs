using R3name.Models.Enums;

namespace R3name.Models;

public class ValidationError
{
    public ErrorTypes ErrorType { get; set; }
    public string Message { get; set; }
}