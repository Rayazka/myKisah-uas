// DOMAIN: AI
// PURPOSE: Model persistensi history chat — ChatMessageItem + ChatSession

namespace myKisah.Models;

public class ChatMessageItem
{
    public string Role { get; set; } = "";
    public string Content { get; set; } = "";
    public string Mood { get; set; } = "Happy";
    public DateTime Time { get; set; }
}

public class ChatSession
{
    public string Id { get; set; } = "";
    public string UserId { get; set; } = "";
    public string CharacterId { get; set; } = "";
    public List<ChatMessageItem> Messages { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
