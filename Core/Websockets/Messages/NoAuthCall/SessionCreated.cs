namespace Core.Websockets.Messages.NoAuthCall;

public class SessionCreated : IMessage
{
    public string Value { get; set; }
}