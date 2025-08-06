
using System.Collections.Concurrent;

public interface ISessionManager
{
    ClientSession GetSocketById(string id);
    void AddSession(ClientSession session);
    Task RemoveSession(ClientSession session);
    string CreateGuId();
    ConcurrentDictionary<string, ClientSession> GetAllSessions();
}
