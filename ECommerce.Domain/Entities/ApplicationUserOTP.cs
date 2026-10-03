namespace ECommerce.Domain.Entities
{
    public class ApplicationUserOTP
    {
        public int Id { get; set; }
        public string OTP { get; set; } = string.Empty;
        public bool IsValid { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ValidTo { get; set; } = DateTime.UtcNow.AddDays(1);
        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
