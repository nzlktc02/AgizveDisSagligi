namespace AğızveDişSağlığı.Models
{
    public record OneriItem(int Id, string Metin);

    public static class OneriStore
    {
        // Tek yerde dursun:
        private static readonly string[] _hamListe = new[]
        {
            "Diş ipini akşam fırçalamadan önce kullan; ara yüz plaklarını çok daha iyi temizler.",
            "Diş fırçanı her 3 ayda bir yenileyin.",
            "Florürlü diş macunu kullanın; çürük riskini azaltır.",
            "Dişlerinizi en az 2 dakika fırçalayın.",
            "Dilinizi de fırçalamayı unutmayın; ağız kokusunu önler.",
            "Şekerli yiyecek ve içecekleri sınırlayın.",
            "Sigara kullanmayın; diş eti hastalıklarını tetikler.",
            "Günde en az 2 kere dişlerinizi fırçalayın.",
            "Asitli içeceklerden sonra hemen diş fırçalamayın; 30 dk bekleyin.",
            "Diş hekiminize yılda en az 2 kez kontrole gidin.",
            "Diş fırçalama hareketlerini dairesel yapın; diş etlerini zedelemeyin.",
            "Dişlerinizi çok sert fırçalamayın; mineye zarar verebilir.",
            "Ağız gargarası kullanarak bakterileri azaltın.",
            "Gece uyumadan önce mutlaka dişlerinizi fırçalayın.",
            "Diş ipi diş fırçasının ulaşamadığı bölgeleri temizler.",
            "Diş etlerinizde kanama varsa geciktirmeden diş hekimine gidin.",
            "Bol su için; ağızda tükürük üretimini artırır.",
            "Diş sıkma alışkanlığınız varsa gece plağı kullanın.",
            "Kahve ve çay tüketimini azaltarak dişlerinizin beyaz kalmasını sağlayın.",
            "Çiğneme sırasında dişlerde ağrı varsa kontrol ettirin; çürük belirtisi olabilir."
        };

        public static IReadOnlyList<OneriItem> TumOneriler { get; } =
            _hamListe.Select((m, i) => new OneriItem(i + 1, m)).ToList();

        private static readonly Random _rnd = new();

        public static OneriItem Rastgele() =>
            TumOneriler[_rnd.Next(TumOneriler.Count)];
    }
}
