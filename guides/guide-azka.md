# Guide: Rayazka (Azka) — Foundation, Layout, Journal, Verifikasi

**Baca panduan ini dari atas ke bawah. Kerjakan task sesuai urutan FASE.**

Jumlah task: **9** | Fase: **3**

---

## Domain

Kamu mengerjakan **pondasi aplikasi** (UserSession, DI, Layout, Navigasi), **halaman Journal** (List + Detail dengan state machine), dan **verifikasi akhir**. Semua anggota lain bergantung pada output FASE 1-mu — kerjakan FASE 1 sebelum mereka mulai.

---

## Prasyarat Sebelum Memulai

- [ ] Pull branch `main` terbaru
- [ ] `dotnet build` — 0 error
- [ ] `dotnet test` — 65 test pass
- [ ] Buka file referensi: `Home.razor`, `MainLayout.razor` existing, `NavMenu.razor` existing, `Program.cs`
- [ ] Baca section **9. Referensi Cepat: Shared Components** di `fr-plan.md`

---

## Daftar Tugas

| FASE | # | Task               | File                                                |
| ---- | - | ------------------ | --------------------------------------------------- |
| 1    | 1 | `UserSession.cs` | `myKisah/Services/UserSession.cs`                 |
| 1    | 2 | Registrasi DI      | `myKisah/Program.cs`                              |
| 1    | 3 | `_Imports.razor` | `Components/_Imports.razor`                       |
| 1    | 4 | `MainLayout`     | `Components/Layout/MainLayout.razor` + `.css`   |
| 1    | 5 | `NavMenu`        | `Components/Layout/NavMenu.razor` + `.css`      |
| 1    | 6 | `Debug.razor`    | `Components/Pages/Debug.razor`                    |
| 2    | 7 | `JournalList`    | `Components/Pages/JournalList.razor` + `.css`   |
| 2    | 8 | `JournalDetail`  | `Components/Pages/JournalDetail.razor` + `.css` |
| 3    | 9 | Verifikasi         | Build + test + smoke test                           |

---

## FASE 1: Pondasi

### Task 1: UserSession.cs

**Apa yang dibuat:** Scoped service untuk menyimpan identitas user yang login per sesi browser.

**Kenapa dibuat:** Blazor Server menggunakan circuit per koneksi browser. `UserSession` memanfaatkan ini — satu instance hidup selama user membuka tab. Semua halaman mengakses service ini untuk tahu siapa yang login dan untuk redirect ke `/` jika user belum login.

**File:** `myKisah/Services/UserSession.cs`

```csharp
using myKisah.Models;

namespace myKisah.Services;

public class UserSession
{
    public User? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;

    public void Login(User user) => CurrentUser = user;
    public void Logout() => CurrentUser = null;
}
```

---

### Task 2: Registrasi DI UserSession

**Apa yang dibuat:** Daftarkan `UserSession` di DI container.

**Kenapa dibuat:** Tanpa registrasi, Blazor tidak bisa meng-inject `UserSession` ke komponen. Harus scoped karena satu sesi browser = satu instance.

**File:** `myKisah/Program.cs`

**tambahkan baris ini setelah `builder.Services.AddScoped<ICharacterService, CharacterService>();`**

```csharp
// Session
builder.Services.AddScoped<UserSession>();
```

> Tambahkan juga `using myKisah.Services;` di atas file jika belum ada.

---

### Task 3: _Imports.razor

**Apa yang dibuat:** Tambahkan global using untuk semua namespace yang dipakai di komponen Razor.

**Kenapa dibuat:** Tanpa ini, setiap halaman harus menulis `@using myKisah.Models`, `@using myKisah.Interfaces`, dll. secara manual. Dengan global using, semua halaman otomatis mengenali model dan interface.

**File:** `Components/_Imports.razor`

**Skeleton kode — tambahkan 3 baris ini di akhir file (setelah `@using myKisah.Components.Layout`):**

```razor
@using myKisah.Models
@using myKisah.Interfaces
@using myKisah.Services
```

---

