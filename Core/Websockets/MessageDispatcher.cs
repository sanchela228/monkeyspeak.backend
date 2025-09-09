using ContextDatabase = Core.Database.Context;

namespace Core.Websockets;

public class MessageDispatcher
{
    private readonly Dictionary<Type, Action<IMessage, Connection>> _handlers = new();

    public void On<T>(Action<T, Connection> handler) where T : IMessage
    {
        _handlers[typeof(T)] = (msg, author) => handler((T)msg, author);
    }

    public void Dispatch(Context context, Connection author)
    {
        var message = context.ToMessage();
        var type = message.GetType();

        if (_handlers.TryGetValue(type, out var handler))
            handler(message, author);
    }

    public void Configure(ContextDatabase dbContext, List<Connection> connections, 
        List<Room> rooms, WebsocketMiddleware middleware)
    {
        On<Messages.Ping>((msg, author) =>
        {
            Console.WriteLine($"Получен Ping: {msg.Value}");
        });

        On<Messages.NoAuthCall.CreateSession>((msg, author) =>
        {
            var room = new Room(author);
            author.Status = Connection.StatusConnection.Connected;
            
            var sessionCreated = new Messages.NoAuthCall.SessionCreated()
            {
                Value = room.Code
            };
            
            author.Send(sessionCreated);
        });
    }
}