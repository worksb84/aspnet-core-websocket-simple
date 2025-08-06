using System.Collections.Concurrent;
using System.Net.WebSockets;

public class SessionManagerService : ISessionManager
{
    private static ConcurrentDictionary<string, ClientSession> _sessions =
        new ConcurrentDictionary<string, ClientSession>();

    public ClientSession GetSocketById(string id)
    {
        return _sessions.FirstOrDefault(p => p.Key == id).Value;
    }

    public void AddSession(ClientSession session)
    {
        string id = CreateGuId();
        session.Id = id;
        _sessions.TryAdd(id, session);
    }

    public async Task RemoveSession(ClientSession session)
    {
        string id = _sessions.FirstOrDefault(p => p.Value == session).Key;
        if (!string.IsNullOrEmpty(id))
        {
            _sessions.TryRemove(id, out _);
        }

        if (session.WebSocket!.State != WebSocketState.Aborted)
        {
            await session.WebSocket!.CloseAsync(
                closeStatus: WebSocketCloseStatus.NormalClosure,
                statusDescription: null,
                cancellationToken: CancellationToken.None
            );
        }
    }

    public string CreateGuId()
    {
        return Guid.NewGuid().ToString();
    }

    public ConcurrentDictionary<string, ClientSession> GetAllSessions()
    {
        return _sessions;
    }
}
