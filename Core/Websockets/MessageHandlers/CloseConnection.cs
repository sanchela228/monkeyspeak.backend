using System.Net.WebSockets;
using Core.Database;

namespace Core.Websockets.MessageHandlers;

public class CloseConnection(Context dbContext) : ISocketMessageHandler
{
    public async Task HandleAsync(WebSocket webSocket, WebsocketMiddleware websocketMiddleware,
        string connectionId, SocketMessage message, Dictionary<Guid, List<string>> sessions)
    {
        if (message.SessionId is null || !sessions.ContainsKey( (Guid) message.SessionId ))
            return;

        if (sessions[(Guid)message.SessionId].Contains(message.Target) == false)
            return;

        sessions[(Guid)message.SessionId].Remove(message.Target);
        websocketMiddleware.CloseSession(message.Target);

        if (sessions.Count <= 0)
        {
            sessions.Remove((Guid)message.SessionId);
        }
    }
}