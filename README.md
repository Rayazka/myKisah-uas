# myKisah 🌱

**myKisah** adalah aplikasi journaling self-improvement berbasis **C#**, **ASP.NET Web API (.NET 10)**, dan **Blazor** yang membantu mahasiswa membangun kebiasaan positif melalui journaling harian dan character companion virtual.

---

## Latar Belakang

Banyak mahasiswa gagal menjaga konsistensi kebiasaan karena tidak memiliki sistem tracking yang terstruktur dan tidak adanya support system. Data penggunaan aplikasi seperti Duolingo menunjukkan bahwa virtual companion mampu meningkatkan retensi harian pengguna. myKisah mengadopsi pendekatan serupa untuk mendukung pembentukan kebiasaan positif.

---

## Solusi yang Ditawarkan

Fitur utama dalam myKisah:

- **Journaling harian** sebagai media refleksi diri
- **Habit tracking** untuk memonitor progres kebiasaan
- **Character companion** untuk meningkatkan engagement

---

## Alur Aplikasi

1. **User Registration** — User membuat akun dengan username unik
2. **Pilih Character Companion** — User memilih karakter virtual favorit
3. **Buat Journal Harian** — User menulis jurnal dengan mood (Happy/Sad/Angry/Anxious/Calm)
4. **Submit Journal** — Journal berpindah dari Draft → Submitted melalui state machine
5. **Character Response** — Character companion memberikan respons emosional berdasarkan mood journal (table-driven lookup dari JSON)
6. **Riwayat Journal** — User melihat daftar journal yang pernah dibuat

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

## Functional Requirements

### FR-01 User Management

| ID      | Requirement                          | Endpoint                  |
| ------- | ------------------------------------ | ------------------------- |
| FR-01.1 | User dapat melakukan registrasi akun | `POST /api/user`        |
| FR-01.2 | User dapat melihat daftar user       | `GET /api/user`         |
| FR-01.3 | User dapat mengubah username         | `PUT /api/user/{id}`    |
| FR-01.4 | User dapat menghapus akun            | `DELETE /api/user/{id}` |

### FR-02 Journal Management

| ID      | Requirement                                | Endpoint                      |
| ------- | ------------------------------------------ | ----------------------------- |
| FR-02.1 | User dapat membuat journal (State: Draft)  | `POST /api/journal`         |
| FR-02.2 | User dapat melihat daftar journal miliknya | `GET /api/journal/{userId}` |
| FR-02.3 | User dapat menghapus journal               | `DELETE /api/journal/{id}`  |

### FR-03 Character Companion

| ID      | Requirement                                                     | Endpoint                                      |
| ------- | --------------------------------------------------------------- | --------------------------------------------- |
| FR-03.1 | User dapat melihat daftar karakter                              | `GET /api/character`                        |
| FR-03.2 | Sistem menampilkan response karakter berdasarkan mood journal   | `GET /api/character/{id}/response?mood=...` |
| FR-03.3 | Response diambil dari table-driven data (JSON, tanpa if/switch) | —                                            |

---

## Arsitektur

Aplikasi menggunakan **Layered Architecture** dengan persistensi data berbasis file JSON:

```
HTTP Request → Controller → Service → Repository → JSON File
                  │              │           │
              (validasi     (business     (data access
               input)        logic)        only)
```

### Struktur Proyek

```
myKisah/
├── Controllers/     # HTTP endpoint handlers (ASP.NET Web API)
├── Services/        # Business logic + validasi
├── Repositories/    # Akses dan persistensi data JSON
├── Models/          # Data model dan enum
├── Interfaces/      # Contract antar layer (dependency injection)
├── Automata/        # State machine untuk Journal
├── Utils/           # Helper: storage, validation, base class
├── Middleware/      # Global error handling
├── Data/            # File JSON persistensi
│   ├── users.json
│   ├── journals.json
│   ├── characters.json
│   └── characterResponses.json
├── Tests/           # Test plan (skeleton)
└── appsettings.json # Runtime configuration
```

