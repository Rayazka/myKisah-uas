# Laporan Tugas Besar CLO4 — myKisah

> **Mata Kuliah:** Konstruksi Perangkat Lunak  
> **Semester:** Genap 2025/2026  
> **Framework:** .NET 10 Blazor Server + ASP.NET Web API  

---

## 1. Deskripsi Aplikasi

### 1.1 Nama & Tagline

**myKisah** — *Build habits, write your journey, grow together.*

### 1.2 Latar Belakang

Banyak mahasiswa gagal menjaga konsistensi kebiasaan karena tidak memiliki sistem tracking yang terstruktur dan tidak adanya support system. Data penggunaan aplikasi seperti Duolingo menunjukkan bahwa virtual companion mampu meningkatkan retensi harian pengguna. myKisah mengadopsi pendekatan serupa untuk mendukung pembentukan kebiasaan positif.

### 1.3 Fitur Utama

| Fitur | Deskripsi |
|-------|-----------|
| **Journaling Harian** | User menulis jurnal dengan mood (Happy/Sad/Angry/Anxious/Calm) — auto-saved jika ada konten |
| **Character Companion** | 4 karakter virtual (Kira, Luna, Ren, Ryo) merespons mood user via table-driven & AI (Ollama) |
| **State Machine** | Journal mengikuti lifecycle: Draft → Submitted → Saved/Rejected melalui automata |
| **AI Chat** | CharacterChat dengan streaming response dari Ollama (llama3.2:3b) — token-by-token |
| **Filter & Edit** | Filter journal by mood, edit journal di semua state, pilih character companion |

### 1.4 Alur Aplikasi

```
User buka / → Login/Register → /journals → Tulis Journal → Auto-Saved
                                          → Klik Journal → Detail + Edit + State Transition
                       → /characters → Pilih Karakter → AI Chat / Static Chat
                                          → Tambah Karakter Baru
                       → /debug → API Test Dashboard
```

---

## 2. Informasi Proyek

### 2.1 Link GitHub

**https://github.com/Rayazka/myKisah-uas** (Public)

### 2.2 Link Video Presentasi

*[TODO: Masukkan link YouTube presentasi]*

### 2.3 Teknologi

| Layer | Teknologi |
|-------|-----------|
| Framework | ASP.NET Web API + Blazor Server (.NET 10) |
| Bahasa | C# |
| UI | Blazor Components (Razor) + Bootstrap 5 + CSS Variables |
| Persistensi | File JSON (System.Text.Json) |
| AI | Ollama API — model `llama3.2:3b` (local GPU) |
| Testing | xUnit + Moq |
| Version Control | Git — GitHub, 6 branch aktif |

### 2.4 Konfigurasi AI (Ollama)

```json
"OllamaConfig": {
    "BaseUrl": "http://192.168.100.105:11434",
    "Model": "llama3.2:3b"
}
```

Ollama berjalan di Docker container (`mnccouk/ollama-gpu-rx580`) pada server lokal. CharacterChat mendukung dual mode: **AI** (streaming response) dan **Static** (table-driven). Jika AI offline, otomatis fallback ke mode Static.

---

## 3. Anggota Kelompok & Kontribusi

### 3.1 Tabel Anggota

