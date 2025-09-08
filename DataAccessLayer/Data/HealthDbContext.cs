using Microsoft.EntityFrameworkCore;
using Entity.Entities;

namespace DataAccessLayer.Data
{
    public class HealthDbContext : DbContext
    {
        public HealthDbContext(DbContextOptions<HealthDbContext> options) : base(options)
        {
        }

        public DbSet<HealthGoal> HealthGoals { get; set; }
        public DbSet<HealthStatus> HealthStatuses { get; set; }
        public DbSet<HealthSuggestion> HealthSuggestions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // HealthGoal entity configuration
            modelBuilder.Entity<HealthGoal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(120);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Period).IsRequired().HasMaxLength(60);
                entity.Property(e => e.Importance).IsRequired().HasMaxLength(20);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            // HealthStatus entity configuration
            modelBuilder.Entity<HealthStatus>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                
                entity.HasOne(d => d.HealthGoal)
                    .WithMany(h => h.Statuses)
                    .HasForeignKey(d => d.HealthGoalId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // HealthSuggestion entity configuration
            modelBuilder.Entity<HealthSuggestion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Text).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            // Seed data for HealthSuggestion
            modelBuilder.Entity<HealthSuggestion>().HasData(
                new HealthSuggestion { Id = 1, Text = "Günde en az 2 kez dişlerinizi fırçalayın.", IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new HealthSuggestion { Id = 2, Text = "Diş ipi kullanmayı unutmayın.", IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new HealthSuggestion { Id = 3, Text = "Ağız gargarası kullanarak bakterileri azaltın.", IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new HealthSuggestion { Id = 4, Text = "Şekerli içecekleri sınırlayın.", IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new HealthSuggestion { Id = 5, Text = "6 ayda bir diş hekiminizi ziyaret edin.", IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new HealthSuggestion { Id = 6, Text = "Florürlü diş macunu kullanın.", IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new HealthSuggestion { Id = 7, Text = "Sigara ve tütün ürünlerinden kaçının.", IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new HealthSuggestion { Id = 8, Text = "Bol su için, ağzınızı nemli tutun.", IsActive = true, CreatedAt = new DateTime(2024, 1, 1) }
            );
        }
    }
}
