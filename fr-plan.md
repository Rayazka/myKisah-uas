# myKisah — Frontend Renovation Plan (FR-Plan)

> Rencana pengembangan UI Blazor — mengubah debug dashboard menjadi aplikasi journaling user-facing.
> **Dokumen ini adalah panduan utama untuk seluruh anggota tim.**

---

## 1. Konteks Proyek

| Aspek         | Detail                                                                    |
| ------------- | ------------------------------------------------------------------------- |
| Framework     | .NET 10 Blazor Server (InteractiveServer)                                 |
| Data access   | Components inject services langsung (tidak perlu HttpClient)              |
| Styling base  | Bootstrap 5 + custom CSS scoped per komponen                              |
| Pola kode     | Mengacu pada `Home.razor` existing (DI, bind, try-catch, message state) |
| User identity | `UserSession` scoped service (per-circuit)                              |

---

## 2. Cara Menggunakan Dokumen Ini

| Langkah | Untuk semua anggota |
|---|---|
| 1 | Baca **Konteks Proyek** dan **Keputusan Desain** — pahami arsitektur dan layout baru |
| 2 | Baca **Prasyarat Sebelum Memulai** — pastikan environment siap |
| 3 | Baca **Referensi Cepat: Shared Components** (section 9) — kenali komponen yang sudah/bakal ada |
| 4 | **Buka file guide kamu di folder `guides/`** — semua petunjuk, skeleton kode, dan konvensi ada di sana. Kerjakan sesuai urutan task. |
| 5 | Setelah selesai, cek **Checklist Verifikasi Akhir** (section 10) |

> **Jangan edit `fr-plan.md` ini.** Setiap anggota memiliki file panduan sendiri di `guides/guide-nama.md` yang berisi: daftar todo, apa yang dibuat & kenapa, skeleton kode lengkap dengan placeholder `// TODO`, konvensi kode, dan checklist testing.

---

## 3. Keputusan Desain

| #               | Keputusan                                                                   |
| --------------- | --------------------------------------------------------------------------- |
| Layout          | **Top navbar + full-width content** (hilangkan sidebar template lama) |
| Debug dashboard | Dipindah ke `/debug` (tidak dibuang)                                      |
| Mood selector   | **Visual**: chip horizontal dengan emoji (`MoodPicker.razor`)       |
| Halaman utama   | Login page (`/`) — pilih user atau registrasi                            |

---

## 4. Struktur File — Final

```
Components/
├── App.razor                              (tetap)
├── _Imports.razor                         (UPDATE: +@using Models, Interfaces, Services)
├── Routes.razor                           (tetap)
├── Layout/
│   ├── MainLayout.razor                   (REWRITE: top navbar, no sidebar)
│   ├── MainLayout.razor.css               (REWRITE: full-width layout)
│   ├── NavMenu.razor                      (REWRITE: horizontal top nav)
│   ├── NavMenu.razor.css                  (REWRITE: horizontal nav styling)
│   ├── ReconnectModal.razor               (tetap)
│   ├── ReconnectModal.razor.css           (tetap)
│   └── ReconnectModal.razor.js            (tetap)
├── Pages/
│   ├── Login.razor + .css                 (BARU, route: /)
│   ├── JournalList.razor + .css           (BARU, route: /journals)
│   ├── JournalCreate.razor + .css         (BARU, route: /journals/new)
│   ├── JournalDetail.razor + .css         (BARU, route: /journals/{id})
│   ├── CharacterList.razor + .css         (BARU, route: /characters)
│   ├── CharacterChat.razor + .css         (BARU, route: /characters/{id})
│   ├── Debug.razor                        (PINDAHAN dari Home.razor, route: /debug)
│   ├── Error.razor                        (tetap)
│   └── NotFound.razor                     (tetap)
├── Shared/
│   ├── StateBadge.razor                   (BARU)
│   ├── MoodPicker.razor + .css            (BARU — chip emoji visual)
│   ├── EmptyState.razor                   (BARU)
│   ├── ConfirmDialog.razor                (BARU)
│   └── AlertMessage.razor                 (BARU)
└── Services/
    └── UserSession.cs                     (BARU — scoped per circuit)
```

---

## 5. Alur Navigasi

