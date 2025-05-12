using System.Net.WebSockets;
using Core.Database;

namespace Core.Websockets.MessageHandlers;

public class ConnectClient(Context dbContext) : ISocketMessageHandler
{
    public async Task HandleAsync(WebSocket webSocket, WebsocketMiddleware websocketMiddleware,
        string connectionId, SocketMessage message, Dictionary<Guid, List<string>> sessions)
    {
        if (message.SessionId is null || !sessions.ContainsKey( (Guid) message.SessionId ))
            return;

        if (sessions[(Guid)message.SessionId].Contains(message.Target) == false)
            return;

        await websocketMiddleware.SendMessage(new SocketMessage
        {
            Type = SocketMessageType.ConnectClient,
            SessionId = message.SessionId,
            ApplicationId = message.ApplicationId,
            Sender = message.Sender,
            Target = message.Target,
            Content = message.Content
        });
    }
}