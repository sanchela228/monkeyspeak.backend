using System.Net.WebSockets;
using System.Text.Json;
using Core.Database;

namespace Core.Websockets.MessageHandlers;

public class ConnectSession(Context dbContext) : ISocketMessageHandler
{
    public async Task HandleAsync(WebSocket webSocket, WebsocketMiddleware websocketMiddleware,
        string connectionId, SocketMessage message, Dictionary<Guid, List<string>> sessions)
    {
        if (message.SessionId is null || !sessions.ContainsKey( (Guid) message.SessionId ))
            return;

        if (sessions.TryGetValue((Guid)message.SessionId, out List<string>? items))
        {
            await websocketMiddleware.SendMessage(new SocketMessage
            {
                Type = SocketMessageType.ConnectSession,
                SessionId = message.SessionId,
                ApplicationId = message.ApplicationId,
                Sender = connectionId,
                Target = connectionId,
                Content = JsonSerializer.Serialize(items)
            });

            sessions[(Guid)message.SessionId].Add(connectionId);
        }
    }
}