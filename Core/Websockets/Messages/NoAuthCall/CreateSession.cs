namespace Core.Websockets.Messages.NoAuthCall;

public class CreateSession : IMessage
{
    public string Value { get; set; }
}