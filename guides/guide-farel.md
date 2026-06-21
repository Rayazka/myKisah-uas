# Guide: Farel — User & Journal Entry

**Baca panduan ini dari atas ke bawah. Kerjakan task sesuai urutan.**

Jumlah task: **2** | Domain: **User + Journal Form**

---

## Domain

Kamu mengerjakan **halaman Login** (gerbang masuk aplikasi) dan **halaman Journal Create** (form untuk menulis journal baru). Kedua halaman ini bergantung pada service yang kamu buat sebelumnya (`UserService`, `JournalService`) dan komponen shared dari Rafly (`MoodPicker`) dan Jojo (`AlertMessage`).

---

## Prasyarat Sebelum Memulai

- [ ] Azka sudah menyelesaikan FASE 1 (task 1–6) — tanya Azka jika belum
- [ ] Pull branch `main` terbaru
- [ ] `dotnet build` — 0 error
- [ ] File referensi terbuka: `Home.razor` (line 24–80, 189–229)
- [ ] Baca section **9. Referensi Cepat: Shared Components** di `fr-plan.md`
- [ ] Komponen shared yang harus sudah ada sebelum kamu mulai: `AlertMessage`, `MoodPicker`

---

## Daftar Tugas

| # | Task | File |
|---|---|---|
| 10 | `Login` | `Components/Pages/Login.razor` + `.css` |
| 11 | `JournalCreate` | `Components/Pages/JournalCreate.razor` + `.css` |

---

### Task 10: Login.razor + CSS

**Apa yang dibuat:** Halaman utama (`/`) — user memilih akun existing dari grid card, atau registrasi user baru via input + tombol.

**Kenapa dibuat:** Aplikasi tidak punya sistem autentikasi. Halaman ini menjadi gerbang masuk — user memilih identitas, `UserSession` menyimpannya, lalu redirect ke `/journals`. Tanpa halaman ini, aplikasi tidak tahu user mana yang sedang aktif.

**File:**
- `Components/Pages/Login.razor`
- `Components/Pages/Login.razor.css`

**Inject:** `IUserService`, `UserSession`, `NavigationManager`
**Shared Components:** `AlertMessage`

**Skeleton kode:**

```razor
@page "/"
@rendermode InteractiveServer

@inject IUserService UserService
@inject UserSession Session
@inject NavigationManager Navigation

<PageTitle>myKisah — Login</PageTitle>

<div class="login-container">
    <div class="login-card">
        <h1 class="brand">🌱 myKisah</h1>
        <p class="tagline">Build habits, write your journey, grow together.</p>

        <AlertMessage Message="_error" IsError="true" Show="_hasError" />

        @* Section A: Pilih user existing *@
        @if (_users.Any())
        {
            <div class="mb-4">
                <h3>Pilih Akun</h3>
                <div class="user-grid">
                    @foreach (var user in _users)
                    {
                        <div class="user-card" @onclick="() => SelectUser(user)">
                            <span class="user-avatar">
                                @user.Username[..1].ToUpper()
                            </span>
                            <span class="user-name">@user.Username</span>
                            <span class="user-date">
                                Joined @user.CreatedAt.ToString("dd MMM yyyy")
                            </span>
                        </div>
                    }
                </div>
            </div>
        }

        @* Section B: Register user baru *@
        <div>
            <h3>@(_users.Any() ? "Atau Buat Akun Baru" : "Buat Akun Baru")</h3>
            <div class="register-row">
                <input @bind="_newUsername"
                       type="text"
                       class="form-control"
                       placeholder="Masukkan username..."
                       disabled="@_loading"
                       @onkeydown="HandleKeyDown" />
                <button class="btn btn-primary"
                        @onclick="RegisterUser"
                        disabled="@_loading || string.IsNullOrWhiteSpace(_newUsername)">
                    @(_loading ? "Mohon tunggu..." : "Mulai Petualangan")
                </button>
            </div>
            @if (!_users.Any())
            {
                <p class="text-muted mt-2 small">
                    Belum ada user terdaftar. Masukkan username untuk memulai.
                </p>
            }
        </div>
    </div>
</div>

@code {
    private List<User> _users = new();
    private string _newUsername = "";
    private string _error = "";
    private bool _hasError;
    private bool _loading;

    protected override void OnInitialized()
    {
        // TODO: Jika Session.IsLoggedIn → langsung redirect ke /journals
        // TODO: Load daftar user: _users = UserService.GetAllUsers().ToList()
        // TODO: Catch exception → _hasError = true, _error = ex.Message
    }

    private async Task SelectUser(User user)
    {
        // TODO: Session.Login(user)
        // TODO: Navigation.NavigateTo("/journals")
    }

    private async Task RegisterUser()
    {
        // TODO: Validasi — _newUsername tidak boleh kosong (trim dulu)
        //   Jika kosong: _hasError = true, _error = "Username tidak boleh kosong."
        // TODO: Set loading = true, reset error
        // TODO: Try:
        //   var user = UserService.RegisterUser(_newUsername.Trim())
        //   Session.Login(user)
        //   Navigation.NavigateTo("/journals")
        // TODO: Catch: _hasError = true, _error = ex.Message
        // TODO: Finally: _loading = false
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        // TODO: Jika e.Key == "Enter" → panggil RegisterUser()
    }
}
```

