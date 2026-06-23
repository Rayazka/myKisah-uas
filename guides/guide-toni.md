# Guide: Toni — Character Companion Module

**Baca panduan ini dari atas ke bawah. Kerjakan task sesuai urutan.**

Jumlah task: **2** | Domain: **Character Companion**

---

## Domain

Kamu mengerjakan **halaman Character List** (browse 3 karakter) dan **halaman Character Chat** (interaksi dengan satu karakter via mood). Kedua halaman menggunakan `CharacterService` yang kamu buat sebelumnya dan komponen `MoodPicker` dari Rafly.

---

## Prasyarat Sebelum Memulai

- [ ] Azka sudah menyelesaikan FASE 1 (task 1–6) — tanya Azka jika belum
- [ ] Pull branch `main` terbaru
- [ ] `dotnet build` — 0 error
- [ ] File referensi terbuka: `Home.razor` line 109–160 (character section)
- [ ] Baca section **9. Referensi Cepat: Shared Components** di `fr-plan.md`
- [ ] Komponen shared yang harus sudah ada: `MoodPicker`, `EmptyState`, `AlertMessage`
- [ ] Data karakter ada di `Data/characters.json` (3 karakter: Kira, Luna, Ren)

---

## Daftar Tugas

| # | Task | File |
|---|---|---|
| 12 | `CharacterList` | `Components/Pages/CharacterList.razor` + `.css` |
| 13 | `CharacterChat` | `Components/Pages/CharacterChat.razor` + `.css` |

---

### Task 12: CharacterList.razor + CSS

**Apa yang dibuat:** Halaman yang menampilkan 3 karakter companion virtual dalam grid card. Setiap card memiliki avatar visual (gradien circle + emoji), nama, deskripsi, dan tombol "Chat".

**Kenapa dibuat:** User perlu melihat dan memilih karakter companion sebelum bisa chat. Halaman ini menampilkan ketiga karakter dengan visual yang menarik — bukan sekadar tabel data seperti di debug dashboard. Ini adalah showcase dari table-driven character system yang kamu bangun.

**File:**
- `Components/Pages/CharacterList.razor`
- `Components/Pages/CharacterList.razor.css`

**Inject:** `ICharacterService`, `NavigationManager`
**Shared Components:** —

**Skeleton kode:**

```razor
@page "/characters"
@rendermode InteractiveServer

@inject ICharacterService CharacterService
@inject NavigationManager Navigation

<PageTitle>Character Companions — myKisah</PageTitle>

<div class="container-fluid">
    <h1 class="mb-2">Character Companions</h1>
    <p class="text-muted mb-4">
        Pilih karakter companion-mu. Mereka akan merespons sesuai mood yang kamu pilih.
    </p>

    @if (_loading)
    {
        <p class="text-muted">Loading characters...</p>
    }
    else if (!_characters.Any())
    {
        <p class="text-muted">Tidak ada karakter tersedia.</p>
    }
    else
    {
        <div class="row">
            @foreach (var character in _characters)
            {
                var style = GetCharacterStyle(character);
                <div class="col-md-4 col-sm-6 mb-4">
                    <div class="card character-card">
                        <div class="card-body text-center">
                            <div class="character-avatar mb-3"
                                 style="background: @style.Gradient;">
                                <span>@style.Emoji</span>
                            </div>
                            <h5 class="card-title">@character.Name</h5>
                            <p class="card-text text-muted small">
                                @character.Description
                            </p>
                            <button class="btn btn-primary btn-sm"
                                    @onclick="() => SelectCharacter(character.Id)">
                                💬 Chat with @character.Name
                            </button>
                        </div>
                    </div>
                </div>
            }
        </div>
    }
</div>

@code {
    private List<Character> _characters = new();
    private bool _loading;

    protected override void OnInitialized()
    {
        // TODO: Load karakter saat halaman pertama kali dibuka
        // Try: _characters = CharacterService.GetAllCharacters().ToList()
        // Catch: (error — tapi ini unlikely karena data dari JSON)
        // Finally: _loading = false
    }

    private void SelectCharacter(string id)
    {
        // TODO: Navigation.NavigateTo($"/characters/{id}")
    }

    private (string Emoji, string Gradient) GetCharacterStyle(Character c)
    {
        // TODO: Kembalikan emoji + gradien berdasarkan nama karakter:
        //   "Kira" → ("💫", "linear-gradient(135deg, #7c3aed, #a78bfa)")
        //   "Luna" → ("🌙", "linear-gradient(135deg, #1e40af, #3b82f6)")
        //   "Ren"  → ("🎧", "linear-gradient(135deg, #059669, #34d399)")
        //   default → ("🌟", "linear-gradient(135deg, #6366f1, #818cf8)")
        return ("🌟", "linear-gradient(135deg, #6366f1, #818cf8)");
    }
}
```

**Skeleton CSS — CharacterList.razor.css:**

