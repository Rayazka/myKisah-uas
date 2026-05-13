using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using System.Diagnostics;
using myKisah.Utils;
namespace myKisah.Tests.Storage;


public class TestModel
{
    // Id digunakan sebagai identitas data dummy.
    public string Id { get; set; } = "";

    // Name digunakan sebagai isi data dummy.
    public string Name { get; set; } = "";
}

// Class ini berisi kumpulan test untuk bagian storage milik Rafly.
// IDisposable dipakai agar setelah test selesai,
// folder temporary yang dibuat bisa dihapus kembali.
public class Rafly_StorageTests : IDisposable
{
    // Object utama yang akan diuji.
    // JsonStorageHelper bertugas membaca dan menulis file JSON.
    private readonly JsonStorageHelper _storage;

    // Object untuk membaca path file dari konfigurasi.
    private readonly FilePathConfig _filePathConfig;

    // Folder sementara untuk menyimpan file JSON saat testing.
    // Tujuannya supaya test tidak mengganggu file asli aplikasi.
    private readonly string _tempDir;

    // Constructor ini akan dijalankan sebelum setiap test dimulai.
    public Rafly_StorageTests()
    {
        // Membuat folder temporary dengan nama unik.
        // Guid.NewGuid() dipakai agar nama folder tidak bentrok dengan test lain.
        _tempDir = Path.Combine(Path.GetTempPath(), "myKisah_test_" + Guid.NewGuid());

        // Membuat folder temporary tersebut.
        Directory.CreateDirectory(_tempDir);

        // Membuat konfigurasi palsu untuk testing.
        // Konfigurasi ini menggantikan appsettings.json asli.
        var configData = new Dictionary<string, string?>
        {
            { "StoragePaths:UsersFile", "Data/users.json" },
            { "StoragePaths:JournalsFile", "Data/journals.json" },
            { "StoragePaths:CharactersFile", "Data/characters.json" },
            { "StoragePaths:ResponsesFile", "Data/characterResponses.json" }
        };

        // Membuat object konfigurasi dari data palsu di atas.
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Membuat FilePathConfig menggunakan konfigurasi palsu.
        // Jadi test bisa mengecek apakah path terbaca dengan benar.
        _filePathConfig = new FilePathConfig(config);

        // Membuat mock untuk IHostEnvironment.
        // Mock ini membuat seolah-olah root project berada di folder temporary.
        var mockEnv = new Mock<IHostEnvironment>();

        // Ketika ContentRootPath dipanggil,
        // hasilnya diarahkan ke folder temporary yang sudah dibuat.
        mockEnv.Setup(e => e.ContentRootPath).Returns(_tempDir);

        // Membuat JsonStorageHelper dengan dependency yang sudah disiapkan.
        // Dependency-nya adalah FilePathConfig dan environment palsu.
        _storage = new JsonStorageHelper(_filePathConfig, mockEnv.Object);
    }

   
    // UNIT TEST 1
    // Tujuan: memastikan ReadJson bisa membaca file JSON yang sudah ada.
    
    [Fact]
    public void ReadJson_ExistingFile_ReturnsData()
    {
        // Arrange / Setup:
        // Menentukan nama file JSON untuk testing.
        var filename = "test_read.json";

        // Membuat path lengkap menuju file testing.
        var fullPath = Path.Combine(_tempDir, filename);

        // Membuat file JSON berisi 3 data dummy.
        File.WriteAllText(fullPath, """
            [
                {"id": "1", "name": "Item1"},
                {"id": "2", "name": "Item2"},
                {"id": "3", "name": "Item3"}
            ]
            """);

        // Act / Action:
        // Membaca file JSON menggunakan JsonStorageHelper.
        var result = _storage.ReadJson<TestModel>(filename);

        // Assert:
        // Mengecek apakah hasilnya tidak null.
        Assert.NotNull(result);

        // Mengecek apakah jumlah data yang terbaca adalah 3.
        Assert.Equal(3, result.Count);

        // Mengecek apakah data pertama memiliki nama "Item1".
        Assert.Equal("Item1", result[0].Name);
    }

    
    // UNIT TEST 2
    // Tujuan: memastikan ReadJson tetap aman saat file belum ada.
    
    [Fact]
    public void ReadJson_MissingFile_ReturnsEmpty()
    {
        // Arrange:
        // Menentukan nama file yang belum ada.
        var filename = "nonexist.json";
        var fullPath = Path.Combine(_tempDir, filename);

        // Jika file kebetulan ada, hapus dulu.
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        // Act:
        // Mencoba membaca file yang belum ada.
        var result = _storage.ReadJson<TestModel>(filename);

        // Assert:
        // Hasil tetap tidak boleh null.
        Assert.NotNull(result);

        // Karena file belum punya data, hasilnya harus list kosong.
        Assert.Empty(result);

        // File harus otomatis dibuat oleh JsonStorageHelper.
        Assert.True(File.Exists(fullPath));
    }

   
    // UNIT TEST 3
    // Tujuan: memastikan WriteJson bisa menyimpan data,
    // lalu ReadJson bisa membaca kembali data yang sama.
    
