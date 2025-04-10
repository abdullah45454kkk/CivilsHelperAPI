using DataAccess.Repository.IRepository;
using Models.Donations;
using Models.Emergencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IModelRepo
{
    public interface ICampaignRepo : IRepository<Campaign>
    {
        Task Update(Campaign obj);
    }
}
