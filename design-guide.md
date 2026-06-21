# myKisah — Design Guide

> **Warm Minimalist Design System** — panduan style untuk semua anggota tim.

---

## 1. Design Philosophy

| Prinsip | Makna |
|---|---|
| **Warm** | Warna earthy, tone encouraging — bukan cold corporate |
| **Clean** | Minimalis, whitespace lega, tidak crowded |
| **Consistent** | Semua warna/spacing/radius pakai CSS variables — tidak hardcode |

**Tone:** Friendly, encouraging, personal. Seperti jurnal pribadi — bukan aplikasi enterprise.

---

## 2. Logo & Branding

**Placeholder** (sampai logo resmi ada):

```
🌱 myKisah
```

**Wordmark spec:**
- Font weight: 800 (extrabold)
- Color: white (navbar) / `var(--color-text)` (halaman)
- No italic, no shadow

**Tagline:**
> Build habits, write your journey, grow together.

---

## 3. CSS Variables — Master Tokens

Copy-paste `:root` block ini ke `wwwroot/app.css`:

```css
:root {
    /* ═══ Primary — Emerald Green ═══ */
    --color-primary: #059669;
    --color-primary-dark: #047857;
    --color-primary-light: #ecfdf5;

    /* ═══ Neutral (Warm Slate) ═══ */
    --color-bg: #f8fafc;
    --color-surface: #ffffff;
    --color-text: #1e293b;
    --color-text-muted: #64748b;
    --color-text-subtle: #94a3b8;
    --color-border: #e2e8f0;
    --color-border-light: #f1f5f9;

    /* ═══ Semantic ═══ */
    --color-success: #16a34a;
    --color-success-bg: #f0fdf4;
    --color-danger: #dc2626;
    --color-danger-bg: #fef2f2;
    --color-warning: #d97706;
    --color-warning-bg: #fffbeb;
    --color-info: #2563eb;
    --color-info-bg: #eff6ff;

    /* ═══ Journal State ═══ */
    --color-state-draft: #d97706;
    --color-state-draft-bg: #fffbeb;
    --color-state-submitted: #2563eb;
    --color-state-submitted-bg: #eff6ff;
    --color-state-saved: #16a34a;
    --color-state-saved-bg: #f0fdf4;
    --color-state-rejected: #dc2626;
    --color-state-rejected-bg: #fef2f2;

    /* ═══ Navbar (Dark) ═══ */
    --color-navbar-bg: #0f172a;
    --color-navbar-text: #e2e8f0;

    /* ═══ Typography ═══ */
    --font-family: -apple-system, BlinkMacSystemFont, Roboto, 'Helvetica Neue', Arial, sans-serif;
    --font-size-xs: 0.75rem;
    --font-size-sm: 0.875rem;
    --font-size-base: 1rem;
    --font-size-lg: 1.125rem;
    --font-size-xl: 1.25rem;
    --font-size-2xl: 1.5rem;
    --font-size-3xl: 1.875rem;
    --font-size-4xl: 2.5rem;
    --font-weight-normal: 400;
    --font-weight-medium: 500;
    --font-weight-semibold: 600;
    --font-weight-bold: 700;
    --font-weight-extrabold: 800;
    --line-height-base: 1.6;

    /* ═══ Spacing (4px grid) ═══ */
    --space-1: 0.25rem;
    --space-2: 0.5rem;
    --space-3: 0.75rem;
    --space-4: 1rem;
    --space-6: 1.5rem;
    --space-8: 2rem;
    --space-12: 3rem;
    --container-max: 1200px;

    /* ═══ Border Radius ═══ */
    --radius-sm: 4px;
    --radius-md: 8px;
    --radius-lg: 12px;
    --radius-full: 9999px;

    /* ═══ Shadows ═══ */
    --shadow-sm: 0 1px 2px rgba(0,0,0,0.05);
    --shadow-md: 0 4px 12px rgba(0,0,0,0.07);
    --shadow-lg: 0 8px 24px rgba(0,0,0,0.09);
}
```

---

## 4. Color System

### Primary Palette (Emerald Green)

