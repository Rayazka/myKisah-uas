namespace myKisah.Utils;

// ═══════════════════════════════════════════════════════════
// KELAS: ValidationHelper
// DOMAIN: Shared Utilities (dipakai semua Service + Repository)
// TEKNIK: Generics + Code Reuse
// PENANGGUNG JAWAB: Josefhint (Jojo)
// ═══════════════════════════════════════════════════════════
//
// 📘 APA INI?
// Helper untuk Design by Contract (DbC) — precondition checks.
// Dipakai di SEMUA method Service dan Repository untuk validasi input
// SEBELUM logic dijalankan (fail fast principle).
//
// 🧠 KENAPA GENERICS?
// Tanpa generics, ValidateNotNull harus dibuat overload untuk setiap tipe.
// Dengan generics: ValidateNotNull<T>(T? value) — satu method untuk semua tipe.
//
// 🧠 KENAPA CODE REUSE?
// Validasi seperti cek null, empty, enum valid dipakai di mana-mana.
// Kalau tidak dibuat shared helper, setiap developer akan menulis ulang kode
// yang sama. Ini = duplikasi + inkonsistensi error message.
//
// Precondition checks melempar exception yang TEPAT:
// - ArgumentNullException: nilai null
// - ArgumentException: nilai tidak valid (empty, invalid enum)
// - KeyNotFoundException: entity tidak ditemukan
//
// Exception ini ditangkap oleh ErrorHandlingMiddleware:
// - ArgumentNullException  → 400 Bad Request
// - ArgumentException      → 400 Bad Request
// - KeyNotFoundException   → 404 Not Found
//
// 📋 TODO:
// [ ] 1. Implement ValidateNotNull<T>(T? value, string name):
//        - Jika value == null → throw new ArgumentNullException(name, $"{name} tidak boleh null")
//
// [ ] 2. Implement ValidateNotEmpty(string value, string name):
//        - Jika string.IsNullOrWhiteSpace(value) → throw new ArgumentException($"{name} tidak boleh kosong")
//
// [ ] 3. Implement ValidateInEnum<T>(T value, string name) where T : struct, Enum:
//        - Jika !Enum.IsDefined(typeof(T), value) → throw new ArgumentException($"'{value}' bukan nilai {name} yang valid")
//
// [ ] 4. Implement ValidateExists<T>(T? entity, string name):
//        - Jika entity == null → throw new KeyNotFoundException($"{name} tidak ditemukan")
//
// Tips:
// - Method ini bukan static — di-instantiate oleh ServiceBase
// - name parameter untuk memberi konteks di error message
// - Enum.IsDefined untuk cek enum valid
//
// Referensi: Task_myKisah.md baris 167-185

public class ValidationHelper
{
    /// <summary>
    /// Memastikan value tidak null.
    /// </summary>
    /// <exception cref="ArgumentNullException">Jika value null</exception>
    public void ValidateNotNull<T>(T? value, string name)
    {
        // TODO: Cek value == null → throw ArgumentNullException
        throw new NotImplementedException("TODO: Implement ValidateNotNull");
    }

    /// <summary>
    /// Memastikan string tidak kosong atau whitespace.
    /// </summary>
    /// <exception cref="ArgumentException">Jika string null/empty/whitespace</exception>
    public void ValidateNotEmpty(string value, string name)
    {
        // TODO: Cek string.IsNullOrWhiteSpace → throw ArgumentException
        throw new NotImplementedException("TODO: Implement ValidateNotEmpty");
    }

    /// <summary>
    /// Memastikan nilai ada di dalam definisi enum.
    /// </summary>
    /// <exception cref="ArgumentException">Jika nilai tidak valid di enum T</exception>
    public void ValidateInEnum<T>(T value, string name) where T : struct, Enum
    {
        // TODO: Cek !Enum.IsDefined(typeof(T), value) → throw ArgumentException
        throw new NotImplementedException("TODO: Implement ValidateInEnum");
    }

    /// <summary>
    /// Memastikan entity ditemukan (tidak null setelah lookup).
    /// </summary>
    /// <exception cref="KeyNotFoundException">Jika entity null</exception>
    public void ValidateExists<T>(T? entity, string name)
    {
        // TODO: Cek entity == null → throw KeyNotFoundException
        throw new NotImplementedException("TODO: Implement ValidateExists");
    }
}
