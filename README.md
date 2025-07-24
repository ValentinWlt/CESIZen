# CESIZen - Plateforme de Gestion du Bien-être en Entreprise

## Vue d'ensemble

CESIZen est une application web de gestion du bien-être et d'évaluation du stress en entreprise développée en ASP.NET Core 8.0 MVC. Elle permet aux organisations de surveiller, évaluer et améliorer le bien-être de leurs employés à travers des questionnaires personnalisés, des analyses de stress et un système de gestion des utilisateurs granulaire.

[![Build Status](https://github.com/valentinwlt/cesizen/workflows/CI%2FCD%20Pipeline/badge.svg)](https://github.com/valentinwlt/cesizen/actions)
[![Security](https://img.shields.io/badge/security-Snyk-brightgreen)](https://snyk.io)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Architecture

### Structure du projet

```
CESIZen/
├── CESIZen/                          # Application principale ASP.NET Core MVC
│   ├── Controllers/                   # Contrôleurs MVC
│   │   ├── Admin/                    # Contrôleurs d'administration
│   │   ├── AccountController.cs      # Authentification
│   │   └── HomeController.cs         # Contrôleur principal
│   ├── Models/                       # Modèles de données et ViewModels
│   ├── Views/                        # Vues Razor
│   │   ├── Admin/                    # Vues d'administration
│   │   ├── Account/                  # Vues d'authentification
│   │   └── Shared/                   # Vues partagées
│   ├── Data/                         # Contexte Entity Framework
│   ├── Migrations/                   # Migrations de base de données
│   ├── wwwroot/                      # Fichiers statiques (CSS, JS, images)
│   └── Program.cs                    # Point d'entrée et configuration
├── CESIZen.Tests/                    # Tests unitaires et d'intégration
│   ├── Controllers/                  # Tests des contrôleurs
│   │   └── Admin/                   # Tests des contrôleurs admin
│   └── Models/                      # Tests des modèles
├── .github/workflows/                # Pipelines CI/CD GitHub Actions
│   ├── 01-1_TestApp.yaml           # Tests automatisés
│   ├── 01-2_SonarQube.yaml         # Analyse qualité code
│   ├── 01-4_Snyk.yaml              # Analyse sécuritaire
│   ├── 02-1_Dockerbuild.yaml       # Construction Docker
│   └── 02-2_Deploy.yaml            # Déploiement Azure
├── docker-compose.yml                # Configuration Docker multi-services
├── Dockerfile                        # Image Docker de l'application
├── deploy.sh                         # Script de déploiement automatisé
└── README.md                         # Documentation du projet
```

### Technologies utilisées

- **Backend**: ASP.NET Core 8.0 MVC
- **Base de données**: SQL Server 2022
- **ORM**: Entity Framework Core 8.0
- **Authentification**: ASP.NET Core Identity
- **Frontend**: Razor Views, Bootstrap 5, JavaScript
- **Tests**: xUnit, Moq, Entity Framework InMemory
- **Containerisation**: Docker & Docker Compose
- **CI/CD**: GitHub Actions
- **Sécurité**: Snyk, OWASP
- **Cloud**: Microsoft Azure

## Modèle de données

### Entités principales

#### Utilisateur (Identity User Extension)
```csharp
public class Utilisateur : IdentityUser<int>
{
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public DateTime? DateNaissance { get; set; }
    public string? Adresse { get; set; }
    public string? CodePostal { get; set; }
    public string? Ville { get; set; }
    
    // Relations
    public int? IdRole { get; set; }
    public virtual Role? Role { get; set; }
    public virtual ICollection<ReponseQuestionnaire>? ReponsesQuestionnaire { get; set; }
    public virtual ICollection<Information>? Informations { get; set; }
}
```

#### QuestionnaireStress
```csharp
public class QuestionnaireStress
{
    public int Id { get; set; }
    public string Libelle { get; set; }
    public int Valeur { get; set; }
    
    // Relations de navigation
    public virtual ICollection<ReponseEvenement>? ReponsesEvenement { get; set; }
}
```

#### ReponseQuestionnaire
```csharp
public class ReponseQuestionnaire
{
    public int Id { get; set; }
    public DateTime DateReponse { get; set; }
    public int ScoreTotal { get; set; }
    
    // Clés étrangères
    public int UtilisateurId { get; set; }
    public virtual Utilisateur Utilisateur { get; set; }
    public virtual ICollection<ReponseEvenement>? ReponsesEvenement { get; set; }
}
```

#### ReponseEvenement
```csharp
public class ReponseEvenement
{
    public int Id { get; set; }
    public int Valeur { get; set; }
    
    // Relations
    public int QuestionnaireStressId { get; set; }
    public virtual QuestionnaireStress QuestionnaireStress { get; set; }
    
    public int ReponseQuestionnaireId { get; set; }
    public virtual ReponseQuestionnaire ReponseQuestionnaire { get; set; }
}
```

#### Role et Permissions
```csharp
public class Role
{
    public int Id { get; set; }
    public string Libelle { get; set; }
    
    // Relations
    public virtual ICollection<Utilisateur>? Utilisateurs { get; set; }
    public virtual ICollection<Droit>? Droits { get; set; }
}

public class Droit
{
    public int Id { get; set; }
    public string Libelle { get; set; }
    public virtual ICollection<Role>? Roles { get; set; }
}
```

#### Information
```csharp
public class Information
{
    public int Id { get; set; }
    public string Titre { get; set; }
    public string Contenu { get; set; }
    public DateTime DatePublication { get; set; }
    public bool EstActive { get; set; }
    
    public virtual ICollection<Utilisateur>? Utilisateurs { get; set; }
}
```

## Système d'authentification et d'autorisation

### Configuration Identity
```csharp
// Program.cs
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
```

### Configuration des cookies
```csharp
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});
```

### Routes et autorisation
```csharp
// Routes administration
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=User}/{action=Index}/{id?}");

// Route par défaut
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

## Fonctionnalités principales

### 1. Gestion des questionnaires de stress
- **Création et édition** : Interface d'administration pour gérer les questions
- **Évaluation** : Système de notation et de calcul de score
- **Suivi** : Historique des réponses par utilisateur
- **Analytics** : Analyse des tendances de stress

### 2. Système utilisateur avancé
- **Authentification sécurisée** : Login/Register avec validation
- **Gestion des rôles** : Système hiérarchique de permissions
- **Profils utilisateur** : Informations personnelles complètes
- **Activation de compte** : Processus de validation

### 3. Interface d'administration
- **Dashboard** : Vue d'ensemble des métriques
- **Gestion utilisateurs** : CRUD complet avec filtrage
- **Modération** : Outils de gestion du contenu
- **Rapports** : Génération de statistiques

### 4. Système d'information
- **Actualités** : Diffusion d'informations ciblées
- **Notifications** : Alertes et communications
- **Personnalisation** : Contenu adapté par utilisateur

## Pipeline CI/CD

### Workflow d'intégration continue

#### 1. Tests automatisés (`01-1_TestApp.yaml`)
**Déclencheur** : Pull Request vers `main`

```yaml
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build --no-restore
      
      - name: Test
        run: dotnet test --no-build --verbosity normal
```

#### 2. Analyse qualité SonarQube (`01-2_SonarQube.yaml`)
```yaml
- name: SonarQube Scan
  uses: sonarqube-quality-gate-action@master
  env:
    SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
```

#### 3. Analyse sécuritaire Snyk (`01-4_Snyk.yaml`)
```yaml
jobs:
  snyk-dependencies:
    name: Snyk Dependencies Scan
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Run Snyk to check for vulnerabilities
        uses: snyk/actions/dotnet@master
        env:
          SNYK_TOKEN: ${{ secrets.SNYK_TOKEN }}
        with:
          args: --severity-threshold=${{ inputs.severity_threshold || 'high' }}
```

### Workflow de déploiement continu

#### 4. Construction Docker (`02-1_Dockerbuild.yaml`)
**Déclencheur** : Push vers `main`

```yaml
jobs:
  docker-build:
    runs-on: ubuntu-latest
    steps:
      - name: Build and push Docker image
        uses: docker/build-push-action@v5
        with:
          context: .
          file: ./Dockerfile
          push: true
          tags: ghcr.io/${{ github.repository }}:latest
```

#### 5. Déploiement Azure (`02-2_Deploy.yaml`)
```yaml
jobs:
  azure-deploy:
    runs-on: ubuntu-latest
    steps:
      - name: Deploy to Azure VM
        uses: appleboy/ssh-action@v1.0.3
        with:
          host: ${{ secrets.AZURE_HOST }}
          username: ${{ secrets.AZURE_LOGIN }}
          script: |
            cd /opt/cesizen
            docker-compose pull
            docker-compose up -d
```

## Tests

### Structure des tests

```
CESIZen.Tests/
├── Controllers/
│   ├── Admin/
│   │   ├── QuestionnaireStressesAdminControllerTests.cs
│   │   ├── UserAdminControllerTests.cs
│   │   └── InformationAdminControllerTests.cs
│   ├── AccountControllerTests.cs
│   └── HomeControllerTests.cs
├── Models/
│   ├── UtilisateurTests.cs
│   ├── QuestionnaireStressTests.cs
│   └── ReponseQuestionnaireTests.cs
└── Integration/
    ├── AuthenticationFlowTests.cs
    └── DatabaseIntegrationTests.cs
```

### Types de tests implémentés

#### Tests unitaires des contrôleurs
```csharp
[TestClass]
public class QuestionnaireStressesAdminControllerTests
{
    [TestMethod]
    public async Task Index_Returns_ViewResult_WithQuestionnaires()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CesiZenDbContext>()
            .UseInMemoryDatabase(databaseName: "Index_Test_Database")
            .Options;

        using (var context = new CesiZenDbContext(options))
        {
            // Setup test data
            context.Questionnaires.Add(new QuestionnaireStress 
            { 
                Id = 1, 
                Valeur = 10, 
                Libelle = "Stress test" 
            });
            context.SaveChanges();

            var controller = new QuestionnaireStressesAdminController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as IEnumerable<QuestionnaireStress>;
            Assert.AreEqual(1, model.Count());
        }
    }
}
```

#### Tests d'intégration
```csharp
[TestMethod]
public async Task Create_Post_ValidModel_AddsEntityAndRedirects()
{
    // Test du workflow complet de création d'entité
    var newItem = new QuestionnaireStress
    {
        Libelle = "Nouveau stress",
        Valeur = 42
    };

    var result = await controller.Create(newItem);
    
    Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
    var itemInDb = await context.Questionnaires.FindAsync(newItem.Id);
    Assert.IsNotNull(itemInDb);
}
```

### Configuration de test avec Entity Framework InMemory
```csharp
var options = new DbContextOptionsBuilder<CesiZenDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .Options;

using var context = new CesiZenDbContext(options);
// Setup test data...
```

## Déploiement et configuration

### Variables d'environnement requises

#### Secrets GitHub Actions
```yaml
# Sécurité et qualité
SNYK_TOKEN              # Token Snyk pour analyse sécuritaire
SONAR_TOKEN             # Token SonarCloud pour qualité code

# Container Registry
GITHUB_TOKEN            # Token GitHub Container Registry (auto-généré)

# Déploiement Azure
AZURE_HOST              # IP/Hostname du serveur Azure
AZURE_LOGIN             # Nom d'utilisateur SSH
AZURE_PORT              # Port SSH (défaut: 22)
AZURE_PWD               # Mot de passe SSH

# Base de données
DB_PASSWORD             # Mot de passe SQL Server
```

### Configuration Docker optimisée

#### Dockerfile multi-stage
```dockerfile
# Image de base pour l'exécution (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Image pour la construction (build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copier les fichiers de projet et restaurer les dépendances
COPY ["CESIZen/CESIZen.csproj", "CESIZen/"]
RUN dotnet restore "CESIZen/CESIZen.csproj"

# Copier tout le code source et construire
COPY . .
WORKDIR "/src/CESIZen"
RUN dotnet build "CESIZen.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Phase de publication
FROM build AS publish
RUN dotnet publish "CESIZen.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Image finale optimisée
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CESIZen.dll"]
```

#### Docker Compose avec optimisations mémoire
```yaml
services:
  cesizen-database:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: "${DB_PASSWORD}"
      ACCEPT_EULA: "Y"
      MSSQL_PID: "Express"
      MSSQL_MEMORY_LIMIT_MB: "2048"
    deploy:
      resources:
        limits:
          memory: 2.5G
        reservations:
          memory: 1.5G
    healthcheck:
      test: ["CMD", "/opt/mssql-tools18/bin/sqlcmd", "-S", "localhost", "-U", "SA", "-P", "${DB_PASSWORD}", "-Q", "SELECT 1", "-No"]
      interval: 30s
      timeout: 10s
      retries: 5

  cesizen-app:
    image: "ghcr.io/valentinwlt/cesizen:latest"
    environment:
      ASPNETCORE_ENVIRONMENT: "Production"
      ConnectionStrings__DefaultConnection: "Server=cesizen-database;Database=CESIZenDB;User Id=sa;Password=${DB_PASSWORD};TrustServerCertificate=true;"
    deploy:
      resources:
        limits:
          memory: 1G
        reservations:
          memory: 512M
    depends_on:
      cesizen-database:
        condition: service_healthy
```

### Script de déploiement automatisé

Le script `deploy.sh` fournit une interface simple pour la gestion de l'application :

```bash
#!/bin/bash
# Gestion complète du cycle de vie de l'application

case "${1:-start}" in
    start)
        log_info "Démarrage de CESIZen..."
        docker-compose up -d
        # Vérification de santé automatique
        ;;
    stop)
        log_info "Arrêt de CESIZen..."
        docker-compose down
        ;;
    restart)
        stop && start
        ;;
    build)
        log_info "Construction des images..."
        docker-compose build --no-cache
        ;;
    logs)
        docker-compose logs -f
        ;;
    migrate)
        log_info "Application des migrations..."
        docker-compose exec cesizen-app dotnet ef database update
        ;;
    clean)
        # Nettoyage complet avec confirmation
        read -p "Supprimer toutes les données ? (y/N): " -r
        [[ $REPLY =~ ^[Yy]$ ]] && docker-compose down -v
        ;;
