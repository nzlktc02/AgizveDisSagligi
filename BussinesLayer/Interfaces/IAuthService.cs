using Entity.Entities;

namespace BussinesLayer.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message)> RegisterAsync(string email, string password, string passwordConfirm, string fullName, DateTime birthDate);
        Task<(bool Success, string Message, User? User)> LoginAsync(string email, string password);
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> CheckEmailAvailableAsync(string email, int exceptUserId = 0);
        Task<(bool Exists, string Message)> CheckUserExistsAsync(string email);
        Task<(bool Success, string Message)> ResetPasswordAsync(string email, string newPassword, string newPasswordConfirm);
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task<bool> UpdateUserProfileAsync(int userId, string email, string ad, string soyad, DateTime? dogumTarihi);
        Task<bool> VerifyPasswordAsync(int userId, string password);
    }
}
