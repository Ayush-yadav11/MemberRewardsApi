using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemberRewardsApi.Models
{
    public class MemberPoint
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PurchaseAmount { get; set; }

        [Required]
        public int PointsEarned { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; } = null!;
    }
}