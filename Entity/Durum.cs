using System.ComponentModel.DataAnnotations;

namespace Entity.Entities
{
    public class Durum
    {
        public int Id { get; set; }

        public int? HedefId { get; set; }
        public Hedef? Hedef { get; set; }

        [Required]
        public DateTime Tarih { get; set; } = DateTime.Today;

        public TimeSpan? Saat { get; set; }
        public int? Sure { get; set; }                // dakika
        public bool Uygulandi { get; set; }
        
        [StringLength(1000)] 
        public string? DurumAciklama { get; set; }
        
        public string? GorselYolu { get; set; }

        [Required, StringLength(100)]
        public string UserId { get; set; } = default!;
    }
}
