namespace myKisah.Utils;

// ═══════════════════════════════════════════════════════════
// KELAS: FilePathConfig
// DOMAIN: Storage Layer (dipakai oleh JsonStorageHelper)
// TEKNIK: Runtime Configuration
// PENANGGUNG JAWAB: Rafly Putra
// ═══════════════════════════════════════════════════════════
//
// 📘 APA INI?
// Kelas yang membaca konfigurasi path file JSON dari appsettings.json.
// Path file TIDAK di-hardcode di kode — bisa diubah di config tanpa recompile.
//
// 🧠 KENAPA RUNTIME CONFIGURATION?
// Masalah: Tanpa ini, path file seperti "Data/users.json" di-hardcode
//   di setiap repository. Kalau mau ganti lokasi file, harus ubah kode.
// Solusi: Baca dari appsettings.json section "StoragePaths".
//   - Ubah path file = cukup edit appsettings.json
//   - Environment berbeda bisa punya path berbeda (Dev/Prod)
//   - Semua path terpusat di satu tempat
//
// Section appsettings.json yang dibaca:
// {
//   "StoragePaths": {
//     "UsersFile": "Data/users.json",
//     "JournalsFile": "Data/journals.json",
//     "CharactersFile": "Data/characters.json",
//     "ResponsesFile": "Data/characterResponses.json"
//   }
// }
//
// 📋 TODO:
// [ ] 1. Constructor: terima IConfiguration via DI
// [ ] 2. Baca section "StoragePaths" dari config
// [ ] 3. Implement 4 property getter:
//        - UsersFile     → config["StoragePaths:UsersFile"]
//        - JournalsFile  → config["StoragePaths:JournalsFile"]
//        - CharactersFile → config["StoragePaths:CharactersFile"]
//        - ResponsesFile → config["StoragePaths:ResponsesFile"]
// [ ] 4. Property harus readonly (hanya get), value dibaca sekali di constructor
//
// Tips:
// - Pakai IConfiguration.GetSection("StoragePaths")
// - Bisa simpan nilai di private field saat constructor
// - Atau baca langsung IConfiguration di getter (lebih sederhana)
//
// Referensi: Task_myKisah.md baris 140-164

public class FilePathConfig
{
    private readonly IConfiguration _configuration;

    public FilePathConfig(IConfiguration configuration)
    {
        // TODO: Simpan IConfiguration
        throw new NotImplementedException("TODO: Implement constructor");
    }

    // TODO: Implement property getter — baca dari config section "StoragePaths"
    public string UsersFile => throw new NotImplementedException("TODO: Read from config");
    public string JournalsFile => throw new NotImplementedException("TODO: Read from config");
    public string CharactersFile => throw new NotImplementedException("TODO: Read from config");
    public string ResponsesFile => throw new NotImplementedException("TODO: Read from config");
}