| Anggota | Branch | Role | Teknik CLO2 | File Utama |
|---------|--------|------|-------------|------------|
| **Rayazka Arisfadhilah (Azka)** | `ray` | Journal System, Layout, Verifikasi, AI Integration | Automata, Runtime Configuration | `JournalStateMachine.cs`, `JournalService.cs`, `MainLayout.razor`, `NavMenu.razor`, `JournalList.razor`, `JournalDetail.razor`, `OllamaService.cs`, `Program.cs` |
| **Farel Ilham** | `farel` | User Management, Login, Journal Form | Generics, API | `UserService.cs`, `JsonUserRepository.cs`, `Login.razor`, `JournalCreate.razor` |
| **Muhamad Toni Kurniawan** | `toni` | Character Companion System | Table-Driven, API | `CharacterService.cs`, `JsonCharacterRepository.cs`, `CharacterList.razor`, `CharacterChat.razor` |
| **Rafly Putra** | `rafly` | Shared Visual Components + JSON Storage | Runtime Configuration, Code Reuse | `MoodPicker.razor`, `ConfirmDialog.razor`, `JsonStorageHelper.cs`, `FilePathConfig.cs` |
| **Josefhint (Jojo)** | `jojo` | Shared Functional Components, Error Handling | Generics, Code Reuse | `StateBadge.razor`, `AlertMessage.razor`, `EmptyState.razor`, `ValidationHelper.cs`, `ServiceBase.cs`, `ErrorHandlingMiddleware.cs` |

### 3.2 Distribusi Branch & Commit

| Akun Git | Commits |
|----------|---------|
| ray (Azka) | 28 |
| Ozz__ (Azka) | 22 |
| raflyputra190 (Rafly) | 22 |
| BetotPetot (Farel) | 13 |
| Muhamad Toni Kurniawan (Toni) | 10 |
| josefhint755-netizen (Jojo) | 10 |
| MuhamadToni23 (Toni) | 3 |

### 3.3 Pembagian Tugas per Fase

```
FASE 1: Azka — Pondasi (Task 1-6)
  UserSession, DI, _Imports, MainLayout, NavMenu, Debug
  ↓

FASE 2: Semua anggota paralel
  Azka:   JournalList, JournalDetail (Task 7-8)
  Farel:  Login, JournalCreate (Task 10-11)
  Toni:   CharacterList, CharacterChat (Task 12-13)
  Rafly:  MoodPicker, ConfirmDialog (Task 14-15)
  Jojo:   StateBadge, AlertMessage, EmptyState (Task 16-18)
  ↓

FASE 3: Azka — Verifikasi + AI Integration (Task 9 + bonus)
  Build, test, smoke test, bug fixing, Ollama AI integration
```

---

## 4. Arsitektur Aplikasi

### 4.1 Layered Architecture

```
HTTP Request → Controller → Service → Repository → JSON File
                   │            │           │
              (validasi    (business    (data access
                input)       logic)       only)
```

Aplikasi menggunakan **Layered Architecture** dengan persistensi data berbasis file JSON. Dependency Injection digunakan untuk menghubungkan antar layer.

### 4.2 Struktur Folder

