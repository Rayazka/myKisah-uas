# Guide: Rafly — Shared Visual Components

**Baca panduan ini dari atas ke bawah. Kerjakan task sesuai urutan.**

Jumlah task: **2** | Domain: **Shared Visual**

---

## Domain

Kamu mengerjakan komponen visual yang dipakai ulang di seluruh aplikasi: **MoodPicker** (chip emoji untuk pilih mood — dipakai di JournalCreate, JournalList, CharacterChat) dan **ConfirmDialog** (modal konfirmasi — dipakai di JournalList, JournalDetail). Komponen ini adalah **pure component** — tidak inject service apapun, hanya menerima parameter.

---

## Prasyarat Sebelum Memulai

- [ ] Azka sudah menyelesaikan FASE 1 (task 1–6) — tanya Azka jika belum
- [ ] Pull branch `main` terbaru
- [ ] `dotnet build` — 0 error
- [ ] Baca section **9. Referensi Cepat: Shared Components** di `fr-plan.md`
- [ ] Tidak ada dependensi ke komponen lain — MoodPicker dan ConfirmDialog adalah leaf components

---

## Pola Komponen Razor

Komponen yang kamu buat adalah **pure Razor component** — tidak inject service, tidak navigasi, tidak cek login. Hanya:

```razor
@* NamaComponent.razor *@

@* HTML template *@

@code {
    [Parameter] public SomeType Property { get; set; }
    [Parameter] public EventCallback<SomeType> PropertyChanged { get; set; }
}
```

---

## Daftar Tugas

| # | Task | File |
|---|---|---|
| 14 | `MoodPicker` | `Components/Shared/MoodPicker.razor` + `.css` |
| 15 | `ConfirmDialog` | `Components/Shared/ConfirmDialog.razor` |

---

### Task 14: MoodPicker.razor + CSS

**Apa yang dibuat:** Komponen visual untuk memilih mood. 5 chip horizontal (Happy, Sad, Angry, Anxious, Calm) dengan emoji dan label. Chip yang aktif di-highlight warna biru.

**Kenapa dibuat:** Menggantikan dropdown `<select>` yang membosankan — user bisa melihat dan memilih mood secara visual dengan sekali klik. Dipakai di 3 halaman berbeda: JournalCreate, JournalList (filter), dan CharacterChat. Dibuat sekali, dipakai di mana-mana. Ini menerapkan teknik Code Reuse yang menjadi salah satu fokus proyek.

**File:**
- `Components/Shared/MoodPicker.razor`
- `Components/Shared/MoodPicker.razor.css`

**Skeleton kode:**

```razor
@namespace myKisah.Components.Shared

<div class="mood-picker">
    @foreach (var mood in _moods)
    {
        <button type="button"
                class="mood-chip @(SelectedMood == mood.Mood ? "active" : "")"
                disabled="@Disabled"
                @onclick="() => OnSelectMood(mood.Mood)"
                title="@mood.Label">
            <span class="mood-emoji">@mood.Emoji</span>
            <span class="mood-label">@mood.Label</span>
        </button>
    }
</div>

@code {
    [Parameter]
    public MoodType SelectedMood { get; set; } = MoodType.Happy;

    [Parameter]
    public EventCallback<MoodType> SelectedMoodChanged { get; set; }

    [Parameter]
    public bool Disabled { get; set; }

    private readonly List<(MoodType Mood, string Emoji, string Label)> _moods = new()
    {
        // TODO: Isi 5 mood dengan emoji yang sesuai
        // (MoodType.Happy,   "😊", "Happy"),
        // (MoodType.Sad,     "😢", "Sad"),
        // (MoodType.Angry,   "😠", "Angry"),
        // (MoodType.Anxious, "😰", "Anxious"),
        // (MoodType.Calm,    "😌", "Calm"),
    };

    private async Task OnSelectMood(MoodType mood)
    {
        // TODO: Jika Disabled → return (jangan proses)
        // TODO: Jika mood == SelectedMood → return (tidak ada perubahan)
        // TODO: Invoke SelectedMoodChanged.InvokeAsync(mood)
    }
}
```

**Skeleton CSS — MoodPicker.razor.css:**

```css
.mood-picker {
    display: flex;
    gap: 0.5rem;
    flex-wrap: wrap;
}

.mood-chip {
    padding: 0.5rem 1rem;
    border-radius: 20px;
    border: 2px solid #dee2e6;
    background: white;
    cursor: pointer;
    transition: all 0.2s ease;
    display: flex;
    align-items: center;
    gap: 0.4rem;
    font-size: 0.9rem;
}

.mood-chip:hover:not(:disabled) {
    border-color: #0d6efd;
    background: #f0f7ff;
}

.mood-chip.active {
    background: #0d6efd;
    color: white;
    border-color: #0d6efd;
}

.mood-chip:disabled {
    opacity: 0.5;
    cursor: not-allowed;
}

.mood-emoji {
    font-size: 1.2rem;
}
```

