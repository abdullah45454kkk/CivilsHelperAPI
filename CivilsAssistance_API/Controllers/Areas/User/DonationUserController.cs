using AutoMapper;
using DataAccess.PaymentService;
using DataAccess.PaymentService.IPaymentService;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Donations;
using System.Security.Claims;
using static Models.EnumsClass;

namespace CivilsAssistance_API.Controllers.Areas.User
{
    [Route("api/user/Donation")]
    [ApiController]
    [Authorize] // Require authentication for all endpoints
    public class DonationUserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaymentServices _Service;

        public DonationUserController(IUnitOfWork unitOfWork, IMapper mapper, IPaymentServices paymentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _Service = paymentService;
        }

        [HttpPost("Donation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Donate(int campaignId, [FromBody] CreateDonationDTO donationDTO)
        {
            try
            {
                if (donationDTO == null || donationDTO.Amount <= 0)
                {
                    return BadRequest(new { Message = "Invalid donation data." });
                }

                var campaign = await _unitOfWork.Campaign.Get(c => c.Id == campaignId);
                if (campaign == null)
                {
                    return NotFound(new { Message = "Campaign not found." });
                }

                if (campaign.CollectedAmount >= campaign.TargetAmount)
                {
                    return BadRequest(new { Message = "Campaign has already reached its target amount." });
                }

                if (campaign.CollectedAmount + donationDTO.Amount > campaign.TargetAmount)
                {
                    return BadRequest(new { Message = "Donation amount exceeds the remaining target amount." });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _unitOfWork.User.GetAsync(u => u.Id == userId);
                if (user == null)
                {
                    return StatusCode(500, new { Message = "Authenticated user not found." });
                }

                var donation = _mapper.Map<Donation>(donationDTO);
                donation.DonatedAt = DateTime.UtcNow;
                donation.UserId = userId;
                donation.CampaignId = campaignId;

                var paymentResult = await _Service.ProcessPaymentAsync(donation,campaign);
                if (!paymentResult.Success)
                {
                    return BadRequest(new { Message = $"Payment failed: {paymentResult.ErrorMessage}" });
                }

                campaign.Donations.Add(donation);
                campaign.CollectedAmount += donation.Amount;

                await _unitOfWork.Donation.AddAsync(donation);
                await _unitOfWork.Campaign.Update(campaign);
                await _unitOfWork.Save();

                var donationResponseDTO = _mapper.Map<DonationDTO>(donation);
                donationResponseDTO.UserId = userId;
                donationResponseDTO.DonorName = user.UserName ?? "Unknown";

                return Ok(new
                {
                    Message = "Donation processed successfully.",
                    Donation = donationResponseDTO,
                    CampaignCollectedAmount = campaign.CollectedAmount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("my-donations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyDonations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var donations = await _unitOfWork.Donation.GetAllAsync(d => d.UserId == userId, "LocalUser,Campaign");
            var donationDTOs = _mapper.Map<IEnumerable<DonationDTO>>(donations);
            return Ok(donationDTOs);
        }
        [HttpGet("Campaigns")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Campaigns()
        {
            
            var Campaigns = await _unitOfWork.Campaign.GetAllAsync();
            var CampaignsDTOs = _mapper.Map<IEnumerable<CampaignDTO>>(Campaigns);
            return Ok(CampaignsDTOs);
        }
    }
}
