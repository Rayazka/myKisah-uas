using myKisah.Models;

namespace myKisah.Components.Shared;

public static class MoodHelper
{
    public static string GetEmoji(MoodType mood) => mood switch
    {
        MoodType.Happy   => "😊",
        MoodType.Sad     => "😢",
        MoodType.Angry   => "😠",
        MoodType.Anxious => "😰",
        MoodType.Calm    => "😌",
        _ => "😊"
    };
}
