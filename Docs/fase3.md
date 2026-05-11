# Fase 3 — Testing & Performance Benchmarks myKisah

> **Status:** Test plan sudah dibuat, TIM HARUS IMPLEMENTASI TEST.
> **Prasyarat:** Semua file Fase 2 harus sudah di-merge ke `main`.
> **Setup:** Buat test project terpisah: `dotnet new xunit -n myKisah.Tests`
> **Pengerjaan:** Masing-masing di branch pribadi.
> **Referensi Utama:** `Task_myKisah.md` baris 244-306

---

## SETUP TEST PROJECT

Sebelum mulai, setiap anggota harus setup project test:

```bash
# 1. Buat test project (dari folder myKisah/)
dotnet new xunit -n myKisah.Tests -o Tests

# 2. Tambah reference ke project utama
cd Tests
dotnet add reference ../myKisah.csproj

# 3. Install Moq (untuk mocking)
dotnet add package Moq
```

Struktur test project:
```
myKisah.Tests/
├── myKisah.Tests.csproj
├── Journal/
│   └── JournalServiceTests.cs
│   └── JournalStateMachineTests.cs
├── Character/
│   └── CharacterServiceTests.cs
├── User/
│   └── UserServiceTests.cs
├── Storage/
│   └── JsonStorageHelperTests.cs
│   └── FilePathConfigTests.cs
├── Utils/
│   └── ValidationHelperTests.cs
│   └── MiddlewareTests.cs
└── Performance/
    └── PerformanceBenchmarks.cs
```

---

---

## RAYAZKA ARIS — Journal Tests (11 unit + 4 performance)

Referensi test plan: `myKisah/Tests/Rayazka_JournalTests.cs`

---

### A. Unit Tests — JournalService (5 test)

#### Test 1: `CreateJournal_ValidInput_Success`
| Atribut | Detail |
|---|---|
| **Tujuan** | Verifikasi CreateJournal sukses dengan input valid |
| **Mock** | `IConfiguration` → return `MaxContentLength=2000`, `ValidMoods=["Happy","Sad","Angry","Anxious","Calm"]` |
| **Action** | `_service.CreateJournal("user1", "Judul", "Konten", MoodType.Happy)` |
| **Assert** | `result != null`, `result.State == JournalState.Draft`, `result.UserId == "user1"`, `result.Title == "Judul"` |

**Kode:**
```csharp
[Fact]
public void CreateJournal_ValidInput_Success()
{
    // Arrange
    var mockConfigSection = new Mock<IConfigurationSection>();
    mockConfigSection.Setup(s => s.Get<string[]>())
        .Returns(new[] { "Happy", "Sad", "Angry", "Anxious", "Calm" });
    _mockConfig.Setup(c => c.GetSection("JournalConfig:ValidMoods"))
        .Returns(mockConfigSection.Object);
    _mockConfig.Setup(c => c.GetValue<int>("JournalConfig:MaxContentLength"))
        .Returns(2000);
    
    // Act
    var result = _service.CreateJournal("user1", "Judul", "Konten yang bermanfaat", MoodType.Happy);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal(JournalState.Draft, result.State);
    Assert.Equal("user1", result.UserId);
    Assert.Equal("Judul", result.Title);
}
```

#### Test 2: `CreateJournal_EmptyTitle_ThrowsException`
| **Action** | `_service.CreateJournal("user1", "", "Konten", MoodType.Happy)` |
| **Assert** | `Assert.Throws<ArgumentException>(...)` |

#### Test 3: `CreateJournal_EmptyContent_ThrowsException`
| **Action** | `_service.CreateJournal("user1", "Judul", "", MoodType.Happy)` |
| **Assert** | `Assert.Throws<ArgumentException>(...)` |

#### Test 4: `CreateJournal_InvalidMood_ThrowsException`
| **Action** | `_service.CreateJournal("user1", "Judul", "Konten", (MoodType)999)` |
| **Assert** | `Assert.Throws<ArgumentException>(...)` |

#### Test 5: `CreateJournal_ContentTooLong_ThrowsException`
| **Setup** | mock `MaxContentLength = 10` |
| **Action** | `_service.CreateJournal("user1", "J", new string('x', 100), MoodType.Happy)` |
| **Assert** | `Assert.Throws<ArgumentException>(...)` dengan message mengandung "maksimal" |

