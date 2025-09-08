using System.ComponentModel.DataAnnotations;

namespace Entity.Entities
{
    public class Oneri
    {
        public int Id { get; set; }

        [Required, StringLength(500)]
        public string Metin { get; set; } = default!;

        public bool Aktif { get; set; } = true;
    }
}