**Testing:**
1. Panggil `<MoodPicker @bind-SelectedMood="_mood" />` di halaman manapun
2. 5 chip tampil horizontal, Happy default aktif (biru)
3. Klik chip lain → chip baru aktif, chip lama kembali putih
4. `Disabled="true"` → semua chip abu-abu, tidak bisa diklik
5. Wrap ke baris baru di layar kecil

---

### Task 15: ConfirmDialog.razor

**Apa yang dibuat:** Modal konfirmasi dengan backdrop. Menampilkan judul, pesan, dan dua tombol: "Ya" (merah) dan "Batal" (outline). Mengembalikan `true`/`false` via `EventCallback<bool>`.

**Kenapa dibuat:** Setiap aksi destruktif (hapus journal, hapus user) harus dikonfirmasi user. Tanpa komponen ini, setiap halaman harus membuat modal sendiri — tidak konsisten. Dibuat sekali sebagai shared component, dipakai di JournalList dan JournalDetail.

**File:** `Components/Shared/ConfirmDialog.razor`

**Skeleton kode:**

```razor
@namespace myKisah.Components.Shared

@if (Show)
{
    <div class="modal-backdrop fade show" style="display: block;"></div>
    <div class="modal fade show d-block" tabindex="-1" role="dialog">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">@Title</h5>
                    <button type="button" class="btn-close"
                            @onclick="() => OnResult.InvokeAsync(false)"
                            aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <p>@Message</p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-outline-secondary"
                            @onclick="() => OnResult.InvokeAsync(false)">
                        Batal
                    </button>
                    <button type="button" class="btn btn-danger"
                            @onclick="() => OnResult.InvokeAsync(true)">
                        Ya
                    </button>
                </div>
            </div>
        </div>
    </div>
}

@code {
    [Parameter]
    public string Title { get; set; } = "Konfirmasi";

    [Parameter]
    public string Message { get; set; } = "Apakah Anda yakin?";

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public EventCallback<bool> OnResult { get; set; }
}
```

**Testing:**
1. Panggil `<ConfirmDialog Title="Hapus?" Message="Yakin?" Show="_show" OnResult="OnConfirm" />`
2. Set `_show = true` → modal muncul dengan backdrop gelap
3. Klik "Ya" → `OnResult(true)` terpanggil
4. Klik "Batal" atau ✕ → `OnResult(false)` terpanggil
5. Di handler, set `_show = false` untuk menyembunyikan modal

---

## Konvensi Kode — Pure Components

### Struktur Komponen

```razor
@namespace myKisah.Components.Shared

<div class="...">
    @* HTML template *@
</div>

@code {
    [Parameter] public SomeType Value { get; set; }
    [Parameter] public EventCallback<SomeType> ValueChanged { get; set; }
    [Parameter] public bool Disabled { get; set; }
}
```

### @bind Pattern (Two-way Binding)

Agar komponen bisa dipakai dengan `@bind-Property`, gunakan konvensi:
- Parameter: `SelectedMood`
- Callback: `SelectedMoodChanged` (harus persis: `{Nama}Changed`)

```csharp
[Parameter] public MoodType SelectedMood { get; set; }
[Parameter] public EventCallback<MoodType> SelectedMoodChanged { get; set; }
```

### CSS Scoped

Semua styling di `.razor.css` — tidak perlu `app.css`. Gunakan class yang deskriptif.

### Testing Komponen

Cara termudah: buat temporary test di Debug.razor:
```razor
<MoodPicker @bind-SelectedMood="_testMood" />
<p>Selected: @_testMood</p>
```

---

## Komponen Shared yang Dipakai

Kamu **tidak memakai** komponen shared lain — MoodPicker dan ConfirmDialog adalah komponen yang kamu buat sendiri. Komponen ini akan dipakai oleh anggota lain (Farel, Toni, Rayazka).

---

## Checklist Selesai

- [ ] MoodPicker — 5 chip tampil dengan emoji 😊😢😠😰😌
- [ ] Klik chip → chip aktif berubah warna biru, chip lain putih
- [ ] Chip yang sama diklik dua kali → tidak ada perubahan (idempoten)
- [ ] `Disabled="true"` → semua chip abu-abu, tidak responsif
- [ ] Responsive — wrap ke baris baru di layar < 400px
- [ ] `@bind-SelectedMood` berfungsi via `SelectedMoodChanged`
- [ ] ConfirmDialog — modal muncul dengan backdrop saat `Show=true`
- [ ] Tombol "Ya" → `OnResult.InvokeAsync(true)` terpanggil
- [ ] Tombol "Batal" / ✕ → `OnResult.InvokeAsync(false)` terpanggil
- [ ] Modal tidak muncul saat `Show=false`
