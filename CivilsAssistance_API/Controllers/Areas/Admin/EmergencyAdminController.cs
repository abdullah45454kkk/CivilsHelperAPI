using AutoMapper;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Models.Emergencies;

namespace CivilsAssistance_API.Controllers.Areas.Admin
{
    [Route("api/admin/emergencies")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Restrict all endpoints to Admins only
    public class EmergencyAdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmergencyAdminController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("missing-persons")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Added for unauthorized access
        public async Task<IActionResult> GetAllMissingPersons()
        {
            var emergencies = await _unitOfWork.EmPerson.GetAllAsync(includeProperties: "LocalUser");
            var mapList = _mapper.Map<IEnumerable<EmergPersonDTO>>(emergencies);
            return Ok(mapList);
        }

        [HttpGet("general")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Added for unauthorized access
        public async Task<IActionResult> GetAllGeneralEmergencies()
        {
            var emergencies = await _unitOfWork.EmAnother.GetAllAsync(includeProperties: "LocalUser");
            var mapList = _mapper.Map<IEnumerable<EmergAnotherDTO>>(emergencies);
            return Ok(mapList);
        }

        [HttpPatch("missing-person/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Added for unauthorized access
        public async Task<IActionResult> UpdateMissingPersonStatus(int id, [FromBody] JsonPatchDocument<EmergPersonDTO> emergPerson)
        {
            if (emergPerson == null)
            {
                return BadRequest(new { Message = "Invalid patch document" });
            }

            var emergency = await _unitOfWork.EmPerson.Get(u => u.Id == id);
            if (emergency == null)
            {
                return NotFound(new { Message = "Emergency case not found" });
            }

            EmergPersonDTO emergPDTO = _mapper.Map<EmergPersonDTO>(emergency);
            emergPerson.ApplyTo(emergPDTO, ModelState);


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(emergPDTO, emergency);
            await _unitOfWork.EmPerson.Update(emergency);
            await _unitOfWork.Save();

            return Ok(new { Message = "Status updated successfully" });
        }

        [HttpPatch("general/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Added for unauthorized access
        public async Task<IActionResult> UpdateGeneralStatus(int id, [FromBody] JsonPatchDocument<EmergAnotherDTO> emergAnother)
        {
            if (emergAnother == null)
            {
                return BadRequest(new { Message = "Invalid patch document" }); // Fixed response to BadRequest
            }

            var emergency = await _unitOfWork.EmAnother.Get(u => u.Id == id);
            if (emergency == null)
            {
                return NotFound(new { Message = "Emergency case not found" });
            }

            EmergAnotherDTO emergAnotherDTO = _mapper.Map<EmergAnotherDTO>(emergency);
            emergAnother.ApplyTo(emergAnotherDTO, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(emergAnotherDTO, emergency);
            await _unitOfWork.EmAnother.Update(emergency);
            await _unitOfWork.Save();

            return Ok(new { Message = "Status updated successfully" }); // Fixed message consistency
        }

        [HttpDelete("missing-person/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Added for unauthorized access
        public async Task<IActionResult> DeleteMissingPerson(int id)
        {
            var emergency = await _unitOfWork.EmPerson.Get(u => u.Id == id);
            if (emergency == null)
            {
                return NotFound(new { Message = "Emergency case not found" });
            }

            await _unitOfWork.EmPerson.DeleteAsync(emergency);
            await _unitOfWork.Save();
            return Ok(new { Message = "Emergency case deleted successfully" });
        }

        [HttpDelete("general/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Added for unauthorized access
        public async Task<IActionResult> DeleteGeneral(int id)
        {
            var emergency = await _unitOfWork.EmAnother.Get(u => u.Id == id);
            if (emergency == null)
            {
                return NotFound(new { Message = "Emergency case not found" });
            }

            await _unitOfWork.EmAnother.DeleteAsync(emergency);
            await _unitOfWork.Save();
            return Ok(new { Message = "Emergency case deleted successfully" });
        }
    }
}