```
/                      → Login.razor          (pilih user / register)
/journals              → JournalList.razor     (wajib login → redirect ke /)
/journals/new          → JournalCreate.razor   (wajib login)
/journals/{id}         → JournalDetail.razor   (wajib login)
/characters            → CharacterList.razor   (wajib login)
/characters/{id}       → CharacterChat.razor   (wajib login)
/debug                 → Debug.razor           (API test dashboard)
```

---

## 6. Prasyarat Sebelum Memulai

**Untuk semua anggota — lakukan ini sebelum menulis kode:**

| # | Prasyarat                                                                                   |
| - | ------------------------------------------------------------------------------------------- |
| 1 | Pull branch `main` terbaru — pastikan backend semua sudah up-to-date                     |
| 2 | Jalankan `dotnet build` — pastikan **0 error** sebelum mulai                       |
| 3 | Jalankan `dotnet test` — pastikan **65 test pass**                                 |
| 4 | **Azka harus menyelesaikan FASE 1 dulu** (task 1–6) sebelum anggota lain mulai       |
| 5 | Baca section**10. Konvensi Kode & Contoh** — semua halaman wajib ikuti template ini  |
| 6 | Kenali**Shared Components** di section 9 — ini komponen yang kamu pakai di halamanmu |
| 7 | Folder kerja semua komponen:`myKisah/Components/`                                         |
| 8 | File `.razor` dan `.razor.css` selalu berpasangan — buat keduanya                      |

---

## 7. Spesifikasi Komponen

### 7.1 UserSession (Scoped Service)

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

**Registrasi DI di `Program.cs`:**

```csharp
builder.Services.AddScoped<UserSession>();
```

---

### 7.2 MainLayout (REWRITE)

**File:** `Components/Layout/MainLayout.razor`

Flex column, min-height 100vh. `<header>` fixed/sticky di atas berisi `<NavMenu>`. `<main>` full-width container dengan padding seragam. **Tanpa sidebar, tanpa link "About" template.**