```
myKisah/
├── Components/
│   ├── App.razor, Routes.razor, _Imports.razor
│   ├── Layout/
│   │   ├── MainLayout.razor + .css          (top navbar, full-width)
│   │   ├── LoginLayout.razor + .css         (centered card)
│   │   ├── NavMenu.razor + .css             (horizontal nav)
│   │   └── ReconnectModal.razor + .css + .js
│   ├── Pages/
│   │   ├── Login.razor + .css               (route: /)
│   │   ├── JournalList.razor + .css         (route: /journals)
│   │   ├── JournalCreate.razor + .css       (route: /journals/new)
│   │   ├── JournalDetail.razor + .css       (route: /journals/{Id})
│   │   ├── CharacterList.razor + .css       (route: /characters)
│   │   ├── CharacterCreate.razor + .css     (route: /characters/new)
│   │   ├── CharacterChat.razor + .css       (route: /characters/{Id})
│   │   ├── Debug.razor + .css               (route: /debug)
│   │   ├── Error.razor, NotFound.razor
│   └── Shared/
│       ├── MoodPicker.razor + .css          (5 mood chips + "Semua")
│       ├── StateBadge.razor                 (4 state colors)
│       ├── AlertMessage.razor               (success/error alerts)
│       ├── EmptyState.razor                 (no-data placeholder)
│       ├── ConfirmDialog.razor              (delete confirmation modal)
│       └── MoodHelper.cs                    (emoji mapping utility)
├── Services/
│   ├── UserSession.cs                       (scoped per-circuit)
│   ├── UserService.cs
│   ├── JournalService.cs
│   ├── CharacterService.cs
│   └── OllamaService.cs                     (AI streaming client)
├── Repositories/
│   ├── JsonUserRepository.cs
│   ├── JsonJournalRepository.cs
│   ├── JsonCharacterRepository.cs
│   └── JsonCharacterResponseRepository.cs
├── Controllers/                             (API endpoints)
│   ├── UserController.cs
│   ├── JournalController.cs
│   └── CharacterController.cs
├── Models/
│   ├── User.cs, Journal.cs, Character.cs
│   ├── CharacterResponse.cs
│   └── AiModels.cs                          (Ollama DTOs)
├── Interfaces/
│   ├── IRepository.cs                       (generic CRUD)
│   ├── IUserRepository.cs, IJournalRepository.cs
│   ├── ICharacterRepository.cs, ICharacterResponseRepository.cs
│   ├── IUserService.cs, IJournalService.cs, ICharacterService.cs
├── Automata/
│   └── JournalStateMachine.cs               (state machine)
├── Utils/
│   ├── JsonStorageHelper.cs
│   ├── FilePathConfig.cs
│   ├── ValidationHelper.cs, Validator.cs
│   ├── ServiceBase.cs
│   ├── MoodMapper.cs                        (5→8 AI mood mapping)
│   └── ErrorHandlingMiddleware.cs
├── Data/
│   ├── users.json, journals.json
│   ├── characters.json, characterResponses.json
├── Program.cs, appsettings.json
└── wwwroot/
    ├── app.css, css/shared.css
    └── lib/bootstrap/
```

### 4.3 Alur Navigasi

```
/                       → Login.razor
/journals               → JournalList.razor       (wajib login)
/journals/new           → JournalCreate.razor     (wajib login)
/journals/{Id}          → JournalDetail.razor     (wajib login)
/characters             → CharacterList.razor     (wajib login)
/characters/new         → CharacterCreate.razor   (wajib login)
/characters/{Id}        → CharacterChat.razor     (wajib login)
/debug                  → Debug.razor             (bebas akses)
```

### 4.4 Dependency Injection (Program.cs)

```csharp
// Config
builder.Services.AddSingleton<FilePathConfig>();

// Utils
builder.Services.AddSingleton<JsonStorageHelper>();
builder.Services.AddSingleton<ValidationHelper>();

// Automata
builder.Services.AddSingleton<JournalStateMachine>();

// Repositories (Singleton)
builder.Services.AddSingleton<IUserRepository, JsonUserRepository>();
builder.Services.AddSingleton<IJournalRepository, JsonJournalRepository>();
builder.Services.AddSingleton<ICharacterRepository, JsonCharacterRepository>();
builder.Services.AddSingleton<ICharacterResponseRepository, JsonCharacterResponseRepository>();

// Services (Scoped)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJournalService, JournalService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();

// Session (Scoped)
builder.Services.AddScoped<UserSession>();

// Ollama AI
builder.Services.AddHttpClient<OllamaService>();
```

---

## 5. Implementasi Teknik CLO2

### 5.1 Automata — JournalStateMachine

**File:** `Automata/JournalStateMachine.cs`

Menggunakan Dictionary sebagai tabel transisi — tanpa if-else:

```
Draft ──[Submit]──► Submitted ──[Save]──► Saved (terminal)
                         │
                      [Reject]
                         │
                         ▼
                     Rejected ──[Reset]──► Draft
```

```csharp
// Tabel transisi sebagai Dictionary
private readonly Dictionary<(JournalState, JournalTrigger), JournalState> _transitions = new()
{
    { (JournalState.Draft, JournalTrigger.Submit), JournalState.Submitted },
    { (JournalState.Submitted, JournalTrigger.Save), JournalState.Saved },
    { (JournalState.Submitted, JournalTrigger.Reject), JournalState.Rejected },
    { (JournalState.Rejected, JournalTrigger.Reset), JournalState.Draft },
};

public JournalState Transition(JournalState current, JournalTrigger trigger)
{
    if (_transitions.TryGetValue((current, trigger), out var next))
        return next;
    throw new InvalidOperationException($"Invalid transition: {current} -> {trigger}");
}
```

