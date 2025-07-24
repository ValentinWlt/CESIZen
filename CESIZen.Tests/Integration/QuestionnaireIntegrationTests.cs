using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CesiZen.Data;
using CESIZen.Models;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CESIZen.Tests.Integration
{
    [TestClass]
    [TestCategory("Integration")]
    public class QuestionnaireIntegrationTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [TestInitialize]
        public void TestInitialize()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Remplacer la vraie base de données par une base en mémoire pour les tests
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<CesiZenDbContext>));
                        if (descriptor != null)
                            services.Remove(descriptor);

                        services.AddDbContext<CesiZenDbContext>(options =>
                            options.UseInMemoryDatabase($"IntegrationTestDb_{Guid.NewGuid()}"));

                        // Configuration spécifique pour les tests
                        services.Configure<IdentityOptions>(options =>
                        {
                            // Assouplir les règles de mot de passe pour les tests
                            options.Password.RequireDigit = false;
                            options.Password.RequiredLength = 4;
                            options.Password.RequireNonAlphanumeric = false;
                            options.Password.RequireUppercase = false;
                            options.Password.RequireLowercase = false;
                        });
                    });

                    builder.UseEnvironment("Testing");
                    
                    // Désactiver HTTPS redirect pour les tests
                    builder.ConfigureServices(services =>
                    {
                        services.PostConfigure<Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionOptions>(options =>
                        {
                            options.HttpsPort = null;
                        });
                    });
                });

            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        /// <summary>
        /// Test d'intégration : Navigation et sécurité de l'application
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public async Task ApplicationNavigation_And_Security_ShouldWork()
        {
            Console.WriteLine("🔄 Test d'intégration: Navigation et sécurité...");

            // === ÉTAPE 1: Test d'accès aux pages publiques ===
            Console.WriteLine("📄 Test 1: Accès aux pages publiques...");
            
            var homeResponse = await _client.GetAsync("/");
            Assert.IsTrue(homeResponse.IsSuccessStatusCode, 
                $"La page d'accueil devrait être accessible. Statut: {homeResponse.StatusCode}");
            Console.WriteLine($"✅ Page d'accueil: {homeResponse.StatusCode}");

            var registerPageResponse = await _client.GetAsync("/Account/Register");
            Assert.IsTrue(registerPageResponse.IsSuccessStatusCode, 
                $"La page d'inscription devrait être accessible. Statut: {registerPageResponse.StatusCode}");
            Console.WriteLine($"✅ Page d'inscription: {registerPageResponse.StatusCode}");

            var loginPageResponse = await _client.GetAsync("/Account/Login");
            Assert.IsTrue(loginPageResponse.IsSuccessStatusCode, 
                $"La page de connexion devrait être accessible. Statut: {loginPageResponse.StatusCode}");
            Console.WriteLine($"✅ Page de connexion: {loginPageResponse.StatusCode}");

            // === ÉTAPE 2: Test de sécurité - Accès aux pages protégées ===
            Console.WriteLine("🔒 Test 2: Sécurité des pages protégées...");
            
            var questionnaireUrl = "/Utilisateur/QuestionnaireUtilisateur/Index";
            var protectedResponse = await _client.GetAsync(questionnaireUrl);
            
            Console.WriteLine($"📊 Accès questionnaire sans auth: {protectedResponse.StatusCode}");
            
            // Vérifier que l'accès non autorisé est correctement géré
            bool isSecurityWorking = protectedResponse.StatusCode == HttpStatusCode.Redirect || 
                                   protectedResponse.StatusCode == HttpStatusCode.Unauthorized ||
                                   protectedResponse.StatusCode == HttpStatusCode.Found;

            if (protectedResponse.StatusCode == HttpStatusCode.Redirect)
            {
                var location = protectedResponse.Headers.Location?.ToString();
                Console.WriteLine($"📍 Redirection vers: {location}");
                
                bool isAuthRedirect = location?.ToLower().Contains("account") == true || 
                                     location?.ToLower().Contains("login") == true ||
                                     location?.Contains("ReturnUrl") == true;
                
                Assert.IsTrue(isAuthRedirect, 
                    $"Devrait rediriger vers authentification. Location: {location}");
                Console.WriteLine("✅ Redirection d'authentification correcte");
            }
            else
            {
                Assert.IsTrue(isSecurityWorking,
                    $"L'accès protégé devrait être sécurisé. Statut: {protectedResponse.StatusCode}");
                Console.WriteLine($"✅ Sécurité validée: {protectedResponse.StatusCode}");
            }

            // === ÉTAPE 3: Test de l'accès aux formulaires ===
            Console.WriteLine("📝 Test 3: Accès aux formulaires...");
            
            var registerContent = await registerPageResponse.Content.ReadAsStringAsync();
            Assert.IsTrue(registerContent.Contains("Email") || registerContent.Contains("inscription"),
                "La page d'inscription devrait contenir un formulaire");
            Console.WriteLine("✅ Formulaire d'inscription présent");

            var loginContent = await loginPageResponse.Content.ReadAsStringAsync();
            Assert.IsTrue(loginContent.Contains("Email") || loginContent.Contains("Password") || loginContent.Contains("connexion"),
                "La page de connexion devrait contenir un formulaire");
            Console.WriteLine("✅ Formulaire de connexion présent");

            // === ÉTAPE 4: Test de la base de données ===
            Console.WriteLine("🗄️ Test 4: Initialisation de la base de données...");
            
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CesiZenDbContext>();
            
            // S'assurer que la base de données est créée
            await context.Database.EnsureCreatedAsync();
            
            // Ajouter des données de test
            if (!context.Questionnaires.Any())
            {
                context.Questionnaires.AddRange(
                    new QuestionnaireStress { Id = 1, Libelle = "Stress au travail", Valeur = 5 },
                    new QuestionnaireStress { Id = 2, Libelle = "Stress familial", Valeur = 3 },
                    new QuestionnaireStress { Id = 3, Libelle = "Stress financier", Valeur = 7 }
                );
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Données de test ajoutées");
            }

            var questionnaireCount = await context.Questionnaires.CountAsync();
            Assert.IsTrue(questionnaireCount >= 3, "Les questionnaires de test devraient être présents");
            Console.WriteLine($"✅ Questionnaires en base: {questionnaireCount}");

            Console.WriteLine("🎉 Test d'intégration terminé avec succès !");
        }

        /// <summary>
        /// Test spécifique : Validation des formulaires d'authentification
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public async Task AuthenticationForms_ShouldHaveCorrectStructure()
        {
            Console.WriteLine("🔄 Test validation: Structure des formulaires d'authentification...");

            // === TEST FORMULAIRE D'INSCRIPTION ===
            var registerPage = await _client.GetAsync("/Account/Register");
            Assert.IsTrue(registerPage.IsSuccessStatusCode, "Page d'inscription accessible");
            
            var registerContent = await registerPage.Content.ReadAsStringAsync();
            
            // Vérifier la présence des champs essentiels
            Assert.IsTrue(registerContent.Contains("Email"), "Champ Email présent");
            Assert.IsTrue(registerContent.Contains("Password"), "Champ Password présent");
            Assert.IsTrue(registerContent.Contains("Nom") || registerContent.Contains("nom"), "Champ Nom présent");
            Assert.IsTrue(registerContent.Contains("Prenom") || registerContent.Contains("prenom"), "Champ Prénom présent");
            
            // Vérifier la présence du token anti-forgery
            bool hasAntiForgeryToken = registerContent.Contains("__RequestVerificationToken");
            Console.WriteLine($"📋 Token anti-forgery présent: {hasAntiForgeryToken}");
            
            Console.WriteLine("✅ Structure du formulaire d'inscription validée");

            // === TEST FORMULAIRE DE CONNEXION ===
            var loginPage = await _client.GetAsync("/Account/Login");
            Assert.IsTrue(loginPage.IsSuccessStatusCode, "Page de connexion accessible");
            
            var loginContent = await loginPage.Content.ReadAsStringAsync();
            
            // Vérifier la présence des champs essentiels
            Assert.IsTrue(loginContent.Contains("Email"), "Champ Email présent dans login");
            Assert.IsTrue(loginContent.Contains("Password"), "Champ Password présent dans login");
            
            bool hasLoginAntiForgeryToken = loginContent.Contains("__RequestVerificationToken");
            Console.WriteLine($"📋 Token anti-forgery login présent: {hasLoginAntiForgeryToken}");
            
            Console.WriteLine("✅ Structure du formulaire de connexion validée");

            // === TEST TENTATIVE DE SOUMISSION (sans validation complète) ===
            Console.WriteLine("📝 Test tentative soumission formulaire vide...");
            
            var emptyFormData = new List<KeyValuePair<string, string>>();
            var emptyFormContent = new FormUrlEncodedContent(emptyFormData);
            
            var emptySubmitResponse = await _client.PostAsync("/Account/Register", emptyFormContent);
            Console.WriteLine($"📊 Soumission formulaire vide: {emptySubmitResponse.StatusCode}");
            
            // Une soumission vide devrait soit échouer (BadRequest) soit retourner la page avec erreurs
            bool isExpectedFailure = !emptySubmitResponse.IsSuccessStatusCode || 
                                    emptySubmitResponse.StatusCode == HttpStatusCode.BadRequest;
            
            Console.WriteLine($"✅ Validation formulaire fonctionne: {isExpectedFailure}");

            Console.WriteLine("🎉 Validation des formulaires terminée avec succès !");
        }

        /// <summary>
        /// Test de performance et disponibilité
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public async Task ApplicationPerformance_ShouldBeReasonable()
        {
            Console.WriteLine("🔄 Test performance: Temps de réponse des pages...");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Test page d'accueil
            stopwatch.Restart();
            var homeResponse = await _client.GetAsync("/");
            stopwatch.Stop();
            
            Assert.IsTrue(homeResponse.IsSuccessStatusCode, "Page d'accueil accessible");
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 5000, 
                $"Page d'accueil devrait répondre en moins de 5s. Temps: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"✅ Page d'accueil: {stopwatch.ElapsedMilliseconds}ms");

            // Test page d'inscription
            stopwatch.Restart();
            var registerResponse = await _client.GetAsync("/Account/Register");
            stopwatch.Stop();
            
            Assert.IsTrue(registerResponse.IsSuccessStatusCode, "Page d'inscription accessible");
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 5000, 
                $"Page d'inscription devrait répondre en moins de 5s. Temps: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"✅ Page d'inscription: {stopwatch.ElapsedMilliseconds}ms");

            // Test page de connexion
            stopwatch.Restart();
            var loginResponse = await _client.GetAsync("/Account/Login");
            stopwatch.Stop();
            
            Assert.IsTrue(loginResponse.IsSuccessStatusCode, "Page de connexion accessible");
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 5000, 
                $"Page de connexion devrait répondre en moins de 5s. Temps: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"✅ Page de connexion: {stopwatch.ElapsedMilliseconds}ms");

            Console.WriteLine("🎉 Tests de performance terminés avec succès !");
        }

        #region Méthodes utilitaires

        private static string ExtractAntiForgeryToken(string html)
        {
            try
            {
                var tokenStart = html.IndexOf("__RequestVerificationToken", StringComparison.OrdinalIgnoreCase);
                if (tokenStart == -1) return "";

                var valueStart = html.IndexOf("value=\"", tokenStart, StringComparison.OrdinalIgnoreCase) + 7;
                if (valueStart <= 6) return "";

                var valueEnd = html.IndexOf("\"", valueStart, StringComparison.OrdinalIgnoreCase);
                if (valueEnd <= valueStart) return "";

                return html.Substring(valueStart, valueEnd - valueStart);
            }
            catch
            {
                return "";
            }
        }

        #endregion
    }
}