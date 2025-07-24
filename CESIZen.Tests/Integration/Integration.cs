// ============================================================================
// CESIZen.IntegrationTests/CESIZen.IntegrationTests.csproj
// ============================================================================
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.4.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.5">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CESIZen\CESIZen.csproj" />
  </ItemGroup>

</Project>


// ============================================================================
// CESIZen.IntegrationTests/CESIZenIntegrationTests.cs
// ============================================================================
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using CesiZen.Data;
using CESIZen.Models;
using System.Net;
using System.Net.Http;
using Xunit;

namespace CESIZen.IntegrationTests
{
    public class CESIZenIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public CESIZenIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    // Remplacer la base de données par InMemory
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CesiZenDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<CesiZenDbContext>(options =>
                        options.UseInMemoryDatabase("TestDatabase_" + Guid.NewGuid()));

                    services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task HomePage_LoadsSuccessfully()
        {
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Home/Privacy")]
        [InlineData("/Account/Login")]
        [InlineData("/Account/Register")]
        public async Task PublicPages_LoadWithoutErrors(string url)
        {
            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Redirect,
                $"Page {url} returned {response.StatusCode}");
        }

        [Fact]
        public async Task DatabaseServices_AreRegisteredCorrectly()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();

            // Act & Assert
            var dbContext = scope.ServiceProvider.GetService<CesiZenDbContext>();
            var userManager = scope.ServiceProvider.GetService<UserManager<Utilisateur>>();
            var signInManager = scope.ServiceProvider.GetService<SignInManager<Utilisateur>>();

            Assert.NotNull(dbContext);
            Assert.NotNull(userManager);
            Assert.NotNull(signInManager);
        }

        [Fact]
        public async Task Database_CanBeCreatedAndConnected()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CesiZenDbContext>();

            // Act
            await context.Database.EnsureCreatedAsync();
            var canConnect = await context.Database.CanConnectAsync();

            // Assert
            Assert.True(canConnect);
        }

        [Fact]
        public async Task UserManager_CanCreateUser()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CesiZenDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Utilisateur>>();

            await context.Database.EnsureCreatedAsync();

            var testUser = new Utilisateur
            {
                UserName = "test@cesizen.com",
                Email = "test@cesizen.com",
                Nom = "Test",
                Prenom = "User",
                Statut = "Actif",
                EmailConfirmed = true
            };

            // Act
            var result = await userManager.CreateAsync(testUser, "TestPassword123!");

            // Assert
            Assert.True(result.Succeeded, $"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            var retrievedUser = await userManager.FindByEmailAsync("test@cesizen.com");
            Assert.NotNull(retrievedUser);
            Assert.Equal("Test", retrievedUser.Nom);
        }

        [Fact]
        public async Task QuestionnaireStress_CanBeCreatedAndRetrieved()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CesiZenDbContext>();
            await context.Database.EnsureCreatedAsync();

            // Act
            var questionnaire = new QuestionnaireStress
            {
                Valeur = 15,
                Libelle = "Test Integration Questionnaire"
            };
            context.Questionnaires.Add(questionnaire);
            await context.SaveChangesAsync();

            // Assert
            var savedQuestionnaire = await context.Questionnaires
                .FirstOrDefaultAsync(q => q.Libelle == "Test Integration Questionnaire");

            Assert.NotNull(savedQuestionnaire);
            Assert.Equal(15, savedQuestionnaire.Valeur);
            Assert.Equal("Test Integration Questionnaire", savedQuestionnaire.Libelle);
        }

        [Fact]
        public async Task DatabaseTables_AreCreatedCorrectly()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CesiZenDbContext>();

            // Act
            await context.Database.EnsureCreatedAsync();

            // Assert
            Assert.NotNull(context.Users);
            Assert.NotNull(context.Questionnaires);

            // Test que les tables sont accessibles
            var userCount = await context.Users.CountAsync();
            var questionnaireCount = await context.Questionnaires.CountAsync();

            Assert.True(userCount >= 0);
            Assert.True(questionnaireCount >= 0);
        }
    }
}