### Task 4: MainLayout.razor + CSS

**Apa yang dibuat:** Layout halaman dengan top navbar (bukan sidebar). Struktur: header sticky di atas + main area full-width di bawah.

**Kenapa dibuat:** Layout existing menggunakan template Blazor sidebar — tidak cocok untuk aplikasi journaling. Layout baru: top navbar horizontal + konten full-width tanpa sidebar. Lebih bersih dan modern.

**File:**

- `Components/Layout/MainLayout.razor`
- `Components/Layout/MainLayout.razor.css`

**Skeleton kode — MainLayout.razor:**

```razor
@inherits LayoutComponentBase

<div class="app">
    <header>
        <NavMenu />
    </header>
    <main class="container-fluid main-content">
        @Body
    </main>
    <ReconnectModal />
    <div id="blazor-error-ui" data-nosnippet>
        An unhandled error has occurred.
        <a href="." class="reload">Reload</a>
        <span class="dismiss">🗙</span>
    </div>
</div>
```

**Skeleton CSS — MainLayout.razor.css:**

```css
.app {
    display: flex;
    flex-direction: column;
    min-height: 100vh;
}

header {
    position: sticky;
    top: 0;
    z-index: 1020;
}

.main-content {
    flex: 1;
    padding: 2rem 1.5rem;
}

#blazor-error-ui {
    background: lightyellow;
    bottom: 0;
    box-shadow: 0 -1px 2px rgba(0, 0, 0, 0.2);
    display: none;
    left: 0;
    padding: 0.6rem 1.25rem 0.7rem 1.25rem;
    position: fixed;
    width: 100%;
    z-index: 1000;
}

#blazor-error-ui .dismiss {
    cursor: pointer;
    position: absolute;
    right: 0.75rem;
    top: 0.5rem;
}
```

**Testing:** Jalankan `dotnet run` → halaman `/` tidak lagi menampilkan sidebar. Harus muncul header di atas + konten full-width di bawah.

---

### Task 5: NavMenu.razor + CSS

**Apa yang dibuat:** Navigasi horizontal di header. Brand "myKisah 🌱" di kiri, 3 link utama di tengah, user dropdown di kanan.

**Kenapa dibuat:** NavMenu existing adalah sidebar vertikal dengan 1 link "Dashboard". Diganti jadi horizontal navbar dengan link: Home, My Journals, Characters, dan dropdown user di kanan. Responsive — collapse ke hamburger di mobile.

**File:**

- `Components/Layout/NavMenu.razor`
- `Components/Layout/NavMenu.razor.css`

**Skeleton kode — NavMenu.razor:**

```razor
@inject UserSession Session
@inject NavigationManager Navigation

<nav class="navbar navbar-expand-md navbar-dark bg-dark">
    <div class="container-fluid">
        <a class="navbar-brand" href="">
            <span class="brand-icon">🌱</span> myKisah
        </a>

        <button class="navbar-toggler" type="button" data-bs-toggle="collapse"
                data-bs-target="#navbarNav" aria-controls="navbarNav"
                aria-expanded="false" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>

        <div class="collapse navbar-collapse" id="navbarNav">
            <ul class="navbar-nav me-auto">
                <li class="nav-item">
                    <NavLink class="nav-link" href="" Match="NavLinkMatch.All">Home</NavLink>
                </li>
                <li class="nav-item">
                    <NavLink class="nav-link" href="journals">My Journals</NavLink>
                </li>
                <li class="nav-item">
                    <NavLink class="nav-link" href="characters">Characters</NavLink>
                </li>
            </ul>

            @if (Session.IsLoggedIn && Session.CurrentUser != null)
            {
                <div class="dropdown">
                    <button class="btn btn-outline-light dropdown-toggle" type="button"
                            data-bs-toggle="dropdown" aria-expanded="false">
                        Hi, @Session.CurrentUser.Username
                    </button>
                    <ul class="dropdown-menu dropdown-menu-end">
                        <li><span class="dropdown-item-text text-muted small">
                            Joined @Session.CurrentUser.CreatedAt.ToString("dd MMM yyyy")
                        </span></li>
                        <li><hr class="dropdown-divider"></li>
                        <li><button class="dropdown-item" @onclick="Logout">Logout</button></li>
                    </ul>
                </div>
            }
            else
            {
                <span class="navbar-text text-muted">Not logged in</span>
            }
        </div>
    </div>
</nav>
```