**Skeleton CSS — Login.razor.css:**

```css
.login-container {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 70vh;
    padding: 2rem;
}

.login-card {
    width: 100%;
    max-width: 560px;
    text-align: center;
}

.brand {
    font-size: 2.5rem;
    font-weight: 800;
    margin-bottom: 0.3rem;
    color: #1e293b;
}

.tagline {
    color: #64748b;
    font-size: 1.05rem;
    margin-bottom: 2rem;
}

.user-grid {
    display: flex;
    flex-wrap: wrap;
    gap: 0.6rem;
    justify-content: center;
    margin-bottom: 1.5rem;
}

.user-card {
    border: 2px solid #dee2e6;
    border-radius: 8px;
    padding: 0.8rem 1.2rem;
    cursor: pointer;
    transition: 0.15s;
    text-align: left;
    min-width: 150px;
}

.user-card:hover {
    border-color: #0d6efd;
    background: #f0f7ff;
}

.user-avatar {
    display: inline-block;
    width: 32px;
    height: 32px;
    line-height: 32px;
    border-radius: 50%;
    background: #0d6efd;
    color: white;
    text-align: center;
    font-weight: 700;
    margin-right: 0.4rem;
    font-size: 0.85rem;
}

.user-name {
    font-weight: 600;
}

.user-date {
    display: block;
    font-size: 0.75rem;
    color: #64748b;
    margin-top: 0.1rem;
}

.register-row {
    display: flex;
    gap: 0.5rem;
    justify-content: center;
}

.register-row input {
    max-width: 260px;
}
```

**Testing:**
1. Buka `/` — tampil branding + daftar user (jika ada)
2. Belum ada user → hanya form register + input + tombol
3. Isi username → Enter / klik "Mulai Petualangan" → redirect ke `/journals`
4. Username kosong → error: "Username tidak boleh kosong."
5. Username duplikat → error dari service tampil di AlertMessage
6. Klik user card → langsung login → redirect ke `/journals`

---

### Task 11: JournalCreate.razor + CSS

**Apa yang dibuat:** Form untuk menulis journal baru. User mengisi title, content, memilih mood via MoodPicker, lalu menyimpan.

**Kenapa dibuat:** User perlu cara untuk menulis journal baru. Halaman ini menyediakan form dengan validasi client-side (tidak boleh kosong, maksimal karakter) dan MoodPicker visual. Setelah simpan sukses, redirect ke JournalList.

**File:**
- `Components/Pages/JournalCreate.razor`
- `Components/Pages/JournalCreate.razor.css`

**Inject:** `IJournalService`, `UserSession`, `NavigationManager`, `IConfiguration`
**Shared Components:** `MoodPicker`, `AlertMessage`

**Skeleton kode:**

```razor
@page "/journals/new"
@rendermode InteractiveServer

@inject IJournalService JournalService
@inject UserSession Session
@inject NavigationManager Navigation
@inject IConfiguration Configuration

<PageTitle>Tulis Journal — myKisah</PageTitle>

<div class="container-fluid">
    <div class="create-container">
        <h1 class="mb-3">Tulis Journal Baru</h1>

        <AlertMessage Message="_error" IsError="true" Show="_hasError" />

        <div class="card">
            <div class="card-body">
                @* Title *@
                <div class="mb-3">
                    <label class="form-label fw-bold">Judul</label>
                    <input @bind="_title"
                           type="text"
                           class="form-control"
                           placeholder="Apa yang ingin kamu tulis hari ini?"
                           disabled="@_saving"
                           maxlength="100" />
                    <small class="text-muted">@_title.Length/100 karakter</small>
                </div>

                @* Content *@
                <div class="mb-3">
                    <label class="form-label fw-bold">Isi Journal</label>
                    <textarea @bind="_content"
                              class="form-control"
                              rows="6"
                              placeholder="Tulis isi journalmu di sini..."
                              disabled="@_saving"></textarea>
                    <small class="text-muted @(_content.Length > _maxLength ? "text-danger" : "")">
                        @_content.Length/@_maxLength karakter
                    </small>
                </div>

                @* Mood *@
                <div class="mb-4">
                    <label class="form-label fw-bold">Mood</label>
                    <MoodPicker @bind-SelectedMood="_mood" Disabled="_saving" />
                </div>

                @* Buttons *@
                <div class="d-flex gap-2">
                    <button class="btn btn-primary"
                            @onclick="CreateJournal"
                            disabled="@_saving">
                        @(_saving ? "Menyimpan..." : "Simpan Journal")
                    </button>
                    <button class="btn btn-outline-secondary"
                            @onclick="Cancel"
                            disabled="@_saving">
                        Batal
                    </button>
                </div>
            </div>
        </div>
    </div>
</div>

@code {
    private string _title = "";
    private string _content = "";
    private MoodType _mood = MoodType.Happy;
    private bool _saving;
    private string _error = "";
    private bool _hasError;
    private int _maxLength = 5000;

    protected override void OnInitialized()
    {
        // TODO: Cek login — redirect ke / jika Session.IsLoggedIn == false
        // TODO: Baca max length dari config:
        //   _maxLength = Configuration.GetValue<int>("JournalConfig:MaxContentLength")
    }

    private async Task CreateJournal()
    {
        // TODO: Reset error
        // TODO: Validasi client-side:
        //   - _title.Trim() kosong? → error "Judul tidak boleh kosong."
        //   - _content.Trim() kosong? → error "Isi journal tidak boleh kosong."
        //   - _content.Length > _maxLength? → error "Isi journal terlalu panjang (max {_maxLength} karakter)."
        // TODO: Set _saving = true
        // TODO: Try:
        //   JournalService.CreateJournal(
        //       Session.CurrentUser.Id,
        //       _title.Trim(),
        //       _content.Trim(),
        //       _mood)
        //   Navigation.NavigateTo("/journals")
        // TODO: Catch: _hasError = true, _error = ex.Message
        // TODO: Finally: _saving = false
    }

    private void Cancel()
    {
        // TODO: Navigation.NavigateTo("/journals")
    }
}
```

