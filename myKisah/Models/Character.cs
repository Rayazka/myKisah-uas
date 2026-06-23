// DOMAIN: Character
// Untuk entitas karakter companion — name, description, personality, greeting

namespace myKisah.Models;

// Character Companion System
// PENANGGUNG JAWAB: Azka

// ** Penjelasan:
// Entitas karakter companion virtual yang bisa dipilih user.
// Disimpan di Data/characters.json.
//
// ** Setiap character memiliki:
// - Id (string GUID) sebagai primary key
// - Name (string) nama karakter
// - Description (string) deskripsi singkat karakter


public class Character
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Personality { get; set; } = string.Empty;
    public string Greeting { get; set; } = string.Empty;
}
