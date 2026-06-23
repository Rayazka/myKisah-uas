# myKisah — Style Guide (Warm Edition)

> **Design Philosophy:** Warm Minimalist — earthy tones, encouraging, personal.  
> Seperti jurnal pribadi — bukan aplikasi enterprise.

---

## 1. Branding

| Element | Spec |
|---------|------|
| Brand name | **myKisah** |
| Font | Plus Jakarta Sans (Google Fonts), weight 800 |
| Color | Emerald green gradient on dark bg, `var(--color-text)` on light bg |
| Tagline | *Build habits, write your journey, grow together.* |

---

## 2. Color Tokens (`:root` in `app.css`)

### Primary — Emerald Green
| Token | Hex | Usage |
|-------|-----|-------|
| `--color-primary` | `#059669` | Main buttons, links, active chips, focus ring |
| `--color-primary-dark` | `#047857` | Button hover/active |
| `--color-primary-light` | `#ecfdf5` | Subtle green bg, selected state |

### Neutral — Warm Beige/Cream
| Token | Hex | Usage |
|-------|-----|-------|
| `--color-bg` | `#faf8f4` | Page background |
| `--color-surface` | `#fffdfb` | Cards, forms, modals |
| `--color-text` | `#1c1917` | Headings, body text, labels |
| `--color-text-muted` | `#78716c` | Secondary text, placeholders |
| `--color-text-subtle` | `#a8a29e` | Captions, timestamps, icons |
| `--color-border` | `#e7ddd0` | Card borders, input borders, dividers |
| `--color-border-light` | `#f2ede3` | Light separators |

### Semantic
| Token | Hex | Usage |
|-------|-----|-------|
| `--color-success` | `#16a34a` | Success alerts, Saved badge |
| `--color-success-bg` | `#f0fdf4` | Success bg |
| `--color-danger` | `#dc2626` | Error alerts, Delete btn, Rejected badge |
| `--color-danger-bg` | `#fef2f2` | Error bg |
| `--color-warning` | `#d97706` | Draft badge |
| `--color-warning-bg` | `#fffbeb` | Warning bg |
| `--color-info` | `#2563eb` | Submitted badge, chat bubble user |
| `--color-info-bg` | `#eff6ff` | Info bg |

### Navbar — Dark Warm
| Token | Hex | Usage |
|-------|-----|-------|
| `--color-navbar-bg` | `#1c1917` | Navbar background (dark warm brown) |
| `--color-navbar-text` | `#e7e5e4` | Navbar text |

---

## 3. Typography

| Token | Value | Usage |
|-------|-------|-------|
| `--font-family` | `'Plus Jakarta Sans', ...` | All text |
| `--font-size-xs` | 12px | Captions, badges, timestamps |
| `--font-size-sm` | 14px | Secondary text, nav links |
| `--font-size-base` | 16px | Body, inputs, buttons |
| `--font-size-lg` | 18px | Lead paragraphs |
| `--font-size-xl` | 20px | Card titles |
| `--font-size-2xl` | 24px | Page headings |
| `--font-size-3xl` | 30px | Hero headings |
| `--font-size-4xl` | 40px | Brand, login |
| `--font-weight-normal` | 400 | Body |
| `--font-weight-medium` | 500 | Buttons |
| `--font-weight-semibold` | 600 | Labels, active nav, badges |
| `--font-weight-bold` | 700 | Headings |
| `--font-weight-extrabold` | 800 | Brand |

---

## 4. Spacing (4px Grid)

| Token | Value | Usage |
|-------|-------|-------|
| `--space-1` | 4px | Icon gap |
| `--space-2` | 8px | Chip gap |
| `--space-3` | 12px | Form group gap |
| `--space-4` | 16px | Card padding, section gap |
| `--space-6` | 24px | Section margin |
| `--space-8` | 32px | Page padding |
| `--space-12` | 48px | Hero spacing |
| `--container-max` | 1200px | Max content width |

---

## 5. Border Radius & Shadows

| Token | Value | Usage |
|-------|-------|-------|
| `--radius-sm` | 4px | Small elements |
| `--radius-md` | 8px | Cards, buttons, inputs |
| `--radius-lg` | 12px | Chat bubbles, large cards |
| `--radius-full` | 9999px | Pills, badges, avatars |
| `--shadow-sm` | `0 2px 4px rgba(0,0,0,.02)` | Card default |
| `--shadow-md` | `0 10px 20px -2px rgba(120,110,90,.05)` | Card hover |
| `--shadow-lg` | `0 20px 25px -5px rgba(0,0,0,.1)` | Modal, dropdown |

