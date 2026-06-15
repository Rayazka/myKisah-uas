# Guide: Jojo (Josefhint) — Shared Functional Components

**Baca panduan ini dari atas ke bawah. Kerjakan task sesuai urutan.**

Jumlah task: **3** | Domain: **Shared Functional**

---

## Domain

Kamu mengerjakan komponen fungsional kecil yang dipakai ulang di seluruh aplikasi: **StateBadge** (badge warna untuk state journal), **AlertMessage** (notifikasi sukses/error), dan **EmptyState** (placeholder saat tidak ada data). Ketiganya adalah **pure component** — tidak inject service apapun, hanya menerima parameter. Komponen ini adalah lapisan UI dari pola defensive programming dan error handling yang sudah kamu bangun di backend (`ValidationHelper`, `ErrorHandlingMiddleware`).

---

## Prasyarat Sebelum Memulai

- [ ] Azka sudah menyelesaikan FASE 1 (task 1–6) — tanya Azka jika belum
- [ ] Pull branch `main` terbaru
- [ ] `dotnet build` — 0 error
- [ ] Baca section **9. Referensi Cepat: Shared Components** di `fr-plan.md`
- [ ] Tidak ada dependensi ke komponen lain — ketiga komponen adalah leaf components

---

## Pola Komponen Razor

Komponen yang kamu buat adalah **pure Razor component** — tidak inject service, tidak navigasi, tidak cek login. Hanya:

```razor
@* NamaComponent.razor *@

@* Conditional render atau simple HTML *@

@code {
    [Parameter] public SomeType Property { get; set; }
}
```

---

## Daftar Tugas

| # | Task | File |
|---|---|---|
| 16 | `StateBadge` | `Components/Shared/StateBadge.razor` |
| 17 | `AlertMessage` | `Components/Shared/AlertMessage.razor` |
| 18 | `EmptyState` | `Components/Shared/EmptyState.razor` |

---

### Task 16: StateBadge.razor

**Apa yang dibuat:** Badge kecil berbentuk pill yang menampilkan state journal dengan warna berbeda: Draft (kuning), Submitted (biru), Saved (hijau), Rejected (merah).

**Kenapa dibuat:** Setiap halaman yang menampilkan journal (JournalList, JournalDetail) perlu menampilkan state dengan jelas. Tanpa komponen ini, setiap halaman akan menulis ulang logika warna yang sama — melanggar prinsip Code Reuse. State machine sudah menentukan transisi, StateBadge menampilkan hasilnya secara visual.

**File:** `Components/Shared/StateBadge.razor`

**Skeleton kode:**

```razor
@namespace myKisah.Components.Shared

<span class="state-badge @GetBadgeClass()">
    @State.ToString()
</span>

@code {
    [Parameter]
    public JournalState State { get; set; }

    private string GetBadgeClass()
    {
        // TODO: Return class CSS berdasarkan State:
        //   JournalState.Draft     → "state-draft"
        //   JournalState.Submitted → "state-submitted"
        //   JournalState.Saved     → "state-saved"
        //   JournalState.Rejected  → "state-rejected"
        //   default → "state-draft"
        return "state-draft";
    }
}
```

**CSS — tambahkan di app.css ATAU buat StateBadge.razor.css:**

> Karena dipakai banyak halaman, lebih baik styling ditaruh di `wwwroot/app.css` agar konsisten di mana-mana.

```css
/* State Badge — tambahkan ke wwwroot/app.css */
.state-badge {
    padding: 0.15rem 0.5rem;
    border-radius: 10px;
    font-size: 0.75rem;
    font-weight: 600;
    display: inline-block;
}

.state-draft     { background: #fff3cd; color: #856404; }
.state-submitted { background: #cce5ff; color: #004085; }
.state-saved     { background: #d4edda; color: #155724; }
.state-rejected  { background: #f8d7da; color: #721c24; }
```

**Testing:**
1. Panggil `<StateBadge State="JournalState.Draft" />` → badge kuning "Draft"
2. Ganti state: Submitted → biru, Saved → hijau, Rejected → merah
3. Dipanggil di loop: setiap journal di list tampil badge sesuai state

---

### Task 17: AlertMessage.razor

**Apa yang dibuat:** Komponen notifikasi — menampilkan pesan sukses (hijau) atau error (merah). Dikontrol oleh parameter `Show` — tidak render apa-apa jika `Show=false`.

**Kenapa dibuat:** Setiap halaman perlu menampilkan feedback ke user setelah aksi (gagal login, sukses submit, error hapus, dll.). Tanpa komponen ini, setiap halaman menulis ulang div alert yang sama. Ini melanjutkan pola error handling yang sudah kamu bangun di backend (`ErrorHandlingMiddleware` → response JSON error) — AlertMessage adalah versi UI-nya.

**File:** `Components/Shared/AlertMessage.razor`

**Skeleton kode:**

```razor
@namespace myKisah.Components.Shared

@if (Show && !string.IsNullOrEmpty(Message))
{
    <div class="alert @(IsError ? "alert-danger" : "alert-success") alert-dismissible fade show"
         role="alert">
        @if (IsError)
        {
            <strong>Error:</strong>
        }
        @Message
        <button type="button" class="btn-close" @onclick="Dismiss"
                aria-label="Close"></button>
    </div>
}

@code {
    [Parameter]
    public string Message { get; set; } = "";

    [Parameter]
    public bool IsError { get; set; }

    [Parameter]
    public bool Show { get; set; }

    private void Dismiss()
    {
        // TODO: Reset Show ke false
        // Note: Karena Show adalah parameter, dismiss hanya bekerja
        //   jika parent component meng-handle event ini.
        //   Alternatif: tambahkan EventCallback untuk dismiss,
        //   atau biarkan user menutup secara manual.
        //   Untuk saat ini, tombol close hanya menyembunyikan alert secara visual.
    }
}
```

