
using Microsoft.Extensions.Configuration;
namespace myKisah.Utils;

// Class ini digunakan untuk mengatur path/lokasi file JSON.
// Contohnya lokasi file users.json, journals.json, characters.json, dan characterResponses.json.
public class FilePathConfig
{
    // Field ini digunakan untuk menyimpan konfigurasi aplikasi.
    // Konfigurasi ini biasanya berasal dari appsettings.json.
    private readonly IConfiguration _configuration;

    // Constructor ini akan dijalankan otomatis saat object FilePathConfig dibuat.
    // Parameter IConfiguration configuration akan dikirim otomatis oleh sistem
    // melalui Dependency Injection.
    public FilePathConfig(IConfiguration configuration)
    {
        // Design by Contract:
        // configuration tidak boleh null.
        // Kalau configuration null, program langsung melempar error.
        // Tujuannya supaya kesalahan lebih cepat diketahui.
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    // Property ini digunakan untuk mengambil lokasi file users.json.
    // Program akan membaca nilai dari appsettings.json dengan key:
    // StoragePaths:UsersFile
    //
    // Jika key tersebut tidak ditemukan,
    // maka program memakai nilai default yaitu "Data/users.json".
    public string UsersFile =>
        _configuration["StoragePaths:UsersFile"] ?? "Data/users.json";

    // Property ini digunakan untuk mengambil lokasi file journals.json.
    // Lokasi file dibaca dari appsettings.json pada key:
    // StoragePaths:JournalsFile
    //
    // Jika tidak ditemukan, maka memakai default "Data/journals.json".
    public string JournalsFile =>
        _configuration["StoragePaths:JournalsFile"] ?? "Data/journals.json";

    // Property ini digunakan untuk mengambil lokasi file characters.json.
    // Lokasi file dibaca dari appsettings.json pada key:
    // StoragePaths:CharactersFile
    // Jika tidak ditemukan, maka memakai default "Data/characters.json".
    public string CharactersFile =>
        _configuration["StoragePaths:CharactersFile"] ?? "Data/characters.json";

    // Property ini digunakan untuk mengambil lokasi file characterResponses.json.
    // Lokasi file dibaca dari appsettings.json pada key:
    // StoragePaths:ResponsesFile
    // Jika tidak ditemukan, maka memakai default "Data/characterResponses.json".
    public string ResponsesFile =>
        _configuration["StoragePaths:ResponsesFile"] ?? "Data/characterResponses.json";
}