esac
```

## Intégration d'une nouvelle fonctionnalité

### 1. Développement local

#### Création d'une nouvelle entité
```csharp
// CESIZen/Models/NouvelleEntite.cs
public class NouvelleEntite
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Le nom est obligatoire")]
    [StringLength(100, ErrorMessage = "Le nom ne peut dépasser 100 caractères")]
    public string Nom { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime DateCreation { get; set; } = DateTime.Now;
    
    // Relations
    public int UtilisateurId { get; set; }
    public virtual Utilisateur? Utilisateur { get; set; }
}
```

#### Mise à jour du DbContext
```csharp
// CesiZen/Data/CesiZenDbContext.cs
public class CesiZenDbContext : IdentityDbContext<Utilisateur, IdentityRole<int>, int>
{
    // Entités existantes...
    public DbSet<QuestionnaireStress> Questionnaires { get; set; }
    public DbSet<ReponseQuestionnaire> ReponsesQuestionnaire { get; set; }
    
    // Nouvelle entité
    public DbSet<NouvelleEntite> NouvellesEntites { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configuration de la nouvelle entité
        builder.Entity<NouvelleEntite>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom).HasMaxLength(100);
            entity.HasOne(e => e.Utilisateur)
                  .WithMany()
                  .HasForeignKey(e => e.UtilisateurId);
        });
    }
}
```

#### Migration de base de données
```bash
# Depuis le répertoire CESIZen/
dotnet ef migrations add AjoutNouvelleEntite
dotnet ef database update
```

### 2. Création du contrôleur

#### Contrôleur Admin avec CRUD complet
```csharp
// CESIZen/Controllers/Admin/NouvelleEntiteAdminController.cs
[Authorize(Roles = "Administrateur,Super-Administrateur")]
public class NouvelleEntiteAdminController : Controller
{
    private readonly CesiZenDbContext _context;

