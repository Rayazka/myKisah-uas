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
        throw new NotImplementedException("TODO: GetAll - baca dari users.json");
    }

    public User? GetById(string id)
    {
        throw new NotImplementedException("TODO: GetById - cari user dengan FirstOrDefault");
    }

    public void Add(User entity)
    {
        throw new NotImplementedException("TODO: Add - generate Id, set CreatedAt, simpan");
    }

    public void Update(User entity)
    {
        throw new NotImplementedException("TODO: Update - cari by Id, ganti data, simpan");
    }

    public void Delete(string id)
    {
        throw new NotImplementedException("TODO: Delete - RemoveAll by Id, simpan");
    }

    // ═══ IUserRepository (spesifik) ═══

    public User? GetByUsername(string username)
    {
        throw new NotImplementedException("TODO: GetByUsername - case-insensitive search");
    }

    public bool UsernameExists(string username)
    {
        throw new NotImplementedException("TODO: UsernameExists - case-insensitive Any");
    }
}
