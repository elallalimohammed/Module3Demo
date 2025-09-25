using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using UsersWebApi_Module3.Controllers;
using UsersWebApi_Module3.Data;
using UsersWebApi_Module3.Models;

namespace UsersWebApiTest_Module3
{
    [TestClass]
    public class UserControllerTestsWithMoq
    {

        [TestMethod]
        public void GetAll_ReturnsOkResult_WithUsers()
        {
            // Arrange
            var mockRepo = new Mock<AuthController.IRepository<User>>();
            var users = new List<User>
        {
            new User { Id = 1, Username = "Alice" },
            new User { Id = 2, Username = "Bob" }
        };

            mockRepo.Setup(r => r.GetAll()).Returns(users);

            var controller = new AuthController(mockRepo.Object);

            // Act
            var result = controller.GetAll().Result; // since GetAll is async
            var okResult = result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var returnedUsers = okResult.Value as IEnumerable<User>;
            Assert.IsNotNull(returnedUsers);
            Assert.AreEqual(2, returnedUsers.Count());
            Assert.AreEqual("Alice", returnedUsers.First().Username);
        }


        [TestMethod]
        public void GetAll_ReturnsOkResult_WithEmptyList_WhenNoUsersExist()
        {
            // Arrange
            var mockRepo = new Mock<AuthController.IRepository<User>>();
            var emptyUsers = new List<User>();

            mockRepo.Setup(r => r.GetAll()).Returns(emptyUsers);

            var controller = new AuthController(mockRepo.Object);

            // Act
            var result = controller.GetAll().Result;
            var okResult = result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var returnedUsers = okResult.Value as IEnumerable<User>;
            Assert.IsNotNull(returnedUsers);
            Assert.AreEqual(0, returnedUsers.Count());
        }

        [TestMethod]
        public void Add_CallsRepositoryAdd_AndReturnsCreatedResult()
        {
            // Arrange
            var mockRepo = new Mock<AuthController.IRepository<User>>();
            var controller = new AuthController(mockRepo.Object);
            var newUser = new User { Id = 3, Username = "Charlie" };

            // Act
            var result = controller.Add(newUser);
            var createdResult = result as CreatedAtActionResult;

            // Assert
            // Verify repository Add was called exactly once with our user
            mockRepo.Verify(r => r.Add(It.Is<User>(u => u.Id == 3 && u.Username == "Charlie")), Times.Once);

            // Verify controller returned CreatedAtActionResult
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);

            // Verify the returned value is the same user
            var returnedUser = createdResult.Value as User;
            Assert.IsNotNull(returnedUser);
            Assert.AreEqual("Charlie", returnedUser.Username);
        }
        [TestMethod]
        public void Add_ReturnsBadRequest_WhenUserIsNull()
        {
            // Arrange
            var mockRepo = new Mock<AuthController.IRepository<User>>();
            var controller = new AuthController(mockRepo.Object);

            // Act
            var result = controller.Add(null);
            var badRequestResult = result as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("User cannot be null", badRequestResult.Value);

            // Verify Add was never called
            mockRepo.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
        }
    }
}