    public NouvelleEntiteAdminController(CesiZenDbContext context)
    {
        _context = context;
    }

    // GET: Admin/NouvelleEntite
    public async Task<IActionResult> Index()
    {
        var entites = await _context.NouvellesEntites
            .Include(n => n.Utilisateur)
            .ToListAsync();
        return View(entites);
    }

    // GET: Admin/NouvelleEntite/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var entite = await _context.NouvellesEntites
            .Include(n => n.Utilisateur)
            .FirstOrDefaultAsync(m => m.Id == id);
            
        return entite == null ? NotFound() : View(entite);
    }

    // GET: Admin/NouvelleEntite/Create
    public IActionResult Create()
    {
        ViewData["UtilisateurId"] = new SelectList(_context.Users, "Id", "Email");
        return View();
    }

    // POST: Admin/NouvelleEntite/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nom,UtilisateurId")] NouvelleEntite entite)
    {
        if (ModelState.IsValid)
        {
            _context.Add(entite);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Entité créée avec succès !";
            return RedirectToAction(nameof(Index));
        }
        
        ViewData["UtilisateurId"] = new SelectList(_context.Users, "Id", "Email", entite.UtilisateurId);
        return View(entite);
    }

    // GET: Admin/NouvelleEntite/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var entite = await _context.NouvellesEntites.FindAsync(id);
        if (entite == null) return NotFound();

        ViewData["UtilisateurId"] = new SelectList(_context.Users, "Id", "Email", entite.UtilisateurId);
        return View(entite);
    }

    // POST: Admin/NouvelleEntite/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,UtilisateurId,DateCreation")] NouvelleEntite entite)
    {
        if (id != entite.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(entite);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Entité modifiée avec succès !";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntiteExists(entite.Id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        
        ViewData["UtilisateurId"] = new SelectList(_context.Users, "Id", "Email", entite.UtilisateurId);
        return View(entite);
    }

    // GET: Admin/NouvelleEntite/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var entite = await _context.NouvellesEntites
            .Include(n => n.Utilisateur)
            .FirstOrDefaultAsync(m => m.Id == id);
            
        return entite == null ? NotFound() : View(entite);
    }

    // POST: Admin/NouvelleEntite/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entite = await _context.NouvellesEntites.FindAsync(id);
        if (entite != null)
        {
            _context.NouvellesEntites.Remove(entite);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Entité supprimée avec succès !";
        }
        return RedirectToAction(nameof(Index));
    }

    private bool EntiteExists(int id)
    {
        return _context.NouvellesEntites.Any(e => e.Id == id);
    }
}
```

### 3. Création des vues Razor

#### Vue Index avec filtrage et pagination
```razor
@* CESIZen/Views/Admin/NouvelleEntite/Index.cshtml *@
@model IEnumerable<CESIZen.Models.NouvelleEntite>

