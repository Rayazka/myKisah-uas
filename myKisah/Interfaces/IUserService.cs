using myKisah.Models;

namespace myKisah.Interfaces;

// User Management
// PENANGGUNG JAWAB: Azka (Interface), Farel  (Implementasi)
// TEKNIK: Generics + API (digunakan oleh UserController)

// ** Penjelasan:
// Kontrak business logic untuk User management.
// Dipanggil oleh UserController (API layer).

// Method:
// - RegisterUser(username) buat Daftarkan user baru, return User yang berhasil dibuat
// - GetAllUsers()  buat Ambil semua user yang terdaftar
// - UpdateUser(id, username) buat Update username user
// - DeleteUser(id) buat Hapus user, return true jika berhasil

// TODO Farel:
// 1. Buat kelas UserService di folder Services/
// 2. Extend ServiceBase
// 3. Inject IUserRepository via constructor
// 4. Implement semua method dengan precondition checks

// - Implementasi di UserService (folder Services/)
// - Validasi username tidak boleh kosong dan tidak boleh duplikat
// - Gunakan IUserRepository untuk akses data
// - Extend ServiceBase (yang dibuat Jojo) untuk shared validation


public interface IUserService
{
    User RegisterUser(string username);
    IEnumerable<User> GetAllUsers();
    User? UpdateUser(string id, string username);
    bool DeleteUser(string id);
}