**Skeleton CSS — JournalCreate.razor.css:**

```css
.create-container {
    max-width: 700px;
    margin: 0 auto;
}

.create-container h1 {
    font-size: 1.5rem;
}
```

**Testing:**
1. Buka `/journals/new` → form tampil: title, content, MoodPicker, counter karakter
2. MoodPicker — klik chip mood, chip aktif berubah warna
3. Content kosong → klik Simpan → error "Isi journal tidak boleh kosong."
4. Content > max → counter merah, klik Simpan → error
5. Isi semua field valid → klik Simpan → redirect `/journals`
6. Di `/journals` → journal baru muncul di list
7. Klik Batal → kembali ke `/journals` tanpa menyimpan

---

## Konvensi Kode

### Error Handling Pattern

```csharp
private async Task DoSomething()
{
    try
    {
        _hasError = false;
        _error = "";
        _loading = true;
        var result = SomeService.SomeMethod(...);
        // TODO: Update state atau navigasi
    }
    catch (Exception ex)
    {
        _hasError = true;
        _error = ex.Message;
    }
    finally
    {
        _loading = false;
    }
}
```

### Navigasi

```csharp
Navigation.NavigateTo("/journals");
Navigation.NavigateTo($"/journals/{id}");
```

### Form Input + Bind

```razor
<input @bind="_field" class="form-control" placeholder="..." disabled="@_loading" />
<MoodPicker @bind-SelectedMood="_mood" Disabled="_saving" />
<button class="btn btn-primary" @onclick="OnSave" disabled="@_saving">
    @(_saving ? "Menyimpan..." : "Simpan")
</button>
```

### Cek Login

```csharp
protected override void OnInitialized()
{
    if (!Session.IsLoggedIn) { Navigation.NavigateTo("/"); return; }
}
```

### Styling

| Aturan | Detail |
|---|---|
| Container | `<div class="container-fluid">` |
| Card center | `max-width: 700px; margin: 0 auto;` |
| Card | `<div class="card"><div class="card-body">` |
| Tombol primary | `btn btn-primary` |
| Tombol outline | `btn btn-outline-secondary` |
| Form spacing | `<div class="mb-3">` antar field |
| CSS | Scoped `.razor.css` per komponen |

---

## Komponen Shared yang Dipakai

| Komponen | Cara Pakai |
|---|---|
| `UserSession` | `@inject UserSession Session` → `Session.CurrentUser`, `Session.Login(user)` |
| `MoodPicker` | `<MoodPicker @bind-SelectedMood="_mood" Disabled="_saving" />` |
| `AlertMessage` | `<AlertMessage Message="_error" IsError="true" Show="_hasError" />` |

---

## Checklist Selesai

- [ ] `/` — branding + daftar user tampil
- [ ] Register user baru — input tidak kosong, sukses → redirect `/journals`
- [ ] Register user duplikat — error tampil
- [ ] Pilih user existing — klik card → login → redirect `/journals`
- [ ] Enter di input register — trigger RegisterUser
- [ ] `/journals/new` — form Create tampil
- [ ] MoodPicker — pilih mood, chip berubah
- [ ] Validasi title/content kosong — error tampil
- [ ] Content > max — counter merah + error
- [ ] Simpan sukses — redirect `/journals`, journal muncul di list
- [ ] Batal — kembali ke `/journals` tanpa perubahan