---

### B. Unit Tests — JournalStateMachine (6 test)

#### Test 6: `StateMachine_Draft_Submit_Submitted`
| **Action** | `_machine.Transition(JournalState.Draft, JournalTrigger.Submit)` |
| **Assert** | `result == JournalState.Submitted` |

#### Test 7: `StateMachine_Submitted_Save_Saved`
| **Setup** | Dari Draft → Submit → Submitted |
| **Action** | `_machine.Transition(JournalState.Submitted, JournalTrigger.Save)` |
| **Assert** | `result == JournalState.Saved` |

#### Test 8: `StateMachine_Submitted_Reject_Rejected`
| **Action** | `_machine.Transition(JournalState.Submitted, JournalTrigger.Reject)` |
| **Assert** | `result == JournalState.Rejected` |

#### Test 9: `StateMachine_Rejected_Reset_Draft`
| **Action** | `_machine.Transition(JournalState.Rejected, JournalTrigger.Reset)` |
| **Assert** | `result == JournalState.Draft` |

#### Test 10: `StateMachine_InvalidTransition_ThrowsException`
| **Action** | `_machine.Transition(JournalState.Saved, JournalTrigger.Submit)` |
| **Assert** | `Assert.Throws<InvalidOperationException>(...)` |
| **Alasan** | Saved adalah terminal — tidak ada transisi keluar |

#### Test 11: `IsTerminal_Saved_ReturnsTrue`
| **Action** | `_machine.IsTerminal(JournalState.Saved)` |
| **Assert** | `true` |
| **Bonus** | `_machine.IsTerminal(JournalState.Draft)` → `false` |

---

### C. Unit Test — JournalService (1 tambahan)

#### Test 12: `GetJournalsByUser_ReturnsCorrectData`
| **Setup** | mock `_mockRepo.GetByUserId("user1")` → return list 2 journal |
| **Action** | `_service.GetJournalsByUser("user1")` |
| **Assert** | `result.Count() == 2` |

---

### D. Performance Benchmarks — Journal (4 test)

Benchmark pakai `System.Diagnostics.Stopwatch`.

#### Perf 1: `GetJournalsByUser_10Entries_Under10ms`
```
1. Generate 10 journal di repository
2. Stopwatch.StartNew()
3. service.GetJournalsByUser("user1")
4. sw.Stop()
5. Assert.True(sw.ElapsedMilliseconds < 10, $"Too slow: {sw.ElapsedMilliseconds}ms")
```

#### Perf 2: `GetJournalsByUser_100Entries_Under50ms`
Sama dengan di atas, generate 100 journal. Target < 50ms.

#### Perf 3: `GetJournalsByUser_1000Entries_Under200ms`
Sama, generate 1000 journal. Target < 200ms.

#### Perf 4: `CreateJournal_Under20ms`
```
1. Stopwatch.StartNew()
2. service.CreateJournal("u1", "Title", "Content", MoodType.Happy)
3. sw.Stop()
4. Assert < 20ms
```

---

---

## TONI KURNIAWAN — Character Tests (6 unit + 2 performance)

Referensi test plan: `myKisah/Tests/Toni_CharacterTests.cs`

> ⚠️ Test ini membuktikan Table-Driven kamu. Pastikan mock `ICharacterResponseRepository.GetByMood()` mengembalikan data yang benar — BUKAN if/switch.

---

### A. Unit Tests — CharacterService (6 test)

#### Test 1: `GenerateResponse_HappyMood_ReturnsResponse`
| **Setup** | `_mockRespRepo.Setup(r => r.GetByMood("char1", MoodType.Happy))` → return list berisi `CharacterResponse { Response = "Yeay! Senang!" }` |
| **Action** | `_service.GenerateResponse("char1", MoodType.Happy)` |
| **Assert** | `result == "Yeay! Senang!"` |

**Kode lengkap:**
```csharp
[Fact]
public void GenerateResponse_HappyMood_ReturnsResponse()
{
    _mockRespRepo.Setup(r => r.GetByMood("char1", MoodType.Happy))
        .Returns(new List<CharacterResponse>
        {
            new() { Response = "Yeay! Senang!" }
        });
    
    var result = _service.GenerateResponse("char1", MoodType.Happy);
    
    Assert.Equal("Yeay! Senang!", result);
}
```

