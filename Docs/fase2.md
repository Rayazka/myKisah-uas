# Fase 2 — Implementasi Domain Backend myKisah

> **Status:** Skeleton file sudah dibuat, TIM HANYA PERLU ISI TODO.
> **Prasyarat:** Semua file Fase 1 harus sudah di-merge ke `main`.
> **Pengerjaan:** Paralel — masing-masing kerja di branch pribadi, merge setelah selesai.
> **Namespace:** `myKisah.Xxx` (m kecil, K besar)

---

---

## FAREL ILHAM — User Management (3 file)

Teknik konstruksi: **Generics** + **API Development**

---

### A1. `Repositories/JsonUserRepository.cs` ⚠️ ADA TODO!
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Repositories/JsonUserRepository.cs` |
| **Namespace** | `myKisah.Repositories` |
| **Implements** | `IUserRepository` (extends `IRepository<User>`) |
| **Teknik** | **Generics** — mengimplementasikan interface generic `IRepository<User>` |
| **Dependency** | `JsonStorageHelper` + `FilePathConfig` (DI inject) |
| **Referensi** | `Task_myKisah.md` baris 188-197 |
| **Status file** | ⚠️ Skeleton ada, TODO harus diisi |

**📋 7 Method yang harus diimplementasikan:**

Semua method menggunakan `_storage.ReadJson<User>(_filePath.UsersFile)` untuk baca data, dan `_storage.WriteJson(...)` untuk tulis.

**1. `GetAll()`:**
```csharp
public IEnumerable<User> GetAll()
{
    return _storage.ReadJson<User>(_filePath.UsersFile);
}
```
Logic: Baca SEMUA user dari `users.json`. Return IEnumerable<User>. Paling sederhana — 1 baris.

**2. `GetById(string id)`:**
```csharp
public User? GetById(string id)
{
    return GetAll().FirstOrDefault(u => u.Id == id);
}
```
Logic: Ambil semua → filter pakai LINQ FirstOrDefault. Return null kalau tidak ketemu.

**3. `Add(User entity)`:**
```csharp
public void Add(User entity)
{
    var users = GetAll().ToList();
    entity.Id = Guid.NewGuid().ToString();
    entity.CreatedAt = DateTime.UtcNow;
    users.Add(entity);
    _storage.WriteJson(_filePath.UsersFile, users);
}
```
Logic: Read → generate GUID untuk Id → set CreatedAt → tambahkan ke list → Write kembali. **Id di-generate di repository**, bukan di service.

**4. `Update(User entity)`:**
```csharp
public void Update(User entity)
{
    var users = GetAll().ToList();
    var index = users.FindIndex(u => u.Id == entity.Id);
    if (index == -1)
        throw new KeyNotFoundException($"User dengan Id '{entity.Id}' tidak ditemukan.");
    users[index] = entity;
    _storage.WriteJson(_filePath.UsersFile, users);
}
```
Logic: Read → cari index by Id → jika tidak ketemu throw KeyNotFoundException → ganti data di index → Write. **Jangan generate Id baru** — Id lama dipertahankan.

**5. `Delete(string id)`:**
```csharp
public void Delete(string id)
{
    var users = GetAll().ToList();
    users.RemoveAll(u => u.Id == id);
    _storage.WriteJson(_filePath.UsersFile, users);
}
```
Logic: Read → RemoveAll yang Id-nya match → Write. Tidak throw exception kalau Id tidak ketemu (silent delete).

**6. `GetByUsername(string username)`:**
```csharp
public User? GetByUsername(string username)
{
    return GetAll().FirstOrDefault(u => 
        u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
}
```
Logic: Search case-insensitive. Return null jika tidak ketemu.

**7. `UsernameExists(string username)`:**
```csharp
public bool UsernameExists(string username)
{
    return GetAll().Any(u => 
        u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
}
```
Logic: Cek apakah username sudah dipakai. Case-insensitive. Dipanggil oleh UserService sebelum RegisterUser.

---

### A2. `Services/UserService.cs` ⚠️ ADA TODO!
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Services/UserService.cs` |
| **Namespace** | `myKisah.Services` |
| **Implements** | `IUserService` |
| **Extends** | `ServiceBase` (dapat `Validator` + `LogError` + `ServiceName`) |
| **Teknik** | **Generics** + **API** |
| **Dependency** | `IUserRepository` (DI inject) |
| **Referensi** | `Task_myKisah.md` baris 201-208, 221-240 |
| **Status file** | ⚠️ Skeleton ada, TODO harus diisi |

**📋 5 Method yang harus diimplementasikan:**

**0. ServiceName:**
```csharp
protected override string ServiceName => "UserService";
```
Override abstract property dari ServiceBase. Dipakai untuk logging.

**1. `RegisterUser(string username)`:**
```csharp
public User RegisterUser(string username)
{
    // Precondition: username tidak boleh kosong
    Validator.ValidateNotEmpty(username, "Username");
    
    // Precondition: username belum terdaftar
    if (_repository.UsernameExists(username))
        throw new ArgumentException($"Username '{username}' sudah terdaftar.");
    
    var user = new User { Username = username };
    _repository.Add(user);  // Id dan CreatedAt di-set di repository
    
    return user;
}
```
Logic: Cek kosong → cek duplikat → buat User → simpan → return. Id dan CreatedAt diisi otomatis oleh repository.

**2. `GetAllUsers()`:**
```csharp
public IEnumerable<User> GetAllUsers()
{
    return _repository.GetAll();
}
```
Logic: Tidak ada validasi. Hanya delegasi ke repository.

**3. `UpdateUser(string id, string username)`:**
```csharp
public User? UpdateUser(string id, string username)
{
    Validator.ValidateNotEmpty(username, "Username");
    
    var user = _repository.GetById(id);
    Validator.ValidateExists(user, $"User dengan Id '{id}'");
    
    // Cek duplikat — tapi jangan cek kalau username tidak berubah
    if (!user!.Username.Equals(username, StringComparison.OrdinalIgnoreCase) 
        && _repository.UsernameExists(username))
        throw new ArgumentException($"Username '{username}' sudah terdaftar.");
    
    user.Username = username;
    _repository.Update(user);
    
    return user;
}
```
Logic: Validasi empty → cek exists → cek duplikat (kecuali username sendiri) → update → return.

**4. `DeleteUser(string id)`:**
```csharp
public bool DeleteUser(string id)
{
    var user = _repository.GetById(id);
    Validator.ValidateExists(user, $"User dengan Id '{id}'");
    
    _repository.Delete(id);
    return true;
}
```
Logic: Cek exists → hapus → return true.

**Design by Contract:**
| Method | Precondition | Postcondition |
|---|---|---|
| `RegisterUser` | Username tidak kosong, belum terdaftar | User tersimpan dengan GUID unik |
| `UpdateUser` | Id exists, username tidak kosong, tidak duplikat (kecuali milik sendiri) | Username berubah |
| `DeleteUser` | Id exists | User terhapus |

---

### A3. `Controllers/UserController.cs` ⚠️ ADA TODO!
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Controllers/UserController.cs` |
| **Namespace** | `myKisah.Controllers` |
| **Teknik** | **API Development** |
| **Route** | `/api/user` |
| **Dependency** | `IUserService` (DI inject) |
| **Referensi** | `Task_myKisah.md` baris 211-217 |
| **Status file** | ⚠️ Skeleton ada, TODO harus diisi |

**📋 4 Endpoint yang harus diimplementasikan:**

**1. `GET /api/user` — GetAll:**
```csharp
[HttpGet]
public IActionResult GetAll()
{
    var users = _service.GetAllUsers();
    return Ok(users);
}
```
Logic: Panggil service → return 200 OK dengan list user.

**2. `POST /api/user` — Register:**
```csharp
[HttpPost]
public IActionResult Register([FromBody] RegisterRequest request)
{
    var user = _service.RegisterUser(request.Username);
    return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
}
```
Logic: Ambil username dari request body → panggil service → return 201 Created dengan user yang baru dibuat. `CreatedAtAction` butuh method `GetById` — kalau tidak ada, return `Ok(user)` saja.

**Request DTO** (sudah ada di file):
```csharp
public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
}
```

**3. `PUT /api/user/{id}` — Update:**
```csharp
[HttpPut("{id}")]
public IActionResult Update(string id, [FromBody] UpdateUserRequest request)
{
    var user = _service.UpdateUser(id, request.Username);
    return Ok(user);
}
```
Logic: Ambil id dari URL + username dari body → panggil service → return 200 OK.

**4. `DELETE /api/user/{id}` — Delete:**
```csharp
[HttpDelete("{id}")]
public IActionResult Delete(string id)
{
    _service.DeleteUser(id);
    return NoContent();
}
```
Logic: Panggil service → return 204 No Content.

**Catatan penting:** Controller TIDAK perlu try-catch. Semua exception dari service ditangkap oleh ErrorHandlingMiddleware (punya Jojo).

---

---

## RAYAZKA ARIS — Journal System (3 file)

Teknik konstruksi: **Automata** + **Runtime Configuration** + **API Development**

---

### B1. `Repositories/JsonJournalRepository.cs` ⚠️ ADA TODO!
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Repositories/JsonJournalRepository.cs` |
| **Namespace** | `myKisah.Repositories` |
| **Implements** | `IJournalRepository` (extends `IRepository<Journal>`) |
| **Teknik** | **Generics** |
| **Dependency** | `JsonStorageHelper` + `FilePathConfig` (DI inject) |
| **Referensi** | `Task_myKisah.md` baris 188-197 |
| **Status file** | ⚠️ Skeleton ada, TODO harus diisi |

**📋 6 Method yang harus diimplementasikan:**

**1. `GetAll()`:**
```csharp
public IEnumerable<Journal> GetAll()
{
    return _storage.ReadJson<Journal>(_filePath.JournalsFile);
}
```

**2. `GetById(string id)`:**
```csharp
public Journal? GetById(string id)
{
    return GetAll().FirstOrDefault(j => j.Id == id);
}
```

**3. `Add(Journal entity)`:**
```csharp
public void Add(Journal entity)
{
    var journals = GetAll().ToList();
    entity.Id = Guid.NewGuid().ToString();
    entity.CreatedAt = DateTime.UtcNow;
    entity.State = JournalState.Draft;  // SELALU Draft saat baru
    journals.Add(entity);
    _storage.WriteJson(_filePath.JournalsFile, journals);
}
```
PENTING: Set `State = JournalState.Draft`. State machine akan digunakan nanti untuk transisi.

**4. `Update(Journal entity)`:**
```csharp
public void Update(Journal entity)
{
    var journals = GetAll().ToList();
    var index = journals.FindIndex(j => j.Id == entity.Id);
    if (index == -1)
        throw new KeyNotFoundException($"Journal dengan Id '{entity.Id}' tidak ditemukan.");
    journals[index] = entity;
    _storage.WriteJson(_filePath.JournalsFile, journals);
}
```

**5. `Delete(string id)`:**
```csharp
public void Delete(string id)
{
    var journals = GetAll().ToList();
    journals.RemoveAll(j => j.Id == id);
    _storage.WriteJson(_filePath.JournalsFile, journals);
}
```

**6. `GetByUserId(string userId)`:**
```csharp
public IEnumerable<Journal> GetByUserId(string userId)
{
    return GetAll().Where(j => j.UserId == userId);
}
```
PENTING: Ini method kunci untuk business rule J-01: "User hanya dapat mengakses journal miliknya sendiri". Filter by UserId.

---

### B2. `Services/JournalService.cs` ⚠️ ADA TODO!
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Services/JournalService.cs` |
| **Namespace** | `myKisah.Services` |
| **Implements** | `IJournalService` |
| **Extends** | `ServiceBase` |
| **Teknik** | **Automata** + **Runtime Configuration** |
| **Dependency** | `IJournalRepository`, `JournalStateMachine`, `IConfiguration` (DI inject) |
| **Referensi** | `Task_myKisah.md` baris 61-65, 150-164, 201-208 |
| **Status file** | ⚠️ Skeleton ada, TODO harus diisi |

**📋 3 Method yang harus diimplementasikan:**

**0. ServiceName:**
```csharp
protected override string ServiceName => "JournalService";
```

**1. `CreateJournal(userId, title, content, mood)` — INI METHOD PALING KOMPLEKS:**
```csharp
public Journal CreateJournal(string userId, string title, string content, MoodType mood)
{
    // === PRECONDITION CHECKS (DbC) ===
    
    // 1. Input tidak boleh kosong
    Validator.ValidateNotEmpty(userId, "UserId");
    Validator.ValidateNotEmpty(title, "Title");
    Validator.ValidateNotEmpty(content, "Content");
    
    // 2. Mood harus valid (enum check)
    Validator.ValidateInEnum(mood, "Mood");
    
    // 3. Runtime Configuration — baca dari appsettings.json
    int maxContentLength = _configuration.GetValue<int>("JournalConfig:MaxContentLength");
    if (content.Length > maxContentLength)
        throw new ArgumentException(
            $"Panjang konten maksimal {maxContentLength} karakter. " +
            $"Konten Anda: {content.Length} karakter.");
    
    // 4. Mood harus ada di daftar ValidMoods (runtime config)
    var validMoods = _configuration.GetSection("JournalConfig:ValidMoods").Get<string[]>() 
                     ?? Array.Empty<string>();
    if (!validMoods.Contains(mood.ToString()))
        throw new ArgumentException(
            $"Mood '{mood}' tidak valid. Mood yang diterima: {string.Join(", ", validMoods)}");
    
    // === CREATE ===
    var journal = new Journal
    {
        UserId = userId,
        Title = title,
        Content = content,
        Mood = mood
    };
    
    _repository.Add(journal);  // Id, CreatedAt, State=Draft di-set di repository
    
    return journal;
}
```
Logic detail:
- **Precondition 1-2:** Validasi input dasar (pakai Validator dari ServiceBase)
- **Precondition 3:** `MaxContentLength` dibaca dari `appsettings.json` → `JournalConfig:MaxContentLength` (2000)
- **Precondition 4:** `ValidMoods` dibaca dari `appsettings.json` → `JournalConfig:ValidMoods` — array string, dicek apakah mood.ToString() ada di array
- **Create:** Buat Journal object → simpan via repository
- **Postcondition:** Journal tersimpan dengan State = Draft

**Kenapa Runtime Configuration?** `MaxContentLength` dan `ValidMoods` TIDAK di-hardcode. Dibaca dari `appsettings.json`. Kalau mau ubah batas karakter dari 2000 ke 5000, cukup edit JSON config, tidak perlu recompile.

**2. `GetJournalsByUser(string userId)`:**
```csharp
public IEnumerable<Journal> GetJournalsByUser(string userId)
{
    Validator.ValidateNotEmpty(userId, "UserId");
    return _repository.GetByUserId(userId);
}
```

**3. `DeleteJournal(string id)`:**
```csharp
public bool DeleteJournal(string id)
{
    var journal = _repository.GetById(id);
    Validator.ValidateExists(journal, $"Journal dengan Id '{id}'");
    
    _repository.Delete(id);
    return true;
}
```

**Design by Contract:**
| Method | Precondition | Postcondition |
|---|---|---|
| `CreateJournal` | userId not empty, title not empty, content not empty, mood valid enum, mood in ValidMoods, content.Length ≤ MaxContentLength | Journal tersimpan, State = Draft |
| `DeleteJournal` | Id exists | Journal terhapus |

---

### B3. `Controllers/JournalController.cs` ⚠️ ADA TODO!
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Controllers/JournalController.cs` |
| **Namespace** | `myKisah.Controllers` |
| **Route** | `/api/journal` |
| **Dependency** | `IJournalService` (DI inject) |
| **Referensi** | `Task_myKisah.md` baris 211-217 |
| **Status file** | ⚠️ Skeleton ada, TODO harus diisi |

**📋 3 Endpoint:**

**1. `GET /api/journal/{userId}` — GetByUser:**
```csharp
[HttpGet("{userId}")]
public IActionResult GetByUser(string userId)
{
    var journals = _service.GetJournalsByUser(userId);
    return Ok(journals);
}
```

**2. `POST /api/journal` — Create:**
```csharp
[HttpPost]
public IActionResult Create([FromBody] CreateJournalRequest request)
{
    var mood = Enum.Parse<MoodType>(request.Mood);
    var journal = _service.CreateJournal(request.UserId, request.Title, request.Content, mood);
    return CreatedAtAction(nameof(GetById), new { id = journal.Id }, journal);
}
```
PENTING: Mood dikirim client sebagai STRING (misal: `"Happy"`). Controller harus parse ke `MoodType enum`. `Enum.Parse<MoodType>()` akan throw `ArgumentException` jika string tidak valid → ditangkap middleware → 400.

**3. `DELETE /api/journal/{id}` — Delete:**
```csharp
[HttpDelete("{id}")]
public IActionResult Delete(string id)
{
    _service.DeleteJournal(id);
    return NoContent();
}
```

**Request DTO** (sudah ada di file):
```csharp
public class CreateJournalRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Mood { get; set; } = string.Empty;
}
```

---

---

## TONI KURNIAWAN — Character Companion System (4 file)

Teknik konstruksi: **Table-Driven** + **API Development**

> ⚠️ PERINGATAN: Teknik Table-Driven kamu dinilai dari apakah `GenerateResponse()` memakai if/switch atau tidak. **TIDAK BOLEH ADA if/switch/else berdasarkan mood di SELURUH flow-mu.** Semua response ada di `characterResponses.json`.

---

### C1. `Repositories/JsonCharacterRepository.cs` ⚠️ ADA TODO!
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Repositories/JsonCharacterRepository.cs` |
| **Namespace** | `myKisah.Repositories` |
| **Implements** | `ICharacterRepository` (extends `IRepository<Character>`) |
| **Dependency** | `JsonStorageHelper` + `FilePathConfig` |
| **Referensi** | `Task_myKisah.md` baris 188-197 |
| **Status file** | ⚠️ Skeleton ada, TODO harus diisi |

**📋 6 Method + 1 tambahan:**

Semua method sama seperti repository lain. Data dari `_filePath.CharactersFile`.

**Method kunci: `GetByName(string name)`:**
```csharp
public Character? GetByName(string name)
{
    return GetAll().FirstOrDefault(c => 
        c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}
```

---

### C2. `Repositories/JsonCharacterResponseRepository.cs` ⚠️ ADA TODO!
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Repositories/JsonCharacterResponseRepository.cs` |
| **Namespace** | `myKisah.Repositories` |
| **Implements** | `ICharacterResponseRepository` (extends `IRepository<CharacterResponse>`) |
| **Teknik** | **TABLE-DRIVEN** — INI ADALAH INTI TABLE-DRIVEN! |
| **Dependency** | `JsonStorageHelper` + `FilePathConfig` |
| **Referensi** | `Task_myKisah.md` baris 46-72, 355-370 |
| **Status file** | ⚠️ Skeleton ada, TODO harus diisi |

**📋 Method KRUSIAL — `GetByMood(string characterId, MoodType mood)`:**
```csharp
public IEnumerable<CharacterResponse> GetByMood(string characterId, MoodType mood)
{
    return GetAll().Where(r => 
        r.CharacterId == characterId && r.Mood == mood);
}
```
**INI ADALAH TABLE-DRIVEN.** Hanya 1 baris LINQ Where. Tidak ada if/switch. Data response ada di `characterResponses.json`. Method ini akan dipanggil oleh CharacterService.GenerateResponse().

**Bagaimana cara kerja Table-Driven:**
1. `characterResponses.json` berisi semua response — 3 karakter x 5 mood = 15 response
2. Repository membaca JSON → LINQ Where memfilter berdasarkan `characterId` dan `mood`
3. Return response yang cocok
4. Kalau mau tambah response baru: edit JSON, tidak perlu ubah kode C#

**Contoh data di `characterResponses.json`:**
```json
{ "characterId": "char1-kdrama", "mood": "Happy", "response": "Yeay! Senang!" }
{ "characterId": "char1-kdrama", "mood": "Sad",   "response": "Tidak apa-apa..." }
```

Method CRUD standar lain (GetAll, GetById, Add, Update, Delete) implementasinya sama seperti repository lain.

---

### C3. `Services/CharacterService.cs` ⚠️ ADA TODO!
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Services/CharacterService.cs` |
| **Namespace** | `myKisah.Services` |
| **Implements** | `ICharacterService` |
| **Extends** | `ServiceBase` |
| **Teknik** | **TABLE-DRIVEN** — GenerateResponse HARUS lookup via repository |
| **Dependency** | `ICharacterRepository`, `ICharacterResponseRepository` (DI inject) |
| **Referensi** | `Task_myKisah.md` baris 67-72, 201-208 |
| **Status file** | ⚠️ Skeleton ada, TODO harus diisi |

**📋 2 Method:**

**0. ServiceName:**
```csharp
protected override string ServiceName => "CharacterService";
```

**1. `GetAllCharacters()`:**
```csharp
public IEnumerable<Character> GetAllCharacters()
{
    return _characterRepo.GetAll();
}
```

**2. `GenerateResponse(string characterId, MoodType mood)` — METHOD KRUSIAL:**
```csharp
public string GenerateResponse(string characterId, MoodType mood)
{
    // Precondition
    Validator.ValidateNotEmpty(characterId, "CharacterId");
    Validator.ValidateInEnum(mood, "Mood");
    
    // Table lookup — TIDAK BOLEH if/switch!
    var responses = _responseRepo.GetByMood(characterId, mood);
    var response = responses.FirstOrDefault();
    
    Validator.ValidateExists(response, 
        $"Response untuk character '{characterId}' dengan mood '{mood}'");
    
    return response!.Response;
}
```

**INI ADALAH TABLE-DRIVEN.** Flow:
1. Validasi input
2. `_responseRepo.GetByMood(characterId, mood)` → LINQ Where di repository → data dari JSON
3. Ambil `.FirstOrDefault()`
4. Return `response.Response` (string text)
5. **TIDAK ADA if (mood == Happy) return "..."** — itu MELANGGAR table-driven!

**Design by Contract:**
| Method | Precondition | Postcondition |
|---|---|---|
| `GenerateResponse` | characterId not empty, mood valid enum, response tersedia di tabel | Return string response, bukan null |

---

### C4. `Controllers/CharacterController.cs` ⚠️ ADA TODO!
| Atribut | Nilai |
|---|---|
| **Lokasi** | `myKisah/Controllers/CharacterController.cs` |
| **Namespace** | `myKisah.Controllers` |
| **Route** | `/api/character` |
| **Dependency** | `ICharacterService` (DI inject) |
| **Referensi** | `Task_myKisah.md` baris 211-217 |
| **Status file** | ⚠️ Skeleton ada, TODO harus diisi |

**📋 2 Endpoint:**

**1. `GET /api/character` — GetAll:**
```csharp
[HttpGet]
public IActionResult GetAll()
{
    var characters = _service.GetAllCharacters();
    return Ok(characters);
}
```

**2. `GET /api/character/{id}/response?mood=Happy` — GetResponse:**
```csharp
[HttpGet("{id}/response")]
public IActionResult GetResponse(string id, [FromQuery] string mood)
{
    var parsedMood = Enum.Parse<MoodType>(mood);
    var response = _service.GenerateResponse(id, parsedMood);
    return Ok(new { response });
}
```
Logic: Ambil character ID dari URL + mood dari query string `?mood=Happy` → parse mood string ke enum → panggil service → return JSON object `{ "response": "..." }`.

Contoh request: `GET /api/character/char1-kdrama/response?mood=Happy`
Response: `{ "response": "Yeay! Senang banget denger kamu happy!..." }`

---

---

## RINGKASAN FASE 2 — YANG HARUS DIKERJAKAN

| Anggota | File | Method Count |
|---|---|---|
| **Farel** | `JsonUserRepository.cs`, `UserService.cs`, `UserController.cs` | 7 + 4 + 4 = 15 method |
| **Rayazka** | `JsonJournalRepository.cs`, `JournalService.cs`, `JournalController.cs` | 6 + 3 + 3 = 12 method |
| **Toni** | `JsonCharacterRepository.cs`, `JsonCharacterResponseRepository.cs`, `CharacterService.cs`, `CharacterController.cs` | 6 + 6 + 2 + 2 = 16 method |

**PENTING: Setelah selesai, commit ke branch pribadi, lalu merge ke `main`. Branch pribadi:**
- Farel: `feature/user-management`
- Rayazka: `feature/journal-system`
- Toni: `feature/character-companion`
