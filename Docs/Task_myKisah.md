# myKisah 🌱

**myKisah** adalah aplikasi digital journaling berbasis **C#**, **ASP.NET Web API**, dan **Blazor** yang membantu mahasiswa membangun kebiasaan positif melalui journaling harian, habit tracking, dukungan sosial (buddy system), dan karakter companion virtual.

> **Konteks proyek:** Dikerjakan oleh kelompok 5 orang dengan spesifikasi: setiap anggota wajib menerapkan 2 teknik konstruksi (maks. 2 orang per teknik), Defensive Programming/Design by Contract, Unit Testing, Performance Testing, dan version control (branch pribadi + merge ke main).

---

## Arsitektur

Aplikasi menggunakan **layered architecture** berbasis ASP.NET Web API dengan persistensi data ke file JSON (tanpa database relasional pada tahap CLO2).

```
myKisah/
├── Controllers/          # HTTP endpoint handlers (ASP.NET Web API)
├── Services/             # Business logic + validasi
├── Repositories/         # Akses dan persistensi data JSON
├── Models/               # Data model dan enum
├── Interfaces/           # Contract antar layer (dependency injection)
├── Utils/                # Helper: storage, validation, base class
├── Automata/             # State machine untuk Journal
├── Data/                 # File JSON persistensi (users, journals, characters, responses)
│   ├── users.json
│   ├── journals.json
│   ├── characters.json
│   └── characterResponses.json
└── appsettings.json      # Runtime configuration (path file, aturan journal)
├
myKisah.Tests		# Unit & Performance Testing
```

### Namespace Mapping

| Namespace                | Isi                                                                                                                     |
| ------------------------ | ----------------------------------------------------------------------------------------------------------------------- |
| `MyKisah.Models`       | Data model dan enum (`User`, `Journal`, `Character`, `CharacterResponse`, `MoodType`, `JournalState`)       |
| `MyKisah.Interfaces`   | Interface contract antar layer (`IRepository<T>`, `IUserService`, `IJournalService`, `ICharacterService`, dll.) |
| `MyKisah.Repositories` | Implementasi akses data JSON (`JsonUserRepository`, `JsonJournalRepository`, dll.)                                  |
| `MyKisah.Services`     | Business logic (`UserService`, `JournalService`, `CharacterService`)                                              |
| `MyKisah.Controllers`  | HTTP endpoint handler (`UserController`, `JournalController`, `CharacterController`)                              |
| `MyKisah.Utils`        | Helper dan utility (`JsonStorageHelper`, `FilePathConfig`, `ValidationHelper`, `ServiceBase`)                   |
| `MyKisah.Automata`     | State machine journal (`JournalStateMachine`, `JournalTrigger`)                                                     |

---

## Tim & Pembagian Modul

| Anggota                       | Modul                                                   | Teknik Konstruksi                  |
| ----------------------------- | ------------------------------------------------------- | ---------------------------------- |
| **Rayazka Aris (Azka)** | Journal System + Shared Foundation (Models, Interfaces) | Automata + Runtime Configuration   |
| **Toni Kurniawan**      | Character Companion System                              | Table-driven + API                 |
| **Farel Ilham**         | User Management                                         | Parameterization/Generics + API    |
| **Rafly Putra**         | JSON Storage Layer                                      | Runtime Configuration + Code Reuse |
| **Josefhint (Jojo)**    | Shared Utilities & Error Handling                       | Code Reuse + Generics              |

### Distribusi Teknik Konstruksi

| Teknik                    | Pemakai         | Slot         |
| ------------------------- | --------------- | ------------ |
| Automata                  | Rayazka         | 1/2          |
| Table-driven              | Toni            | 1/2          |
| Parameterization/Generics | Farel + Jojo    | 2/2 — penuh |
| Runtime Configuration     | Rayazka + Rafly | 2/2 — penuh |
| Code Reuse / Library      | Rafly + Jojo    | 2/2 — penuh |
| API                       | Toni + Farel    | 2/2 — penuh |

---

## Spesifikasi Kelas Lengkap

### Models (`MyKisah.Models`) — dikerjakan Rayazka

