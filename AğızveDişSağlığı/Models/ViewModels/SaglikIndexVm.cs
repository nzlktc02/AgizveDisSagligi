using Entity.Entities;
using Ent = Entity.Entities;

namespace AğızveDişSağlığı.Models.ViewModels
{
    public class SaglikIndexVm
    {
        public List<Ent.Durum> Durumlar { get; set; } = new();
        public List<Ent.Hedef> Hedefler { get; set; } = new();
        public string? GununOnerisi { get; set; }
        public string? DailySuggestion { get; set; }

        // Formlardan gelecek alanlar
        public Ent.Durum YeniDurum { get; set; } = new Durum();
        public Ent.Hedef YeniHedef { get; set; } = new Hedef();
        
        // Health entities için
        public List<HealthGoal> Goals { get; set; } = new();
        public List<HealthStatus> Statuses { get; set; } = new();
        public HealthStatus NewStatus { get; set; } = new HealthStatus();
        public HealthGoal NewGoal { get; set; } = new HealthGoal();
    }
}