```csharp
@code {
    private void Logout()
    {
        Session.Logout();
        Navigation.NavigateTo("/", forceLoad: true);
    }
}
```

**Skeleton CSS — NavMenu.razor.css:**

```css
.navbar {
    padding: 0.5rem 0;
}

.brand-icon {
    font-size: 1.3rem;
    margin-right: 0.3rem;
}

.navbar-brand {
    font-weight: 700;
    font-size: 1.2rem;
}

.nav-link.active {
    font-weight: 600;
    border-bottom: 2px solid #0d6efd;
}

@media (max-width: 768px) {
    .nav-link.active {
        border-bottom: none;
        background: rgba(255, 255, 255, 0.1);
        border-radius: 4px;
    }
}
```

**Testing:** Jalankan aplikasi. Navbar muncul horizontal. Klik "My Journals" → navigasi ke `/journals`. User dropdown muncul saat login. Responsive: hamburger di layar kecil.

---

### Task 6: Debug.razor

**Apa yang dibuat:** Pindahkan konten `Home.razor` ke halaman debug terpisah.

**Kenapa dibuat:** Debug dashboard tetap berguna untuk testing API, tapi tidak boleh jadi halaman utama. Dipindah ke `/debug` agar tetap bisa diakses untuk development.

**File:**

- `Components/Pages/Debug.razor` (BARU)
- Hapus `Components/Pages/Home.razor`

**Skeleton kode — Debug.razor:**

```razor
@page "/debug"
@rendermode InteractiveServer

@using myKisah.Models
@using myKisah.Interfaces

@inject IUserService UserService
@inject IJournalService JournalService
@inject ICharacterService CharacterService

@* TODO: Copy seluruh konten dari Home.razor (HTML + @code block) ke sini *@
@* TODO: JANGAN ubah apapun selain route — ganti @page "/" jadi @page "/debug" *@
@* TODO: Setelah copy selesai, hapus file Home.razor *@
```

**Testing:** Buka `/debug` di browser. Dashboard lama muncul dan berfungsi seperti sebelumnya.

---

## FASE 2: Journal Pages

*Dikerjakan paralel dengan anggota lain setelah FASE 1 selesai.*

---

### Task 7: JournalList.razor + CSS

**Apa yang dibuat:** Halaman daftar journal user yang login. Menampilkan semua journal dalam grid card, filter by mood menggunakan MoodPicker, dan navigasi ke create/detail.

**Kenapa dibuat:** Ini adalah "dashboard utama" setelah login — user melihat semua journal-nya dalam bentuk card yang bisa difilter dan diklik. Tanpa halaman ini, user tidak bisa melihat atau mengelola journal-nya.

**File:**

- `Components/Pages/JournalList.razor`
- `Components/Pages/JournalList.razor.css`

**Inject:** `IJournalService`, `UserSession`, `NavigationManager`
**Shared Components:** `MoodPicker`, `StateBadge`, `EmptyState`, `AlertMessage`, `ConfirmDialog`

**Skeleton kode:**

