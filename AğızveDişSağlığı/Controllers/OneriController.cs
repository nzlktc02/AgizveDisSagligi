
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AğızveDişSağlığı.Models;

namespace AğızveDişSağlığı.Controllers
{
    [Authorize]
    public class OneriController : Controller
    {
        // /Oneri/Index?q=... (opsiyonel arama)
        public IActionResult Index(string? q)
        {
            var data = OneriStore.TumOneriler.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(q))
                data = data.Where(o => o.Metin.Contains(q, StringComparison.OrdinalIgnoreCase));

            ViewBag.Query = q;
            return View(data.ToList());
        }
    }
}
