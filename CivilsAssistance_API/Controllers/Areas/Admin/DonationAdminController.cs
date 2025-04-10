using AutoMapper;
using DataAccess.PaymentService.IPaymentService;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Models.Donations;
using Models.Emergencies;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Models.EnumsClass;

namespace CivilsAssistance_API.Controllers.Areas.Admin
{
    [Route("api/admin/Donation")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Restrict all endpoints to Admins only
    public class DonationAdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaymentServices _Service;


        public DonationAdminController(IUnitOfWork unitOfWork, IMapper mapper , IPaymentServices Service)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _Service = Service;
        }

        [HttpPost("CreateCampaign")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateCampaign([FromBody] CampaignDTO campaignDTO)
        {
            if (campaignDTO == null || campaignDTO.TargetAmount <= 0)
            {
                return BadRequest(new { Message = "Invalid campaign data." });
            }

            var campaign = _mapper.Map<Campaign>(campaignDTO);
            await _unitOfWork.Campaign.AddAsync(campaign);
            await _unitOfWork.Save();

            return Ok(new { Message = "Campaign created successfully.", CampaignId = campaign.Id });
        }

        [HttpGet("GetCampaigns")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCampaign()
        {

            var Campaign = await _unitOfWork.Campaign.GetAllAsync();
            var mapList = _mapper.Map<IEnumerable<CampaignDTO>>(Campaign);
            return Ok(mapList);

        }


        [HttpGet("GetDonations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDonations()
        {

            var Donation = await _unitOfWork.Donation.GetAllAsync(includeProperties: "LocalUser");
            var mapList = _mapper.Map<IEnumerable<DonationDTO>>(Donation);
            return Ok(mapList);

        }


    }
}
