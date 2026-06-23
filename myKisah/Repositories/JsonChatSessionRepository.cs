// DOMAIN: AI
// PURPOSE: CRUD chat sessions ke chatSessions.json — GetLatest, Save, DeleteByUser

using myKisah.Interfaces;
using myKisah.Models;
using myKisah.Utils;

namespace myKisah.Repositories;

public class JsonChatSessionRepository : IChatSessionRepository
{
    private readonly JsonStorageHelper _storage;
    private readonly FilePathConfig _paths;

    public JsonChatSessionRepository(JsonStorageHelper storage, FilePathConfig paths)
    {
        _storage = storage;
        _paths = paths;
    }

    public ChatSession? GetLatest(string userId, string characterId)
    {
        var all = _storage.ReadJson<ChatSession>(_paths.ChatSessionsFile);
        return all.Where(s => s.UserId == userId && s.CharacterId == characterId)
                  .OrderByDescending(s => s.UpdatedAt)
                  .FirstOrDefault();
    }

    public void Save(ChatSession session)
    {
        var all = _storage.ReadJson<ChatSession>(_paths.ChatSessionsFile).ToList();
        var index = all.FindIndex(s => s.Id == session.Id);
        if (index >= 0)
            all[index] = session;
        else
        {
            if (string.IsNullOrEmpty(session.Id))
                session.Id = Guid.NewGuid().ToString();
            all.Add(session);
        }
        _storage.WriteJson(_paths.ChatSessionsFile, all);
    }

    public void DeleteByUser(string userId)
    {
        var all = _storage.ReadJson<ChatSession>(_paths.ChatSessionsFile).ToList();
        all.RemoveAll(s => s.UserId == userId);
        _storage.WriteJson(_paths.ChatSessionsFile, all);
    }
}
