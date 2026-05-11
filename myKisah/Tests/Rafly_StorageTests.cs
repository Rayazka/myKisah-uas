// ═══════════════════════════════════════════════════════════
// TEST PLAN: JsonStorageHelper + FilePathConfig
// DOMAIN: Storage Layer
// PENANGGUNG JAWAB: Rafly Putra
// ═══════════════════════════════════════════════════════════
//
// 📘 PETUNJUK: Setup xUnit project, gunakan temp directory untuk file JSON.
//
// ═══════════════════════════════════════════════════════════
// 📋 TEST CASE LIST (6 test + 2 performance):
// ═══════════════════════════════════════════════════════════
//
// [ ] ReadJson_ExistingFile_ReturnsData
//     Setup: buat file JSON dengan data, panggil ReadJson
//     Assert: return list dengan count sesuai data
//
// [ ] ReadJson_MissingFile_ReturnsEmpty
//     Setup: pastikan file tidak ada
//     Assert: return List<T> kosong (Count=0), bukan null
//     Assert: file otomatis dibuat dengan isi "[]"
//
// [ ] WriteJson_ValidData_PersistsCorrectly
//     Action: WriteJson data → ReadJson
//     Assert: data yang dibaca == data yang ditulis
//
// [ ] ReadJson_NullFilename_ThrowsException
//     Assert: throws ArgumentNullException
//
// [ ] WriteJson_NullData_ThrowsException
//     Assert: throws ArgumentNullException
//
// [ ] FilePathConfig_LoadsPathsFromConfig
//     Setup: in-memory config dengan section StoragePaths
//     Assert: UsersFile, JournalsFile, CharactersFile, ResponsesFile tidak null
//
// PERFORMANCE:
// [ ] ReadJson_100KB_Under100ms
//     Generate JSON ~100KB, ReadJson, assert elapsed < 100ms
//
// [ ] WriteRead_RoundTrip_100Items_Under50ms
//     Generate 100 item, Write + Read, assert total < 50ms
//
// ═══════════════════════════════════════════════════════════
// CONTOH IMPLEMENTASI:
// ═══════════════════════════════════════════════════════════
//
// using Xunit;
// using myKisah.Utils;
// using Microsoft.Extensions.Configuration;
// using System.Diagnostics;
//
// public class JsonStorageHelperTests
// {
//     private readonly string _tempDir;
//     private readonly JsonStorageHelper _storage;
//
//     public JsonStorageHelperTests()
//     {
//         _tempDir = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}");
//         Directory.CreateDirectory(_tempDir);
//         var config = new ConfigurationBuilder()
//             .AddInMemoryCollection(new Dictionary<string, string?> { ... })
//             .Build();
//         var filePathConfig = new FilePathConfig(config);
//         _storage = new JsonStorageHelper(filePathConfig);
//     }
//
//     [Fact]
//     public void ReadJson_MissingFile_ReturnsEmpty()
//     {
//         var result = _storage.ReadJson<TestModel>("nonexist.json");
//         Assert.NotNull(result);
//         Assert.Empty(result);
//     }
// }
