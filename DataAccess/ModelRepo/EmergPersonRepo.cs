using DataAccess.Data;
using DataAccess.IModelRepo;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Models.Emergencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ModelRepo
{
    public class EmergPersonRepo : Repository<EmergPerson>, IEmergPersonRepo
    {
        private readonly CivilsDbContext _context;
        public EmergPersonRepo(CivilsDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task Update(EmergPerson obj)
        {
          _context.EmergPeople.Update(obj);
        }
    }
}
