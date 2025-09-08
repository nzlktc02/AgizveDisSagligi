using Entity.Entities;

namespace BussinesLayer.Abstract
{
    public interface ISaglikService
    {
        Task<List<Hedef>> GetGoalsAsync(int userId);
        Task<List<Durum>> GetLast7DaysEntriesAsync(int userId);
        Task<string?> GetRandomSuggestionAsync();
        Task AddEntryAsync(int userId, int? goalId, DateTime tarih, TimeSpan? saat, int? sure, bool uygulandi, string? aciklama, string? gorselUrl);
        Task CreateGoalAsync(int userId, string baslik, string? aciklama, string periyot, string onem);
        Task<bool> DeleteGoalIfNoEntriesAsync(int userId, int goalId);
        Task DeleteGoalCascadeAsync(int userId, int goalId);
    }
}
