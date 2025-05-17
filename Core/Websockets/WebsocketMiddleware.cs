using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Database;

namespace Core.Websockets;

public class WebsocketMiddleware(WebSocket ws, Context dbContext, Dictionary<string, WebSocket> connections, Dictionary<Guid, List<string>> sessions)
{
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
            using var memoryStream = new MemoryStream();
            WebSocketReceiveResult result;
            
            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                    return;
                }
        
                memoryStream.Write(buffer, 0, result.Count);
            }
            while (!result.EndOfMessage);
            
            var message = Encoding.UTF8.GetString(memoryStream.ToArray());
            Console.WriteLine($"Receive from {connectionId}: {message}");
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter() },
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };
            
            var messageObj = JsonSerializer.Deserialize<SocketMessage>(message, options);
        
            if (messageObj != null && messageObj.ApplicationId != null)
            {
                try
                {
                    var handler = handlerFactory.GetHandler(messageObj);
                    
                    await handler.HandleAsync(webSocket, this, connectionId, messageObj, sessions);
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