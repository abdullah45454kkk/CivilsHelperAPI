using DataAccess.Data;
using DataAccess.IModelRepo;
using DataAccess.Repository;
using Models.Donations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ModelRepo
{
    public class DonationRepo : Repository<Donation>, IDonationRepo
    {
        private readonly CivilsDbContext _context;

        public DonationRepo(CivilsDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task Update(Donation obj)
        {
          _context.Donation.Update(obj);
        }
    }
}
