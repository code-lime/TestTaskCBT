namespace TaskCBT.Models;

public class ErrorResponse : Response
{
    public required string Error { get; set; }
    public override string Status => "error";
}