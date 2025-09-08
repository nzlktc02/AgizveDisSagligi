using System.ComponentModel.DataAnnotations;

namespace Entity.Entities
{
    public class HealthSuggestion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Text { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
