using System.ComponentModel.DataAnnotations;

namespace AğızveDişSağlığı.Models
{
    public class SaglikModels
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Baslik { get; set; } = default!;

        [StringLength(500)]
        public string? Aciklama { get; set; }

        [Required, StringLength(60)]
        public string Periyot { get; set; } = "Günde bir";

        [Required, StringLength(20)]
        public string Onem { get; set; } = "Orta";

        [Required, StringLength(100)]
        public string UserId { get; set; } = default!;     // oturum açan kullanıcı

        public DateTime Olusturma { get; set; } = DateTime.UtcNow;

        public ICollection<DurumKaydi> Durumlar { get; set; } = new List<DurumKaydi>();
    }

    public class DurumKaydi
    {
        public int Id { get; set; }

        [Required] public int HedefId { get; set; }
        public SaglikModels? Hedef { get; set; }

        [Required] public DateTime Tarih { get; set; } = DateTime.Today;
        public TimeSpan? Saat { get; set; }
        public int? Sure { get; set; }                // dakika
        public bool Uygulandi { get; set; }
        [StringLength(1000)] public string? Aciklama { get; set; }
        public string? GorselYol { get; set; }

        [Required, StringLength(100)]
        public string UserId { get; set; } = default!;
    }

    public class Oneri
    {
        public int Id { get; set; }

        [Required, StringLength(500)]
        public string Metin { get; set; } = default!;

        public bool Aktif { get; set; } = true;
    }
}
