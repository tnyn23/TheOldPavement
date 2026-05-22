using Microsoft.EntityFrameworkCore;
using TheOldPavement.Application.Interfaces;
using TheOldPavement.Application.Mappings;
using TheOldPavement.Application.Services;
using TheOldPavement.Domain.Interfaces;
using TheOldPavement.Infrastructure.Context;
using TheOldPavement.Infrastructure.Repositories;

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
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IMomoService, MomoService>();

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

