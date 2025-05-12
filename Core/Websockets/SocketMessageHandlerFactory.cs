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
    
    public ISocketMessageHandler GetHandler(SocketMessage message)
    {
        if (_handlers.TryGetValue(message.Type, out var handler))
        {
            if (dbContext.Applications.Any(x => x.ApplicationId == message.ApplicationId))
                return handler;
            
            throw new Exception("Application unregistered.");
        }
        throw new NotSupportedException($"No handler registered for message type: {message.Type}");
    }
}