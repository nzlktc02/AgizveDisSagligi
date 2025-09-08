using BussinesLayer.Abstract;
using DataAccessLayer.Data;
using Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BussinesLayer.Services
{
    public class SaglikManager : ISaglikService
    {
        private readonly AppDbContext _context;

        public SaglikManager(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Hedef>> GetGoalsAsync(int userId)
        {
            return await _context.Hedefler
                .Where(h => h.UserId == userId.ToString())
                .OrderByDescending(h => h.Olusturma)
                .ToListAsync();
        }

        public async Task<List<Durum>> GetLast7DaysEntriesAsync(int userId)
        {
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
            return await _context.Durumlar
                .Include(d => d.Hedef)
                .Where(d => d.UserId == userId.ToString() && d.Tarih >= sevenDaysAgo)
                .OrderByDescending(d => d.Tarih)
                .ToListAsync();
        }

        public async Task<string?> GetRandomSuggestionAsync()
        {
            var suggestions = await _context.Oneriler
                .Where(o => o.Aktif)
                .ToListAsync();

            if (!suggestions.Any())
                return null;

            var random = new Random();
            var randomSuggestion = suggestions[random.Next(suggestions.Count)];
            return randomSuggestion.Metin;
        }

        public async Task AddEntryAsync(int userId, int? goalId, DateTime tarih, TimeSpan? saat, int? sure, bool uygulandi, string? aciklama, string? gorselUrl)
        {
            Console.WriteLine($"[DEBUG] SaglikManager.AddEntryAsync - UserId: {userId}, GoalId: {goalId}, Tarih: {tarih}");
            
            var durum = new Durum
            {
                HedefId = goalId,
                Tarih = tarih,
                Saat = saat,
                Sure = sure,
                Uygulandi = uygulandi,
                DurumAciklama = aciklama,
                GorselYolu = gorselUrl,
                UserId = userId.ToString()
            };

            Console.WriteLine($"[DEBUG] SaglikManager.AddEntryAsync - Durum oluşturuldu: Tarih={durum.Tarih}, UserId={durum.UserId}");
            
            _context.Durumlar.Add(durum);
            Console.WriteLine($"[DEBUG] SaglikManager.AddEntryAsync - Context'e eklendi");
            
            var result = await _context.SaveChangesAsync();
            Console.WriteLine($"[DEBUG] SaglikManager.AddEntryAsync - SaveChanges sonucu: {result} kayıt etkilendi");
        }

        public async Task CreateGoalAsync(int userId, string baslik, string? aciklama, string periyot, string onem)
        {
            Console.WriteLine($"[DEBUG] SaglikManager.CreateGoalAsync - UserId: {userId}, Baslik: {baslik}");
            
            var hedef = new Hedef
            {
                Baslik = baslik,
                Aciklama = aciklama,
                Periyot = periyot,
                Onem = onem,
                UserId = userId.ToString(),
                Olusturma = DateTime.UtcNow
            };

            Console.WriteLine($"[DEBUG] SaglikManager.CreateGoalAsync - Hedef oluşturuldu: {hedef.Baslik}, UserId: {hedef.UserId}");
            
            _context.Hedefler.Add(hedef);
            Console.WriteLine($"[DEBUG] SaglikManager.CreateGoalAsync - Context'e eklendi");
            
            var result = await _context.SaveChangesAsync();
            Console.WriteLine($"[DEBUG] SaglikManager.CreateGoalAsync - SaveChanges sonucu: {result} kayıt etkilendi");
        }

        public async Task<bool> DeleteGoalIfNoEntriesAsync(int userId, int goalId)
        {
            var hasEntries = await _context.Durumlar
                .AnyAsync(d => d.HedefId == goalId && d.UserId == userId.ToString());

            if (hasEntries)
                return false;

            var hedef = await _context.Hedefler
                .FirstOrDefaultAsync(h => h.Id == goalId && h.UserId == userId.ToString());

            if (hedef != null)
            {
                _context.Hedefler.Remove(hedef);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task DeleteGoalCascadeAsync(int userId, int goalId)
        {
            // Delete related entries first
            var entries = await _context.Durumlar
                .Where(d => d.HedefId == goalId && d.UserId == userId.ToString())
                .ToListAsync();

            _context.Durumlar.RemoveRange(entries);

            // Delete the goal
            var hedef = await _context.Hedefler
                .FirstOrDefaultAsync(h => h.Id == goalId && h.UserId == userId.ToString());

            if (hedef != null)
            {
                _context.Hedefler.Remove(hedef);
            }

            await _context.SaveChangesAsync();
        }
    }
}
