using Microsoft.EntityFrameworkCore;
using CesiZen.Data;
using Microsoft.AspNetCore.Identity;
using CESIZen.Models; // Ajoutez le namespace de votre modèle Utilisateur

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CesiZenDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurer Identity avec Utilisateur personnalisé
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
    name: "admin",
    pattern: "Admin/{controller=User}/{action=Index}/{id?}",
    defaults: new { area = "" }); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Initialisation des rôles et utilisateurs
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<Utilisateur>>();

    // Créer les rôles
    string[] roleNames = { "Admin", "User" };

    foreach (var roleName in roleNames)
    {
        if (!roleManager.RoleExistsAsync(roleName).Result)
        {
            roleManager.CreateAsync(new IdentityRole<int>(roleName)).Wait();
        }
    }

    // Créer l'administrateur
    var adminUser = new Utilisateur
    {
        UserName = "admin@gmail.com",
        Email = "admin@gmail.com",
        Nom = "admin",
        Prenom = "admin",
        EmailConfirmed = true,
        Statut = "Actif"
    };

    if (userManager.FindByEmailAsync(adminUser.Email).Result == null)
    {
        var result = userManager.CreateAsync(adminUser, "root").Result;

        if (result.Succeeded)
        {
            userManager.AddToRoleAsync(adminUser, "Admin").Wait();
            Console.WriteLine("Utilisateur administrateur créé avec succès.");
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            Console.WriteLine($"Erreur lors de la création de l'admin: {errors}");
        }
    }

    // Créer l'utilisateur lambda
    var lambdaUser = new Utilisateur
    {
        UserName = "user@gmail.com",
        Email = "user@gmail.com",
        Nom = "User",
        Prenom = "Test",
        EmailConfirmed = true,
        Statut = "Actif"
    };

    if (userManager.FindByEmailAsync(lambdaUser.Email).Result == null)
    {
        var result = userManager.CreateAsync(lambdaUser, "1234").Result;

        if (result.Succeeded)
        {
            userManager.AddToRoleAsync(lambdaUser, "User").Wait();
            Console.WriteLine("Utilisateur lambda créé avec succès.");
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            Console.WriteLine($"Erreur lors de la création de l'utilisateur lambda: {errors}");
        }
    }
}

app.Run();