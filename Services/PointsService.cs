using Microsoft.EntityFrameworkCore;
using MemberRewardsApi.Data;
using MemberRewardsApi.DTOs;
using MemberRewardsApi.Models;

namespace MemberRewardsApi.Services
{
    public interface IPointsService
    {
        Task<AddPointsResponse> AddPointsAsync(AddPointsRequest request);
        Task<MemberPointsResponse> GetMemberPointsAsync(int memberId);
    }

    public class PointsService : IPointsService
    {
        private readonly MemberRewardsDbContext _context;
        private const decimal PointsPerRupee = 0.1m; // 10 points per ₹100 = 0.1 points per ₹1

        public PointsService(MemberRewardsDbContext context)
        {
            _context = context;
        }

        public async Task<AddPointsResponse> AddPointsAsync(AddPointsRequest request)
        {
            try
            {
                // Verify member exists and is verified
                var member = await _context.Members
                    .FirstOrDefaultAsync(m => m.Id == request.MemberId && m.IsVerified);

                if (member == null)
                {
                    return new AddPointsResponse
                    {
                        Success = false,
                        Message = "Member not found or not verified."
                    };
                }

                // Calculate points: ₹100 = 10 points
                var pointsEarned = (int)(request.PurchaseAmount * PointsPerRupee);

                // Create member point record
                var memberPoint = new MemberPoint
                {
                    MemberId = request.MemberId,
                    PurchaseAmount = request.PurchaseAmount,
                    PointsEarned = pointsEarned,
                    Description = request.Description ?? $"Purchase of ₹{request.PurchaseAmount}",
                    CreatedAt = DateTime.UtcNow
                };

                _context.MemberPoints.Add(memberPoint);
                await _context.SaveChangesAsync();

                // Calculate total points
                var totalPoints = await _context.MemberPoints
                    .Where(mp => mp.MemberId == request.MemberId)
                    .SumAsync(mp => mp.PointsEarned);

                // Subtract redeemed points
                var redeemedPoints = await _context.CouponRedemptions
                    .Where(cr => cr.MemberId == request.MemberId)
                    .SumAsync(cr => cr.PointsRedeemed);

                var availablePoints = totalPoints - redeemedPoints;

                return new AddPointsResponse
                {
                    Success = true,
                    Message = "Points added successfully.",
                    PointsEarned = pointsEarned,
                    TotalPoints = availablePoints
                };
            }
            catch (Exception ex)
            {
                return new AddPointsResponse
                {
                    Success = false,
                    Message = $"Failed to add points: {ex.Message}"
                };
            }
        }

        public async Task<MemberPointsResponse> GetMemberPointsAsync(int memberId)
        {
            try
            {
                var member = await _context.Members
                    .FirstOrDefaultAsync(m => m.Id == memberId && m.IsVerified);

                if (member == null)
                {
                    return new MemberPointsResponse
                    {
                        Success = false,
                        Message = "Member not found or not verified."
                    };
                }

                var pointTransactions = await _context.MemberPoints
                    .Where(mp => mp.MemberId == memberId)
                    .OrderByDescending(mp => mp.CreatedAt)
                    .Select(mp => new PointTransaction
                    {
                        Date = mp.CreatedAt,
                        PurchaseAmount = mp.PurchaseAmount,
                        PointsEarned = mp.PointsEarned,
                        Description = mp.Description
                    })
                    .ToListAsync();

                var totalEarnedPoints = await _context.MemberPoints
                    .Where(mp => mp.MemberId == memberId)
                    .SumAsync(mp => mp.PointsEarned);

                var redeemedPoints = await _context.CouponRedemptions
                    .Where(cr => cr.MemberId == memberId)
                    .SumAsync(cr => cr.PointsRedeemed);

                var availablePoints = totalEarnedPoints - redeemedPoints;

                return new MemberPointsResponse
                {
                    Success = true,
                    Message = "Points retrieved successfully.",
                    MemberId = memberId,
                    MobileNumber = member.MobileNumber,
                    TotalPoints = availablePoints,
                    Transactions = pointTransactions
                };
            }
            catch (Exception ex)
            {
                return new MemberPointsResponse
                {
                    Success = false,
                    Message = $"Failed to retrieve points: {ex.Message}"
                };
            }
        }
    }
}