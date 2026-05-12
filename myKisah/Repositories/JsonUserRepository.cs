using myKisah.Interfaces;
using myKisah.Models;
using myKisah.Utils;

namespace myKisah.Repositories;

// ═══════════════════════════════════════════════════════════
// KELAS: JsonUserRepository
// DOMAIN: User Management
// TEKNIK: Generics (mengimplementasikan IRepository<User>)
// PENANGGUNG JAWAB: Farel Ilham
// ═══════════════════════════════════════════════════════════

// PENJELASAN
// Repository untuk akses data User di users.json.
// Mengimplementasikan IUserRepository (yang extends IRepository<User>).

public class JsonUserRepository : IUserRepository
{
    // Dependency untuk baca/tulis JSON file.
    // Dibuat oleh Rafly (shared utility).
    private readonly JsonStorageHelper _storage;

    // Dependency untuk mengambil path file users.json
    // dari appsettings.json (runtime configuration).
    private readonly FilePathConfig _filePath;

    // Constructor Dependency Injection.
    // ASP.NET otomatis inject object saat aplikasi jalan.
    public JsonUserRepository(JsonStorageHelper storage, FilePathConfig filePath)
    {
        _storage = storage;
        _filePath = filePath;
    }

    // ═══════════════════════════════════════════════════════
    // IRepository<User>
    // Generic CRUD operations
    // ═══════════════════════════════════════════════════════

    // Mengambil seluruh data user dari users.json
    public IEnumerable<User> GetAll()
    {
        // ReadJson<User>()
        // GENERICS:
        // T = User
        //
        // Method ini reusable untuk:
        // ReadJson<Journal>()
        // ReadJson<Character>()
        //
        // Return type:
        // IEnumerable<User> = collection/list user
        return _storage.ReadJson<User>(_filePath.UsersFile);
    }

    // Mengambil 1 user berdasarkan Id
    public User? GetById(string id)
    {
        // GetAll()
        // baca semua user dulu dari JSON

        // FirstOrDefault()
        // cari user pertama yang Id-nya cocok

        // kalau tidak ketemu → return null
        return GetAll().FirstOrDefault(u => u.Id == id);
    }

    public void Add(User entity)
    {
        // Ambil semua data user dulu
        var users = GetAll().ToList();

        // Generate GUID otomatis
        // supaya setiap user punya Id unik
        entity.Id = Guid.NewGuid().ToString();

        // Simpan waktu registrasi UTC
        entity.CreatedAt = DateTime.UtcNow;

        // Tambahkan user baru ke list
        users.Add(entity);

        // Simpan ulang seluruh list ke users.json
        _storage.WriteJson(_filePath.UsersFile, users);
    }

    // Update data user yang usdah ada
    public void Update(User entity)
    {
        // Ambil semua data user
        var users = GetAll().ToList();

        // Cari index user berdasarkan Id
        var index = users.FindIndex(u => u.Id == entity.Id);

        // Defensive Programming / DbC
        // Kalau user tidak ditemukan → exception
        if (index == -1)
            throw new KeyNotFoundException(
                $"User dengan Id '{entity.Id}' tidak ditemukan."
            );

        // Replace data lama dengan data baru
        users[index] = entity;

        // Simpan ulang ke JSON
        _storage.WriteJson(_filePath.UsersFile, users);
    }

    // Hapus user berdasarkan Id
    public void Delete(string id)
    {
        // Ambil semua user
        var users = GetAll().ToList();

        // RemoveAll()
        // hapus semua user yang Id-nya cocok
        users.RemoveAll(u => u.Id == id);

        // Simpan ulang hasil delete ke JSON
        _storage.WriteJson(_filePath.UsersFile, users);
    }

    // ═══════════════════════════════════════════════════════
    // IUserRepository
    // Method spesifik User
    // ═══════════════════════════════════════════════════════

    // Cari user berdasarkan username
    public User? GetByUsername(string username)
    {
        return GetAll().FirstOrDefault(

            // Equals(...OrdinalIgnoreCase)
            // compare string tanpa peduli huruf besar/kecil
            u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
        );
    }

    // Mengecek apakah username sudah terdaftar
    public bool UsernameExists(string username)
    {
        return GetAll().Any(

            // Any()
            // return true kalau ADA minimal 1 user cocok
            u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
        );
    }
}
