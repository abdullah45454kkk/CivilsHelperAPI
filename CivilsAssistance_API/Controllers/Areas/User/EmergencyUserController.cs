using AutoMapper;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Emergencies;
using System.Security.Claims;

namespace CivilsAssistance_API.Controllers.Areas.User
{
    [Route("api/user/emergencies")]
    [ApiController]
    [Authorize] // Require authentication for all endpoints
    public class EmergencyUserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmergencyUserController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost("missing-person")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReportMissingPerson([FromBody] EmergPersonDTO dto)
        {
            try
            {
                if (dto == null || !ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Invalid data.", Errors = ModelState });
                }

                var emergency = _mapper.Map<EmergPerson>(dto);
                emergency.SendAt = DateTime.UtcNow;
                emergency.Status = "Pending";
                emergency.UserId = GetUserId(); // Set UserId from JWT

                await _unitOfWork.EmPerson.AddAsync(emergency);
                await _unitOfWork.Save();

                if (emergency.Id == 0)
                {
                    return StatusCode(500, new { Error = "Failed to insert record." });
                }

                return CreatedAtAction(nameof(GetUserMissingPersons), new { id = emergency.Id }, _mapper.Map<EmergPersonDTO>(emergency));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        [HttpPost("general")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReportGeneralEmergency([FromBody] EmergAnotherDTO dto)
        {
            try
            {
                if (dto == null || !ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Invalid data.", Errors = ModelState });
                }

                var emergency = _mapper.Map<EmergAnother>(dto);
                emergency.SendAt = DateTime.UtcNow;
                emergency.Status = "Pending";
                emergency.UserId = GetUserId();
                await _unitOfWork.EmAnother.AddAsync(emergency);
                await _unitOfWork.Save();

                if (emergency.Id == 0)
                {
                    return StatusCode(500, new { Error = "Failed to insert record." });
                }

                return CreatedAtAction(nameof(GetUserGeneralEmergencies), new { id = emergency.Id }, _mapper.Map<EmergAnotherDTO>(emergency));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("missing-persons")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserMissingPersons()
        {
            var userId = GetUserId();
            var emergencies = await _unitOfWork.EmPerson.GetAllAsync(e => e.UserId == userId, includeProperties: "LocalUser");
            Console.WriteLine($"emergency:{emergencies}");

            return Ok(_mapper.Map<IEnumerable<EmergPersonDTO>>(emergencies));
        }

        [HttpGet("general")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserGeneralEmergencies()
        {
            var userId = GetUserId();
            var emergencies = await _unitOfWork.EmAnother.GetAllAsync(e => e.UserId == userId, includeProperties: "LocalUser");
            Console.WriteLine($"emergency:{emergencies}");
            return Ok(_mapper.Map<IEnumerable<EmergAnotherDTO>>(emergencies));
        }

        [HttpDelete("missing-persons/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteEmergencyUserPerson(int id)
        {
            var emergency = await _unitOfWork.EmPerson.Get(u => u.Id == id);
            if (emergency == null)
            {
                return BadRequest(new { Message = "Emergency case not found" });
            }

            if (emergency.UserId != GetUserId())
            {
                return Forbid("You can only delete your own emergency reports.");
            }

            await _unitOfWork.EmPerson.DeleteAsync(emergency);
            await _unitOfWork.Save();
            return Ok(new { Message = "Emergency case deleted successfully" });
        }

        [HttpDelete("general/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteEmergencyUserGeneral(int id)
        {
            var emergency = await _unitOfWork.EmAnother.Get(u => u.Id == id);
            if (emergency == null)
            {
                return BadRequest(new { Message = "Emergency case not found" });
            }

            if (emergency.UserId != GetUserId())
            {
                return Forbid("You can only delete your own emergency reports.");
            }

            await _unitOfWork.EmAnother.DeleteAsync(emergency);
            await _unitOfWork.Save();
            return Ok(new { Message = "Emergency case deleted successfully" });
        }

        private string GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }
            Console.WriteLine($"UserId: {userId}");
            return userId;
        }
    }
}