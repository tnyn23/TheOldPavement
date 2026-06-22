using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Application.Mappings;
using Application.Options;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// ── Response Compression (Brotli + Gzip) ─────────────────────────────────────
// Reduces HTML/CSS/JS transfer size by ~60-80% on Railway (no CDN)
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "text/html",
        "text/css",
        "application/javascript",
        "application/json",
        "image/svg+xml",
        "font/woff2",
    });
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    options.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    options.Level = CompressionLevel.Fastest);


// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TheOldPavementDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

// Add Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IMomoService, MomoService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<INotificationDispatcher, Web.Services.SignalRNotificationDispatcher>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Add SignalR
builder.Services.AddSignalR();

// ── MoMo: strongly-typed options + named HttpClient ──────────────────────────
builder.Services.Configure<MomoOptions>(
    builder.Configuration.GetSection(MomoOptions.SectionName));

builder.Services.AddHttpClient("MoMo", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Add Razor Pages
builder.Services.AddRazorPages();

// Add Cookie Authentication
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Public/Account/Login";
        options.LogoutPath = "/Public/Account/Logout";
        options.AccessDeniedPath = "/Public/Account/AccessDenied";
        options.Cookie.Name = "TheOldPavementAuth";
    });
builder.Services.AddAuthorization();

// Add Session Services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "TheOldPavementSession";
});

var app = builder.Build();

// Run database migration to support MoMo payment method in MySQL enum/column
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TheOldPavementDbContext>();
    
    // Auto-apply all pending EF Core Migrations (like the new SizeChart columns)
    try 
    {
        context.Database.Migrate();
        Console.WriteLine("EF Core Migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"EF Core Migration Error: {ex.Message}");
    }

    try
    {
        context.Database.ExecuteSqlRaw("ALTER TABLE orders MODIFY COLUMN payment_method VARCHAR(50);");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database Migration Warning: {ex.Message}");
    }
    // Add transaction_id column if not exists
    try
    {
        // Check if column already exists before adding
        var colExists = context.Database
            .SqlQueryRaw<int>(
                "SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'orders' AND COLUMN_NAME = 'transaction_id'")
            .AsEnumerable()
            .FirstOrDefault();

        if (colExists == 0)
        {
            context.Database.ExecuteSqlRaw(
                "ALTER TABLE orders ADD COLUMN transaction_id VARCHAR(100) NULL;");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database Migration Warning (transaction_id): {ex.Message}");
    }
    // Expand payment_status enum to include awaiting_confirmation
    try
    {
        context.Database.ExecuteSqlRaw(
            "ALTER TABLE orders MODIFY COLUMN payment_status ENUM('pending','paid','failed','awaiting_confirmation','refunded') DEFAULT 'pending';");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database Migration Warning (payment_status): {ex.Message}");
    }
    // Add is_approved column to product_reviews if not exists
    try
    {
        var reviewColExists = context.Database
            .SqlQueryRaw<int>(
                "SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'product_reviews' AND COLUMN_NAME = 'is_approved'")
            .AsEnumerable()
            .FirstOrDefault();

        if (reviewColExists == 0)
        {
            context.Database.ExecuteSqlRaw(
                "ALTER TABLE product_reviews ADD COLUMN is_approved TINYINT(1) DEFAULT 1;");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database Migration Warning (product_reviews): {ex.Message}");
    }

    // Add new columns to promo_codes
    try
    {
        var promoCols = context.Database
            .SqlQueryRaw<string>(
                "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'promo_codes'")
            .ToList();

        if (!promoCols.Contains("applies_to_category", StringComparer.OrdinalIgnoreCase))
            context.Database.ExecuteSqlRaw("ALTER TABLE promo_codes ADD COLUMN applies_to_category VARCHAR(100) NULL;");
        
        if (!promoCols.Contains("applies_to_product_ids", StringComparer.OrdinalIgnoreCase))
            context.Database.ExecuteSqlRaw("ALTER TABLE promo_codes ADD COLUMN applies_to_product_ids VARCHAR(255) NULL;");
            
        if (!promoCols.Contains("required_quantity", StringComparer.OrdinalIgnoreCase))
            context.Database.ExecuteSqlRaw("ALTER TABLE promo_codes ADD COLUMN required_quantity INT NULL;");
            
        if (!promoCols.Contains("reward_quantity", StringComparer.OrdinalIgnoreCase))
            context.Database.ExecuteSqlRaw("ALTER TABLE promo_codes ADD COLUMN reward_quantity INT NULL;");
            
        if (!promoCols.Contains("required_user_tier", StringComparer.OrdinalIgnoreCase))
            context.Database.ExecuteSqlRaw("ALTER TABLE promo_codes ADD COLUMN required_user_tier VARCHAR(50) NULL;");
            
        if (!promoCols.Contains("is_combo", StringComparer.OrdinalIgnoreCase))
            context.Database.ExecuteSqlRaw("ALTER TABLE promo_codes ADD COLUMN is_combo TINYINT(1) DEFAULT 0;");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database Migration Warning (promo_codes): {ex.Message}");
    }

    // Add new columns to users
    try
    {
        var userCols = context.Database
            .SqlQueryRaw<string>(
                "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'users'")
            .ToList();

        if (!userCols.Contains("total_spent", StringComparer.OrdinalIgnoreCase))
            context.Database.ExecuteSqlRaw("ALTER TABLE users ADD COLUMN total_spent DECIMAL(18,2) DEFAULT 0;");
            
        if (!userCols.Contains("tier", StringComparer.OrdinalIgnoreCase))
            context.Database.ExecuteSqlRaw("ALTER TABLE users ADD COLUMN tier VARCHAR(50) DEFAULT 'Standard';");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database Migration Warning (users): {ex.Message}");
    }

    // Add new columns to sales
    try
    {
        var saleCols = context.Database
            .SqlQueryRaw<string>(
                "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'sales'")
            .ToList();

        if (!saleCols.Contains("is_flash_sale", StringComparer.OrdinalIgnoreCase))
            context.Database.ExecuteSqlRaw("ALTER TABLE sales ADD COLUMN is_flash_sale TINYINT(1) DEFAULT 0;");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database Migration Warning (sales): {ex.Message}");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// ── Enable compression FIRST (before static files) ───────────────────────────
app.UseResponseCompression();

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var headers = ctx.Context.Response.Headers;
        // Aggressive caching for versioned assets (CSS/JS with ?v= hash)
        if (ctx.Context.Request.Query.ContainsKey("v"))
        {
            headers.Append("Cache-Control", "public, max-age=31536000, immutable");
        }
        else
        {
            // Regular static files: 30 days
            headers.Append("Cache-Control", "public, max-age=2592000");
        }
    }
});

app.UseRouting();

// Enable Session state
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapHub<Web.Hubs.NotificationHub>("/notificationHub");

app.Run();


