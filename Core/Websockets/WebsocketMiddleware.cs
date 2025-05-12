using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Database;

namespace Core.Websockets;

public class WebsocketMiddleware(WebSocket ws, Context dbContext)
{
    Dictionary<string, WebSocket> connections = new();
    private readonly Dictionary<Guid, List<string>> _sessions = new();
    
    public async Task HandleRequest(HttpContext context)
    {
        var connectionId = Guid.NewGuid().ToString();
        connections.Add(connectionId, ws);

        try
        {
            var handlerFactory = new SocketMessageHandlerFactory(dbContext);
            await HandleWebSocket(connectionId, ws, handlerFactory);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Websocket connect failed: {e.Message}");
            throw;
        }
        finally
        {
            connections.Remove(connectionId);
        }
    }

    private async Task HandleWebSocket(string connectionId, WebSocket webSocket, SocketMessageHandlerFactory handlerFactory)
    {
        var buffer = new byte[1024 * 4];

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                break;
            }
            
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Receive from {connectionId}: {message}");
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
            
            var messageObj = JsonSerializer.Deserialize<SocketMessage>(message, options);
        
            if (messageObj != null && messageObj.ApplicationId != null)
            {
                try
                {
                    var handler = handlerFactory.GetHandler(messageObj);
                    
                    await handler.HandleAsync(webSocket, this, connectionId, messageObj, _sessions);
                }
                catch (NotSupportedException ex)
                {
                    Console.WriteLine($"Error handling message: {ex.Message}");
                }
            }
        }
    }
    
    public async Task CloseSession(string target)
    {
        await connections[target].CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
    }
    
    public async Task SendMessage(SocketMessage message)
    {
        var buffer = Encoding.UTF8.GetBytes( JsonSerializer.Serialize(message) );
        await connections[message.Target].SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}