---

## 6. Components

### 6.1 Buttons
```css
.btn-primary {
    background: var(--color-primary);
    border-radius: var(--radius-md);
    box-shadow: 0 4px 12px rgba(5,150,105,0.25);
    transition: all 0.2s ease-in-out;
}
.btn-primary:hover {
    background: var(--color-primary-dark);
    transform: translateY(-1px);
}
.btn-outline-secondary { border-radius: var(--radius-md); }
.btn-danger { background: var(--color-danger); }
```

### 6.2 Cards
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
```

### 6.3 Forms
```css
.form-control {
    border: 1px solid var(--color-border);
    border-radius: var(--radius-md);
    color: var(--color-text);
}
.form-control:focus {
    border-color: var(--color-primary);
    box-shadow: 0 0 0 3px var(--color-primary-light);
}
```

### 6.4 StateBadge
4 state colors with soft backgrounds:
| State | Text | Background |
|-------|------|------------|
| Draft | `#d97706` (amber) | `rgba(217,119,6,0.1)` |
| Submitted | `#2563eb` (blue) | `rgba(37,99,235,0.1)` |
| Saved | `#16a34a` (green) | `rgba(22,163,74,0.1)` |
| Rejected | `#dc2626` (red) | `rgba(220,38,38,0.1)` |

### 6.5 MoodPicker
5 mood chips with rainbow gradients:
| Mood | Active Gradient |
|------|----------------|
| Happy | `#fbbf24 → #f59e0b` (gold) |
| Sad | `#60a5fa → #3b82f6` (blue) |
| Angry | `#f87171 → #ef4444` (red) |
| Anxious | `#c084fc → #a855f7` (purple) |
| Calm | `#2dd4bf → #0d9488` (teal) |

### 6.6 Navbar
- Dark warm brown (`#1c1917`) with frosted glass effect
- Brand: green gradient text
- Links: white with opacity, active state highlighted
- Border: subtle white 8% opacity

### 6.7 Chat Bubbles
```css
.chat-bubble-user     { background: var(--color-info-bg); margin-left: auto; }
.chat-bubble-character { background: var(--color-success-bg); margin-right: auto; }
```

### 6.8 EmptyState
- Centered layout, dashed border, glass background
- Muted icon + message + optional action button

### 6.9 AlertMessage
- Uses Bootstrap `alert` classes
- Error: `alert-danger` with `var(--color-danger-bg)`
- Success: `alert-success` with `var(--color-success-bg)`

### 6.10 Login Page
- Dark glassmorphism card on black gradient background
- Brand with green gradient text
- Input with transparent dark bg
- Emerald gradient button

---

## 7. Responsive Breakpoints

| Breakpoint | Width | Behavior |
|------------|-------|----------|
| Mobile | < 576px | Single column, stacked |
| Tablet | 576–768px | 2 columns, compact nav |
| Desktop | ≥ 768px | 3 columns, full nav, side panel |

Navbar collapses at 768px (`navbar-expand-md`).
Grid cards: `col-md-4` (3 cols) / `col-sm-6` (2 cols) / `col-12` (1 col).

---

## 8. Design Tokens Quick Reference

```css
/* Copy-paste untuk komponen baru */
background: var(--color-bg);
color: var(--color-text);
border: 1px solid var(--color-border);
border-radius: var(--radius-md);
font-family: var(--font-family);
padding: var(--space-4);
box-shadow: var(--shadow-sm);
font-size: var(--font-size-base);
font-weight: var(--font-weight-medium);
```

---

## 9. Do's & Don'ts

```css
/* ✅ DO — always use CSS variables */
color: var(--color-text);
border: 1px solid var(--color-border);
background: var(--color-primary);
border-radius: var(--radius-md);
font-family: var(--font-family);
padding: var(--space-4);
box-shadow: var(--shadow-sm);

/* ❌ DON'T — never hardcode */
color: #333;
border: 1px solid #dee2e6;
background: #0d6efd;
border-radius: 6px;
font-family: 'Segoe UI';
padding: 16px;
```

---

> *myKisah — Build habits, write your journey, grow together.*
