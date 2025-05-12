namespace Core.Websockets;

public enum SocketMessageType
{
    CreateSession,
    CloseSession
}

public class SocketMessage
{
    public SocketMessageType Type { get; set; }
    public string? Content { get; set; }
    public string? Sender { get; set; }
    public string? Target { get; set; }
    
    public string ApplicationId { get; set; }
    public Guid? SessionId { get; set; }
}