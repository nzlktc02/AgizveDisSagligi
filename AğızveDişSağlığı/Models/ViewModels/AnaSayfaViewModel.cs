using Entity.Entities;

namespace AğızveDişSağlığı.Models.ViewModels
{
    public class AnaSayfaViewModel
    {
        public List<Durum> Durumlar { get; set; } = new();
        public List<HealthStatus> HealthStatuses { get; set; } = new();
        public string? GununOnerisi { get; set; }
        public string? UserName { get; set; }
    }
}
