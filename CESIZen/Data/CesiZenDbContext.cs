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
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Droit> Droits { get; set; }
        public DbSet<Information> Informations { get; set; }
        public DbSet<QuestionnaireStress> Questionnaires { get; set; }

        public DbSet<ReponseQuestionnaire> ReponsesQuestionnaire { get; set; }
        public DbSet<ReponseEvenement> ReponsesEvenement { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Utilisateur>()
                .HasMany(u => u.Informations)
                .WithMany(i => i.Utilisateurs)
                .UsingEntity(j => j.ToTable("UtilisateurInformation"));

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Droits)
                .WithMany(d => d.Roles)
                .UsingEntity(j => j.ToTable("Attribuer"));

            modelBuilder.Entity<ReponseEvenement>()
                .HasOne(re => re.ReponseQuestionnaire)
                .WithMany(rq => rq.ReponsesEvenement)
                .HasForeignKey(re => re.ReponseQuestionnaireId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReponseQuestionnaire>()
                .HasOne(rq => rq.Utilisateur)
                .WithMany(u => u.ReponsesQuestionnaire)
                .HasForeignKey(rq => rq.UtilisateurId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuestionnaireStress>().HasData(
               new QuestionnaireStress { Id = 1, Libelle = "Décès du conjoint", Valeur = 100 },
               new QuestionnaireStress { Id = 2, Libelle = "Divorce", Valeur = 73 },
               new QuestionnaireStress { Id = 3, Libelle = "Séparation", Valeur = 65 },
               new QuestionnaireStress { Id = 4, Libelle = "Prison", Valeur = 63 },
               new QuestionnaireStress { Id = 5, Libelle = "Mort d'un proche", Valeur = 63 }
             );
        }
    }

}