```razor
@page "/journals"
@rendermode InteractiveServer

@inject IJournalService JournalService
@inject UserSession Session
@inject NavigationManager Navigation

<PageTitle>My Journals — myKisah</PageTitle>

<div class="container-fluid">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
            <h1 class="mb-0">Journal @Session.CurrentUser?.Username</h1>
            <p class="text-muted mb-0">@_journals.Count journal</p>
        </div>
        <div>
            <button class="btn btn-primary" @onclick="NavigateToCreate">
                + Tulis Journal
            </button>
        </div>
    </div>

    <AlertMessage Message="_error" IsError="true" Show="_hasError" />

    @* Filter MoodPicker *@
    <div class="mb-3">
        <MoodPicker @bind-SelectedMood="_filterMoodNullable" Disabled="_loading" />
        @if (_filterMoodNullable != null)
        {
            <button class="btn btn-sm btn-outline-secondary ms-2" @onclick="ClearFilter">
                ✕ Semua
            </button>
        }
    </div>

    @if (_loading)
    {
        <p class="text-muted">Loading journals...</p>
    }
    else if (!_filteredJournals.Any())
    {
        <EmptyState Message="Belum ada journal. Mulai tulis ceritamu!"
                    ActionLabel="Tulis Journal Pertamamu"
                    OnAction="NavigateToCreate" />
    }
    else
    {
        <div class="row">
            @foreach (var journal in _filteredJournals)
            {
                <div class="col-md-4 col-sm-6 mb-3">
                    <div class="card journal-card" @onclick="() => NavigateToDetail(journal.Id)">
                        <div class="card-body">
                            <div class="d-flex justify-content-between align-items-start">
                                <h5 class="card-title">@journal.Title</h5>
                                <StateBadge State="journal.State" />
                            </div>
                            <div class="text-muted small mb-2">
                                @GetMoodEmoji(journal.Mood) @journal.Mood
                            </div>
                            <p class="card-text text-muted small">
                                @Truncate(journal.Content, 50)
                            </p>
                            <div class="d-flex justify-content-between align-items-center">
                                <small class="text-muted">
                                    @journal.CreatedAt.ToString("dd MMM yyyy")
                                </small>
                                <button class="btn btn-sm btn-outline-danger"
                                        @onclick="(e) => OnDeleteClick(e, journal.Id)"
                                        @onclick:stopPropagation="true">
                                    Delete
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            }
        </div>
    }

    @* ConfirmDialog untuk delete *@
    <ConfirmDialog Title="Hapus Journal?"
                   Message="Journal yang dihapus tidak bisa dikembalikan. Lanjutkan?"
                   Show="_showConfirm"
                   OnResult="OnConfirmDelete" />
</div>

@code {
    private List<Journal> _journals = new();
    private List<Journal> _filteredJournals = new();
    private MoodType? _filterMoodNullable;
    private bool _loading;
    private string _error = "";
    private bool _hasError;

    private bool _showConfirm;
    private string _journalToDelete = "";

    protected override void OnInitialized()
    {
        // TODO: Cek login — redirect ke / jika Session.IsLoggedIn == false
        // TODO: Panggil LoadJournals()
    }

    private async Task LoadJournals()
    {
        // TODO: Reset error, set loading
        // TODO: Panggil JournalService.GetJournalsByUser(Session.CurrentUser.Id)
        // TODO: Simpan ke _journals
        // TODO: Panggil ApplyFilter()
        // TODO: Catch exception → _hasError = true, _error = ex.Message
        // TODO: Finally → _loading = false
    }

    private void ApplyFilter()
    {
        // TODO: Jika _filterMoodNullable == null, _filteredJournals = _journals
        // TODO: Jika tidak null, filter: _journals.Where(j => j.Mood == _filterMoodNullable).ToList()
    }

    private async Task ClearFilter()
    {
        // TODO: Set _filterMoodNullable = null
        // TODO: Panggil ApplyFilter()
    }

    private void NavigateToCreate()
    {
        // TODO: Navigation.NavigateTo("/journals/new")
    }

    private void NavigateToDetail(string id)
    {
        // TODO: Navigation.NavigateTo($"/journals/{id}")
    }

    private async Task OnDeleteClick(MouseEventArgs e, string id)
    {
        // TODO: Stop event propagation supaya tidak trigger NavigateToDetail
        // TODO: Set _journalToDelete = id, _showConfirm = true
    }

    private async Task OnConfirmDelete(bool confirmed)
    {
        // TODO: Jika confirmed == true:
        //   Try: JournalService.DeleteJournal(_journalToDelete), reload LoadJournals()
        //   Catch: _hasError = true, _error = ex.Message
        // TODO: _showConfirm = false
    }

    private static string GetMoodEmoji(MoodType mood) => mood switch
    {
        // TODO: Happy → "😊", Sad → "😢", Angry → "😠", Anxious → "😰", Calm → "😌"
        _ => "😊"
    };

    private static string Truncate(string? text, int max)
    {
        // TODO: Implement truncate: jika text > max → text[..max] + "..."
        return text ?? "";
    }
}
```