```html
@inherits LayoutComponentBase

<div class="app">
    <header>
        <NavMenu />
    </header>
    <main class="container-fluid">
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

**NavMenu — wireframe:**

```
┌──────────────────────────────────────────────────────────────────┐
│  🌱 myKisah    Home   My Journals   Characters     [Hi, Azka ▼]  │
└──────────────────────────────────────────────────────────────────┘
```

- Brand "myKisah 🌱" di kiri → link ke `/`
- NavLink: Home (`/`), My Journals (`/journals`), Characters (`/characters`)
- User dropdown di kanan: "Hi, {username}" + Logout
- Responsive: collapse ke hamburger di mobile
- Active link highlighted dengan class Bootstrap `active`

---

### 7.3 Halaman: Login (`/`)

**File:** `Components/Pages/Login.razor`

| Aspek                 | Detail                                                                                          |
| --------------------- | ----------------------------------------------------------------------------------------------- |
| **UI**          | Card center di tengah layar. Branding besar: "myKisah 🌱" + tagline                             |
|                       | **Section A** — Pilih user existing: grid card user (nama + tanggal join), klik → login |
|                       | **Section B** — User baru: input username + tombol "Mulai Petualangan"                   |
| **Inject**      | `IUserService`, `UserSession`, `NavigationManager`                                        |
| **State**       | `List<User> _users`, `string _newUsername`, `string _error`, `bool _loading`            |
| **Validasi**    | Username tidak kosong, tidak duplikat                                                           |
| **Empty**       | Jika belum ada user → hanya section B yang muncul                                              |
| **After login** | `Session.Login(user)` → `NavigateTo("/journals")`                                          |

---

### 7.4 Halaman: Journal List (`/journals`)

**File:** `Components/Pages/JournalList.razor`

| Aspek                   | Detail                                                                                                 |
| ----------------------- | ------------------------------------------------------------------------------------------------------ |
| **UI**            | Header: "Journal milik {username}" +`MoodPicker` filter + tombol "Tulis Journal"                     |
|                         | Grid card 3 kolom (responsive: 1 kolom di mobile)                                                      |
|                         | Tiap card: mood emoji+label, title, excerpt 50 char,`StateBadge`, tanggal, tombol hapus              |
|                         | Card bisa diklik → navigasi ke `/journals/{id}`                                                     |
|                         | Chip "Semua" di MoodPicker untuk reset filter                                                          |
| **Inject**        | `IJournalService`, `UserSession`, `NavigationManager`                                            |
| **State**         | `List<Journal> _journals`, `List<Journal> _filtered`, `MoodType? _filterMood`, `bool _loading` |
| **OnInitialized** | Redirect ke `/` jika belum login. Load journals via `GetJournalsByUser()`                          |
| **Empty**         | `<EmptyState Message="Belum ada journal." ActionLabel="Tulis Journal Pertamamu" />`                  |

---

### 7.5 Halaman: Journal Create (`/journals/new`)

**File:** `Components/Pages/JournalCreate.razor`

| Aspek                | Detail                                                                                                                       |
| -------------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| **UI**         | Form card di tengah (max-width 700px). Title input, textarea content (rows=6),`MoodPicker`, karakter counter `{n}/{max}` |
|                      | Tombol "Simpan" (btn-primary) + "Batal" (btn-outline)                                                                        |
| **Inject**     | `IJournalService`, `UserSession`, `NavigationManager`, `IConfiguration`                                              |
| **State**      | `string _title`, `string _content`, `MoodType _mood`, `bool _saving`, `string _error`                              |
| **Validasi**   | Title/content tidak kosong, content ≤ MaxContentLength dari config                                                          |
| **After save** | Navigate ke `/journals`                                                                                                    |

---

### 7.6 Halaman: Journal Detail (`/journals/{id}`)

**File:** `Components/Pages/JournalDetail.razor`

| Aspek               | Detail                                                                                |
| ------------------- | ------------------------------------------------------------------------------------- |
| **UI**        | Card detail: title besar,`StateBadge`, mood (emoji+label), content penuh, createdAt |
|                     | **Action bar** — tombol dinamis berdasarkan state (lihat tabel di bawah)       |
|                     | Navigasi "← Kembali ke Journal" + tombol hapus (pakai `ConfirmDialog`)             |
| **Inject**    | `IJournalService`, `UserSession`, `NavigationManager`                           |
| **Parameter** | `[Parameter] public string Id { get; set; }`                                        |
| **State**     | `Journal? _journal`, `string _error`, `bool _loading`                           |

**Tombol aksi berdasarkan state:**

| State     | Tombol aktif         | Memanggil                                                    |
| --------- | -------------------- | ------------------------------------------------------------ |
| Draft     | `Submit`           | `_service.SubmitJournal(Id)`                               |
| Submitted | `Save`, `Reject` | `_service.SaveJournal(Id)`, `_service.RejectJournal(Id)` |
| Rejected  | `Reset`            | `_service.ResetJournal(Id)`                                |
| Saved     | *(terminal)*       | Tampil teks "Journal ini sudah disimpan"                     |

- Setelah transisi sukses → reload journal → state badge & tombol update otomatis
- Error transisi → `AlertMessage` dengan pesan error

---

### 7.7 Halaman: Character List (`/characters`)

**File:** `Components/Pages/CharacterList.razor`

| Aspek            | Detail                                                                                |
| ---------------- | ------------------------------------------------------------------------------------- |
| **UI**     | Full-width. Grid card 3 kolom.                                                        |
|                  | Tiap card: avatar visual (gradien circle + emoji) + nama + deskripsi + tombol "Chat"  |
|                  | Kira: 💫 ungu `#7c3aed`, Luna: 🌙 biru gelap `#1e40af`, Ren: 🎧 hijau `#059669` |
| **Inject** | `ICharacterService`, `NavigationManager`                                          |
| **State**  | `List<Character> _characters`, `bool _loading`                                    |
| **Action** | Klik "Chat" → navigasi ke `/characters/{id}`                                       |

---

### 7.8 Halaman: Character Chat (`/characters/{id}`)

**File:** `Components/Pages/CharacterChat.razor`

