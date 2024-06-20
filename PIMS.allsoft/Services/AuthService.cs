using PIMS.allsoft.Context;
using PIMS.allsoft.Interfaces;
using PIMS.allsoft.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PIMS.allsoft.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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
        public async Task<Role> AddRoleAsync(Role role)
        {
            var roleExists = await _context.Roles.AnyAsync(r => r.RoleName == role.RoleName);
            if (roleExists)
                throw new Exception($"Role with RoleName {role.RoleName} already exists");

            var addedRole = await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
            return addedRole.Entity;
        }

        public async Task<User> AddUserAsync(User user)
        {
            // Check if the username already exists
            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Username == user.Username);
            if (existingUser != null)
            {
                throw new Exception($"Username {user.Username} is already taken.");
            }
            // Hash the password
            user.Password = _passwordHasher.Hash(user.Password);

            // Add the user to the database
            var addedUser = await _context.Users.AddAsync(user);
            _context.SaveChanges();
            return addedUser.Entity;
        }

        //public bool AssignRoleToUser(AddUserRole obj)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<bool> AssignRoleToUserAsync(AddUserRole obj)
        {
            
                var addRoles = new List<UserRole>();
                var user = await _context.Users.SingleOrDefaultAsync(s => s.UserID == obj.UserId);
                if (user == null)
                    throw new Exception("user is not valid");
                foreach (int role in obj.RoleIds)
                {
                var roleExists = await _context.Roles.AnyAsync(r => r.RoleID == role);
                if (!roleExists)
                    throw new Exception($"Role ID {role} is not valid");

                var userRoleExists = await _context.UserRoles.AnyAsync(ur => ur.UserId == user.UserID && ur.RoleId == role);
                if (userRoleExists)
                    throw new Exception($"User {user.UserID} is already assigned to role {role}");

                var userRole = new UserRole();
                    userRole.RoleId = role;
                   // userRole.UserId = user.UserID;
                    addRoles.Add(userRole);
                }
                await _context.UserRoles.AddRangeAsync(addRoles);
                await _context.SaveChangesAsync();
                return true;
           

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
