using Core;
using Core.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrFUEducationalModules.Data;

var builder = WebApplication.CreateBuilder(args);

// В файле appsettings.json настраивается строка подключения к БД. Хостим на локалке
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// DI для всех требующихся сервисов
builder.Services.AddTransient<Func<ApplicationDbContext>>(c => () => c.GetService<ApplicationDbContext>()!);
builder.Services.AddTransient<IHeadRepository, HeadRepository>();
builder.Services.AddTransient<IProgramRepository, ProgramRepository>();
builder.Services.AddTransient<IInstituteRepository, InstituteRepository>();
builder.Services.AddTransient<IModuleRepository, ModuleRepository>();
builder.Services.AddTransient<IDbServiceProvider, DbServiceProvider>();
builder.Services.AddTransient<IDbService, DbService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options
        .UseNpgsql(connectionString)
        .UseLazyLoadingProxies()
        .UseSnakeCaseNamingConvention());
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Сниженная планка валидности пароля, проще тестировать, да и большего не требуется
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "UrFUEducationModules";
    options.Cookie.HttpOnly = true;
    options.LoginPath = "/Account/Login";
    options.SlidingExpiration = true;
});


builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // подключаем миграции БД
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();