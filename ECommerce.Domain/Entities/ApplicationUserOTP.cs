namespace ECommerce.Domain.Entities
{
    public class ApplicationUserOTP
    {
        public int Id { get; set; }
        public string OtpHash { get; set; } = string.Empty;  
        public string Purpose { get; set; } = string.Empty;   
        public bool IsUsed { get; set; } = false;             
        public int AttemptsCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ValidTo { get; set; } = DateTime.UtcNow.AddMinutes(10);
        public string ApplicationUserId { get; set; } = string.Empty;

    }
}
