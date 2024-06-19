using PIMS.allsoft.Context;
using PIMS.allsoft.Interfaces;
using PIMS.allsoft.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PIMS.allsoft.Exceptions;
using Microsoft.AspNetCore.Http;

namespace PIMS.allsoft.Services
{
    public class AuthService : IAuthService
    {
        private readonly PIMSContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(PIMSContext context, IConfiguration configuration, IPasswordHasher passwordHasher)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        public Role AddRole(Role role)
        {
            var addedRole = _context.Roles.Add(role);
            _context.SaveChanges();
            return addedRole.Entity;
        }

        public User AddUser(User user)
        {
            user.Password = _passwordHasher.Hash(user.Password);
            //  user.Password= HashPassword(user.Password);
            var addedUser = _context.Users.Add(user);
            _context.SaveChanges();
            return addedUser.Entity;
        }

        //public bool AssignRoleToUser(AddUserRole obj)
        //{
        //    throw new NotImplementedException();
        //}

        public bool AssignRoleToUser(AddUserRole obj)
        {
            try
            {
                var addRoles = new List<UserRole>();
                var user = _context.Users.SingleOrDefault(s => s.UserID == obj.UserId);
                if (user == null)
                    throw new Exception("user is not valid");
                foreach (int role in obj.RoleIds)
                {
                    var userRole = new UserRole();
                    userRole.RoleId = role;
                    userRole.UserId = user.UserID;
                    addRoles.Add(userRole);
                }
                _context.UserRoles.AddRange(addRoles);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public string Login(LoginRequest loginRequest)
        {
            if (loginRequest.Username != null && loginRequest.Password != null)
            {
                var user = _context.Users.SingleOrDefault(s => s.Username == loginRequest.Username);
                if (user != null)
                {
                    var pass = _passwordHasher.verify(user.Password, loginRequest.Password);

                    if (pass)
                    {

                        var claims = new List<Claim> {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim("UserID", user.UserID.ToString()),
                        new Claim("UserName", user.Username)
                    };
                        var userRoles = _context.UserRoles.Where(u => u.UserId == user.UserID).ToList();
                        var roleIds = userRoles.Select(s => s.RoleId).ToList();
                        var roles = _context.Roles.Where(r => roleIds.Contains(r.RoleID)).ToList();
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
                        }
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Jwt:Issuer"],
                            _configuration["Jwt:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddMinutes(10),
                            signingCredentials: signIn);
                        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                        return jwtToken;
                    }
                    else
                    {
                        throw new BadRequestException("Password is not valid");
                    }
                }
                throw new BadRequestException("user is not valid");
            }
            else
            {
                throw new Exception("credentials are not valid");
            }
        }
    }
}
