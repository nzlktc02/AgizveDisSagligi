using AğızveDişSağlığı.Models.ViewModels;
using BussinesLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AğızveDişSağlığı.Controllers
{
    [Authorize]
    public class HealthController : Controller
    {
        private readonly IHealthService _healthService;

        public HealthController(IHealthService healthService)
        {
            _healthService = healthService;
        }

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
                var viewModel = new HealthIndexViewModel
                {
                    Goals = await _healthService.GetGoalsAsync(userId),
                    Statuses = await _healthService.GetStatusesAsync(userId),
                    DailySuggestion = await _healthService.GetRandomSuggestionAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Veriler yüklenirken bir hata oluştu: " + ex.Message;
                return View(new HealthIndexViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGoal(HealthIndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Lütfen tüm gerekli alanları doldurun.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var userId = GetCurrentUserId();
                var success = await _healthService.CreateGoalAsync(
                    userId,
                    model.NewGoal.Title,
                    model.NewGoal.Description,
                    model.NewGoal.Period,
                    model.NewGoal.Importance
                );

                if (success)
                {
                    TempData["SuccessMessage"] = "Hedef başarıyla oluşturuldu!";
                    TempData["ActiveTab"] = "goals";
                }
                else
                {
                    TempData["ErrorMessage"] = "Hedef oluşturulurken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hedef oluşturulurken bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStatus(HealthIndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Lütfen tüm gerekli alanları doldurun.";
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _healthService.DeleteGoalAsync(id, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Hedef başarıyla silindi!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Hedef silinirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hedef silinirken bir hata oluştu: " + ex.Message;
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
