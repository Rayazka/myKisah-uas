using myKisah.Interfaces;
using myKisah.Models;
using myKisah.Utils;

namespace myKisah.Services;

// ═══════════════════════════════════════════════════════════
// KELAS: UserService
// DOMAIN: User Management
// TEKNIK: Generics + API (digunakan oleh UserController)
// PENANGGUNG JAWAB: Farel Ilham
// ═══════════════════════════════════════════════════════════
//
// 📘 APA INI?
// Business logic untuk User management.
// Extends ServiceBase (shared validation + logging).
// Implements IUserService.
//
// 🧠 ALUR KERJA:
// UserController (HTTP) → UserService (validasi + logic) → JsonUserRepository (data)
//
// Setiap method menerapkan Design by Contract:
// - Precondition: validasi input SEBELUM logic dijalankan
// - Postcondition: data tersimpan/terhapus dengan benar
//
// 📋 TODO:
// [ ] 1. Constructor: terima IUserRepository via DI
//        - base() constructor dipanggil otomatis (ServiceBase)
//
// [ ] 2. Override ServiceName → "UserService"
//
// [ ] 3. Implement RegisterUser(string username):
//        TRY:
//        a. Validator.ValidateNotEmpty(username, "Username")
//        b. Cek _repository.UsernameExists(username)
//           → jika true: throw new ArgumentException($"Username '{username}' sudah terdaftar")
//        c. Buat User baru: new User { Username = username }
//           (Id dan CreatedAt di-set oleh repository.Add)
//        d. _repository.Add(user)
//        e. Return user
//        CATCH (Exception ex):
//        f. LogError("RegisterUser gagal", ex); throw;
//
// [ ] 4. Implement GetAllUsers():
//        → return _repository.GetAll()
//
// [ ] 5. Implement UpdateUser(string id, string username):
//        a. Validator.ValidateNotEmpty(username, "Username")
//        b. var user = _repository.GetById(id)
//        c. Validator.ValidateExists(user, $"User dengan Id '{id}'")
//        d. Cek duplikat: _repository.UsernameExists(username) && user.Username != username
//           → jika true: throw ArgumentException
//        e. user.Username = username
//        f. _repository.Update(user)
//        g. Return user
//
// [ ] 6. Implement DeleteUser(string id):
//        a. var user = _repository.GetById(id)
//        b. Validator.ValidateExists(user, $"User dengan Id '{id}'")
//        c. _repository.Delete(id)
//        d. Return true
//
// Tips:
// - Validator diwarisi dari ServiceBase
// - LogError() untuk mencatat setiap exception
// - Username comparison case-insensitive
// - UpdateUser: jangan cek duplikat kalau username tidak berubah
//
// Referensi: Task_myKisah.md baris 201-208, 221-240

public class UserService : ServiceBase, IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    protected override string ServiceName => throw new NotImplementedException("TODO: return 'UserService'");

    public User RegisterUser(string username)
    {
        throw new NotImplementedException("TODO: RegisterUser - validasi + simpan user baru");
    }

    public IEnumerable<User> GetAllUsers()
    {
        throw new NotImplementedException("TODO: GetAllUsers - return semua user");
    }

    public User? UpdateUser(string id, string username)
    {
        throw new NotImplementedException("TODO: UpdateUser - validasi + update username");
    }

    public bool DeleteUser(string id)
    {
        throw new NotImplementedException("TODO: DeleteUser - validasi exists + hapus");
    }
}
