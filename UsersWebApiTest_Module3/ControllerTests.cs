using Microsoft.VisualStudio.TestTools.UnitTesting; // MSTest attributes & assertions
using Microsoft.AspNetCore.Mvc;                     // IActionResult, OkObjectResult, BadRequestObjectResult
using Microsoft.EntityFrameworkCore;                // UseInMemoryDatabase, DbContextOptionsBuilder
using System.Linq;                                  // LINQ methods like .Any(), .First(), .Count()
using UsersApi.Data;                                // Namespace for AppDbContext
using UsersApi.Controllers;                         // Namespace for UserController
using UsersApi.Models;                              // Namespace for User and UserDto

namespace UsersWebApiTest_Module3
{
    [TestClass]
    public class UserControllerTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            return new AppDbContext(options);
        }

        [TestMethod]
        public void Register_NewUser_ReturnsOk()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new UserController(context);

            var newUser = new UserDto
            {
                Username = "testuser",
                Password = "password123"
            };

            // Act
            IActionResult result = controller.Register(newUser);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("User registered successfully", okResult.Value);

            // Ensure the user was added to the database
            Assert.AreEqual(1, context.Users.Count());
            Assert.AreEqual("testuser", context.Users.First().Username);
        }
    }
}