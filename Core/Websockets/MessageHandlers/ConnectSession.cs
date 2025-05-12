using System.Net.WebSockets;
using Core.Database;

namespace Core.Websockets.MessageHandlers;

public class ConnectSession(Context dbContext) : ISocketMessageHandler
{
    public async Task HandleAsync(WebSocket webSocket, WebsocketMiddleware websocketMiddleware,
        string connectionId, SocketMessage message, Dictionary<Guid, List<string>> sessions)
    {
        if (message.SessionId is null || !sessions.ContainsKey( (Guid) message.SessionId ))
            return;
        
        await websocketMiddleware.SendMessage(new SocketMessage
        {
            Type = SocketMessageType.ConnectSession,
            SessionId = message.SessionId,
            ApplicationId = message.ApplicationId,
            Sender = message.Sender,
            Target =  message.Sender,
            Content = sessions[ (Guid) message.SessionId].ToString()
        });
        
        sessions.Add( (Guid) message.SessionId, [connectionId]);
    }
}