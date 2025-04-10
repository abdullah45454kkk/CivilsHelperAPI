using AutoMapper;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.User;
using System.IdentityModel.Tokens.Jwt;
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
        private readonly IMapper _mapper;
        private readonly UserManager<LocalUser> _userManager; // For Identity operations

        public AdminController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<LocalUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpPost("register-admin")]
        [AllowAnonymous]
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
        [HttpGet("users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                // Get users in the "User" role
                var usersInRole = await _userManager.GetUsersInRoleAsync("User");
                var userDtos = _mapper.Map<IEnumerable<UserDTO>>(usersInRole);
                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("admins")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAdmins()
        {
            try
            {
                // Get users in the "Admin" role
                var adminsInRole = await _userManager.GetUsersInRoleAsync("Admin");
                var adminDtos = _mapper.Map<IEnumerable<UserDTO>>(adminsInRole);
                return Ok(adminDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        [HttpDelete("user/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { Message = "User ID is required." });
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found." });
                }

                // Check if the user is in the "User" role
                if (!await _userManager.IsInRoleAsync(user, "User"))
                {
                    return BadRequest(new { Message = "This user is not in the 'User' role." });
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return StatusCode(500, new { Error = "Failed to delete user.", Details = result.Errors });
                }

                return Ok(new { Message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpDelete("admin/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAdmin(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { Message = "Admin ID is required." });
                }

                var admin = await _userManager.FindByIdAsync(id);
                if (admin == null)
                {
                    return NotFound(new { Message = "Admin not found." });
                }

                // Check if the user is in the "Admin" role
                if (!await _userManager.IsInRoleAsync(admin, "Admin"))
                {
                    return BadRequest(new { Message = "This user is not in the 'Admin' role." });
                }
                var allAdmins = await _userManager.GetUsersInRoleAsync("Admin");
                if (allAdmins.Count == 1 && allAdmins.First().Id == id)
                {
                    return BadRequest(new { Message = "Cannot delete the last admin. At least one admin must remain to control the system." });
                }

                var result = await _userManager.DeleteAsync(admin);
                if (!result.Succeeded)
                {
                    return StatusCode(500, new { Error = "Failed to delete admin.", Details = result.Errors });
                }

                return Ok(new { Message = "Admin deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        [HttpPatch("user/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] JsonPatchDocument<UserDTO> patchDoc)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || patchDoc == null)
                {
                    return BadRequest(new { Message = "User ID and patch document are required." });
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found." });
                }

                if (!await _userManager.IsInRoleAsync(user, "User"))
                {
                    return BadRequest(new { Message = "This user is not in the 'User' role." });
                }

                // Map to DTO, apply patch, and map back
                var userDTO = _mapper.Map<UserDTO>(user);
                patchDoc.ApplyTo(userDTO, ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Update only the patched fields
                user.UserName = userDTO.UserName;
                user.Email = userDTO.Email;
                user.FirstName = userDTO.FirstName;
                user.LastName = userDTO.LastName;
                user.Phone = userDTO.Phone;
                user.Address = userDTO.Address;
                user.SSN = userDTO.SSN;
                user.ImageProfile = userDTO.ImageProfile;
                user.EmailConfirmed = userDTO.EmailConfirmed;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return StatusCode(500, new { Error = "Failed to update user.", Details = result.Errors });
                }

                return Ok(new { Message = "User updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPatch("admin/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAdmin(string id, [FromBody] JsonPatchDocument<UserDTO> patchDoc)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || patchDoc == null)
                {
                    return BadRequest(new { Message = "Admin ID and patch document are required." });
                }

                var admin = await _userManager.FindByIdAsync(id);
                if (admin == null)
                {
                    return NotFound(new { Message = "Admin not found." });
                }

                if (!await _userManager.IsInRoleAsync(admin, "Admin"))
                {
                    return BadRequest(new { Message = "This user is not in the 'Admin' role." });
                }

                // Map to DTO, apply patch, and map back
                var adminDTO = _mapper.Map<UserDTO>(admin);
                patchDoc.ApplyTo(adminDTO, ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Update only the patched fields
                admin.UserName = adminDTO.UserName;
                admin.Email = adminDTO.Email;
                admin.FirstName = adminDTO.FirstName;
                admin.LastName = adminDTO.LastName;
                admin.Phone = adminDTO.Phone;
                admin.Address = adminDTO.Address;
                admin.SSN = adminDTO.SSN;
                admin.ImageProfile = adminDTO.ImageProfile;
                admin.EmailConfirmed = adminDTO.EmailConfirmed;

                var result = await _userManager.UpdateAsync(admin);
                if (!result.Succeeded)
                {
                    return StatusCode(500, new { Error = "Failed to update admin.", Details = result.Errors });
                }

                return Ok(new { Message = "Admin updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        

        [HttpPost("user/{id}/change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeUserPassword(string id, [FromBody] PasswordChangeDTO passwordChange)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || passwordChange == null ||
                    string.IsNullOrEmpty(passwordChange.CurrentPassword) ||
                    string.IsNullOrEmpty(passwordChange.NewPassword))
                {
                    return BadRequest(new { Message = "User ID, current password, and new password are required." });
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found." });
                }

                if (!await _userManager.IsInRoleAsync(user, "User"))
                {
                    return BadRequest(new { Message = "This user is not in the 'User' role." });
                }

                var result = await _userManager.ChangePasswordAsync(user, passwordChange.CurrentPassword, passwordChange.NewPassword);
                if (!result.Succeeded)
                {
                    return BadRequest(new { Error = "Failed to change password.", Details = result.Errors });
                }

                return Ok(new { Message = "User password changed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("admin/{id}/change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeAdminPassword(string id, [FromBody] PasswordChangeDTO passwordChange)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || passwordChange == null ||
                    string.IsNullOrEmpty(passwordChange.CurrentPassword) ||
                    string.IsNullOrEmpty(passwordChange.NewPassword))
                {
                    return BadRequest(new { Message = "Admin ID, current password, and new password are required." });
                }

                var admin = await _userManager.FindByIdAsync(id);
                if (admin == null)
                {
                    return NotFound(new { Message = "Admin not found." });
                }

                if (!await _userManager.IsInRoleAsync(admin, "Admin"))
                {
                    return BadRequest(new { Message = "This user is not in the 'Admin' role." });
                }

                var result = await _userManager.ChangePasswordAsync(admin, passwordChange.CurrentPassword, passwordChange.NewPassword);
                if (!result.Succeeded)
                {
                    return BadRequest(new { Error = "Failed to change password.", Details = result.Errors });
                }

                return Ok(new { Message = "Admin password changed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

    }
}
    
