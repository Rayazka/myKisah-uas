using myKisah.Models;

namespace myKisah.Interfaces;

// ═══════════════════════════════════════════════════════════
// INTERFACE: IUserRepository
// DOMAIN: User Management
// PENANGGUNG JAWAB: Azka (Interface), Farel (Implementasi)
// TEKNIK: Generics (extends IRepository<User>)

// ** Penjelasan:
// Interface untuk akses data User. Extends IRepository<User>.
// Menambah method spesifik User yang tidak ada di generic IRepository.

// ** Method tambahan:
// - GetByUsername(username): Cari user berdasarkan username (case-insensitive)
// - UsernameExists(username) : Cek apakah username sudah terdaftar

// TODO Farel:
// 1. Buat kelas JsonUserRepository di folder Repositories/
// 2. Terima JsonStorageHelper dan FilePathConfig via constructor
// 3. Implement semua method dari interface ini


public interface IUserRepository : IRepository<User>
{
    User? GetByUsername(string username);
    bool UsernameExists(string username);
}