| Token | Hex | Penggunaan |
|---|---|---|
| `--color-primary` | `#059669` | Tombol utama, link, chip aktif, focus ring |
| `--color-primary-dark` | `#047857` | Hover state, active state |
| `--color-primary-light` | `#ecfdf5` | Background subtle, selected state |

### Neutral Palette (Slate)

| Token | Hex | Penggunaan |
|---|---|---|
| `--color-text` | `#1e293b` | Heading, body text, label |
| `--color-text-muted` | `#64748b` | Secondary text, placeholder |
| `--color-text-subtle` | `#94a3b8` | Caption, timestamp, icon |
| `--color-border` | `#e2e8f0` | Card border, input border, divider |
| `--color-bg` | `#f8fafc` | Page background |
| `--color-surface` | `#ffffff` | Card, form, modal |

### Semantic

| Token | Hex | Penggunaan |
|---|---|---|
| `--color-success` / `--color-success-bg` | `#16a34a` / `#f0fdf4` | Alert sukses, Saved badge |
| `--color-danger` / `--color-danger-bg` | `#dc2626` / `#fef2f2` | Alert error, Delete button, Rejected badge |
| `--color-warning` / `--color-warning-bg` | `#d97706` / `#fffbeb` | Draft badge |
| `--color-info` / `--color-info-bg` | `#2563eb` / `#eff6ff` | Submitted badge, chat bubble user |

### Journal State

| State | Text Color | Background |
|---|---|---|
| Draft | `--color-state-draft` `#d97706` | `--color-state-draft-bg` `#fffbeb` |
| Submitted | `--color-state-submitted` `#2563eb` | `--color-state-submitted-bg` `#eff6ff` |
| Saved | `--color-state-saved` `#16a34a` | `--color-state-saved-bg` `#f0fdf4` |
| Rejected | `--color-state-rejected` `#dc2626` | `--color-state-rejected-bg` `#fef2f2` |

---

## 5. Typography

### Font Stack

```css
font-family: -apple-system, BlinkMacSystemFont, Roboto, 'Helvetica Neue', Arial, sans-serif;
```

| Platform | Font |
|---|---|
| macOS / iOS | San Francisco (-apple-system) |
| Chrome OS | BlinkMacSystemFont |
| **Android / Chrome / Web** | **Roboto** |
| Linux | Helvetica Neue → Arial |
| Fallback | sans-serif |

### Scale

| Token | Size | Penggunaan |
|---|---|---|
| `--font-size-xs` | 12px | Caption, timestamp, badge |
| `--font-size-sm` | 14px | Secondary text, table |
| `--font-size-base` | 16px | Body text, input, button |
| `--font-size-lg` | 18px | Lead paragraph |
| `--font-size-xl` | 20px | Card title |
| `--font-size-2xl` | 24px | Page heading |
| `--font-size-3xl` | 30px | Hero heading |
| `--font-size-4xl` | 40px | Brand, login title |

### Weight

| Token | Value | Penggunaan |
|---|---|---|
| `--font-weight-bold` / `--font-weight-extrabold` | 700 / 800 | Heading, brand |
| `--font-weight-semibold` | 600 | Label, active nav, badge |
| `--font-weight-medium` | 500 | Button |
| `--font-weight-normal` | 400 | Body, input |

### Line Height

- Body: `--line-height-base` (1.6) — readable paragraph
- Heading: default (1.2–1.3) — compact title
- Journal content (detail): 1.8 — long-form reading

---

## 6. Spacing & Layout

**Base grid: 4px** (semua spacing kelipatan 4)

| Token | Value | Penggunaan |
|---|---|---|
| `--space-1` | 4px | Icon gap, inline spacing |
| `--space-2` | 8px | Small gap, chip gap |
| `--space-3` | 12px | Form group gap |
| `--space-4` | 16px | Card padding, section gap |
| `--space-6` | 24px | Section margin, layout gap |
| `--space-8` | 32px | Page padding, large gap |
| `--space-12` | 48px | Hero spacing |

**Container:** `max-width: var(--container-max)` (1200px), centered.

