using System.Net.WebSockets;

public abstract class Session
{
    private string? _id = null;
    public string? Id
    {
        get { return _id; }
        set { this._id = value; }
    }
    private WebSocket? _webSocket { get; set; } = null;
    public WebSocket? WebSocket
    {
        get { return _webSocket; }
        set { this._webSocket = value; }
    }
}
