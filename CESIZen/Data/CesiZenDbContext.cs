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

        public DbSet<Utilisateur> Utilisateurs { get; set; }
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


        }
    }

}
