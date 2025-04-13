using CESIZen.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.Data
{
    public class CesiZenDbContext : IdentityDbContext<Utilisateur, IdentityRole<int>, int>
    {
        public CesiZenDbContext(DbContextOptions<CesiZenDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }

        // Ici, remplacez LoginViewModel par Droit après avoir renommé la classe
        public DbSet<Droit> Droits { get; set; }

        public DbSet<Information> Informations { get; set; }
        public DbSet<Questionnaire> Questionnaires { get; set; }
        public DbSet<Question> Questions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Utilisateur>()
                .HasMany(u => u.Informations)
                .WithMany(i => i.Utilisateurs)
                .UsingEntity(j => j.ToTable("UtilisateurInformation"));

            // Assurez-vous que les noms des entités correspondent
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Droits)
                .WithMany(d => d.Roles)
                .UsingEntity(j => j.ToTable("Attribuer"));
        }
    }
}
