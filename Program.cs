var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

var webSocketOptions = new WebSocketOptions { KeepAliveInterval = TimeSpan.FromMinutes(1) };
webSocketOptions.AllowedOrigins.Add("localhost");

builder.Services.AddSingleton<ISessionManager, SessionManagerService>();

app.UseWebSockets(webSocketOptions);
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();