### Namespace Mapping

| Namespace                | Isi                                                                                                             |
| ------------------------ | --------------------------------------------------------------------------------------------------------------- |
| `myKisah.Models`       | Data model & enum (`User`, `Journal`, `Character`, `CharacterResponse`, `MoodType`, `JournalState`) |
| `myKisah.Interfaces`   | Interface contract (`IRepository<T>`, `IUserRepository`, `IJournalRepository`, `IJournalService`, dll.) |
| `myKisah.Repositories` | Implementasi akses data JSON (`JsonUserRepository`, `JsonJournalRepository`, dll.)                          |
| `myKisah.Services`     | Business logic (`UserService`, `JournalService`, `CharacterService`)                                      |
| `myKisah.Controllers`  | HTTP endpoint handler (`UserController`, `JournalController`, `CharacterController`)                      |
| `myKisah.Utils`        | Helper (`JsonStorageHelper`, `FilePathConfig`, `ValidationHelper`, `ServiceBase`)                       |
| `myKisah.Automata`     | State machine journal (`JournalStateMachine`, `JournalTrigger`)                                             |
| `myKisah.Middleware`   | Global error handler (`ErrorHandlingMiddleware`)                                                              |

---

## Teknologi

| Layer           | Teknologi                                            |
| --------------- | ---------------------------------------------------- |
| Framework       | ASP.NET Web API (.NET 10)                            |
| Bahasa          | C#                                                   |
| Persistensi     | File JSON (System.Text.Json)                         |
| Testing         | xUnit + Moq + BenchmarkDotNet / Stopwatch            |
| Version Control | Git — branch pribadi per anggota, merge ke `main` |

---

## Teknik Konstruksi & Implementasi

### Ringkasan Teknik

| Teknik                          | Penjelasan                                                                                                                      | Diimplementasikan di                                                                                            |
| ------------------------------- | ------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------- |
| **Automata**              | Finite State Machine untuk lifecycle journal. Dictionary sebagai tabel transisi. Tanpa if-else.                                 | `Automata/JournalStateMachine.cs`                                                                             |
| **Table-Driven**          | Mapping mood ke response karakter via JSON file. Lookup pakai LINQ Where, tanpa if/switch.                                      | `Repositories/JsonCharacterResponseRepository.cs`, `Services/CharacterService.cs`                           |
| **Generics**              | `IRepository<T>`, `ValidationHelper.ValidateNotNull<T>`, `JsonStorageHelper.ReadJson<T>` — satu method untuk semua tipe. | `Interfaces/IRepository.cs`, `Utils/ValidationHelper.cs`, `Utils/JsonStorageHelper.cs`                    |
| **Runtime Configuration** | MaxContentLength, ValidMoods, path file dibaca dari `appsettings.json` — tidak di-hardcode.                                  | `Utils/FilePathConfig.cs`, `Services/JournalService.cs`, `appsettings.json`                               |
| **Code Reuse**            | `ServiceBase` (base class service), `JsonStorageHelper` (shared JSON I/O), `ValidationHelper` (shared validation).        | `Utils/ServiceBase.cs`, `Utils/JsonStorageHelper.cs`, `Utils/ValidationHelper.cs`                         |
| **API Development**       | RESTful endpoint dengan ASP.NET Web API controller.                                                                             | `Controllers/UserController.cs`, `Controllers/JournalController.cs`, `Controllers/CharacterController.cs` |

### Detail Teknik per Kelas

#### Automata — `JournalStateMachine`

```
Draft ──[Submit]──► Submitted ──[Save]──► Saved (terminal)
                         │
                      [Reject]
                         │
                         ▼
                     Rejected ──[Reset]──► Draft
```

**Kenapa Automata?** Journal punya aturan transisi ketat. Tanpa automata, perlu if-else di banyak tempat. Dengan Dictionary `Dictionary<(JournalState, JournalTrigger), JournalState>`, semua aturan di satu tabel. Invalid transition → `InvalidOperationException` → middleware → 422.