#### Test 2: `GenerateResponse_SadMood_ReturnsResponse`
| **Setup** | mock `GetByMood("char1", MoodType.Sad)` → return response sedih |
| **Action** | `_service.GenerateResponse("char1", MoodType.Sad)` |
| **Assert** | result tidak null, tidak empty |

#### Test 3: `GenerateResponse_AllMoods_Covered`
| **Setup** | mock untuk SEMUA 5 MoodType |
| **Action** | `foreach (MoodType m in Enum.GetValues<MoodType>())` → `_service.GenerateResponse("char1", m)` |
| **Assert** | Semua return string, tidak ada yang throw |

**Kode:**
```csharp
[Fact]
public void GenerateResponse_AllMoods_Covered()
{
    foreach (MoodType mood in Enum.GetValues<MoodType>())
    {
        _mockRespRepo.Setup(r => r.GetByMood("char1", mood))
            .Returns(new List<CharacterResponse>
            {
                new() { Response = $"Response for {mood}" }
            });
    }
    
    foreach (MoodType mood in Enum.GetValues<MoodType>())
    {
        var result = _service.GenerateResponse("char1", mood);
        Assert.False(string.IsNullOrEmpty(result));
    }
}
```

#### Test 4: `GenerateResponse_InvalidMood_ThrowsException`
| **Action** | `_service.GenerateResponse("char1", (MoodType)999)` |
| **Assert** | `Assert.Throws<ArgumentException>(...)` |

#### Test 5: `GenerateResponse_NullCharacterId_ThrowsException`
| **Action** | `_service.GenerateResponse("", MoodType.Happy)` |
| **Assert** | `Assert.Throws<ArgumentException>(...)` |

#### Test 6: `GetAllCharacters_ReturnsAllCharacters`
| **Setup** | `_mockCharRepo.Setup(r => r.GetAll())` → return list 3 karakter |
| **Action** | `_service.GetAllCharacters()` |
| **Assert** | `result.Count() == 3` |

---

### B. Performance Benchmarks — Character (2 test)

#### Perf 1: `GenerateResponse_1000Iterations_Under1msPerCall`
```
1. Setup mock untuk semua mood
2. Warmup: panggil 1x GenerateResponse
3. Stopwatch.StartNew()
4. for (int i = 0; i < 1000; i++)
       _service.GenerateResponse("char1", randomMood)
5. sw.Stop()
6. double avgMs = sw.ElapsedMilliseconds / 1000.0
7. Assert.True(avgMs < 1, $"Too slow: {avgMs}ms per call")
```

#### Perf 2: `LoadCharacterResponses_ColdStart_Under30ms`
```
1. Stopwatch.StartNew()
2. _responseRepo.GetByMood("char1", MoodType.Happy)
3. sw.Stop()
4. Assert < 30ms
```

---

---

## FAREL ILHAM — User Tests (7 unit)

Referensi test plan: `myKisah/Tests/Farel_UserTests.cs`

---

### A. Unit Tests — UserService (7 test)

#### Test 1: `RegisterUser_ValidUsername_Success`
| **Setup** | `_mockRepo.Setup(r => r.UsernameExists("farel")).Returns(false)` |
| **Action** | `_service.RegisterUser("farel")` |
| **Assert** | `result.Username == "farel"`, `!string.IsNullOrEmpty(result.Id)`, `result.CreatedAt != default` |

**Kode:**
```csharp
[Fact]
public void RegisterUser_ValidUsername_Success()
{
    _mockRepo.Setup(r => r.UsernameExists("farel")).Returns(false);
    
    var result = _service.RegisterUser("farel");
    
    Assert.Equal("farel", result.Username);
    Assert.False(string.IsNullOrEmpty(result.Id));
    Assert.NotEqual(default(DateTime), result.CreatedAt);
}
```

#### Test 2: `RegisterUser_DuplicateUsername_ThrowsException`
| **Setup** | `_mockRepo.Setup(r => r.UsernameExists("exist")).Returns(true)` |
| **Action** | `_service.RegisterUser("exist")` |
| **Assert** | `Assert.Throws<ArgumentException>(...)` — message mengandung "sudah terdaftar" |

