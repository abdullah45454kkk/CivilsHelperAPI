using DataAccess.IModelRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IUnitOfWork 
    {
        IEmergPersonRepo EmPerson { get; }
        IEmergAnotherRepo EmAnother { get; }
        IUserRepository User { get; }
        Task Save();
    }
}
