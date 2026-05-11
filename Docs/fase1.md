# Fase 1 — Fondasi Backend myKisah

---

## RAYAZKA ARIS — Models, Interfaces, Automata (16 file)

Teknik konstruksi: **Automata** + **Runtime Configuration**

---

### BAGIAN A: Models & Enums (6 file)

#### A1. `Models/User.cs`
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Models/User.cs` |
| **Namespace** | `myKisah.Models` |

**Isi:**
```csharp
namespace myKisah.Models;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

---

#### A2. `Models/Journal.cs`
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Models/Journal.cs` |
| **Namespace** | `myKisah.Models` |

**Isi:**
```csharp
namespace myKisah.Models;

public class Journal
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public MoodType Mood { get; set; }
    public JournalState State { get; set; } = JournalState.Draft;
    public DateTime CreatedAt { get; set; }
}
```

---

#### A3. `Models/Character.cs`
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Models/Character.cs` |
| **Namespace** | `myKisah.Models` |

**Isi:**
```csharp
namespace myKisah.Models;

public class Character
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

---

#### A4. `Models/CharacterResponse.cs`
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Models/CharacterResponse.cs` |
| **Namespace** | `myKisah.Models` |
| **Teknik** | **Table-Driven** — ini adalah ROW dari tabel lookup |

**Isi:**
```csharp
namespace myKisah.Models;

public class CharacterResponse
{
    public string Id { get; set; } = string.Empty;
    public string CharacterId { get; set; } = string.Empty;
    public MoodType Mood { get; set; }
    public string Response { get; set; } = string.Empty;
}
```

---

#### A5. `Models/MoodType.cs`
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Models/MoodType.cs` |
| **Namespace** | `myKisah.Models` |

**Isi:**
```csharp
namespace myKisah.Models;

public enum MoodType
{
    Happy,
    Sad,
    Angry,
    Anxious,
    Calm
}
```

---

#### A6. `Models/JournalState.cs`
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Models/JournalState.cs` |
| **Namespace** | `myKisah.Models` |

**Isi:**
```csharp
namespace myKisah.Models;

public enum JournalState
{
    Draft,
    Submitted,
    Saved,
    Rejected
}
```

---

### BAGIAN B: Interfaces (8 file)

Semua file di folder `myKisah/Interfaces/`, namespace `myKisah.Interfaces`.

#### B1. `Interfaces/IRepository.cs`
| Atribut | Nilai |
|---|---|
| **Teknik** | **Generics** — generic type parameter `<T>` |

**Interface yang disediakan:**
```csharp
namespace myKisah.Interfaces;

public interface IRepository<T>
{
    IEnumerable<T> GetAll();
    T? GetById(string id);
    void Add(T entity);
    void Update(T entity);
    void Delete(string id);
}
```
Penjelasan: Ini adalah BASE interface untuk semua repository. Semua repository (User, Journal, Character, Response) akan mewarisi interface ini, sehingga setiap repository otomatis punya 5 method CRUD dasar tanpa harus menulis ulang.

---

#### B2. `Interfaces/IUserRepository.cs`
| Atribut | Nilai |
|---|---|
| **Extends** | `IRepository<User>` |
| **Dikerjakan oleh** | Farel (Fase 2) |

```csharp
using myKisah.Models;

namespace myKisah.Interfaces;

public interface IUserRepository : IRepository<User>
{
    User? GetByUsername(string username);
    bool UsernameExists(string username);
}
```
2 method tambahan di luar CRUD dasar: `GetByUsername` dan `UsernameExists`.

---

#### B3. `Interfaces/IJournalRepository.cs`
| Atribut | Nilai |
|---|---|
| **Extends** | `IRepository<Journal>` |
| **Dikerjakan oleh** | Rayazka (Fase 2) |

