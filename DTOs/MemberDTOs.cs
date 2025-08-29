using System.ComponentModel.DataAnnotations;

namespace MemberRewardsApi.DTOs
{
    public class MemberRegistrationRequest
    {
        [Required]
        [StringLength(15, MinimumLength = 10)]
        public required string MobileNumber { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        [EmailAddress]
        [StringLength(200)]
        public string? Email { get; set; }
    }

    public class MemberRegistrationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? MemberId { get; set; }
        public string? Otp { get; set; } // For demo purposes only
    }

    public class OtpVerificationRequest
    {
        [Required]
        public required string MobileNumber { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public required string Otp { get; set; }
    }

    public class OtpVerificationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public int? MemberId { get; set; }
    }
}