using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PIMS.allsoft.Controllers;
using PIMS.allsoft.Interfaces;
using PIMS.allsoft.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProject.MockData;
using Xunit;

namespace TestProject.Controllers
{
    public class AuthControllerTest
    {
        [Fact]
        public async Task  AuthController_Login_ValidResult()
        {
            // Arrange
            var authService = new Mock<IAuthService>();
            var logger = new Mock<ILogger<AuthController>>();
            var loginRequest = new LoginRequest { Username = "admin@123", Password = "Pass@123" };
            var token = "testToken";

            authService.Setup(_ => _.Login(It.IsAny<LoginRequest>())).Returns(token);

            var sut = new AuthController(authService.Object, logger.Object);

            // Act
            var result = sut.Login(loginRequest);

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            jsonResult.StatusCode.Should().Be(200);
            jsonResult.Value.Should().Be(token);

            /////arrange
            //var authService = new Mock<IAuthService>();
            //authService.Setup(_ => _.Login()).resultasync(UserMockData.GetUsers);

            //var sut=new AuthController(authService.Object);
            /////act
            /////
            ///// 
            // var result=await sut.Login();
            /////Assery
            //result.Should().Be(typeof(OkObjectResult));
            //(result as OkObjectResult).StatusCode.should().Be(200);

        }
    }
}
