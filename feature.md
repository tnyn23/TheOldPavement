# 🚧 Current Limitations & Areas For Improvement - Completed ✅

All limitations listed below have been successfully implemented and improved:

## 🛒 Ecommerce Workflow - ✅ Completed
- **Full production-level order status support**: Added and styled badges/status updates for Shipping, Delivered, Refunded, and Cancelled orders.
- **Order Timeline Tracking**: Built a step-by-step visual timeline tracking system on the Customer Orders page.
- **Refund/Cancel flow**: Integrated a complete cancel order handler with automated inventory restoration.
- **Dynamic shipping calculations**: Added dynamic shipping fee calculation rules inside `CheckoutService.cs`.

## ❤️ Wishlist & Recommendation - ✅ Completed
- **Recently Viewed Products**: Created a session-backed logger tracking recently visited items.
- **Related Products**: Categories are checked and matching random products are rendered on product detail pages.
- **Recommendation system**: Random suggestions are loaded dynamically on the customer's Wishlist page.

## 🔐 Authentication & Security - ✅ Completed
- **Forgot Password**: Integrated `ForgotPassword` with secure mock emails.
- **Change Password**: Implemented change password forms inside the new Customer Profile page.

## 📱 UI/UX & Design System - ✅ Completed
- **Responsive design**: Uniform container widths and spacing guidelines have been applied.
- **Standardized Forms**: Login, register, profile, and coupon forms are unified under the Tailwind design style.

## 📊 Admin Dashboard - ✅ Completed
- **Analytics Hub**: Includes total revenue calculations, real-time customer counts, payment method breakdowns, and daily sales charts.
- **Inventory Monitoring & Low Stock Alerts**: Added a panel displaying products with low stock ($\le 15$) directly on the dashboard homepage.


Read "feature.md" first before generating any code.

You are working on a premium streetwear ecommerce platform called **TheOldPavement** built with:

* ASP.NET Core Razor Pages
* Layered Architecture
* Entity Framework Core
* Tailwind CSS
* SQL Server

The project follows a strict architecture and design system.

---

# 🏗 Architecture Rules

The project structure:

```txt
src/
 ├── TheOldPavement.Core
 ├── TheOldPavement.Application
 ├── TheOldPavement.Data
 └── TheOldPavement.Web
```

Rules:

* Core layer contains only entities/interfaces/constants.
* Application layer contains business logic/services/DTOs/validators.
* Data layer handles EF Core/database/repositories only.
* Web layer handles Razor Pages/UI only.

Never:

* put business logic inside Razor Pages
* access DbContext directly from Web layer
* duplicate business logic
* hardcode UI repeatedly

Always:

* use clean service abstraction
* use dependency injection
* use reusable partial components
* keep architecture scalable and maintainable

---

# 🎨 UI / Design Rules

The UI style is:

* premium streetwear
* minimal
* monochrome
* clean
* modern fashion ecommerce
* inspired by:

  * Represent
  * Fear Of God
  * DirtyCoins
  * 5THEWAY
  * Korean/Japanese streetwear brands

Main principles:

* mobile-first
* whitespace heavy
* strong typography
* minimal colors
* clean layout
* product-focused UI

Use:

* Tailwind CSS only
* semantic HTML
* reusable Razor partials
* consistent spacing system

Avoid:

* inline styles
* flashy animations
* glassmorphism
* colorful gradients
* inconsistent spacing

---

# 🚀 Current Features Already Existing

The project already has:

* Shop page
* Product detail
* Cart
* Checkout
* Wishlist
* Related products
* Recently viewed products
* Promo code system
* Dynamic shipping fee
* Momo payment integration
* Customer orders
* Order timeline
* Order cancellation
* Inventory management
* Admin dashboard
* Analytics overview
* Product management
* Inventory warning system
* Promo code management
* Authentication system
* Forgot password
* Guest checkout
* Email service

Do NOT recreate existing features.

---

# 🎯 Current Development Focus

The current goal is NOT deployment.

Focus on:

* improving ecommerce realism
* improving workflow quality
* improving admin experience
* improving UI consistency
* improving maintainability
* improving reusable component architecture

---

# 🔥 Priority Features To Improve

## Priority 1

* Better order timeline UI
* Advanced filter toolbar
* Product badge system
* Cart drawer / mini cart
* Better product review system
* Responsive admin improvements
* Recent orders activity feed
* Quick admin actions
* Better low-stock warning UX

---

## Priority 2

* Product size guide
* Advanced product gallery
* Notification center
* Collection system
* Lookbook/editorial pages
* Hero CMS/banner management
* Improved email templates

---

## Priority 3

* Recommendation engine improvements
* Advanced analytics dashboard
* Real-time admin updates
* AI sizing recommendation

---

# 📱 UX Requirements

Always include:

* loading states
* empty states
* hover states
* smooth transitions
* responsive layouts
* reusable components

Admin dashboard should feel:

* operational
* premium
* clean
* efficient

Customer experience should feel:

* modern
* fashion-oriented
* minimal
* immersive

---

# 🧩 Reusable Components

Prefer reusable partials/components:

```txt
_ProductCard.cshtml
_Button.cshtml
_EmptyState.cshtml
_FilterToolbar.cshtml
_AdminCard.cshtml
_AdminSection.cshtml
_AdminTable.cshtml
```

Avoid duplicated UI structures.

---

# ⚡ Coding Rules

Use:

* async/await
* DTOs
* service abstraction
* pagination
* validation
* clean naming
* scalable structure

Avoid:

* fat PageModels
* duplicated queries
* business logic in UI
* giant Razor files

---

# 🧠 Expected Code Quality

Generated code must feel like:

* a real production ecommerce platform
* a premium local brand experience
* clean and maintainable enterprise code
* scalable architecture

The goal is NOT just functional CRUD.

The goal is:

* realistic ecommerce workflows
* premium brand experience
* maintainable architecture
* polished user experience
