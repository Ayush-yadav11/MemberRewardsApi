using Microsoft.EntityFrameworkCore;
using MemberRewardsApi.Data;
using MemberRewardsApi.DTOs;
using MemberRewardsApi.Models;

namespace MemberRewardsApi.Services
{
    public interface ICouponService
    {
        Task<AvailableCouponsResponse> GetAvailableCouponsAsync(int memberId);
        Task<CouponRedemptionResponse> RedeemCouponAsync(CouponRedemptionRequest request);
    }

    public class CouponService : ICouponService
    {
        private readonly MemberRewardsDbContext _context;

        public CouponService(MemberRewardsDbContext context)
        {
            _context = context;
        }

        public async Task<AvailableCouponsResponse> GetAvailableCouponsAsync(int memberId)
        {
            try
            {
                var member = await _context.Members
                    .FirstOrDefaultAsync(m => m.Id == memberId && m.IsVerified);

                if (member == null)
                {
                    return new AvailableCouponsResponse
                    {
                        Success = false,
                        Message = "Member not found or not verified."
                    };
                }

                // Calculate available points
                var totalEarnedPoints = await _context.MemberPoints
                    .Where(mp => mp.MemberId == memberId)
                    .SumAsync(mp => mp.PointsEarned);

                var redeemedPoints = await _context.CouponRedemptions
                    .Where(cr => cr.MemberId == memberId)
                    .SumAsync(cr => cr.PointsRedeemed);

                var availablePoints = totalEarnedPoints - redeemedPoints;

                // Get all active coupons
                var coupons = await _context.Coupons
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.PointsRequired)
                    .Select(c => new CouponInfo
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        PointsRequired = c.PointsRequired,
                        CouponValue = c.CouponValue,
                        CanRedeem = availablePoints >= c.PointsRequired
                    })
                    .ToListAsync();

                return new AvailableCouponsResponse
                {
                    Success = true,
                    Message = $"Available points: {availablePoints}",
                    Coupons = coupons
                };
            }
            catch (Exception ex)
            {
                return new AvailableCouponsResponse
                {
                    Success = false,
                    Message = $"Failed to retrieve coupons: {ex.Message}"
                };
            }
        }

        public async Task<CouponRedemptionResponse> RedeemCouponAsync(CouponRedemptionRequest request)
        {
            try
            {
                var member = await _context.Members
                    .FirstOrDefaultAsync(m => m.Id == request.MemberId && m.IsVerified);

                if (member == null)
                {
                    return new CouponRedemptionResponse
                    {
                        Success = false,
                        Message = "Member not found or not verified."
                    };
                }

                var coupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.Id == request.CouponId && c.IsActive);

                if (coupon == null)
                {
                    return new CouponRedemptionResponse
                    {
                        Success = false,
                        Message = "Coupon not found or not active."
                    };
                }

                // Calculate available points
                var totalEarnedPoints = await _context.MemberPoints
                    .Where(mp => mp.MemberId == request.MemberId)
                    .SumAsync(mp => mp.PointsEarned);

                var redeemedPoints = await _context.CouponRedemptions
                    .Where(cr => cr.MemberId == request.MemberId)
                    .SumAsync(cr => cr.PointsRedeemed);

                var availablePoints = totalEarnedPoints - redeemedPoints;

                if (availablePoints < coupon.PointsRequired)
                {
                    return new CouponRedemptionResponse
                    {
                        Success = false,
                        Message = $"Insufficient points. Required: {coupon.PointsRequired}, Available: {availablePoints}"
                    };
                }

                // Generate coupon code
                var couponCode = GenerateCouponCode();

                // Create redemption record
                var redemption = new CouponRedemption
                {
                    MemberId = request.MemberId,
                    CouponId = request.CouponId,
                    PointsRedeemed = coupon.PointsRequired,
                    CouponCode = couponCode,
                    RedeemedAt = DateTime.UtcNow,
                    IsUsed = false
                };

                _context.CouponRedemptions.Add(redemption);
                await _context.SaveChangesAsync();

                var remainingPoints = availablePoints - coupon.PointsRequired;

                return new CouponRedemptionResponse
                {
                    Success = true,
                    Message = "Coupon redeemed successfully.",
                    CouponCode = couponCode,
                    CouponValue = coupon.CouponValue,
                    PointsRedeemed = coupon.PointsRequired,
                    RemainingPoints = remainingPoints
                };
            }
            catch (Exception ex)
            {
                return new CouponRedemptionResponse
                {
                    Success = false,
                    Message = $"Failed to redeem coupon: {ex.Message}"
                };
            }
        }

        private string GenerateCouponCode()
        {
            // Generate a unique 8-character coupon code
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}