**Skeleton CSS — JournalList.razor.css:**

```css
.journal-card {
    cursor: pointer;
    transition: transform 0.15s, box-shadow 0.15s;
    border: 1px solid #dee2e6;
}

.journal-card:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    border-color: #0d6efd;
}

.journal-card .card-title {
    font-size: 1.1rem;
    margin-right: 0.5rem;
}
```

**Testing:**

1. Login → redirect ke `/journals`
2. Tampil grid card journal user yang login
3. Klik chip mood di MoodPicker → journal terfilter
4. Klik card → navigasi ke `/journals/{id}`
5. Klik Delete → ConfirmDialog muncul → "Ya" → journal terhapus
6. Jika belum ada journal → tampil EmptyState dengan tombol

---

### Task 8: JournalDetail.razor + CSS

**Apa yang dibuat:** Halaman detail satu journal. Menampilkan isi penuh + StateBadge + action bar dengan tombol yang muncul/hilang berdasarkan state (state machine).

**Kenapa dibuat:** User perlu melihat isi journal secara penuh dan melakukan transisi state (Submit, Save, Reject, Reset). Tombol harus dinamis — hanya muncul yang valid untuk state saat ini. Ini menghubungkan UI dengan JournalStateMachine yang sudah ada di backend.

**File:**

- `Components/Pages/JournalDetail.razor`
- `Components/Pages/JournalDetail.razor.css`

**Inject:** `IJournalService`, `UserSession`, `NavigationManager`
**Shared Components:** `StateBadge`, `AlertMessage`, `ConfirmDialog`

**Skeleton kode:**

