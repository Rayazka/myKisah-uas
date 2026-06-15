// Import library untuk baca/tulis file JSON
using System.Text.Json;
// Import untuk konversi enum (misal: "Happy" → MoodType.Happy)
using System.Text.Json.Serialization;

namespace myKisah.Utils;

public class JsonStorageHelper
{
    // _basePath menyimpan lokasi utama/root project aplikasi.
    // Jadi nanti file JSON akan dicari dari folder utama project.
    private readonly string _basePath;

    // _env menyimpan informasi environment aplikasi.
    // Misalnya lokasi root project, mode development, production, dan lain-lain.
    private readonly IHostEnvironment _env;

    // Constructor ini dijalankan otomatis saat object JsonStorageHelper dibuat.
    // FilePathConfig dan IHostEnvironment dikirim otomatis oleh Dependency Injection.
    public JsonStorageHelper(FilePathConfig filePathConfig, IHostEnvironment env)
    {
        // Simpan environment
        _env = env;

        // Mengambil lokasi root project sebagai base path.
        // Contoh: C:/Project/myKisah/
        // Jadi kalau filename = "Data/users.json",
        // maka full path-nya menjadi C:/Project/myKisah/Data/users.json.
        _basePath = env.ContentRootPath;
    }

    // Method ini digunakan untuk membaca isi file JSON.
    // <T> artinya method ini bersifat generic.
    // Jadi bisa dipakai untuk banyak tipe data:
    // User, Journal, Character, CharacterResponse, dan lainnya.
    public List<T> ReadJson<T>(string filename)
    {
        // Design by Contract:
        // filename wajib diisi.
        // Kalau filename null atau kosong, program langsung memberi error.
        if (string.IsNullOrEmpty(filename))
            throw new ArgumentNullException(nameof(filename));

        // Gabungkan base path + nama file jadi path lengkap
        // Menggabungkan root project dengan nama file.
        // Tujuannya supaya program mendapatkan alamat lengkap file JSON.
        var fullPath = Path.Combine(_basePath, filename);

        // Menampilkan alamat file ke console untuk membantu debugging.
        Console.WriteLine($"[DEBUG] FullPath: {fullPath}");

        // Mengecek apakah file JSON sudah ada atau belum.
        if (!File.Exists(fullPath))
        {
            Console.WriteLine($"[DEBUG] File tidak ditemukan, membuat baru...");

            // Buat folder dulu kalau belum ada
            // Mengambil lokasi folder dari fullPath.
            // Contoh: dari Data/users.json, foldernya adalah Data.
            var directory = Path.GetDirectoryName(fullPath);

            // Kalau misalkan folder belum ada, maka folder dibuat otomatis.
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Membuat file JSON baru dengan isi array kosong.
            // [] artinya belum ada data di dalam file.
            File.WriteAllText(fullPath, "[]");

            // Karena file baru masih kosong, maka method mengembalikan list kosong.
            return new List<T>();
        }

        // Membaca seluruh isi file JSON sebagai teks/string.
        var json = File.ReadAllText(fullPath);

        // Menampilkan sebagian isi JSON ke console untuk debugging.
        Console.WriteLine($"[DEBUG] JSON dibaca: {(json.Length > 100 ? json.Substring(0, 100) + "..." : json)}");

        // Kalau isi file kosong, langsung kembalikan list kosong.
        if (string.IsNullOrWhiteSpace(json))
            return new List<T>();

        // Pengaturan saat membaca JSON.
        var options = new JsonSerializerOptions
        {
            // Supaya enum bisa dibaca dari string.
            // Contoh: "Happy" menjadi MoodType.Happy.
            Converters = { new JsonStringEnumConverter() },

            // Supaya huruf besar/kecil pada nama property tidak bermasalah.
            // Contoh: Username dan username tetap dianggap sama.
            PropertyNameCaseInsensitive = true,

            // Supaya format property mengikuti gaya camelCase.
            // Contoh: UserId menjadi userId.
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Mengubah teks JSON menjadi List<T>.
        // Contoh: isi users.json diubah menjadi List<User>.
        var result = JsonSerializer.Deserialize<List<T>>(json, options);

        // Menampilkan jumlah data yang berhasil dibaca.
        Console.WriteLine($"[DEBUG] Deserialized count: {result?.Count ?? 0}");

        // Jika hasil deserialize null, tetap aman karena dikembalikan list kosong.
        return result ?? new List<T>();
    }

    // Method ini digunakan untuk menulis data ke file JSON.
    // <T> membuat method ini bisa dipakai untuk berbagai jenis data.
    public void WriteJson<T>(string filename, List<T> data)
    {
        // DbC: filename tidak boleh null atau kosong
        if (string.IsNullOrEmpty(filename))
            throw new ArgumentNullException(nameof(filename));

        // DbC: data tidak boleh null
        // data tidak boleh null karena data inilah yang akan disimpan.
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        // Menggabungkan root project dengan nama file.
        var fullPath = Path.Combine(_basePath, filename);

        // Debug untuk memastikan file ditulis ke lokasi yang benar.
        Console.WriteLine($"[DEBUG] WriteJson FullPath: {fullPath}");
        Console.WriteLine($"[DEBUG] WriteJson Count: {data.Count}");

        // Pastikan folder tujuan sudah ada, kalau belum buat dulu
        // Mengambil folder tujuan penyimpanan file.
        var directory = Path.GetDirectoryName(fullPath);

        // Jika folder belum ada, maka folder dibuat otomatis.
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        // Pengaturan saat menulis data menjadi JSON.
        var options = new JsonSerializerOptions
        {
            // Membuat format JSON lebih rapi dan mudah dibaca.
            WriteIndented = true,
            // Membuat nama property di JSON menjadi camelCase.
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

            // Supaya enum disimpan sebagai string.
            // Contoh: MoodType.Happy disimpan sebagai "Happy".
            Converters = { new JsonStringEnumConverter() }
        };

        // Mengubah List<T> menjadi teks JSON.
        var json = JsonSerializer.Serialize(data, options);

        // Menulis teks JSON ke file tujuan.
        File.WriteAllText(fullPath, json);

        // Menampilkan pesan bahwa data berhasil disimpan.
        Console.WriteLine($"[DEBUG] WriteJson berhasil: {data.Count} item tersimpan");
    }
}