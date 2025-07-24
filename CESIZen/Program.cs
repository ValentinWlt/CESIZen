using Microsoft.EntityFrameworkCore;
using CesiZen.Data;
using Microsoft.AspNetCore.Identity;
using CESIZen.Models; // Ajoutez le namespace de votre mod�le Utilisateur

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CesiZenDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurer Identity avec Utilisateur personnalis�
builder.Services.AddIdentity<Utilisateur, IdentityRole<int>>(options => {
    options.Password.RequiredLength = 4;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<CesiZenDbContext>()
.AddDefaultTokenProviders();


// Configuration des cookies
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CesiZenDbContext>(); 

        await context.Database.EnsureCreatedAsync();

        // OU appliquer les migrations automatiquement
        // await context.Database.MigrateAsync();

        // Log de succ�s
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Base de donn�es initialis�e avec succ�s");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Une erreur s'est produite lors de l'initialisation de la base de donn�es");

        // En production, vous pourriez vouloir relancer l'exception
        // throw;
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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=User}/{action=Index}/{id?}",
    defaults: new { area = "" }); 




// Initialisation des r�les et utilisateurs
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<Utilisateur>>();

    // Cr�er les r�les
    string[] roleNames = { "Admin", "User" };

    foreach (var roleName in roleNames)
    {
        if (!roleManager.RoleExistsAsync(roleName).Result)
        {
            roleManager.CreateAsync(new IdentityRole<int>(roleName)).Wait();
        }
    }
}

app.Run();

public partial class Program { }