| Class                   | Deskripsi                                                                      | Properties Utama                                                                                                                          |
| ----------------------- | ------------------------------------------------------------------------------ | ----------------------------------------------------------------------------------------------------------------------------------------- |
| `User`                | Entitas pengguna, disimpan di `users.json`                                   | `string Id` (GUID), `string Username` (unique), `DateTime CreatedAt`                                                                |
| `Journal`             | Entri jurnal harian milik seorang user                                         | `string Id`, `string UserId`, `string Title`, `string Content`, `MoodType Mood`, `JournalState State`, `DateTime CreatedAt` |
| `Character`           | Karakter companion virtual                                                     | `string Id`, `string Name`, `string Description`                                                                                    |
| `CharacterResponse`   | Satu baris response karakter untuk mood tertentu (row pada tabel table-driven) | `string Id`, `string CharacterId`, `MoodType Mood`, `string Response`                                                             |
| `MoodType` (enum)     | Jenis mood yang valid di seluruh aplikasi                                      | `Happy`, `Sad`, `Angry`, `Anxious`, `Calm`                                                                                      |
| `JournalState` (enum) | State journal dalam state machine                                              | `Draft`, `Submitted`, `Saved`, `Rejected`                                                                                         |

---

### Interfaces (`MyKisah.Interfaces`) — dikerjakan Rayazka

| Interface                        | Deskripsi                                     | Methods                                                                            |
| -------------------------------- | --------------------------------------------- | ---------------------------------------------------------------------------------- |
| `IRepository<T>`               | Generic base interface untuk semua repository | `GetAll()`, `GetById(id)`, `Add(entity)`, `Update(entity)`, `Delete(id)` |
| `IUserRepository`              | Extends `IRepository<User>`                 | `GetByUsername(username)`, `UsernameExists(username)`                          |
| `IJournalRepository`           | Extends `IRepository<Journal>`              | `GetByUserId(userId)`                                                            |
| `ICharacterRepository`         | Extends `IRepository<Character>`            | `GetByName(name)`                                                                |
| `ICharacterResponseRepository` | Extends `IRepository<CharacterResponse>`    | `GetByMood(characterId, mood)`                                                   |
| `IUserService`                 | Kontrak business logic user                   | `RegisterUser()`, `GetAllUsers()`, `UpdateUser()`, `DeleteUser()`          |
| `IJournalService`              | Kontrak business logic journal                | `CreateJournal()`, `GetJournalsByUser()`, `DeleteJournal()`                  |
| `ICharacterService`            | Kontrak business logic companion              | `GetAllCharacters()`, `AssignCharacter()`, `GenerateResponse()`              |

---

### Automata (`MyKisah.Automata`) — dikerjakan Rayazka

Teknik konstruksi: **Automata** (state machine).

| Class                     | Deskripsi                                                                                                                                                                                                                |
| ------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `JournalTrigger` (enum) | Trigger yang memicu transisi:`Submit`, `Save`, `Reject`, `Reset`                                                                                                                                                 |
| `JournalStateMachine`   | State machine validasi alur journal. Menggunakan `Dictionary<(JournalState, JournalTrigger), JournalState>` sebagai tabel transisi. `Transition()` melempar `InvalidOperationException` jika transisi tidak valid. |

**Diagram transisi:**

```
Draft ──[Submit]──► Submitted ──[Save]──► Saved (terminal)
                        │
                     [Reject]
                        │
                        ▼
                    Rejected ──[Reset]──► Draft
```

| State Awal    | Trigger        | State Akhir   | Guard                                    |
| ------------- | -------------- | ------------- | ---------------------------------------- |
| `Draft`     | `Submit`     | `Submitted` | Title & content tidak kosong, mood valid |
| `Submitted` | `Save`       | `Saved`     | UserId harus exist di repository         |
| `Submitted` | `Reject`     | `Rejected`  | —                                       |
| `Rejected`  | `Reset`      | `Draft`     | —                                       |
| `Saved`     | *(terminal)* | —            | Tidak bisa transisi dari Saved           |

---

### Utils (`MyKisah.Utils`)

#### JsonStorageHelper — dikerjakan Rafly (teknik: Code Reuse)

Generic utility untuk membaca dan menulis data ke file JSON. Digunakan oleh **semua** repository, tidak ada duplikasi kode.

