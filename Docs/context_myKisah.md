
# Project Context: myKisah - Backend Construction (CLO2)

## 1. Project Overview

**Nama Project:** myKisah
**Jenis:** Aplikasi Journaling Self-Improvement dengan Virtual Companion.
**Fase:** UTS - Backend Construction.
**Tujuan Utama:** Membangun fondasi backend yang scalable, maintainable, dan menerapkan teknik pemrograman defensif serta clean architecture.

### Core Concept

Aplikasi memungkinkan pengguna menulis jurnal harian dengan tagging mood. Karakter companion virtual akan memberikan respons emosional berdasarkan mood tersebut menggunakan pendekatan *table-driven*.

---

## 2. System Architecture

Aplikasi menggunakan **Layered Architecture** dengan pemisahan tanggung jawab sebagai berikut:

* **Presentation Layer:** Blazor UI (Minimalis untuk fase UTS).
* **API Layer (Controllers):** Menangani HTTP Request, validasi input awal, dan routing.
* **Business Logic Layer (Services):** Logika bisnis utama, validasi aturan bisnis (DbC), dan orkestrasi data.
* **Data Access Layer (Repositories):** Abstraksi akses data menggunakan Repository Pattern.
* **Storage Layer:** Persistensi data berbasis file JSON.

---

## 3. Data Models & Enums

### Enums

* **MoodType:** `Happy`, `Sad`, `Angry`, `Anxious`, `Calm`.
* **JournalState:** `Draft`, `Submitted`, `Saved`, `Rejected`.
* **JournalTrigger:** `Submit`, `Save`, `Reject`, `Reset`.

### Models

1. **User:** `Guid Id`, `string Username`, `DateTime CreatedAt`.
2. **Journal:** `Guid Id`, `Guid UserId`, `string Title`, `string Content`, `MoodType Mood`, `JournalState State`, `DateTime CreatedAt`.
3. **Character:** `Guid Id`, `string Name`, `string Description`.
4. **CharacterResponse:** `Guid Id`, `Guid CharacterId`, `MoodType Mood`, `string Response`.

---

## 4. Teknik Konstruksi (CLO2 Requirements)

| Teknik                    | Implementasi                                                                    | Penanggung Jawab        |
| :------------------------ | :------------------------------------------------------------------------------ | :---------------------- |
| **Automata**        | State Machine untuk siklus hidup Jurnal (Draft -> Submitted -> Saved/Rejected). | Rayazka Aris            |
| **Table-Driven**    | Mapping mood ke respons karakter melalui file JSON tanpa logic if-else.         | Toni Kurniawan          |
| **Generics**        | `IRepository<T>`, `ValidationHelper.ValidateNotNull<T>`, `ReadJson<T>`.   | Farel, Josefhint, Rafly |
| **Runtime Config**  | Konfigurasi path file dan aturan jurnal (max length) di `appsettings.json`.   | Rafly, Rayazka          |
| **Code Reuse**      | `ServiceBase`, `JsonStorageHelper`, `ValidationHelper`.                   | Josefhint, Rafly        |
| **API development** | RESTful endpoints untuk User, Journal, dan Character.                           | Farel, Toni, Rayazka    |

---

## 5. Module & Class Details

### 5.1 Journal System (Automata + Runtime Config)

* **Logic:** Menggunakan `JournalStateMachine` dengan `Dictionary<(JournalState, JournalTrigger), JournalState>` sebagai tabel transisi.
* **Validasi:** `MaxContentLength` dan `ValidMoods` dibaca dari `appsettings.json`.
* **State Flow:** `Draft` --(Submit)--> `Submitted` --(Save)--> `Saved` ATAU `Submitted` --(Reject)--> `Rejected`.

### 5.2 Character Companion (Table-Driven)

* **Logic:** `CharacterService.GenerateResponse()` melakukan filter pada `characterResponses.json` berdasarkan `CharacterId` dan `Mood`.
* **Data:** Penambahan respons baru dilakukan cukup dengan mengedit file JSON.

### 5.3 User Management (Generics)

* **Logic:** `JsonUserRepository` mengimplementasikan `IRepository<User>`.
* **Constraint:** Validasi username unik dan tidak boleh kosong.

### 5.4 Storage Layer (Code Reuse)

* **JsonStorageHelper:** Menyediakan `ReadJson<T>()` dan `WriteJson<T>()` dengan penanganan *auto-create* file jika tidak ditemukan.
* **FilePathConfig:** Memetakan path file data (`users.json`, `journals.json`, dll) dari konfigurasi.

### 5.5 Shared Utilities (Defensive Programming)

* **ValidationHelper:** Method generik untuk `ValidateNotNull`, `ValidateNotEmpty`, `ValidateInEnum`.
* **Global Error Handling:** Middleware untuk menangkap exception (ArgumentException -> 400, KeyNotFound -> 404) dan memberikan respons JSON yang konsisten.

---

## 6. API Endpoints

| Domain              | Method | Endpoint                                  | Deskripsi                         |
| :------------------ | :----- | :---------------------------------------- | :-------------------------------- |
| **User**      | GET    | `/api/user`                             | List semua user                   |
| **User**      | POST   | `/api/user`                             | Registrasi user baru              |
| **Journal**   | GET    | `/api/journal/{userId}`                 | Jurnal berdasarkan user           |
| **Journal**   | POST   | `/api/journal`                          | Create jurnal baru (State: Draft) |
| **Character** | GET    | `/api/character`                        | List karakter tersedia            |
| **Character** | GET    | `/api/character/{id}/response?mood=...` | Ambil respons companion           |

---

## 7. Design by Contract (DbC) & Testing

### Preconditions Utama:

* `CreateJournal`: `UserId` != null, `Title` not empty, `Content.Length` < Max, `Mood` valid.
* `RegisterUser`: `Username` unik dan tidak null.
* `Transition`: Trigger harus valid sesuai state saat ini.

### Performance Benchmarks:

* **JSON Read (100 items):** < 20ms.
* **Filter Data:** < 50ms untuk 100 entri.
* **End-to-End Pipeline:** < 50ms per request.

---

## 8. Project Structure

```text
myKisah/
├── Controllers/        # API Endpoints
├── Services/           # Business Logic & Automata
│   ├── Implementations/
│   └── Interfaces/
├── Repositories/       # Data Access (JSON Implementation)
├── Models/             # Data Models & Enums
├── Utils/              # Helpers (Storage, Validation, Base)
├── Data/               # JSON Storage Files
├── Tests/              # Unit & Performance Tests
└── appsettings.json    # Runtime Configuration
```
