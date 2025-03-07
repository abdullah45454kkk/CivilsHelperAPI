using DataAccess.Data;
using DataAccess.IModelRepo;
using DataAccess.Repository;
using Models.Emergencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ModelRepo
{
    class EmergAnotherRepo: Repository<EmergAnother>, IEmergAnotherRepo
    {
        private readonly CivilsDbContext _context;
        public EmergAnotherRepo(CivilsDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task Update(EmergAnother obj)
        {
            _context.EmergAnothers.Update(obj);
        }
    }
}
