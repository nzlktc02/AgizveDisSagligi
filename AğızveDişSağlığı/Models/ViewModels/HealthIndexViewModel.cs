using Entity.Entities;
using System.ComponentModel.DataAnnotations;

namespace AğızveDişSağlığı.Models.ViewModels
{
    public class HealthIndexViewModel
    {
        public List<HealthGoal> Goals { get; set; } = new();
        public List<HealthStatus> Statuses { get; set; } = new();
        public string? DailySuggestion { get; set; }

        // Form models
        public HealthGoalFormModel NewGoal { get; set; } = new();
        public HealthStatusFormModel NewStatus { get; set; } = new();
    }

    public class HealthGoalFormModel
    {
        [Required(ErrorMessage = "Hedef başlığı gereklidir")]
        [StringLength(120, ErrorMessage = "Başlık en fazla 120 karakter olabilir")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Description { get; set; }

        [Required]
        public string Period { get; set; } = "Günde bir";

        [Required]
        public string Importance { get; set; } = "Orta";
    }

    public class HealthStatusFormModel
    {
        public int? HealthGoalId { get; set; }

        public DateTime Date { get; set; } = DateTime.Today;

        public TimeSpan? Time { get; set; }
        public int? Duration { get; set; }
        public bool IsCompleted { get; set; }

        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
    }
}
