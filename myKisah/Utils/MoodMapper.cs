// DOMAIN: AI
// PURPOSE: Mapping 5 MoodType (app) ke 8 CharacterMood (AI) untuk prompt Ollama

using myKisah.Models;
using myKisah.Models.AI;

namespace myKisah.Utils;

public static class MoodMapper
{
    public static CharacterMood ToAiMood(MoodType mood) => mood switch
    {
        MoodType.Happy => CharacterMood.Happy,
        MoodType.Sad => CharacterMood.Sad,
        MoodType.Angry => CharacterMood.Sad,
        MoodType.Anxious => CharacterMood.Thoughtful,
        MoodType.Calm => CharacterMood.Calm,
        _ => CharacterMood.Happy
    };
}