**Kenapa Automata?** Journal punya aturan transisi ketat. Tanpa automata, perlu if-else di banyak tempat. Dengan Dictionary, semua aturan di satu tabel. Invalid transition → `InvalidOperationException` → middleware → 422.

---

### 5.2 Table-Driven — CharacterResponse System

**File:** `Repositories/JsonCharacterResponseRepository.cs`, `Services/CharacterService.cs`

Response karakter tidak di-hardcode di kode C#. Semua data response ada di `characterResponses.json`. Method `GetByMood()` hanya melakukan LINQ Where — **tidak ada if/switch**:

```csharp
// Repository: pure table lookup
public IEnumerable<CharacterResponse> GetByMood(string characterId, MoodType mood)
{
    return GetAll().Where(r => r.CharacterId == characterId && r.Mood == mood);
}

// Service: hanya delegasi ke repository
public string GenerateResponse(string characterId, MoodType mood)
{
    var response = _responseRepository.GetByMood(characterId, mood).FirstOrDefault();
    return response?.Response ?? "Maaf, aku tidak tahu harus merespons apa.";
}
```

**Kenapa Table-Driven?** Tambah response baru = edit JSON file, tidak perlu ubah kode C#. Separation of data dan logic.

---

### 5.3 Generics — IRepository<T>

**File:** `Interfaces/IRepository.cs`, `Utils/JsonStorageHelper.cs`

```csharp
// Generic base interface — satu kontrak untuk semua tipe data
public interface IRepository<T>
{
    IEnumerable<T> GetAll();
    T? GetById(string id);
    void Add(T entity);
    void Update(T entity);
    void Delete(string id);
}

// Spesialisasi via inheritance
public interface IUserRepository : IRepository<User>
{
    User? GetByUsername(string username);
    bool UsernameExists(string username);
}
```

**Kenapa Generics?** Tanpa generics, perlu membuat interface terpisah untuk setiap tipe: `IUserRepository`, `IJournalRepository`, `ICharacterRepository` — masing-masing dengan method identik. Dengan `IRepository<T>`, satu generic type parameter menghandle semuanya.

---

### 5.4 Runtime Configuration

**File:** `appsettings.json`, `Utils/FilePathConfig.cs`, `Services/JournalService.cs`

```json
// appsettings.json
"JournalConfig": {
    "MaxContentLength": 5000,
    "ValidMoods": ["Happy", "Sad", "Angry", "Anxious", "Calm"]
},
"OllamaConfig": {
    "BaseUrl": "http://192.168.100.105:11434",
    "Model": "llama3.2:3b"
},
"StoragePaths": {
    "UsersFile": "Data/users.json",
    "JournalsFile": "Data/journals.json",
    "CharactersFile": "Data/characters.json",
    "ResponsesFile": "Data/characterResponses.json"
}
```

```csharp
// Dibaca saat runtime — tidak di-hardcode
int maxLength = _configuration.GetValue<int>("JournalConfig:MaxContentLength");
var validMoods = _configuration.GetSection("JournalConfig:ValidMoods").Get<string[]>();
```

**Kenapa Runtime Config?** Ubah batas karakter, path file, atau URL AI: cukup edit config, tidak perlu recompile.

---

### 5.5 Code Reuse — Shared Foundation