```csharp
using myKisah.Models;

namespace myKisah.Interfaces;

public interface IJournalRepository : IRepository<Journal>
{
    IEnumerable<Journal> GetByUserId(string userId);
}
```
1 method tambahan: `GetByUserId` untuk filter jurnal milik satu user.

---

#### B4. `Interfaces/ICharacterRepository.cs`
| Atribut | Nilai |
|---|---|
| **Extends** | `IRepository<Character>` |
| **Dikerjakan oleh** | Toni (Fase 2) |

```csharp
using myKisah.Models;

namespace myKisah.Interfaces;

public interface ICharacterRepository : IRepository<Character>
{
    Character? GetByName(string name);
}
```
1 method tambahan: `GetByName`.

---

#### B5. `Interfaces/ICharacterResponseRepository.cs`
| Atribut | Nilai |
|---|---|
| **Extends** | `IRepository<CharacterResponse>` |
| **Teknik** | **Table-Driven** — `GetByMood` adalah inti table lookup |
| **Dikerjakan oleh** | Toni (Fase 2) |

```csharp
using myKisah.Models;

namespace myKisah.Interfaces;

public interface ICharacterResponseRepository : IRepository<CharacterResponse>
{
    IEnumerable<CharacterResponse> GetByMood(string characterId, MoodType mood);
}
```
Method `GetByMood` adalah JANTUNG Table-Driven. Method ini akan melakukan filter ke `characterResponses.json`.

---

#### B6. `Interfaces/IUserService.cs`
| Atribut | Nilai |
|---|---|
| **Dikerjakan oleh** | Farel (Fase 2) |

```csharp
using myKisah.Models;

namespace myKisah.Interfaces;

public interface IUserService
{
    User RegisterUser(string username);
    IEnumerable<User> GetAllUsers();
    User? UpdateUser(string id, string username);
    bool DeleteUser(string id);
}
```

---

#### B7. `Interfaces/IJournalService.cs`
| Atribut | Nilai |
|---|---|
| **Teknik** | **Automata** + **Runtime Configuration** |
| **Dikerjakan oleh** | Rayazka (Fase 2) |

```csharp
using myKisah.Models;

namespace myKisah.Interfaces;

public interface IJournalService
{
    Journal CreateJournal(string userId, string title, string content, MoodType mood);
    IEnumerable<Journal> GetJournalsByUser(string userId);
    bool DeleteJournal(string id);
}
```

---

#### B8. `Interfaces/ICharacterService.cs`
| Atribut | Nilai |
|---|---|
| **Teknik** | **Table-Driven** |
| **Dikerjakan oleh** | Toni (Fase 2) |

```csharp
using myKisah.Models;

namespace myKisah.Interfaces;

public interface ICharacterService
{
    IEnumerable<Character> GetAllCharacters();
    string GenerateResponse(string characterId, MoodType mood);
}
```
Method `GenerateResponse` adalah inti Table-Driven — tidak boleh ada if/switch.

---

### BAGIAN C: Automata (2 file)

#### C1. `Automata/JournalTrigger.cs`
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Automata/JournalTrigger.cs` |
| **Namespace** | `myKisah.Automata` |
| **Teknik** | **Automata** — Input trigger untuk state machine |

```csharp
namespace myKisah.Automata;

public enum JournalTrigger
{
    Submit,
    Save,
    Reject,
    Reset
}
```

---

#### C2. `Automata/JournalStateMachine.cs` ⚠️ ADA TODO!
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Automata/JournalStateMachine.cs` |
| **Namespace** | `myKisah.Automata` |
| **Teknik** | **Automata** — Finite State Machine dengan Dictionary transition table |
| **Referensi** | `Task_myKisah.md` baris 97-124 |
| **Status file** | ⚠️ Skeleton ada, TODO harus diisi |

**📋 TODO yang harus dikerjakan:**

1. **Isi Dictionary `_transitions`** — 4 entry transisi:
   ```
   (Draft, Submit)      → Submitted
   (Submitted, Save)    → Saved
   (Submitted, Reject)  → Rejected
   (Rejected, Reset)    → Draft
   ```

