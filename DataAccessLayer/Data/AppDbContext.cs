using Microsoft.EntityFrameworkCore;
using Entity.Entities;

namespace DataAccessLayer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Hedef> Hedefler { get; set; }
        public DbSet<Durum> Durumlar { get; set; }
        public DbSet<Oneri> Oneriler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Ad).IsRequired().HasMaxLength(40);
                entity.Property(e => e.Soyad).IsRequired().HasMaxLength(40);
            });

            // Hedef entity configuration
            modelBuilder.Entity<Hedef>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Baslik).IsRequired().HasMaxLength(120);
                entity.Property(e => e.Aciklama).HasMaxLength(500);
                entity.Property(e => e.Periyot).IsRequired().HasMaxLength(60);
                entity.Property(e => e.Onem).IsRequired().HasMaxLength(20);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(100);
            });

            // Durum entity configuration
            modelBuilder.Entity<Durum>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DurumAciklama).HasMaxLength(1000);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(100);
                
                entity.HasOne(d => d.Hedef)
                    .WithMany(h => h.Durumlar)
                    .HasForeignKey(d => d.HedefId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Oneri entity configuration
            modelBuilder.Entity<Oneri>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Metin).IsRequired().HasMaxLength(500);
            });

            // Seed data for Oneri
            modelBuilder.Entity<Oneri>().HasData(
                new Oneri { Id = 1, Metin = "Günde en az 2 kez dişlerinizi fırçalayın.", Aktif = true },
                new Oneri { Id = 2, Metin = "Diş ipi kullanmayı unutmayın.", Aktif = true },
                new Oneri { Id = 3, Metin = "Ağız gargarası kullanarak bakterileri azaltın.", Aktif = true },
                new Oneri { Id = 4, Metin = "Şekerli içecekleri sınırlayın.", Aktif = true },
                new Oneri { Id = 5, Metin = "6 ayda bir diş hekiminizi ziyaret edin.", Aktif = true },
                new Oneri { Id = 6, Metin = "Florürlü diş macunu kullanın.", Aktif = true },
                new Oneri { Id = 7, Metin = "Sigara ve tütün ürünlerinden kaçının.", Aktif = true },
                new Oneri { Id = 8, Metin = "Bol su için, ağzınızı nemli tutun.", Aktif = true }
            );
        }
    }
}
