using AutoMapper;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.User;
using System.Linq;
using System.Security.Claims;

namespace CivilsAssistance_API.Controllers.Areas.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Restrict entire controller to Admins
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterationRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _unitOfWork.User.RegisterAdmin(model);
                return Ok(new { Message = "Admin registered successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous] // Allow unauthenticated access to login
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _unitOfWork.User.Login(model);
                if (string.IsNullOrEmpty(response.UserName))
                {
                    return Unauthorized(new { Message = "Invalid username or password" });
                }

                // Parse JWT token to check roles
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(response.Token);
                var isAdmin = token.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin");

                if (!isAdmin)
                {
                    return Unauthorized(new { Message = "User is not an Admin" });
                }

                return Ok(new
                {
                    UserName = response.UserName,
                    Token = response.Token,
                    Role = "Admin"
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }
    }
}