---

## 7. Border Radius & Shadows

| Token | Value | Penggunaan |
|---|---|---|
| `--radius-sm` | 4px | Input, small element |
| `--radius-md` | 8px | Card, button, modal, form group |
| `--radius-lg` | 12px | Chat bubble, large card |
| `--radius-full` | 9999px | Pill badge, chip, avatar circle |

| Token | Value | Penggunaan |
|---|---|---|
| `--shadow-sm` | `0 1px 2px rgba(0,0,0,.05)` | Card default |
| `--shadow-md` | `0 4px 12px rgba(0,0,0,.07)` | Card hover |
| `--shadow-lg` | `0 8px 24px rgba(0,0,0,.09)` | Modal, dropdown |

---

## 8. Component Specifications

### 8.1 Buttons

```css
/* Primary — aksi utama */
.btn-primary {
    background: var(--color-primary);
    border: none;
    border-radius: var(--radius-md);
    padding: 0.5rem 1.25rem;
    font-weight: var(--font-weight-medium);
}
.btn-primary:hover { background: var(--color-primary-dark); }
.btn-primary:disabled { opacity: 0.5; }

/* Outline — aksi sekunder */
.btn-outline-secondary {
    border: 1px solid var(--color-border);
    border-radius: var(--radius-md);
}

/* Danger — hapus */
.btn-danger {
    background: var(--color-danger);
    border: none;
    border-radius: var(--radius-md);
}

/* Small */
.btn-sm { padding: 0.25rem 0.75rem; font-size: var(--font-size-sm); }
```

### 8.2 Cards

```css
.card {
    background: var(--color-surface);
    border: 1px solid var(--color-border);
    border-radius: var(--radius-md);
    box-shadow: var(--shadow-sm);
    transition: transform 0.15s, box-shadow 0.15s;
}
.card:hover {
    transform: translateY(-2px);
    box-shadow: var(--shadow-md);
}
.card-body { padding: var(--space-6); }
```

### 8.3 Forms

```css
.form-control {
    border: 1px solid var(--color-border);
    border-radius: var(--radius-md);
    padding: 0.5rem 0.75rem;
    font-family: var(--font-family);
    font-size: var(--font-size-base);
    color: var(--color-text);
}
.form-control:focus {
    border-color: var(--color-primary);
    box-shadow: 0 0 0 3px var(--color-primary-light);
}
.form-label {
    font-weight: var(--font-weight-semibold);
    color: var(--color-text);
    margin-bottom: var(--space-1);
}
textarea.form-control {
    line-height: var(--line-height-base);
    resize: vertical;
}
```

### 8.4 StateBadge

```css
.state-badge {
    display: inline-block;
    padding: 0.15rem 0.5rem;
    border-radius: var(--radius-full);
    font-size: var(--font-size-xs);
    font-weight: var(--font-weight-semibold);
}
.state-draft     { color: var(--color-state-draft);     background: var(--color-state-draft-bg); }
.state-submitted { color: var(--color-state-submitted); background: var(--color-state-submitted-bg); }
.state-saved     { color: var(--color-state-saved);     background: var(--color-state-saved-bg); }
.state-rejected  { color: var(--color-state-rejected);  background: var(--color-state-rejected-bg); }
```

### 8.5 MoodPicker Chips

```css
.mood-chip {
    padding: 0.5rem 1rem;
    border-radius: var(--radius-full);
    border: 2px solid var(--color-border);
    background: var(--color-surface);
    cursor: pointer;
    transition: all 0.2s;
}
.mood-chip:hover:not(:disabled) {
    border-color: var(--color-primary);
    background: var(--color-primary-light);
}
.mood-chip.active {
    background: var(--color-primary);
    color: white;
    border-color: var(--color-primary);
}
.mood-chip:disabled { opacity: 0.5; cursor: not-allowed; }
```

### 8.6 Navbar

