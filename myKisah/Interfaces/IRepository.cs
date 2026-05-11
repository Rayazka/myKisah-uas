namespace myKisah.Interfaces;

// Data Access Layer
// PENANGGUNG JAWAB: Azka
// TEKNIK: Generics (Parameterization)

// ** Penjelasan:
// Generic base interface untuk SEMUA repository.
// Mendefinisikan operasi CRUD dasar yang berlaku untuk semua tipe data.

// ** GENERICS
// Masalah tanpa generics harus bikin interface terpisah:
//   IUserRepository dengan Add(User), IJournalRepository dengan Add(Journal)...
//   = 4x duplikasi method signature yang identik.
// Solusinya buat IRepository<T> satu generic type parameter,
//   semua repository bisa pakai interface yang sama.

// Method:
// - GetAll()     : Ambil semua data
// - GetById(id)  : Cari data berdasarkan ID (return null jika tidak ketemu)
// - Add(entity)  : Tambah data baru
// - Update(entity): Update data existing
// - Delete(id)   : Hapus data berdasarkan ID
//
// TODO:
// 1. Definisikan 5 method signature di atas dengan generic type T
// 2. Pastikan semua repository nanti meng-extend interface ini

public interface IRepository<T>
{
    IEnumerable<T> GetAll();
    T? GetById(string id);
    void Add(T entity);
    void Update(T entity);
    void Delete(string id);
}
