using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Application.Mappings;
using Application.Options;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Context;
using Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TheOldPavementDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IMomoService, MomoService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();

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
        context.Database.ExecuteSqlRaw(
            "ALTER TABLE orders ADD COLUMN transaction_id VARCHAR(100) NULL;");
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
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable Session state
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();