```csharp
List<T> ReadJson<T>(string filename)   // auto-create file jika belum ada
void WriteJson<T>(string filename, List<T> data)
// Private: string _basePath (dari FilePathConfig)
```

**DbC:** `filename` tidak null/kosong; `data` tidak null; path dari config tidak null; auto-create file jika belum ada.

#### FilePathConfig — dikerjakan Rafly (teknik: Runtime Configuration)

Membaca path file JSON dari `appsettings.json` via `IConfiguration`. Path tidak di-hardcode.

```csharp
string UsersFile { get; }
string JournalsFile { get; }
string CharactersFile { get; }
string ResponsesFile { get; }
```

**appsettings.json — section StoragePaths (Rafly) + JournalConfig (Rayazka):**

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
    "StoragePath": "Data/journals.json",
    "ValidMoods": ["Happy", "Sad", "Angry", "Anxious", "Calm"]
  }
}
```

#### ValidationHelper — dikerjakan Jojo (teknik: Generics + Code Reuse)

Generic helper untuk precondition checks di seluruh layer Service.

```csharp
void ValidateNotNull<T>(T value, string name)
void ValidateNotEmpty(string value, string name)
void ValidateInEnum<T>(T value, string name) where T : struct, Enum
void ValidateExists<T>(T? entity, string name)
```

#### ServiceBase — dikerjakan Jojo (teknik: Code Reuse)

Abstract base class untuk semua Service. Menyediakan shared error propagation.

```csharp
protected ValidationHelper _validator
protected abstract string ServiceName { get; }
void LogError(string msg, Exception ex)
```

---

### Repositories (`MyKisah.Repositories`)

| Class                               | Owner   | Implements                       | Keterangan                                                                   |
| ----------------------------------- | ------- | -------------------------------- | ---------------------------------------------------------------------------- |
| `JsonUserRepository`              | Farel   | `IUserRepository`              | CRUD ke `users.json` via `JsonStorageHelper`                             |
| `JsonJournalRepository`           | Rayazka | `IJournalRepository`           | CRUD ke `journals.json`, mendukung filter `GetByUserId()`                |
| `JsonCharacterRepository`         | Toni    | `ICharacterRepository`         | CRUD ke `characters.json`                                                  |
| `JsonCharacterResponseRepository` | Toni    | `ICharacterResponseRepository` | Membaca `characterResponses.json` sebagai lookup table (inti table-driven) |

Semua repository di-inject dengan `JsonStorageHelper` dan `FilePathConfig`.

---

### Services (`MyKisah.Services`)

| Class                | Owner   | Implements            | Extends         | Key Logic                                                                    |
| -------------------- | ------- | --------------------- | --------------- | ---------------------------------------------------------------------------- |
| `UserService`      | Farel   | `IUserService`      | `ServiceBase` | Validasi username unik sebelum insert                                        |
| `JournalService`   | Rayazka | `IJournalService`   | `ServiceBase` | Inject `JournalStateMachine` + baca `IConfiguration` untuk aturan jurnal |
| `CharacterService` | Toni    | `ICharacterService` | `ServiceBase` | `GenerateResponse()` lookup ke `characterResponses.json` tanpa if/switch |

---

### Controllers (`MyKisah.Controllers`)

| Class                   | Owner   | Route              | Endpoints                                                                                |
| ----------------------- | ------- | ------------------ | ---------------------------------------------------------------------------------------- |
| `UserController`      | Farel   | `/api/user`      | `GET /api/user`, `POST /api/user`, `PUT /api/user/{id}`, `DELETE /api/user/{id}` |
| `JournalController`   | Rayazka | `/api/journal`   | `GET /api/journal/{userId}`, `POST /api/journal`, `DELETE /api/journal/{id}`       |
| `CharacterController` | Toni    | `/api/character` | `GET /api/character`, `GET /api/character/{id}/response?mood={mood}`                 |

---

## Design by Contract (DbC) — Ringkasan

Setiap method di layer Service dan Repository menerapkan precondition/postcondition eksplisit via `ValidationHelper`. Violation dilempar sebagai exception yang ditangkap global middleware Jojo:

| Exception                     | HTTP Response            |
| ----------------------------- | ------------------------ |
| `ArgumentNullException`     | 400 Bad Request          |
| `ArgumentException`         | 400 Bad Request          |
| `KeyNotFoundException`      | 404 Not Found            |
| `InvalidOperationException` | 422 Unprocessable Entity |

Contoh kontrak kritis:

| Method                               | Precondition                                                                                                          | Postcondition                                  |
| ------------------------------------ | --------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------- |
| `CreateJournal()`                  | `userId` tidak null, `title` tidak kosong, `content.Length <= MaxContentLength`, `mood` ada di `ValidMoods` | Journal tersimpan dengan `State = Draft`     |
| `JournalStateMachine.Transition()` | Transisi terdaftar di `_transitions`                                                                                | `CurrentState` berubah sesuai tabel          |
| `RegisterUser()`                   | `username` tidak null/kosong, belum terdaftar                                                                       | User tersimpan dengan GUID unik                |
| `ReadJson<T>()`                    | `filename` tidak null                                                                                               | Return `List<T>`, kosong jika file belum ada |
| `GenerateResponse()`               | `characterId` tidak null, `mood` valid di `MoodType`                                                            | Return string response, bukan null             |

---

## Unit Test Coverage

### Rayazka — JournalService & JournalStateMachine

- `CreateJournal_ValidInput_Success`
- `CreateJournal_EmptyTitle_ThrowsException`
- `CreateJournal_InvalidMood_ThrowsException`
- `CreateJournal_ContentTooLong_ThrowsException`
- `StateMachine_Draft_Submit_Submitted`
- `StateMachine_Submitted_Save_Saved`
- `StateMachine_Submitted_Reject_Rejected`
- `StateMachine_InvalidTransition_ThrowsException`
- `GetJournalsByUser_ReturnsCorrectData`

### Toni — CharacterService

- `GenerateResponse_HappyMood_ReturnsResponse`
- `GenerateResponse_SadMood_ReturnsResponse`
- `GenerateResponse_AllMoods_Covered`
- `GenerateResponse_InvalidMood_ThrowsException`
- `GenerateResponse_NullCharacterId_ThrowsException`

### Farel — UserService

- `RegisterUser_ValidUsername_Success`
- `RegisterUser_DuplicateUsername_ThrowsException`
- `RegisterUser_EmptyUsername_ThrowsException`
- `GetAllUsers_ReturnsAllRecords`
- `DeleteUser_ExistingId_Success`
- `DeleteUser_NonExistingId_ThrowsException`
- `UpdateUser_ValidInput_Success`

### Rafly — JsonStorageHelper

- `ReadJson_ExistingFile_ReturnsData`
- `ReadJson_MissingFile_ReturnsEmpty`
- `WriteJson_ValidData_PersistsCorrectly`
- `ReadJson_NullFilename_ThrowsException`
- `WriteJson_NullData_ThrowsException`
- `FilePathConfig_LoadsPathsFromConfig`

### Jojo — ValidationHelper & Middleware

- `ValidateNotNull_NullValue_ThrowsException`
- `ValidateNotNull_ValidValue_NoException`
- `ValidateNotEmpty_EmptyString_ThrowsException`
- `ValidateInEnum_InvalidValue_ThrowsException`
- `ValidateExists_NullEntity_ThrowsException`
- `Middleware_CatchesArgumentException_Returns400`
- `Middleware_CatchesKeyNotFoundException_Returns404`

---

## Performance Benchmarks (Target)

| Skenario                                                      | Target      |
| ------------------------------------------------------------- | ----------- |
| `GetJournalsByUser()` — 10 entri                           | < 10ms      |
| `GetJournalsByUser()` — 100 entri                          | < 50ms      |
| `GetJournalsByUser()` — 1000 entri                         | < 200ms     |
| `CreateJournal()` termasuk state machine transition         | < 20ms      |
| `GenerateResponse()` setelah tabel ter-cache (1000 iterasi) | < 1ms/call  |
| Load `characterResponses.json` cold start                   | < 30ms      |
| `ReadJson` payload 100KB (~1000 item)                       | < 100ms     |
| Round-trip `WriteJson` + `ReadJson` (100 item)            | < 50ms      |
| ValidationHelper — 1000 panggilan `ValidateNotNull`        | < 5ms total |
| Pipeline end-to-end Controller → Service → Repository       | < 50ms      |

---

## Urutan Pengerjaan & Dependensi

```
Minggu 1
├── Hari 1–2: Rayazka → Models + Enum  (TIDAK ADA DEPENDENSI — mulai pertama)
│             Rafly   → appsettings.json + FilePathConfig + JsonStorageHelper (paralel)
├── Hari 2–3: Rayazka → Interfaces (bergantung: Models)
├── Hari 3–4: Jojo    → ValidationHelper + ServiceBase + Middleware (bergantung: Models + Interfaces)
└── Hari 3–5: Rayazka → JournalStateMachine + JournalTrigger (bergantung: Models)

