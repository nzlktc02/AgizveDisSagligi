using BussinesLayer.Interfaces;
using DataAccessLayer.Data;
using Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BussinesLayer.Services
{
    public class HealthService : IHealthService
    {
        private readonly HealthDbContext _context;

        public HealthService(HealthDbContext context)
        {
            _context = context;
        }

        #region HealthGoal Operations

        public async Task<List<HealthGoal>> GetGoalsAsync(int userId)
        {
            return await _context.HealthGoals
                .Where(g => g.UserId == userId)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<HealthGoal?> GetGoalByIdAsync(int goalId, int userId)
        {
            return await _context.HealthGoals
                .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);
        }

        public async Task<bool> CreateGoalAsync(int userId, string title, string? description, string period, string importance)
        {
            try
            {
                var goal = new HealthGoal
                {
                    Title = title,
                    Description = description,
                    Period = period,
                    Importance = importance,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.HealthGoals.Add(goal);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateGoalAsync(int goalId, int userId, string title, string? description, string period, string importance)
        {
            try
            {
                var goal = await _context.HealthGoals
                    .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);

                if (goal == null) return false;

                goal.Title = title;
                goal.Description = description;
                goal.Period = period;
                goal.Importance = importance;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteGoalAsync(int goalId, int userId)
        {
            try
            {
                var goal = await _context.HealthGoals
                    .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);

                if (goal == null) return false;

                _context.HealthGoals.Remove(goal);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> HasStatusesForGoalAsync(int goalId, int userId)
        {
            try
            {
                return await _context.HealthStatuses
                    .AnyAsync(s => s.HealthGoalId == goalId && s.UserId == userId);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region HealthStatus Operations

        public async Task<List<HealthStatus>> GetStatusesAsync(int userId, int days = 7)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            return await _context.HealthStatuses
                .Include(s => s.HealthGoal)
                .Where(s => s.UserId == userId && s.Date >= startDate)
                .OrderByDescending(s => s.Date)
                .ToListAsync();
        }

        public async Task<HealthStatus?> GetStatusByIdAsync(int statusId, int userId)
        {
            return await _context.HealthStatuses
                .Include(s => s.HealthGoal)
                .FirstOrDefaultAsync(s => s.Id == statusId && s.UserId == userId);
        }

        public async Task<bool> CreateStatusAsync(int userId, int? goalId, DateTime date, TimeSpan? time, int? duration, bool isCompleted, string? description, string? imageUrl)
        {
            try
            {
                var status = new HealthStatus
                {
                    UserId = userId,
                    HealthGoalId = goalId,
                    Date = date,
                    Time = time,
                    Duration = duration,
                    IsCompleted = isCompleted,
                    Description = description,
                    ImageUrl = imageUrl,
                    CreatedAt = DateTime.UtcNow
                };

                _context.HealthStatuses.Add(status);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateStatusAsync(int statusId, int userId, int? goalId, DateTime date, TimeSpan? time, int? duration, bool isCompleted, string? description, string? imageUrl)
        {
            try
            {
                var status = await _context.HealthStatuses
                    .FirstOrDefaultAsync(s => s.Id == statusId && s.UserId == userId);

                if (status == null) return false;

                status.HealthGoalId = goalId;
                status.Date = date;
                status.Time = time;
                status.Duration = duration;
                status.IsCompleted = isCompleted;
                status.Description = description;
                status.ImageUrl = imageUrl;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteStatusAsync(int statusId, int userId)
        {
            try
            {
                var status = await _context.HealthStatuses
                    .FirstOrDefaultAsync(s => s.Id == statusId && s.UserId == userId);

                if (status == null) return false;

                _context.HealthStatuses.Remove(status);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region HealthSuggestion Operations

        public async Task<string?> GetRandomSuggestionAsync()
        {
            try
            {
                var suggestions = await _context.HealthSuggestions.ToListAsync();
                if (!suggestions.Any()) return null;

                var random = new Random();
                var randomSuggestion = suggestions[random.Next(suggestions.Count)];
                return randomSuggestion.Text; // Suggestion yerine Text kullan
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
