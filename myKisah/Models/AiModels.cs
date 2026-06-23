// DOMAIN: AI
// PURPOSE: DTO untuk komunikasi Ollama API — CharacterMood, ChatMessage, OllamaChatRequest, OllamaStreamChunk

namespace myKisah.Models.AI;

public enum CharacterMood
{
    Happy,
    Sad,
    Sleepy,
    Cheerful,
    Calm,
    Energetic,
    Thoughtful,
    Playful
}

public class ChatMessage
{
    public string Role { get; set; } = "";
    public string Content { get; set; } = "";
}

public class OllamaChatRequest
{
    public string model { get; set; } = "llama3.2:3b";
    public List<OllamaMessage> messages { get; set; } = new();
    public bool stream { get; set; } = true;
    public OllamaOptions? options { get; set; }
}

public class OllamaMessage
{
    public string role { get; set; } = "";
    public string content { get; set; } = "";
}

public class OllamaOptions
{
    public double temperature { get; set; } = 0.8;
}

public class OllamaStreamChunk
{
    public OllamaStreamMessage? message { get; set; }
    public bool done { get; set; }
}

public class OllamaStreamMessage
{
    public string content { get; set; } = "";
}
