namespace myKisah.Models;

// Character Companion System
// PENANGGUNG JAWAB: Azka (Model), Toni (Implementasi)
// TEKNIK: Table-Driven
    // Data response disimpan di JSON file terpisah. Kode hanya melakukan
    //   lookup/filter dari tabel ini berdasarkan CharacterId + Mood.
    //   - Tambah response = cukup edit characterResponses.json, tidak perlu ubah kode
    //   - Semua response terlihat jelas di satu file data

// ** Penjelasan:
// Satu baris response karakter untuk mood tertentu.
// Ini adalah ROW dari TABEL table-driven.
// Disimpan di Data/characterResponses.json.

public class CharacterResponse
{
    public string Id { get; set; } = string.Empty;
    public string CharacterId { get; set; } = string.Empty;
    public MoodType Mood { get; set; }
    public string Response { get; set; } = string.Empty;
}