| Aspek               | Detail                                                                                                   |
| ------------------- | -------------------------------------------------------------------------------------------------------- |
| **UI**        | Layout side-by-side (desktop) atau stacked (mobile)                                                      |
|                     | **Panel kiri**: avatar besar, nama, deskripsi, tombol "← Kembali"                                 |
|                     | **Panel kanan**: area chat scrollable — bubble chat berisi history (mood → response + timestamp) |
|                     | **Input bawah**: `MoodPicker` + tombol "Kirim"                                                   |
| **Inject**    | `ICharacterService`, `NavigationManager`                                                             |
| **Parameter** | `[Parameter] public string Id { get; set; }`                                                           |
| **State**     | `Character? _character`, `MoodType _selectedMood`, `List<ChatEntry> _history`                      |
| **ChatEntry** | `record ChatEntry(MoodType Mood, string Response, DateTime Time);`                                     |
| **Empty**     | `<EmptyState Message="Pilih mood untuk memulai percakapan dengan {nama}" />`                           |

---

### 7.9 Halaman: Debug (`/debug`)

**File:** `Components/Pages/Debug.razor`

- Konten **sama persis** dengan `Home.razor` existing
- Route diganti dari `/` menjadi `/debug`
- Berisi: User Management, Journal, Character Companion test sections
- Tidak perlu login — akses bebas untuk development

---

## 8. Pembagian Tugas

Setiap anggota memiliki file panduan terpisah di folder `guides/` yang berisi daftar todo, apa yang dibuat & kenapa, skeleton kode lengkap dengan placeholder `// TODO`, konvensi kode, dan checklist testing.

| Anggota | Guide File | Task | Domain |
|---|---|---|---|
| **Rayazka (Azka)** | [`guides/guide-azka.md`](guides/guide-azka.md) | 9 | Foundation, Layout, Journal, Verifikasi |
| **Farel** | [`guides/guide-farel.md`](guides/guide-farel.md) | 2 | User + Journal Form |
| **Toni** | [`guides/guide-toni.md`](guides/guide-toni.md) | 2 | Character Companion |
| **Rafly** | [`guides/guide-rafly.md`](guides/guide-rafly.md) | 2 | Shared Visual |
| **Jojo** | [`guides/guide-jojo.md`](guides/guide-jojo.md) | 3 | Shared Functional |

### Dependensi & Urutan Eksekusi

```
FASE 1: Rayazka (Azka) — guide-azka.md task 1–6
  UserSession, DI, _Imports, MainLayout, NavMenu, Debug
  ↓ (pondasi selesai — semua anggota bisa mulai paralel)

FASE 2: Semua anggota paralel
  Rayazka: guide-azka.md task 7–8 (JournalList, JournalDetail)
  Farel:   guide-farel.md task 10–11 (Login, JournalCreate)
  Toni:    guide-toni.md task 12–13 (CharacterList, CharacterChat)
  Rafly:   guide-rafly.md task 14–15 (MoodPicker, ConfirmDialog)
  Jojo:    guide-jojo.md task 16–18 (StateBadge, AlertMessage, EmptyState)

FASE 3: Rayazka (Azka) — guide-azka.md task 9
  Verifikasi — build + test + smoke test semua halaman
```

---

## 9. Referensi Cepat: Shared Components

### UserSession
**Deskripsi:** Scoped service yang menyimpan identitas user yang sedang login. Karena Blazor Server menggunakan circuit per koneksi, satu `UserSession` instance hidup selama sesi browser user. Semua halaman mengakses `UserSession` untuk mengetahui siapa yang sedang login dan untuk redirect ke `/` jika belum login.

**Cara pakai:** `@inject UserSession Session` → `Session.CurrentUser`, `Session.IsLoggedIn`, `Session.Login(user)`, `Session.Logout()`

### MoodPicker
**Deskripsi:** Komponen visual untuk memilih mood. Menggantikan dropdown `<select>` — menampilkan 5 chip horizontal dengan emoji dan label. Chip yang aktif di-highlight warna biru. Komponen ini dipakai di JournalCreate (untuk pilih mood saat tulis journal), JournalList (filter), dan CharacterChat (pilih mood untuk chat).

| Parameter | Tipe | Deskripsi |
|---|---|---|
| `SelectedMood` | `MoodType` | Mood yang dipilih |
| `SelectedMoodChanged` | `EventCallback<MoodType>` | Dipanggil saat chip diklik |
| `Disabled` | `bool` | Nonaktifkan semua chip |

**Cara pakai:** `<MoodPicker @bind-SelectedMood="_mood" Disabled="_loading" />`