2. **Implement `Transition(JournalState current, JournalTrigger trigger)`**:
   ```csharp
   public JournalState Transition(JournalState currentState, JournalTrigger trigger)
   {
       if (_transitions.TryGetValue((currentState, trigger), out var nextState))
           return nextState;
       
       throw new InvalidOperationException(
           $"Transisi tidak valid: {currentState} + {trigger}");
   }
   ```
   Logika: Cek apakah kombinasi state+trigger ada di Dictionary. Jika ada → return state tujuan. Jika tidak → throw `InvalidOperationException` (ditangkap middleware → 422).

3. **Implement `IsTerminal(JournalState state)`**:
   ```csharp
   public bool IsTerminal(JournalState state)
   {
       return state == JournalState.Saved;
   }
   ```
   Logika: `Saved` adalah state terminal. Tidak ada transisi keluar dari Saved.

**Diagram transisi:**
```
Draft ──[Submit]──► Submitted ──[Save]──► Saved (terminal)
                         │
                      [Reject]
                         │
                         ▼
                     Rejected ──[Reset]──► Draft
```

---

---

## RAFLY PUTRA — Storage Layer (3 item)

Teknik konstruksi: **Runtime Configuration** + **Code Reuse**

---

### D1. `appsettings.json` ⚠️ SUDAH DIISI (verifikasi)
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/appsettings.json` |
| **Teknik** | **Runtime Configuration** |

**Section yang ada:**
```json
{
  "StoragePaths": {
    "UsersFile": "Data/users.json",
    "JournalsFile": "Data/journals.json",
    "CharactersFile": "Data/characters.json",
    "ResponsesFile": "Data/characterResponses.json"
  },
  "JournalConfig": {
    "MaxContentLength": 2000,
    "ValidMoods": ["Happy", "Sad", "Angry", "Anxious", "Calm"]
  }
}
```
**Tugas Rafly:** Pastikan section ini sudah benar. Tidak ada TODO — tapi pastikan tidak ada yang kurang.

---

### D2. `Utils/FilePathConfig.cs` ADA TODO!
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Utils/FilePathConfig.cs` |
| **Namespace** | `myKisah.Utils` |
| **Teknik** | **Runtime Configuration** — path dibaca dari config, TIDAK di-hardcode |
| **Dependency** | `IConfiguration` (DI inject) |
| **Dipakai oleh** | `JsonStorageHelper` → semua Repository |
| **Status file** |  Skeleton ada, TODO harus diisi |

**📋 TODO yang harus dikerjakan:**

**Constructor:**
```csharp
private readonly IConfiguration _configuration;

public FilePathConfig(IConfiguration configuration)
{
    _configuration = configuration;
}
```
Simpan IConfiguration ke field. Ini akan dipakai di property getter.

**4 Property getter:**
```csharp
public string UsersFile => _configuration["StoragePaths:UsersFile"] ?? "Data/users.json";
public string JournalsFile => _configuration["StoragePaths:JournalsFile"] ?? "Data/journals.json";
public string CharactersFile => _configuration["StoragePaths:CharactersFile"] ?? "Data/characters.json";
public string ResponsesFile => _configuration["StoragePaths:ResponsesFile"] ?? "Data/characterResponses.json";
```
Setiap property membaca dari `IConfiguration` dengan key `"StoragePaths:XxxFile"`. Pakai null-coalescing `??` sebagai fallback default value.

**Kenapa Runtime Configuration?** Path file dibaca dari JSON config, bukan hardcode. Kalau mau ganti lokasi file, cukup edit `appsettings.json`, tidak perlu ubah kode C#.

---

