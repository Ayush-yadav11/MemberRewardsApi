using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MemberRewardsApi.DTOs;
using MemberRewardsApi.Services;
using System.Security.Claims;

namespace MemberRewardsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        /// <summary>
        /// Get available coupons for a member
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <returns>List of available coupons</returns>
        [HttpGet("available/{memberId}")]
        public async Task<ActionResult<AvailableCouponsResponse>> GetAvailableCoupons(int memberId)
        {
            var response = await _couponService.GetAvailableCouponsAsync(memberId);
            
            if (response.Success)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }

        /// <summary>
        /// Get available coupons for the currently authenticated member
        /// </summary>
        /// <returns>List of available coupons</returns>
        [HttpGet("my-coupons")]
        public async Task<ActionResult<AvailableCouponsResponse>> GetMyCoupons()
        {
            var memberIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(memberIdClaim) || !int.TryParse(memberIdClaim, out int memberId))
            {
                return BadRequest(new AvailableCouponsResponse
                {
                    Success = false,
                    Message = "Invalid member ID in token."
                });
            }

            var response = await _couponService.GetAvailableCouponsAsync(memberId);
            
            if (response.Success)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }

        /// <summary>
        /// Redeem a coupon using points
        /// </summary>
        /// <param name="request">Coupon redemption request</param>
        /// <returns>Redemption response with coupon code</returns>
        [HttpPost("redeem")]
        public async Task<ActionResult<CouponRedemptionResponse>> RedeemCoupon(
            [FromBody] CouponRedemptionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new CouponRedemptionResponse
                {
                    Success = false,
                    Message = "Invalid request data."
                });
            }

            var response = await _couponService.RedeemCouponAsync(request);
            
            if (response.Success)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }

        /// <summary>
        /// Redeem a coupon for the currently authenticated member
        /// </summary>
        /// <param name="couponId">Coupon ID to redeem</param>
        /// <returns>Redemption response with coupon code</returns>
        [HttpPost("redeem/{couponId}")]
        public async Task<ActionResult<CouponRedemptionResponse>> RedeemMyCoupon(int couponId)
        {
            var memberIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(memberIdClaim) || !int.TryParse(memberIdClaim, out int memberId))
            {
                return BadRequest(new CouponRedemptionResponse
                {
                    Success = false,
                    Message = "Invalid member ID in token."
                });
            }

            var request = new CouponRedemptionRequest
            {
                MemberId = memberId,
                CouponId = couponId
            };

            var response = await _couponService.RedeemCouponAsync(request);
            
            if (response.Success)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }
    }
}