using AutoMapper;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models.User;

namespace CivilsAssistance_API.Controllers.Areas.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterationUserRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _unitOfWork.User.RegisterUser(model);
                return Ok(new { Message = "Registration successful. Please check your email to verify your account." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
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
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest(new { Message = "Invalid confirmation link" });
            }

            try
            {
                var result = await _unitOfWork.User.ConfirmEmailAsync(userId, token);
                if (result)
                {
                    return Ok(new { Message = "Email confirmed successfully" });
                }
                return BadRequest(new { Message = "Email confirmation failed or user not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}