#### Test 3: `RegisterUser_EmptyUsername_ThrowsException`
| **Action** | `_service.RegisterUser("")` |
| **Assert** | `Assert.Throws<ArgumentException>(...)` |

#### Test 4: `RegisterUser_NullUsername_ThrowsException`
| **Action** | `_service.RegisterUser(null!)` |
| **Assert** | `Assert.Throws<ArgumentException>(...)` |

#### Test 5: `GetAllUsers_ReturnsAllRecords`
| **Setup** | `_mockRepo.Setup(r => r.GetAll())` → return list 3 user |
| **Action** | `_service.GetAllUsers()` |
| **Assert** | `result.Count() == 3` |

#### Test 6: `DeleteUser_ExistingId_Success`
| **Setup** | `_mockRepo.Setup(r => r.GetById("id")).Returns(new User { Id = "id", Username = "test" })` |
| **Action** | `_service.DeleteUser("id")` |
| **Assert** | `result == true` |

#### Test 7: `DeleteUser_NonExistingId_ThrowsException`
| **Setup** | `_mockRepo.Setup(r => r.GetById("nonexist")).Returns((User?)null)` |
| **Action** | `_service.DeleteUser("nonexist")` |
| **Assert** | `Assert.Throws<KeyNotFoundException>(...)` |

#### Test 8 (Bonus): `UpdateUser_ValidInput_Success`
| **Setup** | `_mockRepo.Setup(r => r.GetById("id")).Returns(user)`, `UsernameExists("new")` → false |
| **Action** | `_service.UpdateUser("id", "new")` |
| **Assert** | `result.Username == "new"` |

#### Test 9 (Bonus): `UpdateUser_DuplicateUsername_ThrowsException`
| **Setup** | `GetById` return user1, `UsernameExists("user2")` → true |
| **Action** | `_service.UpdateUser("id1", "user2")` |
| **Assert** | `Assert.Throws<ArgumentException>(...)` |

---

---

## RAFLY PUTRA — Storage Tests (6 unit + 2 performance)

Referensi test plan: `myKisah/Tests/Rafly_StorageTests.cs`

---

### A. Unit Tests — JsonStorageHelper (6 test)

#### Test 1: `ReadJson_ExistingFile_ReturnsData`
| **Setup** | Buat file JSON dengan 3 item test |
| **Action** | `_storage.ReadJson<TestModel>("test.json")` |
| **Assert** | `result.Count == 3`, data sesuai |

#### Test 2: `ReadJson_MissingFile_ReturnsEmpty`
| **Setup** | Pastikan file `nonexist.json` TIDAK ada |
| **Action** | `_storage.ReadJson<TestModel>("nonexist.json")` |
| **Assert** | `result != null`, `result.Count == 0` |
| **Assert** | File otomatis terbuat dengan isi `[]` |

**Kode:**
```csharp
[Fact]
public void ReadJson_MissingFile_ReturnsEmpty()
{
    var filename = "nonexist.json";
    var fullPath = Path.Combine(_tempDir, filename);
    
    // Pastikan file belum ada
    if (File.Exists(fullPath)) File.Delete(fullPath);
    
    var result = _storage.ReadJson<TestModel>(filename);
    
    Assert.NotNull(result);
    Assert.Empty(result);
    Assert.True(File.Exists(fullPath));  // Auto-create
}
```

#### Test 3: `WriteJson_ValidData_PersistsCorrectly`
| **Action** | WriteJson 5 item → ReadJson |
| **Assert** | `result.Count == 5`, data identik |

#### Test 4: `ReadJson_NullFilename_ThrowsException`
| **Action** | `_storage.ReadJson<string>(null!)` |
| **Assert** | `Assert.Throws<ArgumentNullException>(...)` |

#### Test 5: `WriteJson_NullData_ThrowsException`
| **Action** | `_storage.WriteJson<string>("test.json", null!)` |
| **Assert** | `Assert.Throws<ArgumentNullException>(...)` |

#### Test 6: `FilePathConfig_LoadsPathsFromConfig`
| **Setup** | In-memory `IConfiguration` dengan section `StoragePaths` |
| **Assert** | Semua 4 property tidak null dan tidak empty |

