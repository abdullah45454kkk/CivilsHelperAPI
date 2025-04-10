using AutoMapper;
using DataAccess.PaymentService;
using DataAccess.PaymentService.IPaymentService;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Donations;
using Models.Map;
using System.Security.Claims;
using static Models.EnumsClass;

namespace CivilsAssistance_API.Controllers.Areas.User
{
    [Route("api/user/GeographicArea")]
    [ApiController]
    [Authorize] // Require authentication for all endpoints
    public class GeographicAreaUserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GeographicAreaUserController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGeographicAreas()
        {
            var areas = await _unitOfWork.GeographicArea.GetAllAsync(includeProperties:"RelatedAreas");
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
    }
}
