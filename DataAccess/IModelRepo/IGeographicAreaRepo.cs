using DataAccess.Repository.IRepository;
using Models.Donations;
using Models.Emergencies;
using Models.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IModelRepo
{
    public interface IGeographicAreaRepo : IRepository<GeographicArea>
    {
        Task Update(GeographicArea obj);
    }
}