```razor
@page "/journals/{Id}"
@rendermode InteractiveServer

@inject IJournalService JournalService
@inject UserSession Session
@inject NavigationManager Navigation

<PageTitle>@(_journal?.Title ?? "Journal Detail") — myKisah</PageTitle>

<div class="container-fluid">

    @if (_loading)
    {
        <p class="text-muted">Loading journal...</p>
    }
    else if (_journal == null)
    {
        <AlertMessage Message="Journal tidak ditemukan." IsError="true" Show="true" />
    }
    else
    {
        <button class="btn btn-outline-secondary mb-3" @onclick="GoBack">
            ← Kembali ke Journal
        </button>

        <AlertMessage Message="_error" IsError="true" Show="_hasError" />
        <AlertMessage Message="_success" IsError="false" Show="_hasSuccess" />

        <div class="card">
            <div class="card-body">
                <div class="d-flex justify-content-between align-items-start mb-3">
                    <h1 class="card-title mb-0">@_journal.Title</h1>
                    <StateBadge State="_journal.State" />
                </div>

                <div class="mb-3">
                    <span class="badge bg-light text-dark">
                        @GetMoodEmoji(_journal.Mood) @_journal.Mood
                    </span>
                </div>

                <div class="journal-content mb-4">
                    @_journal.Content
                </div>

                <hr>

                <div class="d-flex justify-content-between align-items-center">
                    <small class="text-muted">
                        Dibuat: @_journal.CreatedAt.ToString("dd MMM yyyy HH:mm")
                    </small>

                    <div>
                        @* Action buttons — muncul berdasarkan state *@
                        @switch (_journal.State)
                        {
                            case JournalState.Draft:
                                <button class="btn btn-primary" @onclick="SubmitJournal"
                                        disabled="@_actionLoading">
                                    Submit
                                </button>
                                break;

                            case JournalState.Submitted:
                                <button class="btn btn-success me-2" @onclick="SaveJournal"
                                        disabled="@_actionLoading">
                                    Save
                                </button>
                                <button class="btn btn-warning" @onclick="RejectJournal"
                                        disabled="@_actionLoading">
                                    Reject
                                </button>
                                break;

                            case JournalState.Rejected:
                                <button class="btn btn-outline-primary" @onclick="ResetJournal"
                                        disabled="@_actionLoading">
                                    Reset ke Draft
                                </button>
                                break;

                            case JournalState.Saved:
                                <span class="text-success fw-bold">
                                    ✓ Journal ini sudah disimpan
                                </span>
                                break;
                        }
                    </div>
                </div>
            </div>
        </div>

        @* Delete button *@
        <div class="mt-3">
            <button class="btn btn-outline-danger btn-sm" @onclick="ShowDeleteConfirm">
                Hapus Journal
            </button>
        </div>
    }

    @* ConfirmDialog untuk delete *@
    <ConfirmDialog Title="Hapus Journal?"
                   Message="Journal yang dihapus tidak bisa dikembalikan. Lanjutkan?"
                   Show="_showConfirm"
                   OnResult="OnConfirmDelete" />
</div>

@code {
    [Parameter]
    public string Id { get; set; } = default!;

    private Journal? _journal;
    private bool _loading;
    private bool _actionLoading;
    private string _error = "";
    private bool _hasError;
    private string _success = "";
    private bool _hasSuccess;
    private bool _showConfirm;

    protected override void OnInitialized()
    {
        // TODO: Cek login — redirect ke / jika Session.IsLoggedIn == false
        // TODO: Panggil LoadJournal()
    }

    private async Task LoadJournal()
    {
        // TODO: Reset error/success, set loading
        // TODO: Try: _journal = JournalService.GetById?? (tidak ada GetById di IJournalService)
        //   Gunakan: var journals = JournalService.GetJournalsByUser(Session.CurrentUser.Id)
        //   _journal = journals.FirstOrDefault(j => j.Id == Id)
        // TODO: Catch: _hasError = true, _error = ex.Message
        // TODO: Finally: _loading = false
    }

    private async Task SubmitJournal()
    {
        // TODO: Reset messages, _actionLoading = true
        // TODO: Try: JournalService.SubmitJournal(Id)
        //   → _success = "Journal berhasil disubmit!"
        //   → Reload journal: LoadJournal()
        // TODO: Catch: _hasError = true, _error = ex.Message
        // TODO: Finally: _actionLoading = false
    }

    private async Task SaveJournal()
    {
        // TODO: Reset messages, _actionLoading = true
        // TODO: Try: JournalService.SaveJournal(Id)
        //   → _success = "Journal berhasil disimpan!"
        //   → Reload journal
        // TODO: Catch
        // TODO: Finally
    }

    private async Task RejectJournal()
    {
        // TODO: Reset messages, _actionLoading = true
        // TODO: Try: JournalService.RejectJournal(Id)
        //   → _success = "Journal ditolak."
        //   → Reload journal
        // TODO: Catch
        // TODO: Finally
    }

    private async Task ResetJournal()
    {
        // TODO: Reset messages, _actionLoading = true
        // TODO: Try: JournalService.ResetJournal(Id)
        //   → _success = "Journal di-reset ke Draft."
        //   → Reload journal
        // TODO: Catch
        // TODO: Finally
    }

    private void ShowDeleteConfirm()
    {
        // TODO: _showConfirm = true
    }

    private async Task OnConfirmDelete(bool confirmed)
    {
        // TODO: Jika confirmed: Delete lalu navigasi ke /journals
        // TODO: _showConfirm = false
    }

    private void GoBack()
    {
        // TODO: Navigation.NavigateTo("/journals")
    }

    private static string GetMoodEmoji(MoodType mood) => mood switch
    {
        // TODO: Happy → "😊", Sad → "😢", Angry → "😠", Anxious → "😰", Calm → "😌"
        _ => "😊"
    };
}
```

**Skeleton CSS — JournalDetail.razor.css:**