**Testing:**
1. Panggil `<AlertMessage Message="Sukses!" IsError="false" Show="true" />` → alert hijau
2. Panggil `<AlertMessage Message="Gagal!" IsError="true" Show="true" />` → alert merah dengan "Error:" prefix
3. `Show="false"` → tidak render apa-apa
4. `Message=""` → tidak render apa-apa (meskipun Show=true)
5. Klik ✕ → alert hilang

---

### Task 18: EmptyState.razor

**Apa yang dibuat:** Placeholder yang ditampilkan saat tidak ada data. Menampilkan ikon informasi, pesan, dan tombol aksi opsional.

**Kenapa dibuat:** Setiap halaman yang menampilkan data (JournalList, CharacterChat) perlu empty state agar user tidak bingung saat halaman kosong — bukan bug, memang belum ada data. Empty state memberi konteks ("Belum ada journal") dan ajakan bertindak ("Tulis Journal Pertamamu"). Ini adalah defensive UI — melanjutkan pola defensive programming backend.

**File:** `Components/Shared/EmptyState.razor`

**Skeleton kode:**

```razor
@namespace myKisah.Components.Shared

<div class="empty-state">
    <div class="empty-state-icon">
        <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48"
             fill="currentColor" class="bi bi-info-circle" viewBox="0 0 16 16">
            <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z"/>
            <path d="m8.93 6.588-2.29.287-.082.38.45.083c.294.07.352.176.288.469l-.738 3.468c-.194.897.105 1.319.808 1.319.545 0 1.178-.252 1.465-.598l.088-.416c-.2.176-.492.246-.686.246-.275 0-.375-.193-.304-.533L8.93 6.588zM9 4.5a1 1 0 1 1-2 0 1 1 0 0 1 2 0z"/>
        </svg>
    </div>
    <p class="empty-state-message">@Message</p>
    @if (!string.IsNullOrEmpty(ActionLabel))
    {
        <button class="btn btn-primary" @onclick="OnAction">
            @ActionLabel
        </button>
    }
</div>

@code {
    [Parameter]
    public string Message { get; set; } = "Tidak ada data.";

    [Parameter]
    public string? ActionLabel { get; set; }

    [Parameter]
    public EventCallback OnAction { get; set; }
}
```

**CSS — tambahkan di app.css:**

```css
/* Empty State — tambahkan ke wwwroot/app.css */
.empty-state {
    text-align: center;
    padding: 3rem 1rem;
}

.empty-state-icon {
    color: #94a3b8;
    margin-bottom: 1rem;
}

.empty-state-message {
    color: #64748b;
    font-size: 1.05rem;
    margin-bottom: 1rem;
}
```

**Testing:**
1. Panggil `<EmptyState Message="Belum ada journal." />` → ikon + teks tampil, tanpa tombol
2. Tambah `ActionLabel="Tulis Journal" OnAction="OnAdd"` → tombol muncul di bawah
3. Klik tombol → handler `OnAdd` terpanggil

---

## Konvensi Kode — Pure Components

### Struktur Komponen

```razor
@namespace myKisah.Components.Shared

@if (/* kondisi render */)
{
    <div class="...">
        @* HTML template *@
    </div>
}

@code {
    [Parameter] public SomeType Property { get; set; }
}
```

### Conditional Render

Gunakan `@if (condition)` untuk komponen seperti AlertMessage (jangan render jika Show=false) dan EmptyState (jangan render tombol jika ActionLabel null).

### CSS

Untuk komponen kecil seperti StateBadge dan EmptyState, styling bisa ditaruh di `wwwroot/app.css` (karena dipakai banyak halaman dan tidak spesifik ke satu komponen). Untuk komponen yang lebih kompleks, gunakan scoped `.razor.css`.

### Testing Komponen

Cara termudah: buat temporary test di Debug.razor:
```razor
<StateBadge State="JournalState.Draft" />
<AlertMessage Message="Test error" IsError="true" Show="true" />
<EmptyState Message="Kosong" ActionLabel="Isi" OnAction="() => { }" />
```

---

## Komponen Shared yang Dipakai

Kamu **tidak memakai** komponen shared lain — StateBadge, AlertMessage, dan EmptyState adalah komponen yang kamu buat sendiri. Komponen ini akan dipakai oleh anggota lain (Farel, Toni, Rayazka) di halaman mereka.

---

## Checklist Selesai

- [ ] StateBadge — render pill badge, 4 warna sesuai state
- [ ] StateBadge — warna Draft=kuning, Submitted=biru, Saved=hijau, Rejected=merah
- [ ] StateBadge — warna persis sama dengan referensi `Home.razor.css` line 97–107
- [ ] AlertMessage — `Show=true` + `IsError=true` → alert merah dengan prefix "Error:"
- [ ] AlertMessage — `Show=true` + `IsError=false` → alert hijau tanpa prefix
- [ ] AlertMessage — `Show=false` → tidak render apa-apa
- [ ] AlertMessage — `Message=""` → tidak render meskipun Show=true
- [ ] AlertMessage — tombol ✕ bisa menyembunyikan alert
- [ ] EmptyState — ikon info circle + pesan teks tampil
- [ ] EmptyState — tanpa `ActionLabel` → tidak ada tombol
- [ ] EmptyState — dengan `ActionLabel` + `OnAction` → tombol muncul, handler terpanggil saat klik