**Kode:**
```csharp
[Fact]
public void FilePathConfig_LoadsPathsFromConfig()
{
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
    
    Assert.Equal("Data/users.json", filePathConfig.UsersFile);
    Assert.Equal("Data/journals.json", filePathConfig.JournalsFile);
    Assert.Equal("Data/characters.json", filePathConfig.CharactersFile);
    Assert.Equal("Data/characterResponses.json", filePathConfig.ResponsesFile);
}
```

---

### B. Performance Benchmarks — Storage (2 test)

#### Perf 1: `ReadJson_100KB_Under100ms`
```
1. Generate file JSON ~100KB (banyak item)
2. Warmup
3. Stopwatch → ReadJson → Assert < 100ms
```

#### Perf 2: `WriteRead_RoundTrip_100Items_Under50ms`
```
1. Generate 100 item
2. Stopwatch → WriteJson + ReadJson → Assert total < 50ms
```

---

---

## JOSEFHINT (JOJO) — Utils & Middleware Tests (5 unit + 4 middleware + 1 performance)

Referensi test plan: `myKisah/Tests/Jojo_UtilsTests.cs`

---

### A. Unit Tests — ValidationHelper (5 test)

#### Test 1: `ValidateNotNull_NullValue_ThrowsException`
| **Action** | `_validator.ValidateNotNull<string>(null, "param")` |
| **Assert** | `var ex = Assert.Throws<ArgumentNullException>(...)` |
| **Assert** | `ex.Message` mengandung `"param"` |

#### Test 2: `ValidateNotNull_ValidValue_NoException`
| **Action** | `_validator.ValidateNotNull("hello", "param")` |
| **Assert** | Tidak throw |

#### Test 3: `ValidateNotEmpty_EmptyString_ThrowsException`
| **Action** | `_validator.ValidateNotEmpty("", "param")` |
| **Assert** | `Assert.Throws<ArgumentException>(...)` |
| **Action** | `_validator.ValidateNotEmpty("   ", "param")` (whitespace) |
| **Assert** | `Assert.Throws<ArgumentException>(...)` |

#### Test 4: `ValidateInEnum_InvalidValue_ThrowsException`
| **Action** | `_validator.ValidateInEnum((MoodType)999, "Mood")` |
| **Assert** | `Assert.Throws<ArgumentException>(...)` |

#### Test 5: `ValidateExists_NullEntity_ThrowsException`
| **Action** | `_validator.ValidateExists<object>(null, "Entity")` |
| **Assert** | `var ex = Assert.Throws<KeyNotFoundException>(...)` |
| **Assert** | `ex.Message` mengandung `"Entity"` |

---

### B. Middleware Tests — ErrorHandlingMiddleware (4 test)

Middleware test perlu setup `DefaultHttpContext()` dan `RequestDelegate` mock.

#### Test 6: `Middleware_ArgumentException_Returns400`
| **Setup** | `RequestDelegate next` yang throw `ArgumentException("test error")` |
| **Action** | `await middleware.InvokeAsync(context)` |
| **Assert** | `context.Response.StatusCode == 400` |
| **Assert** | Response body JSON mengandung `"error"` dan `"statusCode"` |

**Kode:**
```csharp
[Fact]
public async Task Middleware_ArgumentException_Returns400()
{
    // Arrange
    var context = new DefaultHttpContext();
    context.Response.Body = new MemoryStream();
    
    RequestDelegate next = _ => throw new ArgumentException("Test error");
    var middleware = new ErrorHandlingMiddleware(next);
    
    // Act
    await middleware.InvokeAsync(context);
    
    // Assert
    Assert.Equal(400, context.Response.StatusCode);
    
    context.Response.Body.Seek(0, SeekOrigin.Begin);
    var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
    Assert.Contains("error", responseBody);
    Assert.Contains("400", responseBody);
}
```

#### Test 7: `Middleware_KeyNotFoundException_Returns404`
| **Setup** | `next` throw `KeyNotFoundException("not found")` |
| **Assert** | `StatusCode == 404` |

#### Test 8: `Middleware_InvalidOperationException_Returns422`
| **Setup** | `next` throw `InvalidOperationException("invalid")` |
| **Assert** | `StatusCode == 422` |

