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

// PENJELASAN
// Business logic untuk User management.
// Extends ServiceBase (shared validation + logging).
// Implements IUserService.

public class UserService : ServiceBase, IUserService
{
    // Dependency repository
    // Digunakan untuk akses data user
    //
    // Type:
    // IUserRepository (abstraction/interface)
    // Tidak langsung ke JsonUserRepository untuk loose coupling
    private readonly IUserRepository _repository;

    // Constructor Dependency Injection
    //
    // ASP.NET otomatis inject repository saat runtime
    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    // Nama service untuk logging/error handling
    //
    // Dipakai oleh ServiceBase
    protected override string ServiceName => "UserService";

    // ═══════════════════════════════════════════════════════
    // REGISTER USER
    // ═══════════════════════════════════════════════════════
    public User RegisterUser(string username)
    {
        try
        {
            // VALIDASI DbC
            // Username tidak boleh kosong/null
            Validator.ValidateNotEmpty(username, "Username");

            // Cek apakah username sudah ada
            if (_repository.UsernameExists(username))
            {
                // Jika sudah ada, lempar exception
                throw new ArgumentException(
                    $"Username '{username}' sudah terdaftar"
                );
            }

            // Membuat object User baru
            //
            // Id & CreatedAt
            // akan diisi repository saat Add()
            var user = new User
            {
                Username = username
            };

            _repository.Add(user);

            return user;
        }
        catch (Exception ex)
        {
            // Logging error
            //
            // ServiceBase menyediakan LogError()
            //
            // Setelah logging exception dilempar lagi ke middleware
            LogError("RegisterUser gagal", ex);

            // RETHROW exception asli
            //
            // Middleware nanti handle HTTP response
            throw;
        }
    }

    // ═══════════════════════════════════════════════════════
    // GET ALL USERS
    // ═══════════════════════════════════════════════════════

    public IEnumerable<User> GetAllUsers()
    {
        // Ambil semua user dari repository
        return _repository.GetAll();
    }

    // ═══════════════════════════════════════════════════════
    // UPDATE USER
    // ═══════════════════════════════════════════════════════
    public User? UpdateUser(string id, string username)
    {
        try
        {
            // VALIDASI DbC
            // Username tidak boleh kosong/null.
            Validator.ValidateNotEmpty(username, "Username");

            // Cari user berdasarkan Id.
            var user = _repository.GetById(id);

            // user harus ada/exist.
            Validator.ValidateExists(user, $"User dengan Id '{id}'");

            // VALIDASI duplicate username
            // Kalau username lama sama dengan yang baru masih boleh update tanpa ganti username
            // Pakai !user!.Username.Equals()
            if (_repository.UsernameExists(username) &&
                !user!.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
            {
                // Jika username sudah ada dan bukan username milik user yang sedang diupdate, lempar exception
                throw new ArgumentException(
                    $"Username '{username}' sudah terdaftar"
                );
            }

            // Update username
            user.Username = username;

            // Simpan perubahan ke repository
            _repository.Update(user);

            // Return user yang sudah diupdate
            return user;
        }
        catch (Exception ex)
        {
            // Logging error
            LogError("UpdateUser gagal", ex);

            // Lempar ke middleware 
            throw;
        }
    }

    // ═══════════════════════════════════════════════════════
    // DELETE USER
    // ═══════════════════════════════════════════════════════
    public bool DeleteUser(string id)
    {
        try
        {
            // Cari user berdasarkan Id
            var user = _repository.GetById(id);

            // VALIDASI DbC
            // User harus ada/exist
            Validator.ValidateExists(user, $"User dengan Id '{id}'");

            // Hapus user dari repository
            _repository.Delete(id);

            // Return true jika berhasil dihapus
            return true;
        }
        catch (Exception ex)
        {
            // Logging error
            LogError("DeleteUser gagal", ex);

            // Lempar ke middlewares
            throw;
        }
    }
}
