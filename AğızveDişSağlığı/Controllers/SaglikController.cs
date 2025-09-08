using AğızveDişSağlığı.Models.ViewModels;
using BussinesLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AğızveDişSağlığı.Controllers
{
    [Authorize]
    public class SaglikController : Controller
    {
        private readonly IHealthService _healthService;
        public SaglikController(IHealthService healthService) => _healthService = healthService;

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetCurrentUserId();
                var viewModel = new SaglikIndexVm
                {
                    Goals = await _healthService.GetGoalsAsync(userId),
                    Statuses = await _healthService.GetStatusesAsync(userId, 7), // Son 7 günlük veriler
                    DailySuggestion = await _healthService.GetRandomSuggestionAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Veriler yüklenirken bir hata oluştu: " + ex.Message;
                return View(new SaglikIndexVm());
            }
        }

        // POST: /Saglik/CreateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStatus(SaglikIndexVm model)
        {
            // ModelState'i temizle ve sadece kritik hataları kontrol et
            ModelState.Clear();
            
            // Sadece temel validasyonları yap
            if (model?.NewStatus == null)
            {
                TempData["ErrorMessage"] = "Form verileri eksik.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var userId = GetCurrentUserId();
                var success = await _healthService.CreateStatusAsync(
                    userId,
                    model.NewStatus.HealthGoalId,
                    model.NewStatus.Date,
                    model.NewStatus.Time,
                    model.NewStatus.Duration,
                    model.NewStatus.IsCompleted,
                    model.NewStatus.Description,
                    model.NewStatus.ImageUrl
                );

                if (success)
                {
                    TempData["SuccessMessage"] = "Durum başarıyla kaydedildi!";
                    TempData["ActiveTab"] = "status";
                }
                else
                {
                    TempData["ErrorMessage"] = "Durum kaydedilirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Durum kaydedilirken bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Saglik/CreateGoal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGoal([FromForm] string NewGoal_Title, [FromForm] string NewGoal_Description, [FromForm] string NewGoal_Period, [FromForm] string NewGoal_Importance)
        {
            try
            {
                // Validasyon
                if (string.IsNullOrWhiteSpace(NewGoal_Title))
                {
                    TempData["ErrorMessage"] = "Hedef başlığı gereklidir.";
                    TempData["ActiveTab"] = "goals";
                    return RedirectToAction(nameof(Index));
                }

                var userId = GetCurrentUserId();
                var success = await _healthService.CreateGoalAsync(
                    userId,
                    NewGoal_Title,
                    string.IsNullOrWhiteSpace(NewGoal_Description) ? null : NewGoal_Description,
                    NewGoal_Period ?? "Günde bir",
                    NewGoal_Importance ?? "Orta"
                );

                if (success)
                {
                    TempData["SuccessMessage"] = "Hedef başarıyla oluşturuldu!";
                    TempData["ActiveTab"] = "goals";
                }
                else
                {
                    TempData["ErrorMessage"] = "Hedef oluşturulurken bir hata oluştu.";
                    TempData["ActiveTab"] = "goals";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hedef oluşturulurken bir hata oluştu: " + ex.Message;
                TempData["ActiveTab"] = "goals";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Saglik/CheckGoalStatuses
        [HttpGet]
        public async Task<IActionResult> CheckGoalStatuses(int goalId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var hasStatuses = await _healthService.HasStatusesForGoalAsync(goalId, userId);
                return Json(new { hasStatuses = hasStatuses });
            }
            catch (Exception ex)
            {
                return Json(new { hasStatuses = false, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                // Önce hedefin durumları var mı kontrol et
                var hasStatuses = await _healthService.HasStatusesForGoalAsync(id, userId);
                
                if (hasStatuses)
                {
                    TempData["ErrorMessage"] = "Bu hedef için daha önce durum bilgisi eklenmiş. Hedefi silmek için önce ilgili durumları silin.";
                    TempData["ActiveTab"] = "goals";
                    return RedirectToAction(nameof(Index));
                }

                var success = await _healthService.DeleteGoalAsync(id, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Hedef başarıyla silindi!";
                    TempData["ActiveTab"] = "goals";
                }
                else
                {
                    TempData["ErrorMessage"] = "Hedef silinirken bir hata oluştu.";
                    TempData["ActiveTab"] = "goals";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hedef silinirken bir hata oluştu: " + ex.Message;
                TempData["ActiveTab"] = "goals";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _healthService.DeleteStatusAsync(id, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Durum başarıyla silindi!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Durum silinirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Durum silinirken bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
