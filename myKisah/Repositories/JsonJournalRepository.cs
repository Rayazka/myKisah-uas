using myKisah.Interfaces;
using myKisah.Models;
using myKisah.Utils;

namespace myKisah.Repositories;

// Journal System
// TEKNIK: Generics (mengimplementasikan IJournalRepository)
// PENANGGUNG JAWAB: Azka

// ** Penjelasan:
// Repository untuk akses data Journal di journals.json.
// Mengimplementasikan IJournalRepository (extends IRepository<Journal>).
//
// METHOD PENTING: GetByUserId
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

public class JsonJournalRepository : IJournalRepository
{
    private readonly JsonStorageHelper _storage; // Helper untuk baca/tulis JSON
    private readonly FilePathConfig _filePath; // PathConfig untuk akses journals.json

    // Constructor terima JsonStorageHelper + FilePathConfig via DI
    public JsonJournalRepository(JsonStorageHelper storage, FilePathConfig filePath)
    {
        _storage = storage;
        _filePath = filePath;
    }

    // GetAll: baca semua journal dari JSON menggunakan helper
    public IEnumerable<Journal> GetAll()
    {
        // Baca semua journal dari JSON menggunakan helper
        return _storage.ReadJson<Journal>(_filePath.JournalsFile);
    }

    // GetById: cari journal dengan FirstOrDefault berdasarkan Id
    public Journal? GetById(string id)
    {
        // Cari journal dengan FirstOrDefault berdasarkan Id
        return GetAll().FirstOrDefault(j => j.Id == id);
    }

    // Add: generate Id, set CreatedAt dan State=Draft, simpan ke JSON
    public void Add(Journal entity)
    {
        var journals = GetAll().ToList(); // Baca semua jurnal yang ada dan Convert ke List supaya bisa Add dan Write

        // Set field yang diperlukan: Id, CreatedAt, State
        entity.Id = Guid.NewGuid().ToString();
        entity.CreatedAt = DateTime.UtcNow;
        entity.State = JournalState.Draft;

        // Tambahkan journal baru ke list dan simpan kembali ke JSON
        journals.Add(entity);
        _storage.WriteJson(_filePath.JournalsFile, journals);
    }

    // Update: cari index journal, ganti dengan entity baru, simpan ke JSON
    public void Update(Journal entity)
    {
        var journals = GetAll().ToList(); // Baca semua jurnal yang ada dan Convert ke List supaya bisa Update dan Write
        var index = journals.FindIndex(j => j.Id == entity.Id); // Cari index journal yang ingin diupdate

        // Jika journal tidak ditemukan, lempar exception. Jika ditemukan, ganti journal lama dengan entity baru dan simpan ke JSON
        if (index == -1)
        {
            throw new KeyNotFoundException($"Journal dengan Id '{entity.Id}' tidak ditemukan");
        }
        
        journals[index] = entity; // Ganti journal lama dengan journal baru
        _storage.WriteJson(_filePath.JournalsFile, journals); // Simpan kembali ke JSON
    }

    // Delete: cari journal berdasarkan Id, hapus dari list, simpan ke JSON
    public void Delete(string id)
    {
        var journals = GetAll().ToList(); // Baca semua jurnal yang ada dan Convert ke List supaya bisa Remove dan Write
        var removedCount = journals.RemoveAll(j => j.Id == id); // Hapus journal dengan Id yang diberikan

        // Jika tidak ada journal yang dihapus, berarti Id tidak ditemukan. Lempar exception. Jika berhasil dihapus, simpan kembali ke JSON
        if (removedCount == 0)
        {
            throw new KeyNotFoundException($"Journal dengan Id '{id}' tidak ditemukan"); 
        }

        _storage.WriteJson(_filePath.JournalsFile, journals); // Setelah dihapus, simpan kembali ke JSON
    }

    // GetByUserId: filter journal berdasarkan UserId
    public IEnumerable<Journal> GetByUserId(string userId)
    {
        return GetAll().Where(j => j.UserId == userId); // Filter journal berdasarkan UserId
    }
}
