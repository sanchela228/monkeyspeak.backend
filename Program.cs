var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions => {
    serverOptions.ConfigureHttpsDefaults(httpsOptions => {
        httpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
    });
});



var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();