| Komponen | Digunakan Oleh | File |
|----------|---------------|------|
| `ServiceBase` | Semua Service | `Utils/ServiceBase.cs` |
| `JsonStorageHelper` | Semua Repository | `Utils/JsonStorageHelper.cs` |
| `ValidationHelper` | Semua Service | `Utils/ValidationHelper.cs` |
| `MoodPicker` | JournalCreate, JournalList, CharacterChat | `Shared/MoodPicker.razor` |
| `StateBadge` | JournalList, JournalDetail, Debug | `Shared/StateBadge.razor` |
| `AlertMessage` | Semua halaman | `Shared/AlertMessage.razor` |
| `EmptyState` | JournalList, CharacterChat | `Shared/EmptyState.razor` |
| `ConfirmDialog` | JournalList, JournalDetail | `Shared/ConfirmDialog.razor` |
| `MoodHelper` | JournalList, JournalDetail | `Shared/MoodHelper.cs` |

---

### 5.6 Defensive Programming

**File:** `Utils/ValidationHelper.cs`, `Utils/ServiceBase.cs`

```csharp
// Precondition checks — fail fast dengan exception jelas
public static void ValidateNotEmpty(string? value, string paramName)
{
    if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException($"{paramName} tidak boleh kosong.", paramName);
}

public static void ValidateExists<T>(T? entity, string message)
{
    if (entity == null)
        throw new KeyNotFoundException(message);
}

public static void ValidateInEnum<T>(T value, string paramName) where T : Enum
{
    if (!Enum.IsDefined(typeof(T), value))
        throw new ArgumentException($"{paramName} tidak valid.", paramName);
}
```

Setiap exception ditangkap oleh `ErrorHandlingMiddleware` dan dikonversi ke HTTP response yang aman (tidak leak stack trace ke client).

---

## 6. Design Pattern — State Machine

**Pattern:** State Pattern via Dictionary-based State Machine

**Lokasi:** `Automata/JournalStateMachine.cs`

**Diagram State:**

```
                    ┌──────────────┐
          ┌────────►│    Draft     │◄────────┐
          │         └──────┬───────┘         │
          │                │ Submit          │
          │                ▼                 │
          │         ┌──────────────┐         │
          │         │  Submitted   │         │
          │         └──┬────────┬──┘         │
          │     Save   │        │  Reject    │
          │            ▼        ▼            │
          │   ┌──────────┐  ┌──────────┐     │
          │   │  Saved   │  │ Rejected │─────┘
          │   │(terminal)│  └──────────┘ Reset
          │   └──────────┘
          │
          └─── (Edit mengembalikan ke state yang diinginkan)
```

**Kenapa State Pattern?** Setiap aksi user (Submit, Save, Reject, Reset) adalah trigger yang mengubah state journal. Tanpa state machine, semua kombinasi state + aksi harus di-handle manual dengan if-else bersarang. Dengan state machine, aturan transisi terpusat di satu dictionary — mudah di-maintain dan di-test.

---

## 7. Clean Code — Contoh Penerapan

### 7.1 Consistent Error Handling Pattern

Semua halaman menggunakan pattern try-catch-finally yang seragam:

```csharp
// Pola standar di semua page
private async Task SomeAction()
{
    try
    {
        _hasError = false;
        _error = "";
        _loading = true;
        // ... business logic ...
    }
    catch (Exception ex)
    {
        _hasError = true;
        _error = ex.Message;
    }
    finally
    {
        _loading = false;  // always reset
    }
}
```

### 7.2 DRY — Login Session Pattern

Semua halaman yang butuh login menggunakan pattern identik untuk session restore:

```csharp
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (!firstRender) return;

    if (!Session.IsLoggedIn)
    {
        var userId = await JS.InvokeAsync<string>("sessionStorage.getItem", "myKisah.userId");
        if (!string.IsNullOrEmpty(userId))
        {
            var user = UserService.GetAllUsers().FirstOrDefault(u => u.Id == userId);
            if (user != null) Session.Login(user);
        }
    }

    if (!Session.IsLoggedIn) { Navigation.NavigateTo("/", forceLoad: true); return; }
    // ... load data ...
    await InvokeAsync(StateHasChanged);
}
```

