using System.ComponentModel.DataAnnotations;

namespace MemberRewardsApi.DTOs
{
    public class CouponRedemptionRequest
    {
        [Required]
        public int MemberId { get; set; }

        [Required]
        public int CouponId { get; set; }
    }

    public class CouponRedemptionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? CouponCode { get; set; }
        public decimal CouponValue { get; set; }
        public int PointsRedeemed { get; set; }
        public int RemainingPoints { get; set; }
    }

    public class AvailableCouponsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<CouponInfo> Coupons { get; set; } = new List<CouponInfo>();
    }

    public class CouponInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int PointsRequired { get; set; }
        public decimal CouponValue { get; set; }
        public bool CanRedeem { get; set; }
    }
}