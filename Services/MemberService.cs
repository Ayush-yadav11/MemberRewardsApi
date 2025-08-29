using Microsoft.EntityFrameworkCore;
using MemberRewardsApi.Data;
using MemberRewardsApi.DTOs;
using MemberRewardsApi.Models;

namespace MemberRewardsApi.Services
{
    public interface IMemberService
    {
        Task<MemberRegistrationResponse> RegisterMemberAsync(MemberRegistrationRequest request);
        Task<OtpVerificationResponse> VerifyOtpAsync(OtpVerificationRequest request);
    }

    public class MemberService : IMemberService
    {
        private readonly MemberRewardsDbContext _context;
        private readonly IJwtService _jwtService;

        public MemberService(MemberRewardsDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<MemberRegistrationResponse> RegisterMemberAsync(MemberRegistrationRequest request)
        {
            try
            {
                // Check if member already exists
                var existingMember = await _context.Members
                    .FirstOrDefaultAsync(m => m.MobileNumber == request.MobileNumber);

                if (existingMember != null)
                {
                    if (existingMember.IsVerified)
                    {
                        return new MemberRegistrationResponse
                        {
                            Success = false,
                            Message = "Mobile number is already registered and verified."
                        };
                    }
                    else
                    {
                        // Update existing unverified member with new OTP
                        existingMember.Otp = GenerateOtp();
                        existingMember.Name = request.Name ?? existingMember.Name;
                        existingMember.Email = request.Email ?? existingMember.Email;
                        existingMember.CreatedAt = DateTime.UtcNow;

                        await _context.SaveChangesAsync();

                        return new MemberRegistrationResponse
                        {
                            Success = true,
                            Message = "New OTP sent successfully.",
                            MemberId = existingMember.Id,
                            Otp = existingMember.Otp // For demo purposes only
                        };
                    }
                }

                // Create new member
                var newMember = new Member
                {
                    MobileNumber = request.MobileNumber,
                    Name = request.Name,
                    Email = request.Email,
                    Otp = GenerateOtp(),
                    IsVerified = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Members.Add(newMember);
                await _context.SaveChangesAsync();

                return new MemberRegistrationResponse
                {
                    Success = true,
                    Message = "Member registered successfully. Please verify OTP.",
                    MemberId = newMember.Id,
                    Otp = newMember.Otp // For demo purposes only
                };
            }
            catch (Exception ex)
            {
                return new MemberRegistrationResponse
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                };
            }
        }

        public async Task<OtpVerificationResponse> VerifyOtpAsync(OtpVerificationRequest request)
        {
            try
            {
                var member = await _context.Members
                    .FirstOrDefaultAsync(m => m.MobileNumber == request.MobileNumber);

                if (member == null)
                {
                    return new OtpVerificationResponse
                    {
                        Success = false,
                        Message = "Member not found."
                    };
                }

                if (member.IsVerified)
                {
                    return new OtpVerificationResponse
                    {
                        Success = false,
                        Message = "Member is already verified."
                    };
                }

                if (member.Otp != request.Otp)
                {
                    return new OtpVerificationResponse
                    {
                        Success = false,
                        Message = "Invalid OTP."
                    };
                }

                // Check OTP expiry (assuming 10 minutes expiry)
                if (DateTime.UtcNow > member.CreatedAt.AddMinutes(10))
                {
                    return new OtpVerificationResponse
                    {
                        Success = false,
                        Message = "OTP has expired."
                    };
                }

                // Verify member
                member.IsVerified = true;
                member.VerifiedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Generate JWT token
                var token = _jwtService.GenerateToken(member.Id, member.MobileNumber);

                return new OtpVerificationResponse
                {
                    Success = true,
                    Message = "OTP verified successfully.",
                    Token = token,
                    MemberId = member.Id
                };
            }
            catch (Exception ex)
            {
                return new OtpVerificationResponse
                {
                    Success = false,
                    Message = $"Verification failed: {ex.Message}"
                };
            }
        }

        private string GenerateOtp()
        {
            // Return fixed dummy OTP for demo purposes
            return "123456";
        }
    }
}