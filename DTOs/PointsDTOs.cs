using System.ComponentModel.DataAnnotations;

namespace MemberRewardsApi.DTOs
{
    public class AddPointsRequest
    {
        [Required]
        public int MemberId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Purchase amount must be greater than 0")]
        public decimal PurchaseAmount { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }

    public class AddPointsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int PointsEarned { get; set; }
        public int TotalPoints { get; set; }
    }

    public class MemberPointsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int MemberId { get; set; }
        public string MobileNumber { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public List<PointTransaction> Transactions { get; set; } = new List<PointTransaction>();
    }

    public class PointTransaction
    {
        public DateTime Date { get; set; }
        public decimal PurchaseAmount { get; set; }
        public int PointsEarned { get; set; }
        public string? Description { get; set; }
    }
}