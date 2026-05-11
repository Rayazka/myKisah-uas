using myKisah.Models;

namespace myKisah.Interfaces;

// Journal System
// PENANGGUNG JAWAB: Azka
// TEKNIK: Generics (extends IRepository<Journal>)

// ** PENJELASAN:
// Interface untuk akses data Journal. Extends IRepository<Journal>.
// Menambah method GetByUserId untuk filter journal milik satu user.
//
// TODO Fase 2:
// 1. Buat kelas JsonJournalRepository
// 2. Gunakan JsonStorageHelper untuk baca/tulis journals.json
// 3. Implement GetByUserId dengan LINQ Where

public interface IJournalRepository : IRepository<Journal>
{
    IEnumerable<Journal> GetByUserId(string userId);
}
