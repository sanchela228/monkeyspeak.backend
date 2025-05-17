using System.Net.WebSockets;
using Core.Database;
using Core.Services;
using Core.Websockets;
using Microsoft.EntityFrameworkCore;

namespace MonkeySpeak.Backend.Core.Configurations;

public class App
{
    static App() => Instance = new();
    public static App Instance { get; private set; }
    
    public Context DbContext { get; }
    
    static Dictionary<string, WebSocket> connections = new();
    private static readonly Dictionary<Guid, List<string>> _sessions = new();
    
    public string BackendVersion { get; set; }
   
    public string FrontendVersion { get; set; }
    public string FrontendAdditionalVersionText { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    public static void BuildDatabase(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        
        builder.Services.AddDbContext<Context>(options =>
            options.UseNpgsql(connectionString));
        
        builder.Services.AddScoped<Session>();
    }
    
    static WebSocketOptions webSocketOptions = new() {
        KeepAliveInterval = TimeSpan.FromMinutes(2)
    };
    
    public static void InitWebsockets(WebApplication app)
    {
        app.UseWebSockets(webSocketOptions);
        
        app.Map("/connector", async context =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                using var scope = app.Services.CreateScope();
                
                var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
                
                WebsocketMiddleware websocketMiddleware = new(webSocket, dbContext, connections, _sessions);
                await websocketMiddleware.HandleRequest(context);
            }
            else context.Response.StatusCode = StatusCodes.Status400BadRequest;
        });
        
        app.UseWebSockets();
    }
    
    public static async Task ApplyMigrations(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
    
        await dbContext.Database.MigrateAsync();
    }
}