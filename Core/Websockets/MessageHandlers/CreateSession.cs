using System.Net.WebSockets;
using Core.Database;
using SessionService = Core.Services.Session;

namespace Core.Websockets.MessageHandlers;

public class CreateSession(Context dbContext) : ISocketMessageHandler
{
    public async Task HandleAsync(WebSocket webSocket, WebsocketMiddleware websocketMiddleware, string connectionId, 
        SocketMessage message, Dictionary<Guid, List<string>> sessions)
    {
        var sessionService = new SessionService(dbContext);
        var sessionId = Guid.NewGuid();
        
        sessions.Add(sessionId, [connectionId]);
        
        var session = await sessionService.CreateSession(message.ApplicationId, sessionId);
        
        await websocketMiddleware.SendMessage(new SocketMessage
        {
            Type = SocketMessageType.CreateSession,
            SessionId = session.WebSocketSessionId,
            ApplicationId = message.ApplicationId,
            Sender = connectionId,
            Target = connectionId,
            Content = session.Code
        });
    }
}