```css
.character-card {
    transition: transform 0.15s, box-shadow 0.15s;
    border: 1px solid #dee2e6;
}

.character-card:hover {
    transform: translateY(-3px);
    box-shadow: 0 6px 16px rgba(0, 0, 0, 0.1);
}

.character-avatar {
    width: 80px;
    height: 80px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    margin: 0 auto;
}

.character-avatar span {
    font-size: 2rem;
}

.character-card .card-title {
    font-size: 1.1rem;
    margin-bottom: 0.3rem;
}
```

**Testing:**
1. Buka `/characters` — 3 card karakter tampil
2. Avatar visual berbeda per karakter (warna gradien + emoji)
3. Hover card — animasi lift + shadow
4. Klik "Chat with [nama]" → navigasi ke `/characters/{id}`

---

### Task 13: CharacterChat.razor + CSS

**Apa yang dibuat:** Halaman interaksi dengan satu karakter. User memilih mood via MoodPicker, klik Kirim, dan karakter merespons dengan teks yang sesuai (table-driven lookup). Chat history disimpan dalam bubble chat.

**Kenapa dibuat:** Ini adalah fitur utama Character Companion — user "berbicara" dengan karakter melalui mood. Setiap kombinasi character + mood menghasilkan respons berbeda dari `characterResponses.json`. History chat memberi pengalaman seperti chat sungguhan.

**File:**
- `Components/Pages/CharacterChat.razor`
- `Components/Pages/CharacterChat.razor.css`

**Inject:** `ICharacterService`, `NavigationManager`
**Shared Components:** `MoodPicker`, `EmptyState`, `AlertMessage`

**Skeleton kode:**

```razor
@page "/characters/{Id}"
@rendermode InteractiveServer

@inject ICharacterService CharacterService
@inject NavigationManager Navigation

<PageTitle>@(_character?.Name ?? "Character Chat") — myKisah</PageTitle>

<div class="container-fluid">
    @if (_loading)
    {
        <p class="text-muted">Loading character...</p>
    }
    else if (_character == null)
    {
        <AlertMessage Message="Karakter tidak ditemukan." IsError="true" Show="true" />
    }
    else
    {
        <div class="chat-layout">
            @* Panel Kiri: Info Karakter *@
            <div class="character-panel">
                <button class="btn btn-outline-secondary btn-sm mb-3" @onclick="GoBack">
                    ← Kembali
                </button>
                <div class="text-center mb-3">
                    <div class="character-avatar-lg mb-2"
                         style="background: @GetGradient(_character);">
                        <span>@GetEmoji(_character)</span>
                    </div>
                    <h4>@_character.Name</h4>
                    <p class="text-muted small">@_character.Description</p>
                </div>
            </div>

            @* Panel Kanan: Chat Area *@
            <div class="chat-area">
                <AlertMessage Message="_error" IsError="true" Show="_hasError" />

                @* Chat History *@
                <div class="chat-messages" id="chatScroll">
                    @if (!_history.Any())
                    {
                        <EmptyState Message="Pilih mood untuk memulai percakapan dengan @_character.Name" />
                    }
                    else
                    {
                        @foreach (var entry in _history)
                        {
                            <div class="chat-bubble chat-bubble-user">
                                <small class="bubble-label">Kamu — @GetMoodEmoji(entry.Mood) @entry.Mood</small>
                                <div class="bubble-content">@GetMoodEmoji(entry.Mood) @entry.Mood</div>
                            </div>
                            <div class="chat-bubble chat-bubble-character">
                                <small class="bubble-label">@_character.Name</small>
                                <div class="bubble-content">@entry.Response</div>
                            </div>
                            <small class="bubble-time">@entry.Time.ToString("HH:mm")</small>
                        }
                    }
                </div>

                @* Input Area *@
                <div class="chat-input">
                    <div class="d-flex align-items-center gap-3 flex-wrap">
                        <MoodPicker @bind-SelectedMood="_selectedMood"
                                    Disabled="_sending" />
                        <button class="btn btn-primary"
                                @onclick="SendMood"
                                disabled="@_sending">
                            @(_sending ? "..." : "Kirim")
                        </button>
                    </div>
                </div>
            </div>
        </div>
    }
</div>

@code {
    [Parameter]
    public string Id { get; set; } = default!;

    private Character? _character;
    private MoodType _selectedMood = MoodType.Happy;
    private List<ChatEntry> _history = new();
    private bool _loading;
    private bool _sending;
    private string _error = "";
    private bool _hasError;

    private record ChatEntry(MoodType Mood, string Response, DateTime Time);

    protected override void OnInitialized()
    {
        // TODO: Load karakter: _character = CharacterService?
        //   Gunakan GetAllCharacters().FirstOrDefault(c => c.Id == Id)
        // TODO: Set _loading = false
    }

    private async Task SendMood()
    {
        // TODO: Reset error, set _sending = true
        // TODO: Try:
        //   var response = CharacterService.GenerateResponse(Id, _selectedMood)
        //   Tambah ke _history: new ChatEntry(_selectedMood, response, DateTime.Now)
        // TODO: Catch: _hasError = true, _error = ex.Message
        // TODO: Finally: _sending = false
        // TODO: Scroll chat ke bawah (pakai JS interop — AFTER_FIRST_RENDER)
    }

    private void GoBack()
    {
        // TODO: Navigation.NavigateTo("/characters")
    }

    private static string GetMoodEmoji(MoodType mood) => mood switch
    {
        // TODO: Happy → "😊", Sad → "😢", Angry → "😠", Anxious → "😰", Calm → "😌"
        _ => "😊"
    };

    private static string GetEmoji(Character c) => c.Name switch
    {
        // TODO: "Kira" → "💫", "Luna" → "🌙", "Ren" → "🎧", default → "🌟"
        _ => "🌟"
    };

    private static string GetGradient(Character c) => c.Name switch
    {
        // TODO: "Kira" → ungu, "Luna" → biru, "Ren" → hijau
        _ => "linear-gradient(135deg, #6366f1, #818cf8)"
    };
}
```

