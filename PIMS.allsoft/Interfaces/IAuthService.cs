using PIMS.allsoft.Models;

namespace PIMS.allsoft.Interfaces
{
    public interface IAuthService
    {
        Task<User> AddUserAsync(User user);
        string Login(LoginRequest loginRequest);
        Task<Role> AddRoleAsync(Role role);
        Task<bool> AssignRoleToUserAsync(AddUserRole obj);

    }
}
