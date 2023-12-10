namespace TaskCBT.Models;

public class SuccessResponse : Response
{
    public override string Status => "success";
}
public class SuccessResponse<T> : SuccessResponse
{
    public required T Response { get; set; }
}
