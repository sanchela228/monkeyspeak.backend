using System.Net.WebSockets;

namespace Core.Websockets;

public interface ISocketMessageHandler
{
    Task HandleAsync(WebSocket webSocket, WebsocketMiddleware websocketMiddleware, string connectionId, SocketMessage message, Dictionary<Guid, List<string>> sessions);
}