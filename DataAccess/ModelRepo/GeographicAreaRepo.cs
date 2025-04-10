using DataAccess.Data;
using DataAccess.IModelRepo;
using DataAccess.Repository;
using Models.Donations;
using Models.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ModelRepo
{
    public class GeographicAreaRepo : Repository<GeographicArea>, IGeographicAreaRepo
    {
        private readonly CivilsDbContext _context;

        public GeographicAreaRepo(CivilsDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task Update(GeographicArea obj)
        {
          _context.GeographicAreas.Update(obj);
        }
    }
}
