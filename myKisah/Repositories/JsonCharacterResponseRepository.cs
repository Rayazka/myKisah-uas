using myKisah.Interfaces;
using myKisah.Models;
using myKisah.Utils;

namespace myKisah.Repositories;

// ═══════════════════════════════════════════════════════════
// KELAS: JsonCharacterResponseRepository
// DOMAIN: Character Companion System
// TEKNIK: TABLE-DRIVEN (INI ADALAH INTI TABLE-DRIVEN!)
// PENANGGUNG JAWAB: Toni Kurniawan
// ═══════════════════════════════════════════════════════════
//
// 📘 APA INI?
// Repository untuk akses tabel response karakter di characterResponses.json.
// INI ADALAH JANTUNG DARI TEKNIK TABLE-DRIVEN!
//
// Method GetByMood(characterId, mood) adalah method INTI yang mendemonstrasikan
// table-driven. Method ini melakukan filter data dari JSON — SEMUA DATA RESPONSE
// ADA DI characterResponses.json, BUKAN di kode C#.
//
// 🧠 KENAPA TABLE-DRIVEN?
// Bandingkan dua pendekatan:
//
// ❌ IF-ELSE (SALAH - JANGAN DILAKUKAN):
//   if (mood == MoodType.Happy) return "Senang mendengarnya!";
//   else if (mood == MoodType.Sad) return "Tidak apa-apa...";
//   // Masalah: tambah mood baru = ubah kode + recompile
//
// ✅ TABLE-DRIVEN (BENAR - LAKUKAN INI):
//   _storage.ReadJson<CharacterResponse>(...)
//       .Where(r => r.CharacterId == characterId && r.Mood == mood)
//   // Keunggulan: tambah response baru = edit characterResponses.json saja
//   // Tidak ada if/switch statement!
//
// 📋 TODO:
// [ ] 1. Constructor: terima JsonStorageHelper + FilePathConfig via DI
//
// [ ] 2. Implement method generic CRUD (GetAll, GetById, Add, Update, Delete)
//        → sama seperti repository lain
//
// [ ] 3. IMPLEMENT GetByMood — METHOD INTI TABLE-DRIVEN:
//        → return GetAll().Where(r => r.CharacterId == characterId && r.Mood == mood)
//        PASTIKAN TIDAK ADA IF/SWITCH STATEMENT!
//        HANYA PAKAI LINQ WHERE DARI DATA JSON!
//
// ⚠️ PERINGATAN UNTUK TONI:
// Method ini HARUS pure LINQ lookup. Kalau reviewer melihat if/switch/else
// di method ini, teknik Table-Driven kamu GAGAL. Semua mapping mood→response
// harus dari characterResponses.json.
//
// Tips:
// - GetByMood return IEnumerable<CharacterResponse> — mungkin lebih dari 1
// - Service yang akan ambil .FirstOrDefault() atau handle multiple
// - Jangan filter di memory dengan if — pakai LINQ Where dari data JSON
//
// Referensi: Task_myKisah.md baris 46-72, 355-370

public class JsonCharacterResponseRepository : ICharacterResponseRepository
{
    private readonly JsonStorageHelper _storage;
    private readonly FilePathConfig _filePath;

    public JsonCharacterResponseRepository(JsonStorageHelper storage, FilePathConfig filePath)
    {
        _storage = storage;
        _filePath = filePath;
    }

    public IEnumerable<CharacterResponse> GetAll()
    {
        throw new NotImplementedException("TODO: GetAll - baca dari characterResponses.json");
    }

    public CharacterResponse? GetById(string id)
    {
        throw new NotImplementedException("TODO: GetById - FirstOrDefault");
    }

    public void Add(CharacterResponse entity)
    {
        throw new NotImplementedException("TODO: Add - simpan (jarang dipakai, data manual di JSON)");
    }

    public void Update(CharacterResponse entity)
    {
        throw new NotImplementedException("TODO: Update - find index, replace, write");
    }

    public void Delete(string id)
    {
        throw new NotImplementedException("TODO: Delete - RemoveAll, write");
    }

    /// <summary>
    /// INTI TABLE-DRIVEN: Filter response berdasarkan CharacterId dan Mood.
    /// PASTIKAN TIDAK ADA IF/SWITCH — hanya LINQ Where.
    /// </summary>
    public IEnumerable<CharacterResponse> GetByMood(string characterId, MoodType mood)
    {
        throw new NotImplementedException("TODO: GetByMood - Where(characterId && mood). NO IF/SWITCH!");
    }
}