### 7.3 Meaningful Names

| Nama | Purpose |
|------|---------|
| `UserSession` | Service scoped per circuit — jelas ini sesi user |
| `JournalStateMachine` | Automata untuk lifecycle journal — deskriptif |
| `MoodMapper.ToAiMood()` | Mapping 5 mood → 8 AI mood — verb yang jelas |
| `StreamCharacterResponseAsync` | Streaming token-by-token — IAsyncEnumerable |
| `ApplyFilter()` | Filter journal list — action yang jelas |

### 7.4 Single Responsibility

| Class | Responsibility |
|-------|---------------|
| `OllamaService` | Hanya komunikasi dengan Ollama API |
| `JournalService` | Hanya business logic journal |
| `JsonStorageHelper` | Hanya baca/tulis JSON file |
| `MoodPicker.razor` | Hanya render chip mood + event callback |
| `ErrorHandlingMiddleware` | Hanya konversi exception → HTTP response |

---

## 8. Secure Coding — Contoh Penerapan

### 8.1 Input Validation

Semua input user divalidasi sebelum diproses:

```csharp
// JournalService.CreateJournal()
Validator.ValidateNotEmpty(userId, "UserId");
Validator.ValidateNotEmpty(title, "Title");
Validator.ValidateNotEmpty(content, "Content");
Validator.ValidateInEnum(mood, "Mood");

// Validasi tambahan: content length dan mood validity
if (content.Length > maxLength)
    throw new ArgumentException($"Content tidak boleh lebih dari {maxLength} karakter");
if (!validMoods.Contains(mood.ToString()))
    throw new ArgumentException($"Mood {mood} tidak valid.");
```

### 8.2 Global Error Handling — No Stack Trace Leak

```csharp
// ErrorHandlingMiddleware.cs
public async Task InvokeAsync(HttpContext context)
{
    try
    {
        await _next(context);
    }
    catch (Exception ex)
    {
        // Stack trace TIDAK dikirim ke client — hanya safe message
        var (statusCode, message) = ex switch
        {
            ArgumentNullException => (400, ex.Message),
            ArgumentException => (400, ex.Message),
            KeyNotFoundException => (404, ex.Message),
            InvalidOperationException => (422, ex.Message),
            _ => (500, "Terjadi kesalahan internal.")
        };
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new { error = message });
    }
}
```

### 8.3 Anti-Forgery & HTTPS

```csharp
// Program.cs
app.UseHttpsRedirection();    // force HTTPS
app.UseAntiforgery();         // CSRF protection
```

### 8.4 No Hardcoded Secrets

Semua konfigurasi sensitif (Ollama URL, file paths, limits) dibaca dari `appsettings.json`:

```csharp
// Ollama URL dari config — bukan hardcoded
_http.BaseAddress = new Uri(configuration["OllamaConfig:BaseUrl"]);

// File paths dari config
public string UsersFile => _configuration["StoragePaths:UsersFile"] ?? "Data/users.json";
```

### 8.5 Null-Safe Code

```csharp
// JournalDetail.razor — defensive null check
var userId = Session.CurrentUser?.Id;
if (userId == null) { Navigation.NavigateTo("/", forceLoad: true); return; }

// Model dengan nullable reference types
public string? CharacterId { get; set; }    // optional field
public User? CurrentUser { get; private set; } // bisa null sebelum login
```

---

## 9. Screenshot Halaman GUI

*[TODO: Masukkan screenshot tiap halaman]*

| # | Halaman | Route | Dibuat Oleh |
|---|---------|-------|-------------|
| 1 | Login | `/` | Farel |
| 2 | Journal List | `/journals` | Azka |
| 3 | Journal Create | `/journals/new` | Farel |
| 4 | Journal Detail | `/journals/{Id}` | Azka |
| 5 | Character List | `/characters` | Toni |
| 6 | Character Create | `/characters/new` | Azka (bonus) |
| 7 | Character Chat | `/characters/{Id}` | Toni + Azka (AI) |
| 8 | Debug Dashboard | `/debug` | Azka |
| 9 | Navbar (Layout) | *(semua halaman)* | Azka |
| 10 | State Components | *(shared)* | Rafly + Jojo |

