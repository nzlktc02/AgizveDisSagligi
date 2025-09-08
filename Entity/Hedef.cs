using System.ComponentModel.DataAnnotations;

namespace Entity.Entities
{
    public class Hedef
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hedef başlığı gereklidir"), StringLength(120, ErrorMessage = "Başlık en fazla 120 karakter olabilir")]
        public string Baslik { get; set; } = default!;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Aciklama { get; set; }

        [Required, StringLength(60)]
        public string Periyot { get; set; } = "Günde bir";

        [Required, StringLength(20)]
        public string Onem { get; set; } = "Orta";

        [Required, StringLength(100)]
        public string UserId { get; set; } = default!;

        public DateTime Olusturma { get; set; } = DateTime.UtcNow;

        public ICollection<Durum> Durumlar { get; set; } = new List<Durum>();
    }
}
