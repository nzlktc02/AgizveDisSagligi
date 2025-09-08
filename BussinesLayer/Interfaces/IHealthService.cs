using Entity.Entities;

namespace BussinesLayer.Interfaces
{
    public interface IHealthService
    {
        // HealthGoal operations
        Task<List<HealthGoal>> GetGoalsAsync(int userId);
        Task<HealthGoal?> GetGoalByIdAsync(int goalId, int userId);
        Task<bool> CreateGoalAsync(int userId, string title, string? description, string period, string importance);
        Task<bool> UpdateGoalAsync(int goalId, int userId, string title, string? description, string period, string importance);
        Task<bool> DeleteGoalAsync(int goalId, int userId);
        Task<bool> HasStatusesForGoalAsync(int goalId, int userId);

        // HealthStatus operations
        Task<List<HealthStatus>> GetStatusesAsync(int userId, int days = 7);
        Task<HealthStatus?> GetStatusByIdAsync(int statusId, int userId);
        Task<bool> CreateStatusAsync(int userId, int? goalId, DateTime date, TimeSpan? time, int? duration, bool isCompleted, string? description, string? imageUrl);
        Task<bool> UpdateStatusAsync(int statusId, int userId, int? goalId, DateTime date, TimeSpan? time, int? duration, bool isCompleted, string? description, string? imageUrl);
        Task<bool> DeleteStatusAsync(int statusId, int userId);

        // HealthSuggestion operations
        Task<string?> GetRandomSuggestionAsync();
    }
}
