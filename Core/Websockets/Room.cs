namespace Core.Websockets;

public class Room
{
    public readonly Guid Id;
    public readonly string Code;
    public readonly List<Connection> Connections = [];

    private Connection _creator;

    public Room(Connection creator)
    {
        Id = Guid.NewGuid();
        Code = "AA44CC";
        Connections.Add(creator);
        
        _creator = creator;
    }
}