using DataAccess.Data;
using DataAccess.EmailServices.IEmailService;
using DataAccess.IModelRepo;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ModelRepo
{
    public class UserRepository : IUserRepository
    {
        private readonly CivilsDbContext _context;
        private readonly UserManager<LocalUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public UserRepository(
            CivilsDbContext context,
            UserManager<LocalUser> userManager,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        public bool IsUniqueSSN(string ssn)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.SSN == ssn);
            return user == null;
        }

        public bool IsUniqueUser(string username)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.UserName.ToLower() == username.ToLower());
            return user == null;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await _userManager.FindByNameAsync(loginRequestDTO.UserName.ToLower());
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password))
            {
                return new LoginResponseDTO();
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new Exception("Please confirm your email before logging in");
            }

            var token = await GenerateJwtToken(user);
            return new LoginResponseDTO
            {
                UserName = user.UserName,
                Token = token
            };
        }

        public async Task<LocalUser> RegisterAdmin(RegisterationRequestDTO registerAdmin)
        {
            if (!IsUniqueSSN(registerAdmin.SSN) || !IsUniqueUser(registerAdmin.UserName))
            {
                throw new Exception("SSN or Username already exists");
            }

            var user = new LocalUser
            {
                UserName = registerAdmin.UserName,
                Email = registerAdmin.Email,
                FirstName = registerAdmin.FirstName,
                LastName = registerAdmin.LastName,
                Phone = registerAdmin.Phone,
                Address = registerAdmin.Address,
                SSN = registerAdmin.SSN,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerAdmin.Password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, "Admin");
            return user;
        }

        public async Task<LocalUser> RegisterUser(RegisterationUserRequestDTO registerUser)
        {
            if (!IsUniqueSSN(registerUser.SSN) || !IsUniqueUser(registerUser.UserName))
            {
                throw new Exception("SSN or Username already exists");
            }

            var user = new LocalUser
            {
                UserName = registerUser.UserName,
                Email = registerUser.Email,
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                Phone = registerUser.Phone,
                Address = registerUser.Address,
                SSN = registerUser.SSN,
                ImageProfile = registerUser.ImageProfile
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, "User");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = GenerateConfirmationLink(user.Id, token);
            var emailBody = $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>.";
            await _emailService.SendEmailAsync(user.Email, "Confirm Your Email", emailBody);

            return user;
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }

        private async Task<string> GenerateJwtToken(LocalUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateConfirmationLink(string userId, string token)
        {
            var baseUrl = _configuration["App:BaseUrl"] ?? "https://localhost:5001";
            return $"{baseUrl}/api/user/confirm-email?userId={Uri.EscapeDataString(userId)}&token={Uri.EscapeDataString(token)}";
        }
    }
}