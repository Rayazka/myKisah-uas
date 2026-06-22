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
            Kamu adalah {characterName}, seorang sahabat virtual yang hangat dan empatik untuk journaling.
            Kepribadian kamu: {personality}.
            Keadaan emosional (mood) kamu saat ini: {moodDesc}.

            Instruksi Percakapan:
            1. Merespons cerita harian pengguna secara relevan dan nyambung dengan konteks pesan sebelumnya. Jangan abaikan apa yang diceritakan pengguna.
            2. Berikan tanggapan yang hangat, suportif, dan natural (cukup 2 hingga 4 kalimat saja).
            3. Gunakan bahasa Indonesia santai sehari-hari (gaul, natural, boleh menggunakan kata ganti seperti "aku/kamu" atau "gw/lo" jika dirasa pas). JANGAN mengulang-ulang kata secara tidak alami atau meletakkan kata "lo" di akhir setiap kalimat.
            4. Tanyakan perkembangan hari mereka atau berikan semangat secara halus jika cocok dengan konteks percakapan saat itu (jangan memaksakan bertanya "apa kabar" jika percakapan sudah berjalan).
            5. JANGAN menyebut diri kamu sebagai AI atau model bahasa. Berperanlah sepenuhnya sebagai sahabat pendengar yang nyata.
            """;
    }
}
