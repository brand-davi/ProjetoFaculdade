using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManagerMvc.Data;
using ProjectManagerMvc.Models;
using ProjectManagerMvc.Services;

var builder = WebApplication.CreateBuilder(args);

// MySQL + EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Cookies (Login path)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", p => p.RequireRole("Admin"));
    options.AddPolicy("RequireManagerOrAdmin", p => p.RequireRole("Admin", "Manager"));
});

// MVC
builder.Services.AddControllersWithViews();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();

// Apply migrations and seed roles/admin at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();

        await SeedData.InitializeAsync(
            services,
            adminEmail: app.Configuration["Seed:AdminEmail"] ?? "admin@local",
            adminPassword: app.Configuration["Seed:AdminPassword"] ?? "Admin@12345"
        );
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating/seeding the database.");
        throw;
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