### StateBadge
**Deskripsi:** Badge kecil berbentuk pill yang menampilkan state journal. Warna mengikuti state: Draft (kuning), Submitted (biru), Saved (hijau), Rejected (merah). Dipakai di JournalList (tiap card) dan JournalDetail (header).

| Parameter | Tipe | Deskripsi |
|---|---|---|
| `State` | `JournalState` | State journal yang akan ditampilkan |

**Cara pakai:** `<StateBadge State="journal.State" />`

### AlertMessage
**Deskripsi:** Komponen notifikasi yang menampilkan pesan sukses (hijau) atau error (merah). Dipakai di semua halaman untuk feedback ke user setelah aksi (create, delete, submit, dll.). Gunakan `IsError="true"` untuk error (merah) dan `IsError="false"` untuk sukses (hijau).

| Parameter | Tipe | Deskripsi |
|---|---|---|
| `Message` | `string` | Teks pesan |
| `IsError` | `bool` | `true` = style merah, `false` = style hijau |
| `Show` | `bool` | Kontrol visibilitas — sembunyikan saat tidak ada pesan |

**Cara pakai:** `<AlertMessage Message="_error" IsError="true" Show="_hasError" />`

### EmptyState
**Deskripsi:** Placeholder yang ditampilkan saat tidak ada data. Menampilkan ikon, pesan, dan tombol aksi opsional. Dipakai di JournalList (saat user belum punya journal) dan CharacterChat (saat belum ada chat history).

| Parameter | Tipe | Deskripsi |
|---|---|---|
| `Message` | `string` | Pesan yang ditampilkan |
| `ActionLabel` | `string?` | Label tombol aksi (null = tanpa tombol) |
| `OnAction` | `EventCallback` | Handler saat tombol diklik |

**Cara pakai:** `<EmptyState Message="Belum ada data" ActionLabel="Tambah" OnAction="OnAdd" />`

### ConfirmDialog
**Deskripsi:** Modal konfirmasi untuk aksi destruktif (seperti hapus journal). Menampilkan backdrop gelap, judul, pesan, dan dua tombol: "Ya" (merah, konfirmasi) dan "Batal" (outline, tutup). Dipakai di JournalList dan JournalDetail untuk konfirmasi sebelum delete.

| Parameter | Tipe | Deskripsi |
|---|---|---|
| `Title` | `string` | Judul modal (contoh: "Hapus Journal?") |
| `Message` | `string` | Pesan konfirmasi |
| `Show` | `bool` | Kontrol visibilitas modal |
| `OnResult` | `EventCallback<bool>` | `true` jika user klik Ya, `false` jika Batal/tutup |

**Cara pakai:** `<ConfirmDialog Title="Hapus?" Message="Yakin?" Show="_showConfirm" OnResult="OnConfirmResult" />`

### _Imports.razor
**Deskripsi:** File global using untuk semua komponen Razor. Setelah FASE 1, file ini sudah otomatis meng-include `myKisah.Models`, `myKisah.Interfaces`, dan `myKisah.Services` — kamu **tidak perlu menulis `@using` manual** di setiap halaman.

---

## 10. Checklist Verifikasi Akhir

- [ ] `dotnet build` — 0 error
- [ ] `dotnet test` — 65 test pass
- [ ] `/` — Login page tampil dengan branding + daftar user
- [ ] Register user baru — sukses, redirect ke `/journals`
- [ ] `/journals` — tampil journal user yang login beserta MoodPicker filter
- [ ] MoodPicker filter — tap mood chip, journal terfilter
- [ ] `/journals/new` — form create berfungsi, mood visual, karakter counter
- [ ] `/journals/{id}` — detail journal + StateBadge + action bar sesuai state
- [ ] State transition — Submit/Save/Reject/Reset berfungsi, UI update otomatis
- [ ] `/characters` — list 3 karakter dengan avatar visual
- [ ] `/characters/{id}` — chat dengan MoodPicker, history bubble chat
- [ ] `/debug` — API test dashboard tetap bisa diakses
- [ ] Navbar — horizontal, responsive hamburger di mobile
- [ ] User dropdown — tampil username, tombol Logout berfungsi
- [ ] Empty states — tampil di JournalList, CharacterChat saat belum ada data
- [ ] ConfirmDialog — muncul saat delete, bisa cancel/confirm

---

> *myKisah — Build habits, write your journey, grow together. 🌱*
