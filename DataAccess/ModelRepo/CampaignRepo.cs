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
    public class CampaignRepo : Repository<Campaign>, ICampaignRepo
    {
        private readonly CivilsDbContext _context;

        public CampaignRepo(CivilsDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task Update(Campaign obj)
        {
          _context.Campaigns.Update(obj);
        }
    }
}
