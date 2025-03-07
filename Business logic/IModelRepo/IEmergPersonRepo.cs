using DataAccess.Repository.IRepository;
using Models.Emergencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IModelRepo
{
    public interface IEmergPersonRepo : IRepository<EmergPerson>
    {
        Task Update(EmergPerson obj);

    }
}
