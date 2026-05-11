using myKisah.Interfaces;
using myKisah.Models;
using myKisah.Utils;

namespace myKisah.Repositories;

// ═══════════════════════════════════════════════════════════
// KELAS: JsonJournalRepository
// DOMAIN: Journal System
// TEKNIK: Generics (mengimplementasikan IJournalRepository)
// PENANGGUNG JAWAB: Rayazka Aris
// ═══════════════════════════════════════════════════════════
//
// 📘 APA INI?
// Repository untuk akses data Journal di journals.json.
// Mengimplementasikan IJournalRepository (extends IRepository<Journal>).
//
// 🧠 METHOD PENTING: GetByUserId
// Ini untuk business rule J-01: "User hanya dapat mengakses journal miliknya sendiri".
// GetByUserId(userId) akan filter data dari JSON berdasarkan UserId.
//
// 📋 TODO:
// [ ] 1. Constructor: terima JsonStorageHelper + FilePathConfig via DI
//
// [ ] 2. Implement GetAll()          → _storage.ReadJson<Journal>(_filePath.JournalsFile)
//
// [ ] 3. Implement GetById(string id) → GetAll().FirstOrDefault(j => j.Id == id)
//
// [ ] 4. Implement Add(Journal entity):
//        - var journals = GetAll().ToList()
//        - entity.Id = Guid.NewGuid().ToString()
//        - entity.CreatedAt = DateTime.UtcNow
//        - entity.State = JournalState.Draft (selalu Draft saat baru)
//        - journals.Add(entity)
//        - _storage.WriteJson(_filePath.JournalsFile, journals)
//
// [ ] 5. Implement Update(Journal entity):
//        - var journals = GetAll().ToList()
//        - var index = journals.FindIndex(j => j.Id == entity.Id)
//        - journals[index] = entity
//        - _storage.WriteJson(_filePath.JournalsFile, journals)
//
// [ ] 6. Implement Delete(string id)  → RemoveAll + Write
//
// [ ] 7. Implement GetByUserId(string userId):
//        - return GetAll().Where(j => j.UserId == userId)
//
// Tips:
// - Pakai .ToList() setelah GetAll() karena ReadJson return IEnumerable
// - Add: set State = Draft, biarkan service yang mengatur transisi
// - Update: pastikan index != -1
//
// Referensi: Task_myKisah.md baris 188-197

public class JsonJournalRepository : IJournalRepository
{
    private readonly JsonStorageHelper _storage;
    private readonly FilePathConfig _filePath;

    public JsonJournalRepository(JsonStorageHelper storage, FilePathConfig filePath)
    {
        _storage = storage;
        _filePath = filePath;
    }

    public IEnumerable<Journal> GetAll()
    {
        throw new NotImplementedException("TODO: GetAll - baca dari journals.json");
    }

    public Journal? GetById(string id)
    {
        throw new NotImplementedException("TODO: GetById - cari journal dengan FirstOrDefault");
    }

    public void Add(Journal entity)
    {
        throw new NotImplementedException("TODO: Add - generate Id, set CreatedAt dan State=Draft, simpan");
    }

    public void Update(Journal entity)
    {
        throw new NotImplementedException("TODO: Update - find index, replace, write");
    }

    public void Delete(string id)
    {
        throw new NotImplementedException("TODO: Delete - RemoveAll, write");
    }

    public IEnumerable<Journal> GetByUserId(string userId)
    {
        throw new NotImplementedException("TODO: GetByUserId - Where(j => j.UserId == userId)");
    }
}
