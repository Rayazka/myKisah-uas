using myKisah.Interfaces;
using myKisah.Models;
using myKisah.Utils;

namespace myKisah.Repositories;

// ═══════════════════════════════════════════════════════════
// KELAS: JsonCharacterRepository
// DOMAIN: Character Companion System
// TEKNIK: Generics (mengimplementasikan ICharacterRepository)
// PENANGGUNG JAWAB: Toni Kurniawan
// ═══════════════════════════════════════════════════════════
//
// 📘 APA INI?
// Repository untuk akses data Character di characters.json.
// Mengimplementasikan ICharacterRepository (extends IRepository<Character>).
//
// Method tambahan: GetByName — mencari karakter berdasarkan nama.
//
// 📋 TODO:
// [ ] 1. Constructor: terima JsonStorageHelper + FilePathConfig via DI
//
// [ ] 2. Implement semua method dari IRepository<Character> (sama seperti repository lain)
//
// [ ] 3. Implement GetByName(string name):
//        → GetAll().FirstOrDefault(c =>
//            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
//
// Tips:
// - Karakter biasanya di-seed manual (tidak dibuat oleh user),
//   jadi method Add mungkin jarang dipakai. Tapi tetap implementasikan.
//
// Referensi: Task_myKisah.md baris 188-197

public class JsonCharacterRepository : ICharacterRepository
{
    private readonly JsonStorageHelper _storage;
    private readonly FilePathConfig _filePath;

    public JsonCharacterRepository(JsonStorageHelper storage, FilePathConfig filePath)
    {
        _storage = storage;
        _filePath = filePath;
    }

    public IEnumerable<Character> GetAll()
    {
        throw new NotImplementedException("TODO: GetAll - baca dari characters.json");
    }

    public Character? GetById(string id)
    {
        throw new NotImplementedException("TODO: GetById - FirstOrDefault");
    }

    public void Add(Character entity)
    {
        throw new NotImplementedException("TODO: Add - generate Id, simpan");
    }

    public void Update(Character entity)
    {
        throw new NotImplementedException("TODO: Update - find index, replace, write");
    }

    public void Delete(string id)
    {
        throw new NotImplementedException("TODO: Delete - RemoveAll, write");
    }

    public Character? GetByName(string name)
    {
        throw new NotImplementedException("TODO: GetByName - case-insensitive FirstOrDefault");
    }
}
