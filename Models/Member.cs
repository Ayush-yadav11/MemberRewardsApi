using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemberRewardsApi.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(15)]
        [Phone]
        public required string MobileNumber { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(200)]
        public string? Email { get; set; }

        [Required]
        [StringLength(6)]
        public required string Otp { get; set; }

        public bool IsVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? VerifiedAt { get; set; }

        // Navigation property
        public virtual ICollection<MemberPoint> MemberPoints { get; set; } = new List<MemberPoint>();
        public virtual ICollection<CouponRedemption> CouponRedemptions { get; set; } = new List<CouponRedemption>();
    }
}