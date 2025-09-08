
using BussinesLayer.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AğızveDişSağlığı.Models.ViewModels;
using AğızveDişSağlığı.Services;

namespace AğızveDişSağlığı.Controllers
{
    public class HesapController : Controller
    {
        private readonly IAuthService _auth;
        private readonly IEmailService _emailService;
        public HesapController(IAuthService auth, IEmailService emailService)
        {
            _auth = auth;
            _emailService = emailService;
        }

        private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // ===== KAYIT =====
        [HttpGet, AllowAnonymous]
        public IActionResult Kayit() => View();

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Kayit(
            string mail, string mail2,
            string sifre, string sifre2,
            string ad, string soyad,
            DateTime dogumTarihi)
        {
            // E-posta kontrolü
            if (string.IsNullOrWhiteSpace(mail) || mail != mail2)
            {
                ViewBag.Error = "E-postalar eşleşmiyor.";
                return View();
            }

            // Ad soyad kontrolü
            if (string.IsNullOrWhiteSpace(ad) || string.IsNullOrWhiteSpace(soyad))
            {
                ViewBag.Error = "Ad ve soyad alanları doldurulmalıdır.";
                return View();
            }

            string adSoyad = $"{ad.Trim()} {soyad.Trim()}";

            var (ok, msg) = await _auth.RegisterAsync(mail, sifre, sifre2, adSoyad, dogumTarihi);
            if (!ok)
            {
                ViewBag.Error = msg;
                return View();
            }

            // Hoş geldin e-postası
            try
            {
                await _emailService.SendWelcomeEmailAsync(mail, adSoyad);
            }
            catch
            {
                // Email gönderilemese bile kayıt başarılıdır
            }

            TempData["Ok"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
            return RedirectToAction(nameof(Giris));
        }

        // ===== GİRİŞ =====
        [HttpGet, AllowAnonymous]
        public IActionResult Giris() => View();

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Giris(string mail, string sifre, bool beniHatirla)
        {
            if (string.IsNullOrWhiteSpace(mail) || string.IsNullOrWhiteSpace(sifre))
            {
                ViewBag.Error = "E-posta ve parola gereklidir.";
                return View();
            }

            var (ok, msg, user) = await _auth.LoginAsync(mail, sifre);
            if (!ok)
            {
                ViewBag.Error = "E-posta veya parola hatalı.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Ad ?? ""),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = beniHatirla
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "AnaSayfa");
        }

        // ===== ÇIKIŞ =====
        [Authorize]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Cikis()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Giris));
        }

        // ====== PROFİL (GET) ======
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profil()
        {
            var u = await _auth.GetUserByIdAsync(CurrentUserId());
            if (u == null) return RedirectToAction(nameof(Giris));

            var vm = new ProfileViewModel
            {
                Mail = u.Email,
                Ad = u.Ad ?? string.Empty,
                Soyad = u.Soyad ?? string.Empty,
                DogumTarihi = u.DogumTarihi
            };
            return View(vm);
        }

        // ====== PROFİL (POST - Güncelle) ======
        [Authorize]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Profil(ProfileViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Lütfen tüm gerekli alanları doldurun.";
                return View(vm);
            }

            if (!IsValidEmail(vm.Mail))
            {
                ModelState.AddModelError(nameof(vm.Mail), "Geçerli bir e-posta adresi giriniz.");
                TempData["ErrorMessage"] = "Geçerli bir e-posta adresi giriniz.";
                return View(vm);
            }

            var myId = CurrentUserId();

            var available = await _auth.CheckEmailAvailableAsync(vm.Mail, exceptUserId: myId);
            if (!available)
            {
                ModelState.AddModelError(nameof(vm.Mail), "Bu e-posta başka bir kullanıcıda kayıtlı.");
                TempData["ErrorMessage"] = "Bu e-posta başka bir kullanıcıda kayıtlı.";
                return View(vm);
            }

            var ok = await _auth.UpdateUserProfileAsync(
               myId,
                vm.Mail,
                vm.Ad,
                vm.Soyad,
                vm.DogumTarihi);

            if (!ok)
            {
                TempData["ErrorMessage"] = "Profil güncellenemedi. Lütfen tekrar deneyiniz.";
                return View(vm);
            }

            if (!string.IsNullOrWhiteSpace(vm.YeniParola))
            {
                var currentUser = await _auth.GetUserByIdAsync(myId);
                if (currentUser == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                    return View(vm);
                }

                var passwordOk = await _auth.VerifyPasswordAsync(myId, vm.MevcutParola);
                if (!passwordOk)
                {
                    ModelState.AddModelError(nameof(vm.MevcutParola), "Mevcut parola yanlış.");
                    TempData["ErrorMessage"] = "Mevcut parola yanlış.";
                    return View(vm);
                }

                if (vm.YeniParola.Length < 8 ||
                    !vm.YeniParola.Any(char.IsUpper) ||
                    !vm.YeniParola.Any(char.IsLower) ||
                    !vm.YeniParola.Any(char.IsDigit))
                {
                    ModelState.AddModelError(nameof(vm.YeniParola), "Parola kriterlerine uygun değil.");
                    TempData["ErrorMessage"] = "Parola en az 8 karakter, 1 büyük harf, 1 küçük harf ve 1 rakam içermelidir.";
                    return View(vm);
                }

                if (vm.YeniParola != vm.YeniParolaTekrar)
                {
                    ModelState.AddModelError(nameof(vm.YeniParolaTekrar), "Yeni parolalar uyuşmuyor.");
                    TempData["ErrorMessage"] = "Yeni parolalar uyuşmuyor.";
                    return View(vm);
                }

                var passwordChanged = await _auth.ChangePasswordAsync(myId, vm.MevcutParola, vm.YeniParola);
                if (!passwordChanged)
                {
                    TempData["ErrorMessage"] = "Parola değiştirilemedi.";
                    return View(vm);
                }
            }

            TempData["SuccessMessage"] = "Profil bilgileriniz başarıyla güncellendi.";
            return RedirectToAction(nameof(Profil));
        }

        // ====== PAROLA DEĞİŞTİR ======
        [Authorize]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ParolaDegistir(ChangePasswordViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["PwdErrors"] = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction(nameof(Profil));
            }

            var ok = await _auth.ChangePasswordAsync(CurrentUserId(), vm.EskiParola, vm.YeniParola);
            if (!ok)
            {
                TempData["PwdErrors"] = "Eski parola yanlış ya da işlem başarısız.";
                return RedirectToAction(nameof(Profil));
            }

            TempData["Ok"] = "Parolanız güncellendi.";
            return RedirectToAction(nameof(Profil));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> CheckEmail(string mail, int? exceptUserId)
        {
            var available = await _auth.CheckEmailAvailableAsync(mail, exceptUserId ?? 0);
            return Json(new { available });
        }

        [HttpGet, AllowAnonymous]
        public IActionResult ParolaHatirlat()
        {
            ViewBag.ShowPasswordForm = false;
            return View();
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ParolaHatirlat(string mail, string? newPassword, string? confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(mail))
            {
                ViewBag.Error = "E-posta adresi gereklidir.";
                ViewBag.ShowPasswordForm = false;
                return View();
            }

            if (!IsValidEmail(mail))
            {
                ViewBag.Error = "Geçerli bir e-posta adresi giriniz.";
                ViewBag.ShowPasswordForm = false;
                return View();
            }

            var user = await _auth.GetUserByEmailAsync(mail);
            if (user == null)
            {
                ViewBag.Error = "Bu e-posta adresine kayıtlı kullanıcı bulunamadı.";
                ViewBag.ShowPasswordForm = false;
                return View();
            }

            if (!string.IsNullOrWhiteSpace(newPassword) && !string.IsNullOrWhiteSpace(confirmPassword))
            {
                if (newPassword.Length < 8 ||
                    !newPassword.Any(char.IsUpper) ||
                    !newPassword.Any(char.IsLower) ||
                    !newPassword.Any(char.IsDigit))
                {
                    ViewBag.Error = "Parola kriterlerine uygun değil.";
                    ViewBag.ShowPasswordForm = true;
                    ViewBag.Mail = mail;
                    return View();
                }

                if (newPassword != confirmPassword)
                {
                    ViewBag.Error = "Parolalar uyuşmuyor.";
                    ViewBag.ShowPasswordForm = true;
                    ViewBag.Mail = mail;
                    return View();
                }

                var (success, message) = await _auth.ResetPasswordAsync(mail, newPassword, confirmPassword);
                if (success)
                {
                    ViewBag.Success = "Parolanız başarıyla sıfırlandı.";
                    ViewBag.ShowPasswordForm = false;
                    ViewBag.Mail = "";
                }
                else
                {
                    ViewBag.Error = message;
                    ViewBag.ShowPasswordForm = true;
                    ViewBag.Mail = mail;
                }
            }
            else
            {
                ViewBag.Success = "E-posta adresiniz doğrulandı. Yeni parolanızı belirleyin.";
                ViewBag.ShowPasswordForm = true;
                ViewBag.Mail = mail;
            }

            return View();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
