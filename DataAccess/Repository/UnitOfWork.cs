using DataAccess.Data;
using DataAccess.EmailServices.IEmailService;
using DataAccess.IModelRepo;
using DataAccess.ModelRepo;
using DataAccess.PaymentService.IPaymentService;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Models.Map;
using Models.User;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly CivilsDbContext _context;
        private readonly UserManager<LocalUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IPaymentServices _paymentServices;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        public IEmergPersonRepo EmPerson { get; private set; }
        public IEmergAnotherRepo EmAnother { get; private set; }
        public IUserRepository User { get; private set; }
        public IDonationRepo Donation { get; private set; }
        public ICampaignRepo Campaign { get; private set; }
        public IGeographicAreaRepo GeographicArea { get; private set; }

        public UnitOfWork(
            CivilsDbContext context,
            UserManager<LocalUser> userManager,
            IEmailService emailService,
            IPaymentServices paymentServices,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager)


        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
            _paymentServices = paymentServices;
            EmPerson = new EmergPersonRepo(_context);
            EmAnother = new EmergAnotherRepo(_context);
            User = new UserRepository(_context, _userManager, _emailService, _configuration, _roleManager);
            Donation = new DonationRepo(_context);
            Campaign = new CampaignRepo(_context);
            GeographicArea = new GeographicAreaRepo(_context);

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