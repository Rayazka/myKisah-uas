using myKisah.Models;

namespace myKisah.Interfaces;

public interface IChatSessionRepository
{
    ChatSession? GetLatest(string userId, string characterId);
    void Save(ChatSession session);
    void DeleteByUser(string userId);
}
