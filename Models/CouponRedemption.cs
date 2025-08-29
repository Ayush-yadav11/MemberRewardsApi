using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemberRewardsApi.Models
{
    public class CouponRedemption
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        public int CouponId { get; set; }

        [Required]
        public int PointsRedeemed { get; set; }

        [StringLength(50)]
        public string CouponCode { get; set; } = string.Empty;

        public DateTime RedeemedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UsedAt { get; set; }

        public bool IsUsed { get; set; } = false;

        // Navigation properties
        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; } = null!;

        [ForeignKey("CouponId")]
        public virtual Coupon Coupon { get; set; } = null!;
    }
}