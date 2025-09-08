using BussinesLayer.Abstract;
using BussinesLayer.Interfaces;
using BussinesLayer.Services;
using DataAccessLayer.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using AğızveDişSağlığı.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// DbContext (appsettings.json -> "DefaultConnection")
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        "Server=localhost;Database=nazliDb;Trusted_Connection=True;TrustServerCertificate=True",
        b => b.MigrationsAssembly("DataAccessLayer")
    )
);

// Yeni Health DbContext
builder.Services.AddDbContext<HealthDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        "Server=localhost;Database=NazliDb;Trusted_Connection=True;TrustServerCertificate=True",
        b => b.MigrationsAssembly("DataAccessLayer")
    )
);

// ---- DI ----
builder.Services.AddScoped<IAuthService, AuthManager>();
builder.Services.AddScoped<ISaglikService, SaglikManager >();
builder.Services.AddScoped<IEmailService, EmailService>();

// Yeni Health Service
builder.Services.AddScoped<BussinesLayer.Interfaces.IHealthService, BussinesLayer.Services.HealthService>();

// ---- COOKIE AUTH ----
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Hesap/Giris";
        o.LogoutPath = "/Hesap/Cikis";


        o.Cookie.Name = "AuthCookie";


        //o.AccessDeniedPath = "/Hesap/Giris";
    });

var app = builder.Build();

// Hata sayfası + HSTS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   // önce
app.UseAuthorization();    // sonra

// ---> KÖK İSTEKTE LOGIN AÇILSIN
//app.MapControllerRoute(
//    name: "root",
//    pattern: "",
//    defaults: new { controller = "Hesap", action = "Giris" }
//);

// Normal default route (uygulama içi linkler için)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Hesap}/{action=Giris}/{id?}"
);

app.Run();