#### Test 9: `Middleware_UnknownException_Returns500`
| **Setup** | `next` throw `new Exception("unknown error")` |
| **Assert** | `StatusCode == 500` |
| **Assert** | Response body tidak mengandung message internal (keamanan) |

---

### C. Performance Benchmark — Validation (1 test)

#### Perf 1: `ValidateNotNull_1000Calls_Under5ms`
```
1. Stopwatch.StartNew()
2. for (int i = 0; i < 1000; i++)
       _validator.ValidateNotNull("test", "param")
3. sw.Stop()
4. Assert.True(sw.ElapsedMilliseconds < 5, $"Too slow: {sw.ElapsedMilliseconds}ms")
```

**Kode:**
```csharp
[Fact]
public void ValidateNotNull_1000Calls_Under5ms()
{
    var sw = Stopwatch.StartNew();
    for (int i = 0; i < 1000; i++)
        _validator.ValidateNotNull("test", "param");
    sw.Stop();
    
    Assert.True(sw.ElapsedMilliseconds < 5, 
        $"Too slow: {sw.ElapsedMilliseconds}ms (target < 5ms)");
}
```

---

### D. Pipeline End-to-End Test (1 test — Semua)

#### Perf 2: `PipelineE2E_Under50ms`
| **Setup** | Gunakan `WebApplicationFactory<Program>` untuk integration test |
| **Action** | HTTP request ke endpoint (misal: GET /api/user) |
| **Assert** | Response time < 50ms |

**Kode (dengan WebApplicationFactory):**
```csharp
[Fact]
public async Task E2E_Pipeline_Under50ms()
{
    await using var app = new WebApplicationFactory<Program>();
    var client = app.CreateClient();
    
    var sw = Stopwatch.StartNew();
    var response = await client.GetAsync("/api/user");
    sw.Stop();
    
    Assert.True(sw.ElapsedMilliseconds < 50, 
        $"Pipeline too slow: {sw.ElapsedMilliseconds}ms (target < 50ms)");
    Assert.True(response.IsSuccessStatusCode);
}
```
**Catatan:** Test ini memerlukan package `Microsoft.AspNetCore.Mvc.Testing`.

---

---

## RINGKASAN FASE 3 — TEST COVERAGE PER ANGGOTA

| Anggota | Unit Test | Performance Test | Total |
|---|---|---|---|
| **Rayazka** | 11 (Journal + StateMachine) | 4 | **15** |
| **Toni** | 6 (CharacterService) | 2 | **8** |
| **Farel** | 7 (UserService) | 0 | **7** |
| **Rafly** | 6 (JsonStorageHelper) | 2 | **8** |
| **Jojo** | 5 (Validation) + 4 (Middleware) | 1 | **10** |
| **Semua** | — | 1 (E2E) | **1** |

---

## TARGET PERFORMANCE BENCHMARKS

| Skenario | Target | PIC |
|---|---|---|
| `GetJournalsByUser()` — 10 entri | < 10ms | Rayazka |
| `GetJournalsByUser()` — 100 entri | < 50ms | Rayazka |
| `GetJournalsByUser()` — 1000 entri | < 200ms | Rayazka |
| `CreateJournal()` + state machine | < 20ms | Rayazka |
| `GenerateResponse()` — 1000 iterasi | < 1ms/call | Toni |
| Load `characterResponses.json` cold start | < 30ms | Toni |
| `ReadJson` payload 100KB (~1000 item) | < 100ms | Rafly |
| Round-trip `WriteJson` + `ReadJson` — 100 item | < 50ms | Rafly |
| `ValidationHelper` — 1000 panggilan | < 5ms total | Jojo |
| Pipeline end-to-end | < 50ms | Semua |

---

## PERHATIAN UNTUK SEMUA ANGGOTA

1. **Test harus independen** — satu test tidak boleh bergantung pada test lain
2. **Gunakan Moq** — jangan test dengan data asli dari JSON file
3. **Cover edge cases** — bukan hanya happy path, tapi juga error case
4. **Performance test** — gunakan Stopwatch, pastikan warmup dulu
5. **Commit per batch** — selesaikan semua test dulu, baru commit
6. **Merge ke main** setelah semua test PASS