#### Table-Driven — `CharacterResponse` system

**Kenapa Table-Driven?** Response karakter tidak di-hardcode di kode C#. Semua data response ada di `characterResponses.json`. Method `GetByMood()` hanya melakukan LINQ Where — tidak ada if/switch. Tambah response baru = edit JSON file, tidak perlu ubah kode.

#### Generics — `IRepository<T>`, `JsonStorageHelper<T>`, `ValidationHelper<T>`

**Kenapa Generics?** Tanpa generics, perlu membuat overload untuk setiap tipe data. Dengan generics: satu method untuk semua tipe (`ReadJson<User>()`, `ReadJson<Journal>()`).

#### Runtime Configuration — `FilePathConfig`, `JournalService`

**Kenapa Runtime Config?** `MaxContentLength` dan `ValidMoods` tidak di-hardcode. Dibaca dari `appsettings.json`. Ubah batas karakter: cukup edit config, tidak perlu recompile.

#### Code Reuse — `ServiceBase`, `JsonStorageHelper`, `ValidationHelper`

**Kenapa Code Reuse?** Tanpa shared utility, setiap developer akan menulis ulang kode validasi, JSON I/O, dan logging. Ini = duplikasi + inkonsistensi.

---

## Tim & Pembagian Tugas

### Anggota

| Anggota                       | Modul                              | Teknik Konstruksi                      | File                                                                                   |
| ----------------------------- | ---------------------------------- | -------------------------------------- | -------------------------------------------------------------------------------------- |
| **Rayazka Aris (Azka)** | Journal System + Shared Foundation | Automata + Runtime Configuration + API | 18 file (Models, Interfaces, Automata, JournalRepo, JournalService, JournalController) |
| **Toni Kurniawan**      | Character Companion System         | Table-driven + API                     | 6 file (CharRepo, CharResponseRepo, CharService, CharController + test plan)           |
| **Farel Ilham**         | User Management                    | Generics + API                         | 5 file (UserRepo, UserService, UserController + test plan)                             |
| **Rafly Putra**         | JSON Storage Layer                 | Runtime Configuration + Code Reuse     | 4 file (FilePathConfig, JsonStorageHelper, appsettings.json + test plan)               |
| **Josefhint (Jojo)**    | Shared Utilities & Error Handling  | Code Reuse + Generics                  | 5 file (ValidationHelper, ServiceBase, Middleware, Program.cs + test plan)             |

### Distribusi Teknik Konstruksi

| Teknik                          | Anggota         | Slot        |
| ------------------------------- | --------------- | ----------- |
| **Automata**              | Rayazka         | 1/2         |
| **Table-driven**          | Toni            | 1/2         |
| **Generics**              | Farel + Jojo    | 2/2 (penuh) |
| **Runtime Configuration** | Rayazka + Rafly | 2/2 (penuh) |
| **Code Reuse**            | Rafly + Jojo    | 2/2 (penuh) |
| **API**                   | Toni + Farel    | 2/2 (penuh) |

---

## 📡 API Endpoints

| Method     | Endpoint                                  | Deskripsi                         | Controller              |
| ---------- | ----------------------------------------- | --------------------------------- | ----------------------- |
| `GET`    | `/api/user`                             | List semua user                   | `UserController`      |
| `POST`   | `/api/user`                             | Registrasi user baru              | `UserController`      |
| `PUT`    | `/api/user/{id}`                        | Update username                   | `UserController`      |
| `DELETE` | `/api/user/{id}`                        | Hapus user                        | `UserController`      |
| `GET`    | `/api/journal/{userId}`                 | Jurnal berdasarkan user           | `JournalController`   |
| `POST`   | `/api/journal`                          | Create jurnal baru (State: Draft) | `JournalController`   |
| `DELETE` | `/api/journal/{id}`                     | Hapus jurnal                      | `JournalController`   |
| `GET`    | `/api/character`                        | List karakter tersedia            | `CharacterController` |
| `GET`    | `/api/character/{id}/response?mood=...` | Ambil respons companion           | `CharacterController` |

