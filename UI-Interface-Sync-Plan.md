# The Old Pavement - UI Design System

## Brand Identity

The Old Pavement là local brand streetwear mang tinh thần phố cổ Hà Nội.

UI phải mang cảm giác:

* Premium
* Tối giản
* Streetwear hiện đại
* High-fashion
* Clean
* Mature
* Đậm chất local brand

Nguồn cảm hứng:

* Represent
* Fear Of God
* DirtyCoins
* 5THEWAY
* Streetwear Nhật / Hàn

---

# Core UI Principles

* Mobile-first responsive
* Nhiều khoảng trắng
* Typography mạnh
* Hạn chế màu sắc dư thừa
* Layout sạch sẽ
* Không dùng hiệu ứng rối mắt
* Tập trung vào sản phẩm

---

# Color System

## Primary Colors

* Black: `#000000`
* White: `#FFFFFF`

## Neutral Colors

* Gray 50
* Gray 100
* Gray 200
* Gray 400
* Gray 500
* Gray 800

## Rules

* Nền chủ yếu màu trắng
* Text màu đen
* Border xám nhạt
* Hover chuyển đen/xám
* Không dùng:

  * neon
  * gradient màu mè
  * màu quá saturated

---

# Typography

## Headings

* uppercase
* font-black hoặc font-bold
* tracking rộng
* line-height chặt

Ví dụ:

```html
<h1 class="text-4xl md:text-5xl font-black uppercase tracking-tight">
```

## Paragraph

* text-sm hoặc text-base
* text-gray-500
* leading-relaxed

## Buttons

* uppercase
* tracking-widest
* font-bold
* text-xs hoặc text-sm

---

# Layout Rules

## Container

```html
max-w-[1400px] mx-auto px-4 md:px-8 lg:px-16
```

## Section Spacing

* Desktop:

  * py-20
  * py-24
* Mobile:

  * py-12
  * py-16

## Grid System

### Product Grid

```html
grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4
```

### Gap

```html
gap-x-4 gap-y-12
```

---

# Component Rules

## Buttons

### Primary Button

```html
bg-black text-white hover:bg-gray-800
```

### Secondary Button

```html
bg-white border border-gray-300 hover:bg-gray-100
```

### Style

* rounded nhẹ hoặc rounded-full
* transition-colors
* uppercase
* tracking rộng

---

# Forms

## Inputs

```html
bg-white border border-gray-300
focus:border-black
focus:outline-none
```

## Select

* cursor-pointer
* font-semibold
* clean minimal

---

# Cards

## Product Card

Style:

* tối giản
* focus vào ảnh
* hover nhẹ
* không shadow đậm
* spacing thoáng

Hover:

```html
transition-all duration-300
```

---

# Header Sections

Hero sections nên có:

* large image banner
* dark overlay
* centered content
* uppercase heading

Ví dụ:

```html
bg-black/40
text-white
```

---

# Empty States

Style:

* border-dashed
* gray tone
* centered
* minimal icon

Ví dụ:

```html
border-2 border-dashed border-gray-200
```

---

# Animations

Cho phép:

* transition-colors
* transition-all
* duration-200
* duration-300

Không dùng:

* animation phức tạp
* bouncing
* flashy effect
* parallax nặng

---

# Shadows

Ưu tiên:

* shadow-sm
* hoặc không dùng shadow

Không dùng:

* shadow quá đậm
* glassmorphism

---

# Border Radius

Ưu tiên:

* rounded
* rounded-lg
* rounded-full

Không dùng:

* bo góc quá lớn

---

# Sticky Elements

Toolbar/filter:

```html
sticky top-16 z-20
```

---

# Reusable Components

Nên tạo:

* _Button.cshtml
* _ProductCard.cshtml
* _SectionTitle.cshtml
* _Input.cshtml
* _EmptyState.cshtml
* _FilterToolbar.cshtml

---

# Coding Style

* Razor Pages ASP.NET Core
* Tailwind CSS
* Semantic HTML
* Clean component structure
* Reusable partials
* Không inline style
* Không hardcode spacing lung tung

---

# AI Prompt Rule

Mỗi lần generate UI mới, luôn đọc file này trước.

Prompt mẫu:

```txt
Read ui-system.md first before generating code.

Create a new Razor Pages UI for The Old Pavement that follows the exact same design system and visual language.
```

---

# Design Goal

Mục tiêu cuối cùng:

Tạo cảm giác như:

* một local brand thật sự
* premium streetwear
* tối giản nhưng mạnh mẽ
* mang tinh thần Hà Nội hiện đại
* clean như ecommerce fashion brand quốc tế
