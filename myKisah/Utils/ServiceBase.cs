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
        // Validator untuk memeriksa input dan entity
        protected readonly ValidationHelper Validator = new ValidationHelper();

        // Nama service, wajib di-override di setiap service
        protected abstract string ServiceName { get; }

        // Shared logging error, bisa dipakai di semua service
        protected void LogError(string message, Exception ex)
        {
            Console.WriteLine($"[ERROR] [{ServiceName}] {message}: {ex.Message}");
        }
    }
}