---

## Data Models

### `User`

| Property      | Type         | Keterangan           |
| ------------- | ------------ | -------------------- |
| `Id`        | `string`   | GUID, primary key    |
| `Username`  | `string`   | Unique, not empty    |
| `CreatedAt` | `DateTime` | Timestamp registrasi |

### `Journal`

| Property      | Type             | Keterangan                      |
| ------------- | ---------------- | ------------------------------- |
| `Id`        | `string`       | GUID                            |
| `UserId`    | `string`       | FK ke User                      |
| `Title`     | `string`       | Tidak boleh kosong              |
| `Content`   | `string`       | Maks. MaxContentLength (config) |
| `Mood`      | `MoodType`     | Happy/Sad/Angry/Anxious/Calm    |
| `State`     | `JournalState` | Draft/Submitted/Saved/Rejected  |
| `CreatedAt` | `DateTime`     | Timestamp                       |

### `Character`

| Property        | Type       |
| --------------- | ---------- |
| `Id`          | `string` |
| `Name`        | `string` |
| `Description` | `string` |

### `CharacterResponse` (Tabel Table-Driven)

| Property        | Type         |
| --------------- | ------------ |
| `Id`          | `string`   |
| `CharacterId` | `string`   |
| `Mood`        | `MoodType` |
| `Response`    | `string`   |

### Enums

| Enum               | Value                                                |
| ------------------ | ---------------------------------------------------- |
| `MoodType`       | `Happy`, `Sad`, `Angry`, `Anxious`, `Calm` |
| `JournalState`   | `Draft`, `Submitted`, `Saved`, `Rejected`    |
| `JournalTrigger` | `Submit`, `Save`, `Reject`, `Reset`          |

---

## Design by Contract (DbC)

Setiap method di layer Service dan Repository menerapkan precondition/postcondition via `ValidationHelper`. Violation dilempar sebagai exception yang ditangkap global `ErrorHandlingMiddleware`:

| Exception                     | HTTP Response            |
| ----------------------------- | ------------------------ |
| `ArgumentNullException`     | 400 Bad Request          |
| `ArgumentException`         | 400 Bad Request          |
| `KeyNotFoundException`      | 404 Not Found            |
| `InvalidOperationException` | 422 Unprocessable Entity |

Contoh kontrak kritis:

| Method                 | Precondition                                                                             | Postcondition                      |
| ---------------------- | ---------------------------------------------------------------------------------------- | ---------------------------------- |
| `CreateJournal()`    | userId not null, title/content not empty, content.Length ≤ MaxContentLength, mood valid | Journal tersimpan, State = Draft   |
| `RegisterUser()`     | username not null/empty, belum terdaftar                                                 | User tersimpan, Id unik (GUID)     |
| `GenerateResponse()` | characterId not null, mood valid                                                         | Return string response, bukan null |
| `Transition()`       | Transisi valid di tabel                                                                  | State berubah sesuai transisi      |

---

## Performance Benchmarks (Target)

| Skenario                                         | Target      | PIC     |
| ------------------------------------------------ | ----------- | ------- |
| `GetJournalsByUser()` — 10 entri              | < 10ms      | Rayazka |
| `GetJournalsByUser()` — 100 entri             | < 50ms      | Rayazka |
| `CreateJournal()` + state machine              | < 20ms      | Rayazka |
| `GenerateResponse()` — 1000 iterasi           | < 1ms/call  | Toni    |
| `ReadJson` payload 100KB                       | < 100ms     | Rafly   |
| `WriteJson` + `ReadJson` round-trip 100 item | < 50ms      | Rafly   |
| `ValidationHelper` — 1000 panggilan           | < 5ms total | Jojo    |
| Pipeline end-to-end                              | < 50ms      | Semua   |

---

✨ *myKisah — Build habits, write your journey, grow together.* 🌱
