using DataAccess.Data;
using DataAccess.EmailServices.IEmailService;
using DataAccess.IModelRepo;
using DataAccess.ModelRepo;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Models.User;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly CivilsDbContext _context;
        private readonly UserManager<LocalUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public IEmergPersonRepo EmPerson { get; private set; }
        public IEmergAnotherRepo EmAnother { get; private set; }
        public IUserRepository User { get; private set; }

        public UnitOfWork(
            CivilsDbContext context,
            UserManager<LocalUser> userManager,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;

            EmPerson = new EmergPersonRepo(_context);
            EmAnother = new EmergAnotherRepo(_context);
            User = new UserRepository(_context, _userManager, _emailService, _configuration);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}