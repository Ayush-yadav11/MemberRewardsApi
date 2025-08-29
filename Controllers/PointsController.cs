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
    public class PointsController : ControllerBase
    {
        private readonly IPointsService _pointsService;

        public PointsController(IPointsService pointsService)
        {
            _pointsService = pointsService;
        }

        /// <summary>
        /// Add points to a member's account based on purchase amount
        /// </summary>
        /// <param name="request">Add points request</param>
        /// <returns>Points addition response</returns>
        [HttpPost("add")]
        public async Task<ActionResult<AddPointsResponse>> AddPoints(
            [FromBody] AddPointsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AddPointsResponse
                {
                    Success = false,
                    Message = "Invalid request data."
                });
            }

            // Optional: Verify that the requesting user can add points for this member
            // For this demo, we'll allow any authenticated user to add points to any member
            
            var response = await _pointsService.AddPointsAsync(request);
            
            if (response.Success)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }

        /// <summary>
        /// Get total points and transaction history for a member
        /// </summary>
        /// <param name="memberId">Member ID</param>
        /// <returns>Member points details</returns>
        [HttpGet("{memberId}")]
        public async Task<ActionResult<MemberPointsResponse>> GetMemberPoints(int memberId)
        {
            var response = await _pointsService.GetMemberPointsAsync(memberId);
            
            if (response.Success)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }

        /// <summary>
        /// Get points for the currently authenticated member
        /// </summary>
        /// <returns>Current member's points details</returns>
        [HttpGet("my-points")]
        public async Task<ActionResult<MemberPointsResponse>> GetMyPoints()
        {
            var memberIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(memberIdClaim) || !int.TryParse(memberIdClaim, out int memberId))
            {
                return BadRequest(new MemberPointsResponse
                {
                    Success = false,
                    Message = "Invalid member ID in token."
                });
            }

            var response = await _pointsService.GetMemberPointsAsync(memberId);
            
            if (response.Success)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }
    }
}