```css
.navbar {
    background: var(--color-navbar-bg);
    padding: 0.5rem 0;
}
.navbar-brand {
    font-weight: var(--font-weight-extrabold);
    font-size: 1.2rem;
    color: white;
}
.nav-link {
    color: var(--color-navbar-text);
}
.nav-link.active {
    font-weight: var(--font-weight-semibold);
    border-bottom: 2px solid var(--color-primary);
}
```

### 8.7 Chat Bubbles

```css
.chat-bubble {
    border-radius: var(--radius-lg);
    padding: 0.6rem 1rem;
    max-width: 75%;
}
.chat-bubble-user {
    background: var(--color-info-bg);
    margin-left: auto;
}
.chat-bubble-character {
    background: var(--color-success-bg);
    margin-right: auto;
}
```

### 8.8 EmptyState

```css
.empty-state {
    text-align: center;
    padding: var(--space-12) var(--space-4);
    color: var(--color-text-muted);
}
```

### 8.9 AlertMessage

Menggunakan class Bootstrap `alert`:
- Error: `alert-danger` → background `var(--color-danger-bg)`
- Success: `alert-success` → background `var(--color-success-bg)`

---

## 9. Responsive Breakpoints

| Breakpoint | Min Width | Penggunaan |
|---|---|---|
| Mobile | < 576px | Single column, stack elements |
| Tablet | 576px – 768px | 2 column grid, compact nav |
| Desktop | ≥ 768px | 3 column grid, full nav, side panel |

**Navbar:** Collapse di bawah 768px (`navbar-expand-md`).

**Grid card:**
```css
.col-md-4  /* 3 kolom desktop */
.col-sm-6  /* 2 kolom tablet */
.col-12    /* 1 kolom mobile */
```

---

## 10. Do's & Don'ts

### Always use CSS Variables ✅

```css
/* ✅ DO */
color: var(--color-text);
border: 1px solid var(--color-border);
background: var(--color-primary);
border-radius: var(--radius-md);
font-family: var(--font-family);
padding: var(--space-4);
box-shadow: var(--shadow-sm);
```

```css
/* ❌ DON'T — never hardcode */
color: #333;
border: 1px solid #dee2e6;
background: #0d6efd;
border-radius: 6px;
font-family: 'Segoe UI';
padding: 16px;
```

### Radius Consistency ✅

```css
/* ✅ DO */
border-radius: var(--radius-md);   /* 8px — card, button, input */
border-radius: var(--radius-full); /* pill — badge, chip, avatar */
border-radius: var(--radius-lg);   /* 12px — chat bubble */

/* ❌ DON'T — random radius */
border-radius: 5px;
border-radius: 7px;
border-radius: 9px;
```

### Font ✅

```css
/* ✅ DO */
font-family: var(--font-family);

/* ❌ DON'T */
font-family: 'Segoe UI', Tahoma;
font-family: Arial;
```

---

## 11. Checklist — Files to Update

File yang perlu diupdate agar comply dengan design system ini:

| # | File | Status |
|---|---|---|
| 1 | `wwwroot/app.css` — tambah `:root` tokens + update font-family | ✅ Done |
| 2 | `wwwroot/css/shared.css` — StateBadge + MoodPicker → CSS variables | ✅ Done |
| 3 | `Layout/NavMenu.razor.css` — navbar bg → `var(--color-navbar-bg)` | ✅ Done |
| 4 | `Components/Pages/Login.razor.css` — color text → variables | ✅ Done |
| 5 | `Components/Pages/JournalList.razor.css` — hover → `var(--color-primary)` | ✅ Done |
| 6 | `Components/Pages/CharacterChat.razor.css` — bubbles → semantic colors | ✅ Done |
| 7 | `Components/Pages/CharacterList.razor.css` — (OK, no hardcoded colors) | N/A |
| 8 | `Components/Pages/JournalCreate.razor.css` — (OK, only sizing) | N/A |
| 9 | `Components/Pages/JournalDetail.razor.css` — (OK, only sizing) | N/A |
| 10 | `Components/Pages/Debug.razor.css` — (keep, debug page) | N/A |

---

> *All team members: use `var(--token)` for any new CSS. Never hardcode colors, radii, or spacing. Refer to this guide.*
