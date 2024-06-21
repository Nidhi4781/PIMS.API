using PIMS.allsoft.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.MockData
{
    public class UserMockData
    {
        public static List<User> GetUsers()
        {
            return new List<User>
            {
                new User
                {
                    Username = "admin@123",
                    Password = "Pass@123",
                },
                 new User
                {
                    Username = "Ajay@123",
                    Password = "Pass@123",
                }
            };
        }
    }
}
