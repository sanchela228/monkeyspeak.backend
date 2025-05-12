using Core.Database;
using Microsoft.EntityFrameworkCore;
using Core.Websockets.MessageHandlers;

namespace Core.Websockets;

public class SocketMessageHandlerFactory(Context dbContext)
{
    private readonly Dictionary<SocketMessageType, ISocketMessageHandler> _handlers = new()
    {
        { SocketMessageType.CreateSession, new CreateSession(dbContext) },
    };
    
    public ISocketMessageHandler GetHandler(SocketMessageType messageType)
    {
        if (_handlers.TryGetValue(messageType, out var handler))
        {
            return handler;
        }
        throw new NotSupportedException($"No handler registered for message type: {messageType}");
    }
}