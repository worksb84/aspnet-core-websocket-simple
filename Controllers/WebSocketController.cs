using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace AspNETCoreWS.Controllers;

public class WebSocketController : Controller
{
    private readonly ILogger<WebSocketController> _logger;
    private ISessionManager _sessionManager;

    public WebSocketController(ILogger<WebSocketController> logger, ISessionManager sessionManager)
    {
        _logger = logger;
        _sessionManager = sessionManager;
    }

    [Route("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var session = new ClientSession();
            session.WebSocket = webSocket;
            _sessionManager.AddSession(session);

            await Receive(
                webSocket,
                async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        return;
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        return;
                    }
                }
            );
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    public async Task SendMessageToAllAsync(string message)
    {
        foreach (var pair in _sessionManager.GetAllSessions())
        {
            if (pair.Value.WebSocket!.State == WebSocketState.Open)
                await SendMessageAsync(pair.Value.WebSocket, message);
        }
    }

    public async Task SendMessageAsync(WebSocket socket, string message)
    {
        if (socket.State != WebSocketState.Open)
        {
            return;
        }

        var buffer = new ArraySegment<byte>(Encoding.ASCII.GetBytes(message), 0, message.Length);
        await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task Receive(
        WebSocket socket,
        Action<WebSocketReceiveResult, byte[]> handleMessage
    )
    {
        var buffer = new byte[1024 * 4];

        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(
                    buffer: new ArraySegment<byte>(buffer),
                    cancellationToken: CancellationToken.None
                );

                handleMessage(result, buffer);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
