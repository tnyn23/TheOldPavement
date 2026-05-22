using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

using TheOldPavement.Core.Models;

namespace TheOldPavement.Data.Context;

public partial class TheOldPavementDbContext : DbContext
{
    public TheOldPavementDbContext()
    {
    }

    public TheOldPavementDbContext(DbContextOptions<TheOldPavementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AddToCartEvent> AddToCartEvents { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Collaboration> Collaborations { get; set; }

    public virtual DbSet<CollaborationProduct> CollaborationProducts { get; set; }

    public virtual DbSet<Collection> Collections { get; set; }

    public virtual DbSet<CommercialProject> CommercialProjects { get; set; }

    public virtual DbSet<NewsletterSubscriber> NewsletterSubscribers { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductDetail> ProductDetails { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductReview> ProductReviews { get; set; }

    public virtual DbSet<ProductVariant> ProductVariants { get; set; }

    public virtual DbSet<ProductView> ProductViews { get; set; }

    public virtual DbSet<ProjectDeliverable> ProjectDeliverables { get; set; }

    public virtual DbSet<ProjectProduct> ProjectProducts { get; set; }

    public virtual DbSet<PromoCode> PromoCodes { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SaleProduct> SaleProducts { get; set; }

    public virtual DbSet<ShippingAddress> ShippingAddresses { get; set; }

    public virtual DbSet<SizeChart> SizeCharts { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAddress> UserAddresses { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=the_old_pavement;user id=root;password=123456", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.46-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AddToCartEvent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("add_to_cart_events");

            entity.HasIndex(e => e.AddedAt, "idx_added_at");

            entity.HasIndex(e => e.ProductId, "idx_product_id");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.HasIndex(e => e.VariantId, "variant_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("added_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.SessionId)
                .HasMaxLength(255)
                .HasColumnName("session_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VariantId).HasColumnName("variant_id");

            entity.HasOne(d => d.Product).WithMany(p => p.AddToCartEvents)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("add_to_cart_events_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.AddToCartEvents)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("add_to_cart_events_ibfk_3");

            entity.HasOne(d => d.Variant).WithMany(p => p.AddToCartEvents)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("add_to_cart_events_ibfk_2");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("carts");

            entity.HasIndex(e => e.SessionId, "idx_session_id");

            entity.HasIndex(e => e.UserId, "idx_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("carts_ibfk_1");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("cart_items");

            entity.HasIndex(e => e.CartId, "idx_cart_id");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.HasIndex(e => e.VariantId, "variant_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("added_at");
            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.PriceAtAdd)
                .HasPrecision(10)
                .HasColumnName("price_at_add");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'")
                .HasColumnName("quantity");
            entity.Property(e => e.VariantId).HasColumnName("variant_id");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("cart_items_ibfk_1");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("cart_items_ibfk_2");

            entity.HasOne(d => d.Variant).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("cart_items_ibfk_3");
        });

        modelBuilder.Entity<Collaboration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("collaborations");

            entity.HasIndex(e => e.Status, "idx_status");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.HasIndex(e => e.Slug, "slug").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AuthenticityInfo).HasColumnName("authenticity_info");
            entity.Property(e => e.BannerImageUrl)
                .HasMaxLength(500)
                .HasColumnName("banner_image_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DetailImages)
                .HasColumnType("json")
                .HasColumnName("detail_images");
            entity.Property(e => e.IsFeatured)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_featured");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.PartnerName)
                .HasMaxLength(255)
                .HasColumnName("partner_name");
            entity.Property(e => e.Slug).HasColumnName("slug");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'available'")
                .HasColumnType("enum('available','sold_out','coming_soon')")
                .HasColumnName("status");
            entity.Property(e => e.Tagline)
                .HasMaxLength(500)
                .HasColumnName("tagline");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        modelBuilder.Entity<CollaborationProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("collaboration_products");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.HasIndex(e => new { e.CollaborationId, e.ProductId }, "unique_collab_product").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CollaborationId).HasColumnName("collaboration_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Collaboration).WithMany(p => p.CollaborationProducts)
                .HasForeignKey(d => d.CollaborationId)
                .HasConstraintName("collaboration_products_ibfk_1");

            entity.HasOne(d => d.Product).WithMany(p => p.CollaborationProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("collaboration_products_ibfk_2");
        });

        modelBuilder.Entity<Collection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("collections");

            entity.HasIndex(e => e.IsActive, "idx_is_active");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.HasIndex(e => e.Slug, "slug").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DisplayOrder)
                .HasDefaultValueSql("'0'")
                .HasColumnName("display_order");
            entity.Property(e => e.HeroImageUrl)
                .HasMaxLength(500)
                .HasColumnName("hero_image_url");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Season)
                .HasMaxLength(100)
                .HasColumnName("season");
            entity.Property(e => e.Slug).HasColumnName("slug");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        modelBuilder.Entity<CommercialProject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("commercial_projects");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'mockup'")
                .HasColumnType("enum('mockup','in_progress','completed')")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<NewsletterSubscriber>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("newsletter_subscribers");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.IsActive, "idx_is_active");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.SubscribedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("subscribed_at");
            entity.Property(e => e.UnsubscribedAt)
                .HasColumnType("datetime")
                .HasColumnName("unsubscribed_at");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("notifications");

            entity.HasIndex(e => e.IsRead, "idx_is_read");

            entity.HasIndex(e => e.UserId, "idx_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_read");
            entity.Property(e => e.Link)
                .HasMaxLength(500)
                .HasColumnName("link");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasColumnType("enum('order_update','promotion','restock')")
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("notifications_ibfk_1");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.CreatedAt, "idx_created_at");

            entity.HasIndex(e => e.OrderNumber, "idx_order_number").IsUnique();

            entity.HasIndex(e => new { e.UserId, e.CreatedAt }, "idx_order_user_date");

            entity.HasIndex(e => e.PaymentStatus, "idx_payment_status");

            entity.HasIndex(e => e.Status, "idx_status");

            entity.HasIndex(e => e.UserId, "idx_user_id");

            entity.HasIndex(e => e.PromoCodeId, "promo_code_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DiscountAmount)
                .HasPrecision(10)
                .HasDefaultValueSql("'0'")
                .HasColumnName("discount_amount");
            entity.Property(e => e.Note)
                .HasColumnType("text")
                .HasColumnName("note");
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(50)
                .HasColumnName("order_number");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','paid','failed')")
                .HasColumnName("payment_status");
            entity.Property(e => e.PromoCodeId).HasColumnName("promo_code_id");
            entity.Property(e => e.ShippingFee)
                .HasPrecision(10)
                .HasDefaultValueSql("'0'")
                .HasColumnName("shipping_fee");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','confirmed','processing','shipped','delivered','cancelled')")
                .HasColumnName("status");
            entity.Property(e => e.Subtotal)
                .HasPrecision(10)
                .HasColumnName("subtotal");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10)
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.PromoCode).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PromoCodeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("orders_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("orders_ibfk_1");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("order_items");

            entity.HasIndex(e => e.OrderId, "idx_order_id");

            entity.HasIndex(e => new { e.OrderId, e.ProductId }, "idx_order_item_order");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.HasIndex(e => e.VariantId, "variant_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasColumnName("product_name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Size)
                .HasColumnType("enum('S','M','L','XL','XXL')")
                .HasColumnName("size");
            entity.Property(e => e.Subtotal)
                .HasPrecision(10)
                .HasColumnName("subtotal");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10)
                .HasColumnName("unit_price");
            entity.Property(e => e.VariantId).HasColumnName("variant_id");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_items_ibfk_1");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_items_ibfk_2");

            entity.HasOne(d => d.Variant).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_items_ibfk_3");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("products");

            entity.HasIndex(e => e.CollectionId, "collection_id");

            entity.HasIndex(e => e.Category, "idx_category");

            entity.HasIndex(e => e.IsOnSale, "idx_is_on_sale");

            entity.HasIndex(e => e.IsOutlet, "idx_is_outlet");

            entity.HasIndex(e => new { e.Category, e.Status }, "idx_product_category_status");

            entity.HasIndex(e => new { e.Status, e.IsFeatured }, "idx_product_status_featured");

            entity.HasIndex(e => e.Slug, "idx_slug").IsUnique();

            entity.HasIndex(e => e.Status, "idx_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Category)
                .HasColumnType("enum('tee','hoodie','accessories')")
                .HasColumnName("category");
            entity.Property(e => e.CollabPartner)
                .HasMaxLength(255)
                .HasColumnName("collab_partner");
            entity.Property(e => e.CollectionId).HasColumnName("collection_id");
            entity.Property(e => e.Condition)
                .HasDefaultValueSql("'new'")
                .HasColumnType("enum('new','outlet')")
                .HasColumnName("condition");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DiscountPercentage)
                .HasDefaultValueSql("'0'")
                .HasColumnName("discount_percentage");
            entity.Property(e => e.IsCollab)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_collab");
            entity.Property(e => e.IsFeatured)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_featured");
            entity.Property(e => e.IsLimitedEdition)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_limited_edition");
            entity.Property(e => e.IsOnSale)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_on_sale");
            entity.Property(e => e.IsOutlet)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_outlet");
            entity.Property(e => e.LimitedQuantity).HasColumnName("limited_quantity");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.OriginalPrice)
                .HasPrecision(10)
                .HasColumnName("original_price");
            entity.Property(e => e.OutletReason)
                .HasMaxLength(255)
                .HasColumnName("outlet_reason");
            entity.Property(e => e.Price)
                .HasPrecision(10)
                .HasColumnName("price");
            entity.Property(e => e.SerialNumberPrefix)
                .HasMaxLength(50)
                .HasColumnName("serial_number_prefix");
            entity.Property(e => e.Slug).HasColumnName("slug");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'available'")
                .HasColumnType("enum('available','sold_out','coming_soon','discontinued')")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Collection).WithMany(p => p.Products)
                .HasForeignKey(d => d.CollectionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("products_ibfk_1");
        });

        modelBuilder.Entity<ProductDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("product_details");

            entity.HasIndex(e => e.ProductId, "product_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CareInstructions)
                .HasColumnType("json")
                .HasColumnName("care_instructions");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Features)
                .HasColumnType("json")
                .HasColumnName("features");
            entity.Property(e => e.Fit)
                .HasColumnType("enum('oversized','regular','slim')")
                .HasColumnName("fit");
            entity.Property(e => e.MadeIn)
                .HasMaxLength(100)
                .HasColumnName("made_in");
            entity.Property(e => e.Material)
                .HasMaxLength(255)
                .HasColumnName("material");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Weight)
                .HasMaxLength(50)
                .HasColumnName("weight");

            entity.HasOne(d => d.Product).WithOne(p => p.ProductDetail)
                .HasForeignKey<ProductDetail>(d => d.ProductId)
                .HasConstraintName("product_details_ibfk_1");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("product_images");

            entity.HasIndex(e => e.DisplayOrder, "idx_display_order");

            entity.HasIndex(e => e.ProductId, "idx_product_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AltText)
                .HasMaxLength(255)
                .HasColumnName("alt_text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DisplayOrder)
                .HasDefaultValueSql("'0'")
                .HasColumnName("display_order");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.IsPrimary)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_primary");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("product_images_ibfk_1");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("product_reviews");

            entity.HasIndex(e => e.ProductId, "idx_product_id");

            entity.HasIndex(e => e.Rating, "idx_rating");

            entity.HasIndex(e => e.OrderId, "order_id");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.HelpfulCount)
                .HasDefaultValueSql("'0'")
                .HasColumnName("helpful_count");
            entity.Property(e => e.Images)
                .HasColumnType("json")
                .HasColumnName("images");
            entity.Property(e => e.IsVerifiedPurchase)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_verified_purchase");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Order).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("product_reviews_ibfk_3");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("product_reviews_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("product_reviews_ibfk_2");
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("product_variants");

            entity.HasIndex(e => e.IsAvailable, "idx_is_available");

            entity.HasIndex(e => e.ProductId, "idx_product_id");

            entity.HasIndex(e => e.Sku, "idx_sku").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .HasColumnName("color");
            entity.Property(e => e.ColorHex)
                .HasMaxLength(7)
                .HasColumnName("color_hex");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_available");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Size)
                .HasColumnType("enum('S','M','L','XL','XXL')")
                .HasColumnName("size");
            entity.Property(e => e.Sku)
                .HasMaxLength(100)
                .HasColumnName("sku");
            entity.Property(e => e.StockQuantity)
                .HasDefaultValueSql("'0'")
                .HasColumnName("stock_quantity");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("product_variants_ibfk_1");
        });

        modelBuilder.Entity<ProductView>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("product_views");

            entity.HasIndex(e => e.ProductId, "idx_product_id");

            entity.HasIndex(e => e.UserId, "idx_user_id");

            entity.HasIndex(e => e.ViewedAt, "idx_viewed_at");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .HasColumnName("ip_address");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.SessionId)
                .HasMaxLength(255)
                .HasColumnName("session_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ViewedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("viewed_at");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductViews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("product_views_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.ProductViews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("product_views_ibfk_2");
        });

        modelBuilder.Entity<ProjectDeliverable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("project_deliverables");

            entity.HasIndex(e => e.ProjectId, "idx_project_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IconName)
                .HasMaxLength(100)
                .HasColumnName("icon_name");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.Link)
                .HasMaxLength(500)
                .HasColumnName("link");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Specifications)
                .HasMaxLength(255)
                .HasColumnName("specifications");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectDeliverables)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("project_deliverables_ibfk_1");
        });

        modelBuilder.Entity<ProjectProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("project_products");

            entity.HasIndex(e => e.ProjectId, "idx_project_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Category)
                .HasColumnType("enum('tee','hoodie','accessories')")
                .HasColumnName("category");
            entity.Property(e => e.Colors)
                .HasColumnType("json")
                .HasColumnName("colors");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Price)
                .HasPrecision(10)
                .HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasColumnName("product_name");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Sizes)
                .HasColumnType("json")
                .HasColumnName("sizes");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectProducts)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("project_products_ibfk_1");
        });

        modelBuilder.Entity<PromoCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("promo_codes");

            entity.HasIndex(e => e.Code, "code").IsUnique();

            entity.HasIndex(e => e.IsActive, "idx_is_active");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.MaxDiscount)
                .HasPrecision(10)
                .HasColumnName("max_discount");
            entity.Property(e => e.MinOrderValue)
                .HasPrecision(10)
                .HasDefaultValueSql("'0'")
                .HasColumnName("min_order_value");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Type)
                .HasColumnType("enum('percentage','fixed_amount','free_shipping')")
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UsageLimit).HasColumnName("usage_limit");
            entity.Property(e => e.UsedCount)
                .HasDefaultValueSql("'0'")
                .HasColumnName("used_count");
            entity.Property(e => e.Value)
                .HasPrecision(10)
                .HasColumnName("value");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("sales");

            entity.HasIndex(e => new { e.StartDate, e.EndDate }, "idx_date_range");

            entity.HasIndex(e => e.IsActive, "idx_is_active");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BannerImageUrl)
                .HasMaxLength(500)
                .HasColumnName("banner_image_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.TermsAndConditions).HasColumnName("terms_and_conditions");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<SaleProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("sale_products");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.HasIndex(e => new { e.SaleId, e.ProductId }, "unique_sale_product").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DiscountPercentage).HasColumnName("discount_percentage");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.SaleId).HasColumnName("sale_id");
            entity.Property(e => e.SalePrice)
                .HasPrecision(10)
                .HasColumnName("sale_price");

            entity.HasOne(d => d.Product).WithMany(p => p.SaleProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("sale_products_ibfk_2");

            entity.HasOne(d => d.Sale).WithMany(p => p.SaleProducts)
                .HasForeignKey(d => d.SaleId)
                .HasConstraintName("sale_products_ibfk_1");
        });

        modelBuilder.Entity<ShippingAddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("shipping_addresses");

            entity.HasIndex(e => e.OrderId, "order_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasColumnType("text")
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.District)
                .HasMaxLength(100)
                .HasColumnName("district");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Ward)
                .HasMaxLength(100)
                .HasColumnName("ward");

            entity.HasOne(d => d.Order).WithOne(p => p.ShippingAddress)
                .HasForeignKey<ShippingAddress>(d => d.OrderId)
                .HasConstraintName("shipping_addresses_ibfk_1");
        });

        modelBuilder.Entity<SizeChart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("size_charts");

            entity.HasIndex(e => new { e.ProductCategory, e.Size }, "unique_category_size").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Chest)
                .HasMaxLength(50)
                .HasColumnName("chest");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Length)
                .HasMaxLength(50)
                .HasColumnName("length");
            entity.Property(e => e.ProductCategory)
                .HasColumnType("enum('tee','hoodie','accessories')")
                .HasColumnName("product_category");
            entity.Property(e => e.Shoulder)
                .HasMaxLength(50)
                .HasColumnName("shoulder");
            entity.Property(e => e.Size)
                .HasColumnType("enum('S','M','L','XL','XXL')")
                .HasColumnName("size");
            entity.Property(e => e.Sleeve)
                .HasMaxLength(50)
                .HasColumnName("sleeve");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("stores");

            entity.HasIndex(e => e.City, "idx_city");

            entity.HasIndex(e => e.IsActive, "idx_is_active");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasColumnType("text")
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.GoogleMapsUrl)
                .HasMaxLength(500)
                .HasColumnName("google_maps_url");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.OpeningHours)
                .HasColumnType("json")
                .HasColumnName("opening_hours");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.Role, "idx_role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(500)
                .HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.EmailVerified)
                .HasDefaultValueSql("'0'")
                .HasColumnName("email_verified");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'customer'")
                .HasColumnType("enum('customer','admin')")
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_addresses");

            entity.HasIndex(e => e.UserId, "idx_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasColumnType("text")
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.District)
                .HasMaxLength(100)
                .HasColumnName("district");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.IsDefault)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_default");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Ward)
                .HasMaxLength(100)
                .HasColumnName("ward");

            entity.HasOne(d => d.User).WithMany(p => p.UserAddresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_addresses_ibfk_1");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("wishlists");

            entity.HasIndex(e => e.UserId, "idx_user_id");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.HasIndex(e => new { e.UserId, e.ProductId }, "unique_user_product").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("added_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("wishlists_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("wishlists_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
