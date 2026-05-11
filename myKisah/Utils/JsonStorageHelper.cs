using System.Text.Json;

namespace myKisah.Utils;

// ═══════════════════════════════════════════════════════════
// KELAS: JsonStorageHelper
// DOMAIN: Storage Layer (dipakai semua Repository)
// TEKNIK: Code Reuse / Library + Generics
// PENANGGUNG JAWAB: Rafly Putra
// ═══════════════════════════════════════════════════════════
//
// 📘 APA INI?
// Helper generic untuk baca/tulis data ke file JSON.
// Dipakai oleh SEMUA repository (User, Journal, Character, Response).
// Ini adalah SATU-SATUNYA tempat kode baca/tulis JSON — tidak boleh
// ada duplikasi kode JSON di tempat lain.
//
// 🧠 KENAPA CODE REUSE?
// Tanpa helper ini, setiap repository akan punya kode baca/tulis JSON
// yang SAMA PERSIS — 4x duplikasi. Kalau format berubah, harus ubah 4 file.
// Dengan helper ini: 1 file, 1 tempat perubahan.
//
// 🧠 KENAPA GENERICS (<T>)?
// ReadJson<User>() dan ReadJson<Journal>() pakai method yang SAMA.
// Tanpa generics: ReadUsers(), ReadJournals(), ReadCharacters()...
// Dengan generics: ReadJson<T>() — hanya 1 method.
//
// Design by Contract:
// - filename tidak boleh null (precondition)
// - data tidak boleh null saat write (precondition)
// - File yang tidak ada di-auto-create dengan array kosong "[]"
// - Return List<T> kosong (bukan null) jika file baru dibuat
//
// 📋 TODO:
// [ ] 1. Constructor: terima FilePathConfig via DI
//        - Simpan Directory.GetCurrentDirectory() + filePathConfig sebagai base path
//        - Atau bisa pakai cara lain untuk resolve full path
//
// [ ] 2. Implement ReadJson<T>(string filename):
//        a. Gabungkan _basePath + filename jadi fullPath
//        b. Cek File.Exists(fullPath):
//           - Jika TIDAK: File.WriteAllText(fullPath, "[]"), return new List<T>()
//           - Jika ADA: baca File.ReadAllText → JsonSerializer.Deserialize<List<T>> → return
//        c. Wrap dengan try-catch: jika JSON corrupt, return empty list
//
// [ ] 3. Implement WriteJson<T>(string filename, List<T> data):
//        a. Gabungkan _basePath + filename jadi fullPath
//        b. Pastikan Directory.CreateDirectory(parent directory) — jaga-jaga
//        c. JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true })
//        d. File.WriteAllText(fullPath, json)
//
// Tips:
// - Pakai System.Text.Json (bawaan .NET, tidak perlu NuGet tambahan)
// - JsonSerializerOptions dengan WriteIndented = true biar JSON rapi
// - Jangan lupa using System.Text.Json di atas
//
// Referensi: Task_myKisah.md baris 128-138

public class JsonStorageHelper
{
    private readonly string _basePath;

    public JsonStorageHelper(FilePathConfig filePathConfig)
    {
        // TODO: Tentukan base path untuk file JSON
        // _basePath = Path.Combine(Directory.GetCurrentDirectory(), "Data") atau dari config
        throw new NotImplementedException("TODO: Implement constructor");
    }

    public List<T> ReadJson<T>(string filename)
    {
        // TODO: 1. Build fullPath = _basePath + filename
        // TODO: 2. Jika file tidak ada → buat file "[]", return new List<T>()
        // TODO: 3. Jika file ada → baca, deserialize, return
        throw new NotImplementedException("TODO: Implement ReadJson");
    }

    public void WriteJson<T>(string filename, List<T> data)
    {
        // TODO: 1. Build fullPath
        // TODO: 2. Pastikan directory exists
        // TODO: 3. Serialize to JSON (WriteIndented)
        // TODO: 4. Tulis ke file
        throw new NotImplementedException("TODO: Implement WriteJson");
    }
}
