// Import library untuk bisa pakai IConfiguration
using Microsoft.Extensions.Configuration;

// Namespace = "alamat" kelas ini di dalam project
namespace myKisah.Utils;

public class FilePathConfig
{
    // Field untuk menyimpan konfigurasi dari appsettings.json
    private readonly IConfiguration _configuration;

    // Constructor: dipanggil otomatis saat kelas ini dibuat
    // IConfiguration di-inject otomatis oleh sistem (Dependency Injection)
    public FilePathConfig(IConfiguration configuration)
    {
        // DbC: kalau configuration null, langsung lempar error
        // Supaya tidak ada bug tersembunyi di kemudian hari
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    // Property: baca path file users.json dari appsettings.json
    // Kalau tidak ketemu di config, pakai nilai default "Data/users.json"
    public string UsersFile => _configuration["StoragePaths:UsersFile"] ?? "Data/users.json";

    // Property: baca path file journals.json dari appsettings.json
    public string JournalsFile => _configuration["StoragePaths:JournalsFile"] ?? "Data/journals.json";

    // Property: baca path file characters.json dari appsettings.json
    public string CharactersFile => _configuration["StoragePaths:CharactersFile"] ?? "Data/characters.json";

    // Property: baca path file characterResponses.json dari appsettings.json
    public string ResponsesFile => _configuration["StoragePaths:ResponsesFile"] ?? "Data/characterResponses.json";
}