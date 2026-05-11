using myKisah.Models;

namespace myKisah.Interfaces;

// Character Companion System
// PENANGGUNG JAWAB: Azka (Interface), Toni (Implementasi)
// TEKNIK: Generics (extends IRepository<Character>)

// ** PENJELASAN:
// Interface untuk akses data Character. Extends IRepository<Character>.
// Menambah method GetByName untuk mencari karakter berdasarkan nama.

// TODO untuk Toni:
// 1. Buat kelas JsonCharacterRepository
// 2. Gunakan JsonStorageHelper untuk baca/tulis characters.json
// 3. Implement GetByName (bisa case-insensitive)


public interface ICharacterRepository : IRepository<Character>
{
    Character? GetByName(string name);
}
