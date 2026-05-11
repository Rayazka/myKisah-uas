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
//
// 📘 APA INI?
// Repository untuk akses data User di users.json.
// Mengimplementasikan IUserRepository (yang extends IRepository<User>).
//
// Repository ini HANYA bertanggung jawab untuk baca/tulis data.
// TIDAK ADA business logic di sini — semua validasi di UserService.
//
// 🧠 KENAPA GENERICS?
// IRepository<User> adalah hasil dari generic interface IRepository<T>.
// Kamu tidak perlu membuat method generic sendiri — cukup implementasi
// interface yang sudah diberi tipe konkret (User).
//
// 🧠 KENAPA REPOSITORY PATTERN?
// Memisahkan data access dari business logic. Kalau nanti ganti
// dari JSON ke database, cukup ganti repository — service tidak berubah.
//
// 📋 TODO:
// [ ] 1. Constructor: terima JsonStorageHelper + FilePathConfig via DI
//        - Simpan di private field _storage dan _filePath
//
// [ ] 2. Implement method dari IRepository<User>:
//        GetAll()    → _storage.ReadJson<User>(_filePath.UsersFile)
//        GetById(id) → GetAll().FirstOrDefault(u => u.Id == id)
//        Add(user)   → Read existing → set user.Id = Guid.NewGuid().ToString()
//                       set user.CreatedAt = DateTime.UtcNow
//                       users.Add(user) → _storage.WriteJson(...)
//        Update(user)→ Read → find index → replace → Write
//        Delete(id)  → Read → RemoveAll(u => u.Id == id) → Write
//
// [ ] 3. Implement method dari IUserRepository:
//        GetByUsername(username) → GetAll().FirstOrDefault(
//            u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
//        UsernameExists(username) → GetAll().Any(
//            u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
//
// Tips:
// - Pakai case-insensitive comparison untuk username
// - Add() harus generate Id baru (Guid.NewGuid().ToString())
// - Jangan lupa set CreatedAt = DateTime.UtcNow saat Add
// - Update() cari user by Id, ganti username-nya saja
//
// Referensi: Task_myKisah.md baris 188-197

public class JsonUserRepository : IUserRepository
{
    private readonly JsonStorageHelper _storage;
    private readonly FilePathConfig _filePath;

    public JsonUserRepository(JsonStorageHelper storage, FilePathConfig filePath)
    {
        _storage = storage;
        _filePath = filePath;
    }

    // ═══ IRepository<User> ═══

    public IEnumerable<User> GetAll()
    {
        return _storage.ReadJson<User>(_filePath.UsersFile);
    }

    public User? GetById(string id)
    {
        return GetAll().FirstOrDefault(u => u.Id == id);
    }

    public void Add(User entity)
    {
        var users = GetAll().ToList();
        entity.Id = Guid.NewGuid().ToString();
        entity.CreatedAt = DateTime.UtcNow;
        users.Add(entity);
        _storage.WriteJson(_filePath.UsersFile, users);
    }

    public void Update(User entity)
    {
        var users = GetAll().ToList();
        var index = users.FindIndex(u => u.Id == entity.Id);

        if (index == -1)
            throw new KeyNotFoundException($"User dengan Id '{entity.Id}' tidak ditemukan.");
        users[index] = entity;
        _storage.WriteJson(_filePath.UsersFile, users);
    }

    public void Delete(string id)
    {
        var users = GetAll().ToList();
        users.RemoveAll(u => u.Id == id);
        _storage.WriteJson(_filePath.UsersFile, users);
    }

    // ═══ IUserRepository (spesifik) ═══

    public User? GetByUsername(string username)
    {
        return GetAll().FirstOrDefault(
            u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
        );
    }

    public bool UsernameExists(string username)
    {
        return GetAll().Any(
            u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
        );
    }
}
