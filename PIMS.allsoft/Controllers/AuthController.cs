using PIMS.allsoft.Interfaces;
using PIMS.allsoft.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using PIMS.allsoft.Exceptions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PIMS.allsoft.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth, ILogger<AuthController> logger)
        {
            _auth = auth;
            _logger = logger;
            _logger.LogDebug("Nlog is integrated to Auth Controller");
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest obj)
        {
              var token = _auth.Login(obj);
            //  return Ok(token);           
            return new JsonResult(token);
        }

        [HttpPost("assignRole")]
        public bool AssignRoleToUser([FromBody] AddUserRole userRole)
        {
            var addedUserRole = _auth.AssignRoleToUser(userRole);
            return addedUserRole;
        }

        [HttpPost("addUser")]
        public User AddUser([FromBody] User user)
        {
            var addeduser = _auth.AddUser(user);
            return addeduser;
        }

        [HttpPost("addRole")]
        public Role AddRole([FromBody] Role role)
        {
            var addedRole = _auth.AddRole(role);
            return addedRole;
        }

    }
}
