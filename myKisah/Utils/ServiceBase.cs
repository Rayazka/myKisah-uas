namespace myKisah.Utils;

// ═══════════════════════════════════════════════════════════
// KELAS: ServiceBase
// DOMAIN: Shared Utilities (dipakai semua Service)
// TEKNIK: Code Reuse
// PENANGGUNG JAWAB: Josefhint (Jojo)
// ═══════════════════════════════════════════════════════════
//
// 📘 APA INI?
// Abstract base class untuk SEMUA Service (UserService, JournalService,
// CharacterService). Menyediakan shared functionality agar tidak
// diulang di setiap service.
//
// 🧠 KENAPA CODE REUSE?
// Tanpa ServiceBase, setiap service akan punya kode yang sama:
// - Field ValidationHelper
// - Method LogError()
// - Property ServiceName
// Dengan ServiceBase: semua di satu tempat, konsisten.
//
// Cara pakai:
// public class UserService : ServiceBase, IUserService
// {
//     protected override string ServiceName => "UserService";
//     // ... implementasi
// }
//
// 📋 TODO:
// [ ] 1. Constructor: instantiate ValidationHelper ke field _validator
// [ ] 2. Definisikan abstract property ServiceName (setiap service override)
// [ ] 3. Implement method LogError(string message, Exception ex):
//        - Console.WriteLine($"[ERROR] [{ServiceName}] {message}: {ex.Message}")
//        - (Bisa juga pakai ILogger kalau mau, tapi Console.WriteLine cukup untuk fase ini)
//
// Tips:
// - Abstract class, bukan interface — karena ada implementasi shared
// - ServiceName dipakai di log untuk identifikasi service mana yang error
//
// Referensi: Task_myKisah.md baris 177-185

public abstract class ServiceBase
{
    protected readonly ValidationHelper Validator = new();

    protected abstract string ServiceName { get; }

    protected void LogError(string message, Exception ex)
    {
        // TODO: Tampilkan log error dengan format [ERROR] [ServiceName] message: exception
        throw new NotImplementedException("TODO: Implement LogError");
    }
}