**Skeleton CSS — CharacterChat.razor.css:**

```css
.chat-layout {
    display: flex;
    gap: 1.5rem;
    align-items: flex-start;
}

.character-panel {
    width: 240px;
    flex-shrink: 0;
    padding: 1rem;
    border: 1px solid #dee2e6;
    border-radius: 8px;
    background: #f8f9fa;
    position: sticky;
    top: 5rem;
}

.character-avatar-lg {
    width: 100px;
    height: 100px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    margin: 0 auto;
}

.character-avatar-lg span {
    font-size: 2.5rem;
}

.chat-area {
    flex: 1;
    display: flex;
    flex-direction: column;
    min-height: 60vh;
}

.chat-messages {
    flex: 1;
    overflow-y: auto;
    padding: 1rem;
    border: 1px solid #dee2e6;
    border-radius: 8px;
    margin-bottom: 1rem;
    max-height: 55vh;
    background: #fafbfc;
}

.chat-bubble {
    margin-bottom: 0.5rem;
    padding: 0.6rem 1rem;
    border-radius: 12px;
    max-width: 75%;
}

.chat-bubble-user {
    background: #e7f1ff;
    margin-left: auto;
}

.chat-bubble-character {
    background: #f0fdf4;
    margin-right: auto;
}

.bubble-label {
    display: block;
    font-size: 0.7rem;
    color: #64748b;
    margin-bottom: 0.15rem;
}

.bubble-content {
    font-size: 0.95rem;
}

.bubble-time {
    display: block;
    font-size: 0.65rem;
    color: #94a3b8;
    text-align: center;
    margin-bottom: 0.75rem;
}

.chat-input {
    padding: 1rem;
    border: 1px solid #dee2e6;
    border-radius: 8px;
    background: white;
}

@media (max-width: 768px) {
    .chat-layout {
        flex-direction: column;
    }
    .character-panel {
        width: 100%;
        position: static;
    }
}
```

**Testing:**
1. Dari CharacterList → klik karakter → `/characters/{id}`
2. Panel kiri: avatar besar, nama, deskripsi
3. EmptyState: "Pilih mood untuk memulai percakapan dengan {nama}"
4. Pilih mood di MoodPicker → klik Kirim → respons karakter muncul
5. Bubble chat: user di kanan (biru), karakter di kiri (hijau)
6. Kirim beberapa mood berbeda → history menumpuk
7. Klik "← Kembali" → balik ke CharacterList

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
        // TODO: Update state
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
Navigation.NavigateTo($"/characters/{character.Id}");
Navigation.NavigateTo("/characters");
```

### Bind + Event

```razor
<MoodPicker @bind-SelectedMood="_mood" Disabled="_sending" />
<button @onclick="SendMood" disabled="@_sending">Kirim</button>
```

### Styling

| Aturan | Detail |
|---|---|
| Container | `<div class="container-fluid">` |
| Grid 3 kolom | `<div class="col-md-4 col-sm-6">` |
| Card | `<div class="card"><div class="card-body">` |
| Layout side-by-side | Flex container dengan dua child |
| Responsive | `@media (max-width: 768px)` untuk stack vertikal |
| CSS | Scoped `.razor.css` per komponen |

---

## Komponen Shared yang Dipakai

| Komponen | Cara Pakai |
|---|---|
| `MoodPicker` | `<MoodPicker @bind-SelectedMood="_mood" Disabled="_sending" />` |
| `EmptyState` | `<EmptyState Message="Pilih mood..." />` |
| `AlertMessage` | `<AlertMessage Message="_error" IsError="true" Show="_hasError" />` |

---

## Checklist Selesai

- [ ] `/characters` — 3 card karakter tampil dengan avatar visual (gradien + emoji)
- [ ] Avatar berbeda per karakter: Kira 💫 ungu, Luna 🌙 biru, Ren 🎧 hijau
- [ ] Hover card — animasi lift
- [ ] Klik "Chat" → navigasi ke `/characters/{id}`
- [ ] CharacterChat — panel kiri tampil info karakter
- [ ] EmptyState tampil saat belum ada history
- [ ] Pilih mood → Kirim → respons muncul di bubble chat
- [ ] Bubble user (kanan, biru) + bubble karakter (kiri, hijau)
- [ ] History bertambah setiap kirim
- [ ] Mood berbeda menghasilkan respons berbeda (table-driven)
- [ ] "← Kembali" → balik ke CharacterList
- [ ] Responsive: mobile layout stacked vertikal