### D3. `Utils/JsonStorageHelper.cs` ⚠️ ADA TODO!
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Utils/JsonStorageHelper.cs` |
| **Namespace** | `myKisah.Utils` |
| **Teknik** | **Code Reuse** + **Generics** — dipakai semua repository |
| **Dependency** | `FilePathConfig` (DI inject) |
| **Dipakai oleh** | Semua Repository (User, Journal, Character, Response) |
| **Referensi** | `Task_myKisah.md` baris 128-138 |
| **Status file** | ⚠️ Skeleton ada, TODO harus diisi |

**📋 TODO yang harus dikerjakan:**

**Constructor:**
```csharp
private readonly string _basePath;

public JsonStorageHelper(FilePathConfig filePathConfig)
{
    // Asumsikan semua file ada di folder relatif dari project root
    _basePath = Path.Combine(Directory.GetCurrentDirectory(), filePathConfig.UsersFile);
    // HACK: ambil directory dari UsersFile, semua file JSON ada di folder yang sama
    _basePath = Path.GetDirectoryName(_basePath) ?? string.Empty;
}
```
Atau alternatif lebih sederhana — baca base path dari config:
```csharp
_basePath = Directory.GetCurrentDirectory();
```
Di method baca/tulis, gabung `_basePath` dengan filename dari `FilePathConfig`.

**Method `ReadJson<T>(string filename)`:**
```csharp
public List<T> ReadJson<T>(string filename)
{
    var fullPath = Path.Combine(_basePath, filename);
    
    if (!File.Exists(fullPath))
    {
        File.WriteAllText(fullPath, "[]");
        return new List<T>();
    }
    
    var json = File.ReadAllText(fullPath);
    return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
}
```
Logika: Jika file belum ada → auto-create file berisi `[]` (array kosong) → return List kosong. Jika file sudah ada → baca → deserialize JSON → return List. Jika JSON corrupt → return List kosong (defensive).

**Method `WriteJson<T>(string filename, List<T> data)`:**
```csharp
public void WriteJson<T>(string filename, List<T> data)
{
    var fullPath = Path.Combine(_basePath, filename);
    
    var directory = Path.GetDirectoryName(fullPath);
    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        Directory.CreateDirectory(directory);
    
    var options = new JsonSerializerOptions { WriteIndented = true };
    var json = JsonSerializer.Serialize(data, options);
    File.WriteAllText(fullPath, json);
}
```
Logika: Pastikan directory exists → serialize data dengan WriteIndented (supaya JSON rapi) → tulis ke file.

**Kenapa Code Reuse?** Method ini akan dipakai oleh 4 repository. Tanpa helper ini, setiap repository akan punya kode baca/tulis JSON yang sama — 4x duplikasi. Dengan 1 helper, semua repository tinggal panggil `_storage.ReadJson<T>(...)`.

**Kenapa Generics?** `<T>` memungkinkan method yang sama dipakai untuk `User`, `Journal`, `Character`, `CharacterResponse` tanpa duplikasi.

**Design by Contract:**
- `filename` tidak boleh null → throw ArgumentNullException
- `data` tidak boleh null saat Write → throw ArgumentNullException
- File tidak ada → auto-create dengan `[]`
- Return tidak pernah null → selalu List kosong minimal

---

---

## JOSEFHINT (JOJO) — Shared Utilities & Error Handling (4 item)

Teknik konstruksi: **Code Reuse** + **Generics**

---

### E1. `Utils/ValidationHelper.cs` ⚠️ ADA TODO!
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Utils/ValidationHelper.cs` |
| **Namespace** | `myKisah.Utils` |
| **Teknik** | **Generics** + **Code Reuse** |
| **Dipakai oleh** | `ServiceBase` → semua Service |
| **Referensi** | `Task_myKisah.md` baris 167-185 |
| **Status file** | ⚠️ Skeleton ada, TODO harus diisi |

**📋 4 Method yang harus diimplementasikan:**