---

## 10. Integrasi Halaman

### 10.1 Diagram Koneksi Antar Halaman

```
                        ┌─────────────────┐
                        │   NavMenu.razor  │
                        │ myKisah | Journal│
                        │ | Characters     │
                        └────────┬────────┘
                                 │
              ┌──────────────────┼──────────────────┐
              ▼                  ▼                  ▼
     ┌────────────┐    ┌──────────────┐    ┌──────────────┐
     │ Login.razor│    │JournalList   │    │CharacterList │
     │     /      │───►│  /journals   │    │ /characters  │
     └────────────┘    └──┬───────┬───┘    └──┬───────┬───┘
                          │       │            │       │
                 ┌────────▼──┐ ┌──▼────────┐  │  ┌────▼─────────┐
                 │JournalNew │ │JournalDtl  │  │  │CharacterChat │
                 │/journals/ │ │/journals/  │  │  │/characters/  │
                 │   new     │ │   {Id}     │  │  │    {Id}      │
                 └───────────┘ └────────────┘  │  └──────────────┘
                                               │
                                               │  ┌───────────────┐
                                               └──►│CharacterNew   │
                                                   │/characters/new│
                                                   └───────────────┘
```

Semua halaman terhubung melalui NavMenu dan navigasi internal Blazor. Tidak ada halaman yang terisolasi.

---

## 11. Development History

### 11.1 Fase 1: Backend Foundation (CLO2)

| Timeline | Milestone | Detail |
|----------|-----------|--------|
| Awal | Inisialisasi Project | Pembuatan struktur folder, Models, Interfaces |
| Minggu 1-2 | Automata + Generics | Azka: JournalStateMachine, IRepository<T> |
| Minggu 2-3 | Repositories | Semua anggota: JsonUserRepo, JsonJournalRepo, JsonCharacterRepo, JsonCharResponseRepo |
| Minggu 3-4 | Services + Controllers | Farel: UserService, Toni: CharacterService, Azka: JournalService |
| Minggu 4-5 | Utils + Error Handling | Jojo: ValidationHelper, ServiceBase, ErrorHandlingMiddleware |
| Minggu 5-6 | Testing | 65 unit tests across all modules (17 test files) |

### 11.2 Fase 2: Blazor Frontend (CLO4)

| Timeline | Milestone | Detail |
|----------|-----------|--------|
| Minggu 6-7 | Layout & Navigation | Azka: MainLayout, NavMenu, UserSession, Debug |
| Minggu 7-8 | Shared Components | Rafly: MoodPicker, ConfirmDialog; Jojo: StateBadge, AlertMessage, EmptyState |
| Minggu 8-9 | Journal Pages | Azka: JournalList, JournalDetail; Farel: Login, JournalCreate |
| Minggu 9-10 | Character Pages | Toni: CharacterList, CharacterChat |
| Minggu 10-11 | Bug Fixing & Polish | Azka: session management, StateHasChanged fixes, edit mode |
| Minggu 11-12 | AI Integration | Azka: OllamaService, MoodMapper, streaming chat |

### 11.3 Timeline Commit (Branch `ray` — 106 commits)

