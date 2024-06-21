using PIMS.allsoft.Interfaces;
using PIMS.allsoft.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using PIMS.allsoft.Exceptions;
using System.Net.Mime;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PIMS.allsoft.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Produces(MediaTypeNames.Application.Json)]
    //[Consumes(MediaTypeNames.Application.Json)]
    [Consumes("application/json")] //only accept `application/json`
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



        /// <summary>
        /// Login by user Name and Password.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /api/login
        ///     {
        ///         "username": "Ajay@123",
        ///         "password": "Pass@123"
        ///     }
        ///     
        /// 
        /// Sample Response:
        /// 
        ///     200 OK
        ///     {
        ///         "token": "TOKENdjhfyfierf17192"
        ///     }
        ///
        /// </remarks>
        /// <param name="username">The username of the user attempting to login.</param>
        /// <param name="password">The password of the user attempting to login.</param>
        /// <returns>Returns a JWT token if login is successful.</returns>
        /// <response code="200">Login successful, returns JWT token.</response>
        /// <response code="400">Bad request, missing or invalid parameters.</response>
        /// <response code="500">Internal server error.</response>

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest obj)
        {
            var token = _auth.Login(obj);
            if (token != null)
            {
                return new JsonResult(new { token = token })
                {
                    StatusCode = 200 // Set the StatusCode explicitly
                };
            }
            else
            {
                return new JsonResult(new { message = "Invalid login attempt" })
                {
                    StatusCode = 401 // Unauthorized status code for failed login
                };
            }
           // return new JsonResult(token);
        }
        /// <summary>
        /// Assign roles to a user.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /api/Auth/assignRole
        ///     {
        ///         "UserId": 1,
        ///         "RoleIds": [1, 2, 3]
        ///     }
        /// 
        /// Sample Response:
        /// 
        ///     200 OK
        ///     true
        /// 
        ///     400 Bad Request
        ///     "User is not valid"
        /// 
        ///     400 Bad Request
        ///     "Role ID {roleId} is not valid"
        /// 
        ///     400 Bad Request
        ///     "User {userId} is already assigned to role {roleId}"
        /// 
        ///     500 Internal Server Error
        ///     false
        /// 
        /// </remarks>
        /// <param name="userRole">The user role assignment object containing the user ID and a list of role IDs.</param>

        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AddUserRole userRole)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //var result = await _auth.AssignRoleToUserAsync(userRole);

            bool addedUserRole =await  _auth.AssignRoleToUserAsync(userRole);
            if (addedUserRole)
            {
                return Ok(addedUserRole);
            }
            return BadRequest(userRole);
        }


        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /api/Auth/addUser
        ///     {
        ///         "name": "John Doe",
        ///         "username": "johndoe",
        ///         "password": "password123",
        ///         "createdDate": "2024-06-20T17:25:08.702Z"
        ///     }
        /// 
        /// Sample Response:
        /// 
        ///     200 OK
        ///     {
        ///         "userID": 1,
        ///         "name": "John Doe",
        ///         "username": "johndoe",
        ///         "password": "hashedpassword",
        ///         "createdDate": "2024-06-20T17:25:08.702Z"
        ///     }
        /// 
        ///     400 Bad Request
        ///     "Username johndoe is already taken."
        /// 
        ///     500 Internal Server Error
        ///     false
        /// 
        /// </remarks>
        /// <param name="user">The user object containing the user's details.</param>
        /// <response code="200">If the user is successfully added.</response>
        /// <response code="400">If the username is already taken or user data is invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost("addUser")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var addedUser = await _auth.AddUserAsync(user);
            return (IActionResult)addedUser;
        }

        /// <summary>
        /// Adds a new role.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /api/Auth/addRole
        ///     {
        ///         "RoleName": "Admin",
        ///         "Description": "Add Description"
        ///     }
        /// 
        /// Sample Response:
        /// 
        ///     200 OK
        ///     {
        ///         "RoleId": 1,
        ///         "RoleName": "Admin",
        ///         "Description": "Add Description"
        ///     }
        /// 
        ///     400 Bad Request
        ///     "Role with ID 1 already exists"
        /// 
        ///     500 Internal Server Error
        ///     false
        /// 
        /// </remarks>
        /// <param name="role">The role object containing the role ID and role name.</param>
        /// <response code="200">If the role is successfully added.</response>
        /// <response code="400">If the role already exists.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost("addRole")]
        public async Task<IActionResult> AddRole([FromBody] Role role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var addedRole = await _auth.AddRoleAsync(role);
                return Ok(addedRole);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public string Index()
        {
            var DATA = "HELLO";
            return DATA;
        }
    }
}
