using System;

namespace myKisah.Utils
{
    // ═══════════════════════════════════════════════════════════
    // KELAS: ServiceBase
    // DOMAIN: Shared Utilities (dipakai semua Service)
    // TEKNIK: Code Reuse
    // PENANGGUNG JAWAB: Josefhint (Jojo)
    // ═══════════════════════════════════════════════════════════


    // Abstract base class untuk SEMUA Service (UserService, JournalService,
    // CharacterService). Menyediakan shared functionality agar tidak
    // diulang di setiap service.

    public abstract class ServiceBase
    {
        protected readonly ValidationHelper Validator = new ValidationHelper();

        // Harus di-override tiap service, misal "UserService"
        protected abstract string ServiceName { get; }

        // Logging sederhana
        protected void LogError(string message, Exception ex)
        {
            Console.WriteLine($"[ERROR] [{ServiceName}] {message}: {ex.Message}");
        }
    }
}