    [Fact]
    public void WriteJson_ValidData_PersistsCorrectly()
    {
        // Arrange:
        // Menentukan nama file untuk testing.
        var filename = "test_write.json";

        // Membuat 5 data dummy yang akan disimpan ke JSON.
        var data = new List<TestModel>
        {
            new TestModel { Id = "1", Name = "Item1" },
            new TestModel { Id = "2", Name = "Item2" },
            new TestModel { Id = "3", Name = "Item3" },
            new TestModel { Id = "4", Name = "Item4" },
            new TestModel { Id = "5", Name = "Item5" }
        };

        // Act:
        // Menulis data ke file JSON.
        _storage.WriteJson(filename, data);

        // Membaca kembali data dari file JSON.
        var result = _storage.ReadJson<TestModel>(filename);

        // Assert:
        // Jumlah data yang dibaca harus 5.
        Assert.Equal(5, result.Count);

        // Data pertama harus sama dengan data yang ditulis.
        Assert.Equal("Item1", result[0].Name);

        // Data terakhir juga harus sama.
        Assert.Equal("Item5", result[4].Name);
    }

    
    // UNIT TEST 4
    // Tujuan: memastikan ReadJson menolak filename yang null.
    // Ini termasuk penerapan Design by Contract.
   
    [Fact]
    public void ReadJson_NullFilename_ThrowsException()
    {
        // Assert:
        // Kalau filename null, method harus melempar ArgumentNullException.
        Assert.Throws<ArgumentNullException>(() =>
            _storage.ReadJson<TestModel>(null!));
    }

    
    // UNIT TEST 5
    // Tujuan: memastikan WriteJson menolak data yang null.
    // Ini juga termasuk Design by Contract.
    
    [Fact]
    public void WriteJson_NullData_ThrowsException()
    {
        // Assert:
        // Kalau data null, method harus melempar ArgumentNullException.
        Assert.Throws<ArgumentNullException>(() =>
            _storage.WriteJson<TestModel>("test.json", null!));
    }

    
    // UNIT TEST 6
    // Tujuan: memastikan FilePathConfig berhasil membaca path dari konfigurasi.
    
    [Fact]
    public void FilePathConfig_LoadsPathsFromConfig()
    {
        // Arrange:
        // Membuat konfigurasi palsu berisi StoragePaths.
        var configData = new Dictionary<string, string?>
        {
            { "StoragePaths:UsersFile", "Data/users.json" },
            { "StoragePaths:JournalsFile", "Data/journals.json" },
            { "StoragePaths:CharactersFile", "Data/characters.json" },
            { "StoragePaths:ResponsesFile", "Data/characterResponses.json" }
        };

        // Membuat object IConfiguration dari data palsu.
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Membuat FilePathConfig menggunakan konfigurasi tersebut.
        var filePathConfig = new FilePathConfig(config);

        // Assert:
        // Semua property harus sesuai dengan isi konfigurasi.
        Assert.Equal("Data/users.json", filePathConfig.UsersFile);
        Assert.Equal("Data/journals.json", filePathConfig.JournalsFile);
        Assert.Equal("Data/characters.json", filePathConfig.CharactersFile);
        Assert.Equal("Data/characterResponses.json", filePathConfig.ResponsesFile);
    }

    
    // PERFORMANCE TEST 1
    // Tujuan: menguji apakah ReadJson tetap cepat
    // saat membaca data besar, sekitar 1000 item.
    
    [Fact]
    public void ReadJson_100KB_Under100ms()
    {
        // Arrange:
        // Menentukan nama file untuk performance test.
        var filename = "perf_test_100kb.json";

        // Membuat 1000 item dummy.
        // PaddingDataForSize dipakai agar ukuran data lebih besar.
        var items = Enumerable.Range(1, 1000)
            .Select(i => new TestModel
            {
                Id = i.ToString(),
                Name = $"Item{i}PaddingDataForSize"
            })
            .ToList();

        // Menulis data dummy ke file JSON.
        _storage.WriteJson(filename, items);

        // Warmup:
        // Membaca sekali sebelum pengukuran.
        // Tujuannya agar hasil pengukuran lebih stabil.
        _storage.ReadJson<TestModel>(filename);

        // Act:
        // Mulai menghitung waktu baca file JSON.
        var sw = Stopwatch.StartNew();

        // Membaca data dari file JSON.
        var result = _storage.ReadJson<TestModel>(filename);

        // Menghentikan stopwatch setelah proses baca selesai.
        sw.Stop();

        // Assert:
        // Jumlah data yang terbaca harus 1000.
        Assert.Equal(1000, result.Count);

        // Waktu baca harus kurang dari 100 ms.
        Assert.True(sw.ElapsedMilliseconds < 100,
            $"ReadJson terlalu lambat: {sw.ElapsedMilliseconds}ms (max 100ms)");
    }

    
    // PERFORMANCE TEST 2
    // Tujuan: menguji kecepatan proses tulis dan baca kembali.
    // Ini disebut round-trip, karena data ditulis lalu dibaca lagi.
   
    [Fact]
    public void WriteRead_RoundTrip_100Items_Under200ms()
    {
        // Arrange:
        // Menentukan nama file untuk testing.
        var filename = "perf_roundtrip.json";

        // Membuat 100 item dummy.
        var items = Enumerable.Range(1, 100)
            .Select(i => new TestModel
            {
                Id = i.ToString(),
                Name = $"Item{i}"
            })
            .ToList();

        // Act:
        // Mulai menghitung waktu proses write + read.
        var sw = Stopwatch.StartNew();

        // Menulis 100 item ke file JSON.
        _storage.WriteJson(filename, items);

        // Membaca kembali file JSON yang baru ditulis.
        var result = _storage.ReadJson<TestModel>(filename);

        // Menghentikan stopwatch.
        sw.Stop();

        // Assert:
        // Proses write + read harus selesai kurang dari 200 ms.
        Assert.True(sw.ElapsedMilliseconds < 200,
            $"Round-trip terlalu lambat: {sw.ElapsedMilliseconds}ms (max 200ms)");
    }

    // Dispose akan berjalan setelah test selesai.
    // Fungsinya untuk membersihkan folder temporary.
    public void Dispose()
    {
        // Jika folder temporary masih ada, maka folder dihapus.
        // recursive: true artinya semua isi folder ikut dihapus.
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }
}