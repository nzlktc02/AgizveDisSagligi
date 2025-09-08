using BussinesLayer.Interfaces;
using DataAccessLayer.Data;
using Entity.Entities;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace BussinesLayer.Services
{
    public class AuthManager : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthManager(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string email, string password, string passwordConfirm, string fullName, DateTime birthDate)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullName))
                return (false, "Tüm alanlar doldurulmalıdır.");

            if (password != passwordConfirm)
                return (false, "Parolalar eşleşmiyor.");

            if (password.Length < 8)
                return (false, "Parola en az 8 karakter olmalıdır.");

            if (!password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit))
                return (false, "Parola en az bir büyük harf, bir küçük harf ve bir rakam içermelidir.");

            if (!IsValidEmail(email))
                return (false, "Geçerli bir e-posta adresi giriniz.");

            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == email))
                return (false, "Bu e-posta adresi zaten kullanılıyor.");

            // Parse full name
            var nameParts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length < 2)
                return (false, "Ad ve soyad bilgilerini giriniz.");

            var ad = nameParts[0];
            var soyad = string.Join(" ", nameParts.Skip(1));

            // Create user
            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Ad = ad,
                Soyad = soyad,
                DogumTarihi = birthDate,
                Olusturma = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return (true, "Kayıt başarılı!");
        }

        public async Task<(bool Success, string Message, User? User)> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return (false, "E-posta veya parola hatalı.", null);

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return (false, "E-posta veya parola hatalı.", null);

            return (true, "Giriş başarılı!", user);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> CheckEmailAvailableAsync(string email, int exceptUserId = 0)
        {
            return !await _context.Users.AnyAsync(u => u.Email == email && u.Id != exceptUserId);
        }

        public async Task<(bool Exists, string Message)> CheckUserExistsAsync(string email)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == email);
            return (exists, exists ? "Kullanıcı bulundu." : "Kullanıcı bulunamadı.");
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(string email, string newPassword, string newPasswordConfirm)
        {
            if (newPassword != newPasswordConfirm)
                return (false, "Parolalar eşleşmiyor.");

            if (newPassword.Length < 8)
                return (false, "Parola en az 8 karakter olmalıdır.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return (false, "Kullanıcı bulunamadı.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            return (true, "Parola sıfırlandı!");
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateUserProfileAsync(int userId, string email, string ad, string soyad, DateTime? dogumTarihi)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            user.Email = email;
            user.Ad = ad;
            user.Soyad = soyad;
            user.DogumTarihi = dogumTarihi;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VerifyPasswordAsync(int userId, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
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
