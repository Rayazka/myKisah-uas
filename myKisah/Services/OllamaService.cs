using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using myKisah.Models.AI;
using Microsoft.Extensions.Configuration;

namespace myKisah.Services;

public class OllamaService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _defaultModel;

    public OllamaService(HttpClient http, IConfiguration configuration)
    {
        _http = http;
        _http.BaseAddress = new Uri(configuration["OllamaConfig:BaseUrl"] ?? "http://172.17.0.1:11434");
        _defaultModel = configuration["OllamaConfig:Model"] ?? "llama3.2:3b";

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true
        };
    }

    public async IAsyncEnumerable<string> StreamCharacterResponseAsync(
        string characterName,
        CharacterMood mood,
        string personality,
        string journalText,
        List<ChatMessage>? history = null,
        string? model = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var systemPrompt = BuildSystemPrompt(characterName, mood, personality);

        var messages = new List<OllamaMessage>
        {
            new() { role = "system", content = systemPrompt }
        };

        if (history != null)
        {
            foreach (var msg in history.TakeLast(10))
            {
                messages.Add(new OllamaMessage
                {
                    role = msg.Role,
                    content = msg.Content
                });
            }
        }

        messages.Add(new OllamaMessage
        {
            role = "user",
            content = journalText
        });

        var requestBody = new OllamaChatRequest
        {
            model = model ?? _defaultModel,
            messages = messages,
            stream = true,
            options = new OllamaOptions { temperature = 0.8 }
        };

        var jsonPayload = JsonSerializer.Serialize(requestBody, _jsonOptions);
        var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        using var httpResponse = await _http.PostAsync("/api/chat", httpContent, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();

        using var responseStream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(responseStream);

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (line == null)
                break;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            OllamaStreamChunk? chunk = null;
            try
            {
                chunk = JsonSerializer.Deserialize<OllamaStreamChunk>(line, _jsonOptions);
            }
            catch (JsonException)
            {
                continue;
            }

            if (chunk?.message?.content != null)
            {
                yield return chunk.message.content;
            }

            if (chunk?.done == true)
                break;
        }
    }

    public async Task<string> GetCharacterResponseAsync(
        string characterName,
        CharacterMood mood,
        string personality,
        string journalText,
        List<ChatMessage>? history = null,
        string? model = null)
    {
        var fullResponse = new StringBuilder();

        await foreach (var token in StreamCharacterResponseAsync(
            characterName, mood, personality, journalText,
            history, model, CancellationToken.None))
        {
            fullResponse.Append(token);
        }

        return fullResponse.ToString();
    }

    private static string BuildSystemPrompt(string characterName, CharacterMood mood, string personality)
    {
        var moodDesc = mood switch
        {
            CharacterMood.Happy => "sedang bahagia dan ceria",
            CharacterMood.Sad => "sedang sedih dan sendu",
            CharacterMood.Sleepy => "sedang ngantuk dan malas",
            CharacterMood.Cheerful => "sedang riang dan penuh semangat",
            CharacterMood.Calm => "sedang tenang dan damai",
            CharacterMood.Energetic => "penuh energi dan antusias",
            CharacterMood.Thoughtful => "sedang bijak dan suka merenung",
            CharacterMood.Playful => "sedang usil dan suka bercanda",
            _ => "ceria dan perhatian"
        };

        return $"""
            Kamu adalah {characterName}, seorang sahabat virtual untuk journaling.
            Personality: {personality}.
            Mood kamu saat ini: {moodDesc}.

            Aturan:
            - Jawab dengan hangat dan natural, 2-4 kalimat saja.
            - Gunakan bahasa Indonesia sehari-hari (boleh pake "gw", "lo", "lu").
            - Tanyakan kabar atau beri semangat sesuai mood kamu.
            - Jangan memberikan saran medis/psikolog serius.
            - Jangan berpura-pura jadi AI atau menyebut bahwa kamu adalah AI.
            - Jadilah teman yang sedang mendengarkan cerita harian.
            """;
    }
}
