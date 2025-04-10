using Models.User;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccess.IModelRepo
{
    public interface IUserRepository
    {
        bool IsUniqueSSN(string ssn);
        bool IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<LocalUser> RegisterAdmin(RegisterationRequestDTO registerAdmin);
        Task<LocalUser> RegisterUser(RegisterationUserRequestDTO registerUser);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<LocalUser> GetAsync(Expression<Func<LocalUser, bool>> filter,
        string? includeProperties = null);
    }
}