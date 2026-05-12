// Import library yang dibutuhkan untuk testing
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using System.Diagnostics;
using myKisah.Utils;

namespace myKisah.Tests.Storage;

// Model sederhana untuk keperluan testing
public class TestModel
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
}

public class Rafly_StorageTests : IDisposable
{
    private readonly JsonStorageHelper _storage;
    private readonly FilePathConfig _filePathConfig;
    private readonly string _tempDir;

    public Rafly_StorageTests()
    {
        // Buat folder temporary untuk testing
        // Supaya test tidak mengganggu data asli aplikasi
        _tempDir = Path.Combine(Path.GetTempPath(), "myKisah_test_" + Guid.NewGuid());
        Directory.CreateDirectory(_tempDir);

        // Setup konfigurasi palsu (in-memory) untuk testing
        var configData = new Dictionary<string, string?>
        {
            { "StoragePaths:UsersFile", "Data/users.json" },
            { "StoragePaths:JournalsFile", "Data/journals.json" },
            { "StoragePaths:CharactersFile", "Data/characters.json" },
            { "StoragePaths:ResponsesFile", "Data/characterResponses.json" }
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Buat FilePathConfig dengan config palsu
        _filePathConfig = new FilePathConfig(config);

        // Buat mock IHostEnvironment
        // Supaya ContentRootPath mengarah ke folder temporary
        var mockEnv = new Mock<IHostEnvironment>();
        mockEnv.Setup(e => e.ContentRootPath).Returns(_tempDir);

        // Buat JsonStorageHelper dengan dependency yang sudah disiapkan
        _storage = new JsonStorageHelper(_filePathConfig, mockEnv.Object);
    }

    // ═══════════════════════════════════════
    // UNIT TEST 1: Baca file yang sudah ada
    // ═══════════════════════════════════════
    [Fact]
    public void ReadJson_ExistingFile_ReturnsData()
    {
        // Setup: buat file JSON dengan 3 item
        var filename = "test_read.json";
        var fullPath = Path.Combine(_tempDir, filename);
        File.WriteAllText(fullPath, """
            [
                {"id": "1", "name": "Item1"},
                {"id": "2", "name": "Item2"},
                {"id": "3", "name": "Item3"}
            ]
            """);

        // Action: baca file JSON
        var result = _storage.ReadJson<TestModel>(filename);

        // Assert: harus dapat 3 item
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("Item1", result[0].Name);
    }

    // ═══════════════════════════════════════
    // UNIT TEST 2: Baca file yang belum ada
    // ═══════════════════════════════════════
    [Fact]
    public void ReadJson_MissingFile_ReturnsEmpty()
    {
        // Setup: pastikan file tidak ada
        var filename = "nonexist.json";
        var fullPath = Path.Combine(_tempDir, filename);
        if (File.Exists(fullPath)) File.Delete(fullPath);

        // Action: baca file yang tidak ada
        var result = _storage.ReadJson<TestModel>(filename);

        // Assert: harus return list kosong dan file otomatis dibuat
        Assert.NotNull(result);
        Assert.Empty(result);
        Assert.True(File.Exists(fullPath)); // File otomatis terbuat
    }

    // ═══════════════════════════════════════
    // UNIT TEST 3: Tulis lalu baca kembali
    // ═══════════════════════════════════════
    [Fact]
    public void WriteJson_ValidData_PersistsCorrectly()
    {
        // Setup: buat 5 item untuk ditulis
        var filename = "test_write.json";
        var data = new List<TestModel>
        {
            new TestModel { Id = "1", Name = "Item1" },
            new TestModel { Id = "2", Name = "Item2" },
            new TestModel { Id = "3", Name = "Item3" },
            new TestModel { Id = "4", Name = "Item4" },
            new TestModel { Id = "5", Name = "Item5" }
        };

        // Action: tulis lalu baca kembali
        _storage.WriteJson(filename, data);
        var result = _storage.ReadJson<TestModel>(filename);

        // Assert: data yang dibaca harus sama dengan yang ditulis
        Assert.Equal(5, result.Count);
        Assert.Equal("Item1", result[0].Name);
        Assert.Equal("Item5", result[4].Name);
    }

    // ═══════════════════════════════════════
    // UNIT TEST 4: Filename null harus error
    // ═══════════════════════════════════════
    [Fact]
    public void ReadJson_NullFilename_ThrowsException()
    {
        // Assert: harus throw ArgumentNullException kalau filename null
        Assert.Throws<ArgumentNullException>(() =>
            _storage.ReadJson<TestModel>(null!));
    }

    // ═══════════════════════════════════════
    // UNIT TEST 5: Data null harus error
    // ═══════════════════════════════════════
    [Fact]
    public void WriteJson_NullData_ThrowsException()
    {
        // Assert: harus throw ArgumentNullException kalau data null
        Assert.Throws<ArgumentNullException>(() =>
            _storage.WriteJson<TestModel>("test.json", null!));
    }

    // ═══════════════════════════════════════
    // UNIT TEST 6: FilePathConfig baca dari config
    // ═══════════════════════════════════════
    [Fact]
    public void FilePathConfig_LoadsPathsFromConfig()
    {
        // Setup: buat config dengan StoragePaths
        var configData = new Dictionary<string, string?>
        {
            { "StoragePaths:UsersFile", "Data/users.json" },
            { "StoragePaths:JournalsFile", "Data/journals.json" },
            { "StoragePaths:CharactersFile", "Data/characters.json" },
            { "StoragePaths:ResponsesFile", "Data/characterResponses.json" }
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        var filePathConfig = new FilePathConfig(config);

        // Assert: semua property harus terisi dengan benar
        Assert.Equal("Data/users.json", filePathConfig.UsersFile);
        Assert.Equal("Data/journals.json", filePathConfig.JournalsFile);
        Assert.Equal("Data/characters.json", filePathConfig.CharactersFile);
        Assert.Equal("Data/characterResponses.json", filePathConfig.ResponsesFile);
    }

    // ═══════════════════════════════════════
    // PERFORMANCE TEST 1: Baca file 100KB
    // ═══════════════════════════════════════
    [Fact]
    public void ReadJson_100KB_Under100ms()
    {
        // Setup: buat file JSON ~100KB (banyak item)
        var filename = "perf_test_100kb.json";
        var fullPath = Path.Combine(_tempDir, filename);
        var items = Enumerable.Range(1, 1000)
            .Select(i => new TestModel { Id = i.ToString(), Name = $"Item{i}PaddingDataForSize" })
            .ToList();
        _storage.WriteJson(filename, items);

        // Warmup: baca sekali dulu supaya tidak terhitung cold start
        _storage.ReadJson<TestModel>(filename);

        // Action: ukur waktu baca
        var sw = Stopwatch.StartNew();
        var result = _storage.ReadJson<TestModel>(filename);
        sw.Stop();

        // Assert: harus selesai dalam 100ms
        Assert.Equal(1000, result.Count);
        Assert.True(sw.ElapsedMilliseconds < 100,
            $"ReadJson terlalu lambat: {sw.ElapsedMilliseconds}ms (max 100ms)");
    }

    // ═══════════════════════════════════════
    // PERFORMANCE TEST 2: Write + Read 100 item
    // ═══════════════════════════════════════
    [Fact]
    public void WriteRead_RoundTrip_100Items_Under50ms()
    {
        // Setup: buat 100 item
        var filename = "perf_roundtrip.json";
        var items = Enumerable.Range(1, 100)
            .Select(i => new TestModel { Id = i.ToString(), Name = $"Item{i}" })
            .ToList();

        // Action: ukur waktu write + read sekaligus
        var sw = Stopwatch.StartNew();
        _storage.WriteJson(filename, items);
        var result = _storage.ReadJson<TestModel>(filename);
        sw.Stop();

        // Assert: total harus selesai dalam 200ms
        Assert.True(sw.ElapsedMilliseconds < 200,
        $"Round-trip terlalu lambat: {sw.ElapsedMilliseconds}ms (max 200ms)");
    }

    // Cleanup: hapus folder temporary setelah semua test selesai
    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }
}