Minggu 2 (semua paralel, bergantung pada output Minggu 1 di main branch)
├── Farel   → JsonUserRepository + UserService + UserController
├── Rayazka → JsonJournalRepository + JournalService + JournalController
└── Toni    → JsonCharacterRepository + JsonCharacterResponseRepository + CharacterService + CharacterController

Minggu 3
└── Semua   → Unit test + Performance test masing-masing modul
```

> **Aturan penting:** Output Minggu 1 (Models, Interfaces, JsonStorageHelper) **wajib di-commit ke branch main** sebelum Minggu 2 dimulai. Jangan merge ke main dalam keadaan incomplete karena semua anggota bergantung pada class-class ini.

---

## Struktur File Data (JSON)

### `users.json`

```json
[
  { "id": "uuid", "username": "rayazka", "createdAt": "2025-01-01T00:00:00Z" }
]
```

### `journals.json`

```json
[
  {
    "id": "uuid",
    "userId": "uuid",
    "title": "Hari yang produktif",
    "content": "Isi jurnal...",
    "mood": "Happy",
    "state": "Draft",
    "createdAt": "2025-01-01T00:00:00Z"
  }
]
```

### `characters.json`

```json
[
  { "id": "uuid", "name": "Kira", "description": "Karakter yang selalu optimis" }
]
```

### `characterResponses.json`

Tabel lookup untuk teknik table-driven Toni. Menambah response baru cukup edit file ini tanpa ubah kode.

```json
[
  { "id": "uuid", "characterId": "uuid", "mood": "Happy", "response": "Senang mendengarnya! Terus semangat ya!" },
  { "id": "uuid", "characterId": "uuid", "mood": "Sad",   "response": "Tidak apa-apa, aku di sini untukmu." },
  { "id": "uuid", "characterId": "uuid", "mood": "Angry", "response": "Tarik napas dulu, ceritakan padaku." }
]
```

---

## Teknologi

| Layer           | Teknologi                                            |
| --------------- | ---------------------------------------------------- |
| Framework       | ASP.NET Web API (.NET 8)                             |
| Bahasa          | C#                                                   |
| Persistensi     | File JSON (System.Text.Json)                         |
| Testing         | xUnit + BenchmarkDotNet / Stopwatch                  |
| Version Control | Git — branch pribadi per anggota, merge ke `main` |

---

## Business Rules

### User

| ID   | Rule                                |
| ---- | ----------------------------------- |
| U-01 | Setiap user memiliki Id unik (GUID) |
| U-02 | Username tidak boleh duplikat       |
| U-03 | Username tidak boleh kosong         |

### Journal

| ID   | Rule                                                                          |
| ---- | ----------------------------------------------------------------------------- |
| J-01 | User hanya dapat mengakses journal miliknya sendiri                           |
| J-02 | Journal wajib punya title dan content tidak kosong                            |
| J-03 | Panjang content maksimal `MaxContentLength` (dari config)                   |
| J-04 | Mood wajib merupakan nilai yang ada di `ValidMoods` (dari config)           |
| J-05 | Journal mengikuti alur state machine:`Draft → Submitted → Saved/Rejected` |

### Character Companion

| ID   | Rule                                                                                           |
| ---- | ---------------------------------------------------------------------------------------------- |
| C-01 | Response karakter diambil dari `characterResponses.json` (table-driven, tidak ada if/switch) |
| C-02 | Setiap kombinasi `characterId + mood` harus memiliki entry di tabel                          |
| C-03 | `characterId` tidak boleh null saat meminta response                                         |

---

\---

*myKisah — Build habits, write your journey, grow together. 🌱*
