using System.ComponentModel.DataAnnotations;

namespace AğızveDişSağlığı.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "E-posta adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Mail { get; set; } = "";

        [Required(ErrorMessage = "Ad gereklidir.")]
        [StringLength(40, ErrorMessage = "Ad en fazla 40 karakter olabilir.")]
        public string Ad { get; set; } = "";

        [Required(ErrorMessage = "Soyad gereklidir.")]
        [StringLength(40, ErrorMessage = "Soyad en fazla 40 karakter olabilir.")]
        public string Soyad { get; set; } = "";

        [DataType(DataType.Date)]
        public DateTime? DogumTarihi { get; set; }

        // Parola değiştirme için
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Parola en az 8 karakter olmalıdır.")]
        public string? YeniParola { get; set; }

        [DataType(DataType.Password)]
        [Compare("YeniParola", ErrorMessage = "Parolalar uyuşmuyor.")]
        public string? YeniParolaTekrar { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Mevcut parola gereklidir.")]
        public string MevcutParola { get; set; } = "";
    }

    public class ChangePasswordViewModel
    {
        [Required, DataType(DataType.Password)]
        public string EskiParola { get; set; } = "";

        [Required, DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Parola en az 8 karakter olmalı.")]
        public string YeniParola { get; set; } = "";

        [Required, DataType(DataType.Password)]
        [Compare("YeniParola", ErrorMessage = "Parolalar uyuşmuyor.")]
        public string YeniParolaTekrar { get; set; } = "";
    }
}
