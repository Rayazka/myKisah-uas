// DOMAIN: AI
// PURPOSE: Kontrak akses data chat sessions — GetLatest, Save, DeleteByUser

using myKisah.Models;

namespace myKisah.Interfaces;

public interface IChatSessionRepository
{
    ChatSession? GetLatest(string userId, string characterId);
    void Save(ChatSession session);
    void DeleteByUser(string userId);
}
