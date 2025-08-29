using Microsoft.AspNetCore.Mvc;
using MemberRewardsApi.DTOs;
using MemberRewardsApi.Services;

namespace MemberRewardsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        /// <summary>
        /// Register a new member with mobile number
        /// </summary>
        /// <param name="request">Member registration details</param>
        /// <returns>Registration response with OTP</returns>
        [HttpPost("register")]
        public async Task<ActionResult<MemberRegistrationResponse>> RegisterMember(
            [FromBody] MemberRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new MemberRegistrationResponse
                {
                    Success = false,
                    Message = $"Invalid request data: {errors}"
                });
            }

            var response = await _memberService.RegisterMemberAsync(request);
            
            if (response.Success)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }

        /// <summary>
        /// Verify OTP for member registration
        /// </summary>
        /// <param name="request">OTP verification details</param>
        /// <returns>Verification response with JWT token</returns>
        [HttpPost("verify")]
        public async Task<ActionResult<OtpVerificationResponse>> VerifyOtp(
            [FromBody] OtpVerificationRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new OtpVerificationResponse
                {
                    Success = false,
                    Message = $"Invalid request data: {errors}"
                });
            }

            var response = await _memberService.VerifyOtpAsync(request);
            
            if (response.Success)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }
    }
}