```css
.journal-content {
    font-size: 1.05rem;
    line-height: 1.8;
    white-space: pre-wrap;
    padding: 1rem 0;
}

.card-title {
    font-size: 1.5rem;
    flex: 1;
    margin-right: 1rem;
}
```

**Testing:**

1. Klik journal di JournalList → detail muncul
2. State badge tampil sesuai state (Draft/Submitted/Saved/Rejected)
3. Tombol aksi sesuai state — coba semua transisi:
   - Draft → klik Submit → state jadi Submitted → tombol berubah
   - Submitted → klik Save → state jadi Saved → tampil "✓ Journal ini sudah disimpan"
   - Submitted → klik Reject → state jadi Rejected → tampil "Reset ke Draft"
   - Rejected → klik Reset → state jadi Draft → kembali ke awal
4. Klik "Hapus Journal" → ConfirmDialog → "Ya" → redirect `/journals`
5. Error transisi (misal Submit dari Saved) → tampil AlertMessage error

---

## FASE 3: Verifikasi

---

### Task 9: Verifikasi Akhir

**Apa yang dibuat:** Final check — memastikan semua komponen berfungsi, build sukses, test pass.

**Kenapa dibuat:** Sebagai inisiator, kamu bertanggung jawab memastikan aplikasi siap dipresentasikan/didemokan.

**Langkah:**

```
1. dotnet build   → pastikan 0 error
2. dotnet test    → pastikan 65 test pass
3. jalankan aplikasi dengan dotnet run
4. test semua halaman sesuai checklist di bawah
5. jika ada bug, koordinasikan dengan anggota terkait
6. merge semua branch ke main
```

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
Navigation.NavigateTo($"/journals/{journal.Id}");
```

### Form Bind

```razor
<input @bind="_field" class="form-control" />
<button @onclick="OnSave" disabled="@_loading">Simpan</button>
```

### Cek Login

```csharp
protected override void OnInitialized()
{
    if (!Session.IsLoggedIn) { Navigation.NavigateTo("/"); return; }
}
```

### Styling

| Aturan         | Detail                                        |
| -------------- | --------------------------------------------- |
| Container      | `<div class="container-fluid">`             |
| Grid 3 kolom   | `<div class="col-md-4 col-sm-6">`           |
| Card           | `<div class="card"><div class="card-body">` |
| Tombol primary | `btn btn-primary`                           |
| Tombol danger  | `btn btn-danger`                            |
| CSS            | Scoped `.razor.css` per komponen            |

---

## Komponen Shared yang Dipakai

| Komponen          | Cara Pakai                                                                         |
| ----------------- | ---------------------------------------------------------------------------------- |
| `UserSession`   | `@inject UserSession Session`                                                    |
| `MoodPicker`    | `<MoodPicker @bind-SelectedMood="_mood" />`                                      |
| `StateBadge`    | `<StateBadge State="journal.State" />`                                           |
| `AlertMessage`  | `<AlertMessage Message="_error" IsError="true" Show="_hasError" />`              |
| `EmptyState`    | `<EmptyState Message="..." ActionLabel="..." OnAction="..." />`                  |
| `ConfirmDialog` | `<ConfirmDialog Title="..." Message="..." Show="_showConfirm" OnResult="..." />` |

---

## Checklist Selesai

- [ ] `dotnet build` — 0 error
- [ ] `dotnet test` — 65 test pass
- [ ] `dotnet run` → `/debug` — dashboard lama berfungsi
- [ ] `/` — navbar horizontal tampil, belum ada Login page (dikerjakan Farel)
- [ ] `/journals` — redirect ke `/` jika belum login
- [ ] Login via debug → `/journals` — grid card journal tampil
- [ ] MoodPicker filter — journal terfilter, "Semua" reset
- [ ] Klik card → `/journals/{id}` — detail tampil
- [ ] State transition — Submit/Save/Reject/Reset berfungsi, UI update
- [ ] Delete journal → ConfirmDialog → redirect `/journals`
- [ ] Navbar responsive — hamburger di mobile
- [ ] User dropdown — tampil username, Logout berfungsi
- [ ] EmptyState tampil saat belum ada journal
