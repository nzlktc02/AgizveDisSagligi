using System.ComponentModel.DataAnnotations;

namespace Entity.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = default!;

        [Required, StringLength(100)]
        public string PasswordHash { get; set; } = default!;

        [Required, StringLength(40)]
        public string Ad { get; set; } = default!;

        [Required, StringLength(40)]
        public string Soyad { get; set; } = default!;

        public DateTime? DogumTarihi { get; set; }

        public DateTime Olusturma { get; set; } = DateTime.UtcNow;

        public string FullName => $"{Ad} {Soyad}";
    }
}
