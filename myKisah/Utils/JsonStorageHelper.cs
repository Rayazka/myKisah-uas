// Import library untuk baca/tulis file JSON
using System.Text.Json;
// Import untuk konversi enum (misal: "Happy" → MoodType.Happy)
using System.Text.Json.Serialization;

namespace myKisah.Utils;

public class JsonStorageHelper
{
    // Field untuk menyimpan folder utama tempat file JSON disimpan
    private readonly string _basePath;

    // Field untuk menyimpan environment aplikasi
    private readonly IHostEnvironment _env;

    // Constructor: dipanggil saat kelas ini dibuat
    // Menerima FilePathConfig dan IHostEnvironment via Dependency Injection
    public JsonStorageHelper(FilePathConfig filePathConfig, IHostEnvironment env)
    {
        // Simpan environment
        _env = env;

        // Gunakan ContentRootPath sebagai base path
        // Lebih akurat dari GetCurrentDirectory() karena mengikuti root project
        _basePath = env.ContentRootPath;
    }

    // Method generic untuk MEMBACA file JSON
    // <T> bisa diisi apapun: User, Journal, Character, dll
    public List<T> ReadJson<T>(string filename)
    {
        // DbC: filename tidak boleh null atau kosong
        if (string.IsNullOrEmpty(filename))
            throw new ArgumentNullException(nameof(filename));

        // Gabungkan base path + nama file jadi path lengkap
        var fullPath = Path.Combine(_basePath, filename);

        // Debug: tampilkan path lengkap di console
        Console.WriteLine($"[DEBUG] FullPath: {fullPath}");

        // Kalau file belum ada → auto-create dengan array kosong
        if (!File.Exists(fullPath))
        {
            Console.WriteLine($"[DEBUG] File tidak ditemukan, membuat baru...");

            // Buat folder dulu kalau belum ada
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Buat file baru dengan isi array kosong "[]"
            File.WriteAllText(fullPath, "[]");

            // Kembalikan list kosong
            return new List<T>();
        }

        // Baca isi file JSON sebagai string
        var json = File.ReadAllText(fullPath);

        // Debug: tampilkan 100 karakter pertama dari JSON
        Console.WriteLine($"[DEBUG] JSON dibaca: {(json.Length > 100 ? json.Substring(0, 100) + "..." : json)}");

        // Kalau file kosong → return empty list
        if (string.IsNullOrWhiteSpace(json))
            return new List<T>();

        // Setting untuk deserialize JSON
        // JsonStringEnumConverter: supaya "Happy" bisa dibaca sebagai MoodType.Happy
        // PropertyNameCaseInsensitive: supaya "Username" dan "username" dianggap sama
        // CamelCase: supaya format JSON mengikuti camelCase (userId, bukan UserId)
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Ubah string JSON menjadi List<T>
        var result = JsonSerializer.Deserialize<List<T>>(json, options);

        // Debug: tampilkan jumlah data yang berhasil dibaca
        Console.WriteLine($"[DEBUG] Deserialized count: {result?.Count ?? 0}");

        // Kalau hasilnya null, kembalikan list kosong
        return result ?? new List<T>();
    }

    // Method generic untuk MENULIS data ke file JSON
    // <T> bisa diisi apapun: User, Journal, Character, dll
    public void WriteJson<T>(string filename, List<T> data)
    {
        // DbC: filename tidak boleh null atau kosong
        if (string.IsNullOrEmpty(filename))
            throw new ArgumentNullException(nameof(filename));

        // DbC: data tidak boleh null
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        // Gabungkan base path + nama file jadi path lengkap
        var fullPath = Path.Combine(_basePath, filename);

        // Debug: tampilkan path dan jumlah data
        Console.WriteLine($"[DEBUG] WriteJson FullPath: {fullPath}");
        Console.WriteLine($"[DEBUG] WriteJson Count: {data.Count}");

        // Pastikan folder tujuan sudah ada, kalau belum buat dulu
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        // Setting untuk serialize JSON
        // WriteIndented: supaya JSON rapi dan mudah dibaca manusia
        // Pakai options SAMA seperti ReadJson supaya konsisten
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

        // Ubah List<T> menjadi string JSON
        var json = JsonSerializer.Serialize(data, options);

        // Tulis string JSON ke file
        File.WriteAllText(fullPath, json);

        // Debug: konfirmasi berhasil tersimpan
        Console.WriteLine($"[DEBUG] WriteJson berhasil: {data.Count} item tersimpan");
    }
}