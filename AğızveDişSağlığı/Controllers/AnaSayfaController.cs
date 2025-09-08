using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using AğızveDişSağlığı.Models;  // << OneriStore için
using AğızveDişSağlığı.Models.ViewModels;
using BussinesLayer.Abstract;
using BussinesLayer.Interfaces;

namespace AğızveDişSağlığı.Controllers
{
    [Authorize]
    public class AnaSayfaController : Controller
    {
        private readonly ISaglikService _saglikService;
        private readonly IHealthService _healthService;

        public AnaSayfaController(ISaglikService saglikService, IHealthService healthService)
        {
            _saglikService = saglikService;
            _healthService = healthService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var displayName = User.FindFirstValue(ClaimTypes.Name)
                              ?? User.Identity?.Name
                              ?? User.FindFirstValue(ClaimTypes.Email)
                              ?? "Kullanıcı";

            // Rastgele öneriyi tek merkezden al
            var oneri = OneriStore.Rastgele().Metin;

            // Son 7 günlük verileri getir
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Her iki DbContext'ten verileri al
            var durumlar = await _saglikService.GetLast7DaysEntriesAsync(userId);
            var healthStatuses = await _healthService.GetStatusesAsync(userId, 7);

            var viewModel = new AnaSayfaViewModel
            {
                Durumlar = durumlar,
                HealthStatuses = healthStatuses,
                GununOnerisi = oneri,
                UserName = displayName
            };

            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Cikis()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Giris", "Hesap");
        }
    }
}
