using System.ComponentModel.DataAnnotations;

namespace Entity.Entities
{
    public class HealthStatus
    {
        public int Id { get; set; }

        public int? HealthGoalId { get; set; }
        public HealthGoal? HealthGoal { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Today;

        public TimeSpan? Time { get; set; }
        public int? Duration { get; set; } // dakika
        public bool IsCompleted { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
