using DataAccess.Data;
using DataAccess.EmailServices.IEmailService;
using DataAccess.IModelRepo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.User;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserRepository(
            CivilsDbContext context,
            UserManager<LocalUser> userManager,
            IEmailService emailService,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _roleManager = roleManager;
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
                gender = registerAdmin.gender,
                Phone = registerAdmin.Phone,
                Address = registerAdmin.Address,
                SSN = registerAdmin.SSN,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerAdmin.Password);
            if (!result.Succeeded)
            {
                Console.WriteLine(string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            //Console.WriteLine("Adding admin to role...");
            //var roleExists = await _roleManager.RoleExistsAsync("Admin");
            //if (!roleExists)
            //{
            //    Console.WriteLine("Role 'Admin' does not exist, creating it...");
            //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
            //}

            await _userManager.AddToRoleAsync(user, "Admin");
            return user;
        }

        public async Task<LocalUser> RegisterUser(RegisterationUserRequestDTO registerUser)
        {
            Console.WriteLine("Starting RegisterUser...");
            if (!IsUniqueSSN(registerUser.SSN) || !IsUniqueUser(registerUser.UserName))
            {
                Console.WriteLine("SSN or Username already exists");
                throw new Exception("SSN or Username already exists");
            }

            var user = new LocalUser
            {
                UserName = registerUser.UserName,
                Email = registerUser.Email,
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                gender = registerUser.gender,
                Phone = registerUser.Phone,
                Address = registerUser.Address,
                SSN = registerUser.SSN,
                ImageProfile = registerUser.ImageProfile
            };

            Console.WriteLine("Creating user...");
            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (!result.Succeeded)
            {
                Console.WriteLine("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            //Console.WriteLine("Adding user to role...");
            //var roleExists = await _roleManager.RoleExistsAsync("User");
            //if (!roleExists)
            //{
            //    Console.WriteLine("Role 'User' does not exist, creating it...");
            //    await _roleManager.CreateAsync(new IdentityRole("User"));
            //}

            Console.WriteLine("Adding user to role...");
            await _userManager.AddToRoleAsync(user, "User");

            Console.WriteLine("Generating token...");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            Console.WriteLine("Generating confirmation link...");
            var confirmationLink = GenerateConfirmationLink(user.Id, token);

            Console.WriteLine("Sending email...");
            var emailBody = $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>.";
            await _emailService.SendEmailAsync(user.Email, "Confirm Your Email", emailBody);
            Console.WriteLine("Email sent successfully");

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
                new Claim(ClaimTypes.NameIdentifier, user.Id), // Ensure UserId is included
                new Claim(ClaimTypes.Email, user.Email)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(15);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<LocalUser> GetAsync(Expression<Func<LocalUser, bool>> filter,
        string? includeProperties = null)
        {
            try
            {
                IQueryable<LocalUser> query = _context.Users.Where(filter);

                // Apply eager loading if includeProperties is provided
                if (!string.IsNullOrEmpty(includeProperties))
                {
                    foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProperty);
                    }
                }

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // Log the error (customize this according to your needs)
                Console.WriteLine($"Error in GetAsync: {ex.Message}");
                return null; // Prevent application crashes by returning null
            }
        }
        private string GenerateConfirmationLink(string userId, string token)
        {
            var baseUrl = _configuration["App:BaseUrl"] ?? "https://localhost:5001";
            return $"{baseUrl}/api/user/confirm-email?userId={Uri.EscapeDataString(userId)}&token={Uri.EscapeDataString(token)}";
        }

   
    }
}