**1. `ValidateNotNull<T>(T? value, string name)`:**
```csharp
public void ValidateNotNull<T>(T? value, string name)
{
    if (value == null)
        throw new ArgumentNullException(name, $"{name} tidak boleh null.");
}
```
Logic: Cek null untuk SEMUA tipe. Generic `<T>` berarti bisa untuk string, User, Journal, dll. Exception ditangkap middleware → 400.

**2. `ValidateNotEmpty(string value, string name)`:**
```csharp
public void ValidateNotEmpty(string value, string name)
{
    if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException($"{name} tidak boleh kosong.", name);
}
```
Logic: Cek string kosong/null/whitespace. Exception ditangkap middleware → 400.

**3. `ValidateInEnum<T>(T value, string name) where T : struct, Enum`:**
```csharp
public void ValidateInEnum<T>(T value, string name) where T : struct, Enum
{
    if (!Enum.IsDefined(typeof(T), value))
        throw new ArgumentException($"'{value}' bukan nilai {name} yang valid.", name);
}
```
Logic: Cek apakah nilai enum valid. Generic constraint `where T : struct, Enum` memastikan hanya enum yang bisa dipakai. `Enum.IsDefined` mengecek apakah value terdaftar di enum. Exception → 400.

**4. `ValidateExists<T>(T? entity, string name)`:**
```csharp
public void ValidateExists<T>(T? entity, string name)
{
    if (entity == null)
        throw new KeyNotFoundException($"{name} tidak ditemukan.");
}
```
Logic: Cek apakah entity ada setelah lookup (GetById). Exception → 404.

**Kenapa Generics?** Tanpa generics, harus buat ValidateNotNull untuk setiap tipe (string, User, Journal...). Dengan generics: 1 method untuk semua.

**Kenapa Code Reuse?** Method validasi ini dipakai di SEMUA Service. Tanpa shared helper, setiap developer akan menulis ulang validasi — inkonsistensi error message.

**PENTING — Exception Mapping:**
| Exception yang dilempar | HTTP Status | Arti |
|---|---|---|
| `ArgumentNullException` | 400 Bad Request | Input null |
| `ArgumentException` | 400 Bad Request | Input tidak valid |
| `KeyNotFoundException` | 404 Not Found | Data tidak ditemukan |

---

### E2. `Utils/ServiceBase.cs` ⚠️ ADA TODO!
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Utils/ServiceBase.cs` |
| **Namespace** | `myKisah.Utils` |
| **Teknik** | **Code Reuse** — base class untuk semua Service |
| **Dipakai oleh** | `UserService`, `JournalService`, `CharacterService` |
| **Referensi** | `Task_myKisah.md` baris 177-185 |
| **Status file** | ⚠️ Skeleton ada, TODO harus diisi |

**📋 TODO yang harus dikerjakan:**

```csharp
public abstract class ServiceBase
{
    protected readonly ValidationHelper Validator = new();

    protected abstract string ServiceName { get; }

    protected void LogError(string message, Exception ex)
    {
        Console.WriteLine($"[ERROR] [{ServiceName}] {message}: {ex.Message}");
    }
}
```
Logika: Abstract class menyediakan `Validator` (instance ValidationHelper) untuk semua service. Abstract property `ServiceName` harus di-override oleh setiap service (misal: `"UserService"`). `LogError` untuk logging error — simple Console.WriteLine untuk fase ini.

**Kenapa Code Reuse?** Kalau tidak ada ServiceBase, field `Validator` dan method `LogError` akan diduplikasi di 3 service. ServiceBase memusatkan semua shared logic.

---

### E3. `Middleware/ErrorHandlingMiddleware.cs` ⚠️ ADA TODO!
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Middleware/ErrorHandlingMiddleware.cs` |
| **Namespace** | `myKisah.Middleware` |
| **Teknik** | **Defensive Programming** — global exception handler |
| **Referensi** | `Task_myKisah.md` baris 221-240 |
| **Status file** | ⚠️ Skeleton ada, TODO harus diisi |

