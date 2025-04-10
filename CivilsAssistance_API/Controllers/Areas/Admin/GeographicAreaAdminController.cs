using AutoMapper;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Map;
using Models.User;

namespace CivilsAssistance_API.Controllers.Areas.Admin
{
    [Route("api/admin/geographic-areas")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Restrict entire controller to Admins
    public class GeographicAreaAdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<LocalUser> _userManager; // For Identity operations

        public GeographicAreaAdminController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<LocalUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("GetAreas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllGeographicAreas()
        {
            var areas = await _unitOfWork.GeographicArea.GetAllAsync(includeProperties: "RelatedAreas");
            var areaDTOs = areas.Select(area => new GeographicAreaDto
            {
                Id = area.Id,
                AreaName = area.AreaName,
                NorthWest = new GeoCoordinateDto
                {
                    Latitude = area.NorthWest.Latitude,
                    Longitude = area.NorthWest.Longitude
                },
                SouthEast = new GeoCoordinateDto
                {
                    Latitude = area.SouthEast.Latitude,
                    Longitude = area.SouthEast.Longitude
                },
                RelatedAreaIds = area.RelatedAreas.Select(ra => ra.Id).ToList()
            }).ToList();

            return Ok(areaDTOs);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGeographicArea(int id)
        {
            var area = await _unitOfWork.GeographicArea.Get(a => a.Id == id, "RelatedAreas");
            if (area == null)
            {
                return NotFound(new { Message = "Geographic area not found." });
            }

            var areaDTO = new GeographicAreaDto
            {
                Id = area.Id,
                AreaName = area.AreaName,
                NorthWest = new GeoCoordinateDto
                {
                    Latitude = area.NorthWest.Latitude,
                    Longitude = area.NorthWest.Longitude
                },
                SouthEast = new GeoCoordinateDto
                {
                    Latitude = area.SouthEast.Latitude,
                    Longitude = area.SouthEast.Longitude
                },
                RelatedAreaIds = area.RelatedAreas.Select(ra => ra.Id).ToList()
            };

            return Ok(areaDTO);
        }

        [HttpPost("CreateArea")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateGeographicArea([FromBody] CreateGeographicAreaDto createDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var area = new GeographicArea
            {
                AreaName = createDTO.AreaName,
                NorthWest = new GeoCoordinate
                {
                    Latitude = createDTO.NorthWest.Latitude,
                    Longitude = createDTO.NorthWest.Longitude
                },
                SouthEast = new GeoCoordinate
                {
                    Latitude = createDTO.SouthEast.Latitude,
                    Longitude = createDTO.SouthEast.Longitude
                }
            };

            if (createDTO.RelatedAreaIds != null && createDTO.RelatedAreaIds.Any())
            {
                foreach (var relatedAreaId in createDTO.RelatedAreaIds)
                {
                    var relatedArea = await _unitOfWork.GeographicArea.Get(a => a.Id == relatedAreaId);
                    if (relatedArea != null)
                    {
                        area.RelatedAreas.Add(relatedArea);
                    }
                }
            }

            await _unitOfWork.GeographicArea.AddAsync(area);
            await _unitOfWork.Save();

            return CreatedAtAction(nameof(GetGeographicArea), new { id = area.Id }, new { Message = "Geographic area created successfully." });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateGeographicArea(int id, [FromBody] CreateGeographicAreaDto updateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var area = await _unitOfWork.GeographicArea.Get(a => a.Id == id, "RelatedAreas");
            if (area == null)
            {
                return NotFound(new { Message = "Geographic area not found." });
            }

            area.AreaName = updateDTO.AreaName;
            area.NorthWest.Latitude = updateDTO.NorthWest.Latitude;
            area.NorthWest.Longitude = updateDTO.NorthWest.Longitude;
            area.SouthEast.Latitude = updateDTO.SouthEast.Latitude;
            area.SouthEast.Longitude = updateDTO.SouthEast.Longitude;

            area.RelatedAreas.Clear();
            if (updateDTO.RelatedAreaIds != null && updateDTO.RelatedAreaIds.Any())
            {
                foreach (var relatedAreaId in updateDTO.RelatedAreaIds)
                {
                    var relatedArea = await _unitOfWork.GeographicArea.Get(a => a.Id == relatedAreaId);
                    if (relatedArea != null)
                    {
                        area.RelatedAreas.Add(relatedArea);
                    }
                }
            }

            await _unitOfWork.GeographicArea.Update(area);
            await _unitOfWork.Save();

            return Ok(new { Message = "Geographic area updated successfully." });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteGeographicArea(int id)
        {
            var area = await _unitOfWork.GeographicArea.Get(a => a.Id == id);
            if (area == null)
            {
                return NotFound(new { Message = "Geographic area not found." });
            }

            await _unitOfWork.GeographicArea.DeleteAsync(area);
            await _unitOfWork.Save();

            return Ok(new { Message = "Geographic area deleted successfully." });
        }
    }
}
