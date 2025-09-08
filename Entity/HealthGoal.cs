using System.ComponentModel.DataAnnotations;

namespace Entity.Entities
{
    public class HealthGoal
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hedef başlığı gereklidir")]
        [StringLength(120, ErrorMessage = "Başlık en fazla 120 karakter olabilir")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Description { get; set; }

        [Required]
        [StringLength(60)]
        public string Period { get; set; } = "Günde bir";

        [Required]
        [StringLength(20)]
        public string Importance { get; set; } = "Orta";

        [Required]
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<HealthStatus> Statuses { get; set; } = new List<HealthStatus>();
    }
}
