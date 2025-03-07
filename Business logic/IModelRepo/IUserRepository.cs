using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.User;
namespace DataAccess.IModelRepo
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        bool IsUniqueSSN(string ssn);
        Task<LoginResponseDTO> Login (LoginRequestDTO loginRequestDTO);
        Task<LocalUser> RegisterAdmin(RegisterationRequestDTO registerAdmin);
        Task<LocalUser> RegisterUser(RegisterationUserRequestDTO registerUser);
    }
}