**📋 TODO yang harus dikerjakan:**

**Constructor:**
```csharp
private readonly RequestDelegate _next;

public ErrorHandlingMiddleware(RequestDelegate next)
{
    _next = next;
}
```
Simpan RequestDelegate (standar ASP.NET middleware pattern).

**Method `InvokeAsync(HttpContext context)`:**
```csharp
public async Task InvokeAsync(HttpContext context)
{
    try
    {
        await _next(context);
    }
    catch (ArgumentNullException ex)
    {
        await SetResponse(context, 400, ex.Message);
    }
    catch (ArgumentException ex)
    {
        await SetResponse(context, 400, ex.Message);
    }
    catch (KeyNotFoundException ex)
    {
        await SetResponse(context, 404, ex.Message);
    }
    catch (InvalidOperationException ex)
    {
        await SetResponse(context, 422, ex.Message);
    }
    catch (Exception ex)
    {
        await SetResponse(context, 500, "Internal server error");
    }
}
```
Logic: Coba jalankan request. Jika exception terjadi → tangkap → mapping ke HTTP status code → kirim response JSON.

**Mapping Exception → HTTP:**
| Exception | Status | Dibutuhkan oleh |
|---|---|---|
| `ArgumentNullException` | 400 | ValidationHelper.ValidateNotNull |
| `ArgumentException` | 400 | ValidationHelper.ValidateNotEmpty / InEnum |
| `KeyNotFoundException` | 404 | ValidationHelper.ValidateExists |
| `InvalidOperationException` | 422 | JournalStateMachine (transisi invalid) |
| `Exception` (fallback) | 500 | Semua exception lain |

**Method `SetResponse`:**
```csharp
private static async Task SetResponse(HttpContext context, int statusCode, string message)
{
    context.Response.StatusCode = statusCode;
    context.Response.ContentType = "application/json";
    
    var response = new { error = message, statusCode };
    var json = JsonSerializer.Serialize(response);
    await context.Response.WriteAsync(json);
}
```
Logic: Set HTTP status code → set content-type → serialize response object → kirim.

**Response format:**
```json
{
    "error": "Username tidak boleh kosong.",
    "statusCode": 400
}
```

---

### E4. `Program.cs` ⚠️ SUDAH DIISI (verifikasi)
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Program.cs` |
| **Referensi** | `Task_myKisah.md` |
| **Status** | ✅ SUDAH DIISI |

**Yang harus diverifikasi:**
1. Middleware terdaftar: `app.UseMiddleware<ErrorHandlingMiddleware>();` — paling atas
2. Controllers terdaftar: `app.MapControllers();` — sebelum MapRazorComponents
3. Semua DI registration ada (services, repositories, utils, automata)

Jojo tidak perlu ubah Program.cs karena sudah di-setup. Tapi pastikan tidak ada yang terhapus.

---

---

## RINGKASAN FASE 1 — YANG HARUS DIKERJAKAN

| Anggota | File | Total TODO |
|---|---|---|
| **Rayazka** | `Automata/JournalStateMachine.cs` | 3 method (Transition, IsTerminal, isi Dictionary) |
| **Rafly** | `Utils/FilePathConfig.cs`, `Utils/JsonStorageHelper.cs` | 2 file penuh |
| **Jojo** | `Utils/ValidationHelper.cs`, `Utils/ServiceBase.cs`, `Middleware/ErrorHandlingMiddleware.cs` | 3 file penuh |

**Semua file lain sudah SELESAI.** Tugas anggota hanya mengisi TODO di file yang disebutkan di atas.

**Urutan commit:**
1. Rayazka commit Models + Interfaces + Automata dulu
2. Rafly commit JsonStorageHelper + FilePathConfig
3. Jojo commit ValidationHelper + ServiceBase + Middleware
4. Setelah semua OK → MERGE ke `main`