```
Commit Highlights (kronologis):

Fase 1 (Backend):
  ad5986d  create model class
  b243b95  Perbaikan struktur folder + Update Models
  7cf1b92  Middleware + folder kerangka kerja
  9282c6f  Automata - Journal State Machine
  0a1ea6b  Repositories Jurnal
  90cdff7  Services Jurnal
  beed057  Controller Jurnal
  e81d756  Modul User Repository, Service, Controller (Farel)
  7d6fbeb  FilePathConfig (Rafly)
  87ae300  Utils - ServiceBase, ValidationHelper, ErrorHandling (Jojo)
  e9b61bf  Character modul (Toni)

Fase 2 (Frontend):
  4daf7c4  MoodPicker chip mood emoji + CSS (Rafly)
  b0b5349  Login + JournalCreate (Farel)
  5dd7214  Shared components: StateBadge, AlertMessage, EmptyState (Jojo)
  6bfe3df  CharacterList + CharacterChat (Toni)
  79a8b24  JournalList + JournalDetail + create pages (Azka)
  bed628d  Fix login session, StateHasChanged, edit mode, companion (Azka)
  
Fase 3 (Polish + AI):
  (current) AI Integration — OllamaService, MoodMapper, streaming chat
```

---

## 12. Testing

### 12.1 Unit Tests

| Metrik | Nilai |
|--------|-------|
| Total test | **65** |
| Passed | 65 (100%) |
| Failed | 0 |
| Test files | 17 `.cs` files |
| Framework | xUnit + Moq |

### 12.2 Test Coverage per Modul

| Modul | Test Files |
|-------|-----------|
| User | `UserServiceTests.cs` |
| Journal | `JournalServiceTests.cs`, `JournalStateMachineTests.cs` |
| Character | `CharacterTest.cs` |
| Validation | `ValidationHelperTests.cs` |
| Storage | `StorageTest.cs`, `JsonStorageHelperTests.cs` |
| Performance | `PerformanceBenchmarks.cs` |

---

## 13. Compliance Checklist vs Spesifikasi CLO4

### Kelompok (35%)

| # | Kriteria | Bobot | Status | Bukti |
|---|----------|-------|--------|-------|
| 1 | Remote Repository | 5% | ✅ | `github.com/Rayazka/myKisah-uas` — public, 6 branch aktif |
| 2 | Laporan | 5% | ✅ | Dokumen ini |
| 3 | Design Pattern | 15% | ✅ | State Machine Pattern (`JournalStateMachine`) |
| 4 | Integrasi Halaman | 10% | ✅ | Semua 10 halaman terhubung via NavMenu + navigasi internal |

### Individu (65%)

| # | Kriteria | Bobot | Status | Bukti |
|---|----------|-------|--------|-------|
| 1 | Version Control | 10% | ✅ | Branch `ray`, 106 commits, merge ke `main` |
| 2 | Halaman GUI | 10% | ✅ | 10 halaman Blazor (konfirmasi Blazor approved oleh dosen) |
| 3 | Implementasi CLO2 | 15% | ✅ | 6 teknik: Automata, Table-Driven, Generics, Runtime Config, Code Reuse, Defensive Programming |
| 4 | Clean Code | 10% | ✅ | Consistent patterns, DRY, meaningful names, 0 TODO di code |
| 5 | Secure Coding | 10% | ✅ | Input validation, error middleware, anti-forgery, HTTPS, no hardcoded secrets |
| 6 | Presentasi | 10% | ❓ | [TODO: rekam & upload video] |

### Laporan LMS

| # | Komponen | Status |
|---|----------|--------|
| 1 | Deskripsi singkat aplikasi | ✅ Section 1 |
| 2 | Link GitHub public | ✅ Section 2.1 |
| 3 | Link video YouTube | ❓ [TODO] |
| 4 | Daftar anggota + teknik | ✅ Section 3.1 |
| 5 | Screenshot halaman GUI | ❓ [TODO] |
| 6 | Contoh clean code | ✅ Section 7 |
| 7 | Contoh secure coding | ✅ Section 8 |

---

## Status Akhir: ✅ 90% Complete

Semua requirement kode terpenuhi. Tersisa 2 item non-kode:
- Screenshot halaman GUI
- Rekaman & upload video presentasi

---

> *myKisah — Build habits, write your journey, grow together.*  
> *Dibuat dengan ❤️ oleh Kelompok KPL — Semester Genap 2025/2026*