@{
    ViewData["Title"] = "Gestion des Nouvelles Entités";
    Layout = "~/Views/Shared/_Layout.cshtml";
}

<div class="container-fluid">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h1 class="h3 mb-0">@ViewData["Title"]</h1>
        <a asp-action="Create" class="btn btn-primary">
            <i class="bi bi-plus-circle"></i> Créer une nouvelle entité
        </a>
    </div>

    @if (TempData["SuccessMessage"] != null)
    {
        <div class="alert alert-success alert-dismissible fade show" role="alert">
            @TempData["SuccessMessage"]
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    }

    <div class="card">
        <div class="card-body">
            <div class="table-responsive">
                <table class="table table-striped table-hover">
                    <thead class="table-dark">
                        <tr>
                            <th>@Html.DisplayNameFor(model => model.Nom)</th>
                            <th>@Html.DisplayNameFor(model => model.DateCreation)</th>
                            <th>@Html.DisplayNameFor(model => model.Utilisateur)</th>
                            <th class="text-center">Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        @foreach (var item in Model)
                        {
                            <tr>
                                <td>@Html.DisplayFor(modelItem => item.Nom)</td>
                                <td>@Html.DisplayFor(modelItem => item.DateCreation)</td>
                                <td>@Html.DisplayFor(modelItem => item.Utilisateur.Email)</td>
                                <td class="text-center">
                                    <div class="btn-group" role="group">
                                        <a asp-action="Details" asp-route-id="@item.Id" 
                                           class="btn btn-sm btn-outline-info" title="Voir">
                                            <i class="bi bi-eye"></i>
                                        </a>
                                        <a asp-action="Edit" asp-route-id="@item.Id" 
                                           class="btn btn-sm btn-outline-warning" title="Modifier">
                                            <i class="bi bi-pencil"></i>
                                        </a>
                                        <a asp-action="Delete" asp-route-id="@item.Id" 
                                           class="btn btn-sm btn-outline-danger" title="Supprimer">
                                            <i class="bi bi-trash"></i>
                                        </a>
                                    </div>
                                </td>
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</div>
```

#### Vue Create avec validation
```razor
@* CESIZen/Views/Admin/NouvelleEntite/Create.cshtml *@
@model CESIZen.Models.NouvelleEntite

@{
    ViewData["Title"] = "Créer une nouvelle entité";
}

<div class="container">
    <div class="row justify-content-center">
        <div class="col-md-8">
            <div class="card">
                <div class="card-header">
                    <h4 class="card-title mb-0">@ViewData["Title"]</h4>
                </div>
                <div class="card-body">
                    <form asp-action="Create" method="post">
                        <div asp-validation-summary="ModelOnly" class="text-danger mb-3"></div>
                        
                        <div class="mb-3">
                            <label asp-for="Nom" class="form-label"></label>
                            <input asp-for="Nom" class="form-control" />
                            <span asp-validation-for="Nom" class="text-danger"></span>
                        </div>
                        
                        <div class="mb-3">
                            <label asp-for="UtilisateurId" class="form-label">Utilisateur</label>
                            <select asp-for="UtilisateurId" class="form-select" asp-items="ViewBag.UtilisateurId">
                                <option value="">-- Sélectionner un utilisateur --</option>
                            </select>
                            <span asp-validation-for="UtilisateurId" class="text-danger"></span>
                        </div>
                        
                        <div class="d-flex justify-content-between">
                            <a asp-action="Index" class="btn btn-secondary">
                                <i class="bi bi-arrow-left"></i> Retour à la liste
                            </a>
                            <button type="submit" class="btn btn-primary">
                                <i class="bi bi-check"></i> Créer
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

### 4. Tests unitaires

#### Tests du contrôleur
```csharp
// CESIZen.Tests/Controllers/Admin/NouvelleEntiteAdminControllerTests.cs
[TestClass]
public class NouvelleEntiteAdminControllerTests
{
    private DbContextOptions<CesiZenDbContext> GetInMemoryDbOptions(string dbName)
    {
        return new DbContextOptionsBuilder<CesiZenDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }

    [TestMethod]
    public async Task Index_ReturnsViewResult_WithEntitesList()
    {
        // Arrange
        var options = GetInMemoryDbOptions("Index_Test");
        
        using (var context = new CesiZenDbContext(options))
        {
            var utilisateur = new Utilisateur 
            { 
                Id = 1, 
                Email = "test@test.com",
                UserName = "test@test.com"
            };
            context.Users.Add(utilisateur);
            
            context.NouvellesEntites.Add(new NouvelleEntite 
            { 
                Id = 1, 
                Nom = "Test Entité",
                UtilisateurId = 1
            });
            context.SaveChanges();

            var controller = new NouvelleEntiteAdminController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            
            var model = viewResult.Model as IEnumerable<NouvelleEntite>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count());
        }
    }

    [TestMethod]
    public async Task Create_Post_ValidModel_RedirectsToIndex()
    {
        // Arrange
        var options = GetInMemoryDbOptions("Create_Test");
        
        using (var context = new CesiZenDbContext(options))
        {
            var utilisateur = new Utilisateur 
            { 
                Id = 1, 
                Email = "test@test.com",
                UserName = "test@test.com"
            };
            context.Users.Add(utilisateur);
            context.SaveChanges();

            var controller = new NouvelleEntiteAdminController(context);
            var newEntite = new NouvelleEntite 
            { 
                Nom = "Nouvelle Entité Test",
                UtilisateurId = 1
            };

            // Act
            var result = await controller.Create(newEntite);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
            
            // Vérifier que l'entité a été créée
            var entiteInDb = await context.NouvellesEntites
                .FirstOrDefaultAsync(e => e.Nom == "Nouvelle Entité Test");
            Assert.IsNotNull(entiteInDb);
        }
    }

    [TestMethod]
    public async Task Delete_ValidId_RemovesEntityAndRedirects()
    {
        // Arrange
        var options = GetInMemoryDbOptions("Delete_Test");
        
        using (var context = new CesiZenDbContext(options))
        {
            var entiteToDelete = new NouvelleEntite 
            { 
                Id = 1, 
                Nom = "À supprimer",
                UtilisateurId = 1
            };
            context.NouvellesEntites.Add(entiteToDelete);
            context.SaveChanges();

            var controller = new NouvelleEntiteAdminController(context);

            // Act
            var result = await controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
            
            // Vérifier que l'entité a été supprimée
            var deletedEntity = await context.NouvellesEntites.FindAsync(1);
            Assert.IsNull(deletedEntity);
        }
    }
}
```

### 5. Mise à jour de la navigation

#### Ajout au menu d'administration
```razor
@* CESIZen/Views/Shared/_Layout.cshtml *@
<nav class="navbar navbar-expand-lg navbar-dark bg-primary">
    <div class="container">
        <!-- Navigation existante... -->
        
        @if (User.IsInRole("Administrateur") || User.IsInRole("Super-Administrateur"))
        {
            <div class="navbar-nav">
                <div class="nav-item dropdown">
                    <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
                        <i class="bi bi-gear"></i> Administration
                    </a>
                    <ul class="dropdown-menu">
                        <li><a class="dropdown-item" asp-controller="UserAdmin" asp-action="Index">Utilisateurs</a></li>
                        <li><a class="dropdown-item" asp-controller="QuestionnaireStressesAdmin" asp-action="Index">Questionnaires</a></li>
                        <li><a class="dropdown-item" asp-controller="NouvelleEntiteAdmin" asp-action="Index">Nouvelles Entités</a></li>
                        <li><hr class="dropdown-divider"></li>
                        <li><a class="dropdown-item" asp-controller="InformationAdmin" asp-action="Index">Informations</a></li>
                    </ul>
                </div>
            </div>
        }
    </div>
</nav>
```

### 6. Workflow de validation et déploiement

#### Développement en feature branch
```bash
# 1. Créer la branche feature
git checkout -b feature/nouvelle-entite-management

# 2. Développement avec commits atomiques
git add -A
git commit -m "feat: add NouvelleEntite model and migration"

git add -A  
git commit -m "feat: add NouvelleEntiteAdmin controller with CRUD operations"

git add -A
git commit -m "feat: add admin views for NouvelleEntite management"

git add -A
git commit -m "test: add unit tests for NouvelleEntiteAdmin controller"

# 3. Push de la branche
git push origin feature/nouvelle-entite-management
```

#### Pull Request et CI
1. **Créer la PR** vers `main` sur GitHub
2. **Déclenchement automatique** :
   - **Tests automatisés** : `01-1_TestApp.yaml`
   - **Analyse qualité** : `01-2_SonarQube.yaml`
   - **Scan sécuritaire** : `01-4_Snyk.yaml`

#### Review et merge
3. **Code review** par l'équipe
4. **Merge vers main** après validation
5. **Déploiement automatique** :
   - **Build Docker** : `02-1_Dockerbuild.yaml`
   - **Déploiement Azure** : `02-2_Deploy.yaml`

## Monitoring et observabilité

### Health Checks
```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContext<CesiZenDbContext>()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

app.MapHealthChecks("/health");
```

### Logging structuré
```csharp
// Configuration du logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddEventSourceLogger();

// Dans les contrôleurs
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInformation("Page d'accueil visitée à {Time}", DateTime.UtcNow);
        return View();
    }
}
```

### Métriques de performance
- **Temps de réponse** : < 2 secondes pour les pages standard
- **Disponibilité** : 99.9% SLA target
- **Throughput** : Support de 100+ utilisateurs concurrents
- **Health checks** : Vérification DB toutes les 30 secondes

## Sécurité et bonnes pratiques

### Validation et protection
```csharp
// Validation des modèles
public class QuestionnaireStressViewModel
{
    [Required(ErrorMessage = "Le libellé est obligatoire")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Le libellé doit contenir entre 3 et 100 caractères")]
    public string Libelle { get; set; }

    [Range(1, 10, ErrorMessage = "La valeur doit être comprise entre 1 et 10")]
    public int Valeur { get; set; }
}

// Protection CSRF
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("Libelle,Valeur")] QuestionnaireStress questionnaire)
{
    // ...
}
```

### Autorisation granulaire
```csharp
[Authorize(Roles = "Super-Administrateur")]
public class SuperAdminController : Controller { }

[Authorize(Roles = "Administrateur,Super-Administrateur")]
public class AdminController : Controller { }

// Dans les vues
@if (User.IsInRole("Administrateur"))
{
    <a href="#" class="btn btn-danger">Action sensible</a>
}
```

### Configuration sécurisée
```csharp
// Program.cs - Configuration sécurisée
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
    app.UseHsts();
    
    // Headers de sécurité
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        await next();
    });
}
```

## Bonnes pratiques de développement

### Convention de nommage
- **Contrôleurs** : `{Entity}Controller.cs` ou `{Entity}AdminController.cs`
- **Modèles** : PascalCase, noms singuliers
- **Vues** : PascalCase, organisées par contrôleur
- **Tests** : `{ClassUnderTest}Tests.cs`, méthodes descriptives

### Structure de commit
```
type(scope): description

Types: feat, fix, docs, style, refactor, test, chore
Scopes: model, controller, view, test, ci, config

Exemples:
feat(model): add NouvelleEntite with validation
fix(controller): handle null reference in UserAdmin
test(admin): add unit tests for QuestionnaireStress CRUD
docs(readme): update deployment instructions
```

### Gestion des erreurs
```csharp
public async Task<IActionResult> Edit(int id, QuestionnaireStress questionnaire)
{
    try
    {
        if (ModelState.IsValid)
        {
            _context.Update(questionnaire);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Questionnaire modifié avec succès";
            return RedirectToAction(nameof(Index));
        }
    }
    catch (DbUpdateConcurrencyException ex)
    {
        _logger.LogError(ex, "Erreur de concurrence lors de la modification du questionnaire {Id}", id);
        ModelState.AddModelError("", "Une erreur de concurrence s'est produite. Veuillez réessayer.");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erreur lors de la modification du questionnaire {Id}", id);
        ModelState.AddModelError("", "Une erreur inattendue s'est produite.");
    }
    
    return View(questionnaire);
}
```

### Performance et optimisation
```csharp
// Pagination efficace
public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
{
    var totalItems = await _context.Questionnaires.CountAsync();
    var questionnaires = await _context.Questionnaires
        .OrderBy(q => q.Libelle)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var viewModel = new QuestionnaireIndexViewModel
    {
        Questionnaires = questionnaires,
        CurrentPage = page,
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
    };

    return View(viewModel);
}

// Requêtes optimisées avec Include
var utilisateurs = await _context.Users
    .Include(u => u.Role)
    .Include(u => u.ReponsesQuestionnaire)
        .ThenInclude(r => r.ReponsesEvenement)
    .Where(u => u.IsAccountActivated)
    .ToListAsync();
```

---

## 🚀 Démarrage rapide pour nouveaux développeurs

### Checklist de setup
- [ ] Cloner le repository : `git clone https://github.com/valentinwlt/cesizen.git`
- [ ] Installer .NET 8.0 SDK
- [ ] Installer Docker Desktop
- [ ] Créer le fichier `.env` avec `DB_PASSWORD`
- [ ] Lancer : `./deploy.sh start`
- [ ] Accéder à : http://localhost:8080
- [ ] Configurer l'IDE avec les extensions Entity Framework
- [ ] Exécuter les tests : `dotnet test`

### Premier développement
1. **Explorer la structure** : Comprendre l'architecture MVC
2. **Étudier les modèles existants** : `CESIZen/Models/`
3. **Analyser les contrôleurs** : `CESIZen/Controllers/Admin/`
4. **Examiner les tests** : `CESIZen.Tests/`
5. **Suivre le guide d'intégration** ci-dessus pour ajouter une fonctionnalité

### Ressources utiles
- **ASP.NET Core MVC** : https://docs.microsoft.com/aspnet/core/mvc/
- **Entity Framework Core** : https://docs.microsoft.com/ef/core/
- **xUnit Testing** : https://xunit.net/docs/getting-started/netcore/cmdline
- **Bootstrap 5** : https://getbootstrap.com/docs/5.3/

---

*Documentation maintenue à jour - Dernière révision : Juillet 2025*