using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using UsersWebApi_Module3.Controllers;
using UsersWebApi_Module3.Data;
using UsersWebApi_Module3.Models;
using static UsersWebApi_Module3.Controllers.AuthController;

namespace UsersWebApiTest_Module3
{
    [TestClass]
    public class UserControllerTestsWithMoq
    {

        // Test for GetAll method 

        // Testing successful retrieval of users  
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

        // tersting GetAll returns empty list
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

        // Test for exception in GetAll
        [TestMethod]
        public void GetAll_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var mockRepo = new Mock<AuthController.IRepository<User>>();
            mockRepo.Setup(r => r.GetAll()).Throws(new Exception("Database error"));

            var controller = new AuthController(mockRepo.Object);

            // Act
            var result = controller.GetAll().Result;
            var statusResult = result as ObjectResult;

            // Assert
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(500, statusResult.StatusCode);
            StringAssert.Contains(statusResult.Value.ToString(), "Database error");
        }


        // Edge case: GetAll returns null (simulating repository returning null instead of empty list)
        [TestMethod]
        public async Task GetAll_ReturnsInternalServerError_WhenRepositoryReturnsNull()
        {
            // Arrange
            var mockRepo = new Mock<AuthController.IRepository<User>>();
            mockRepo.Setup(r => r.GetAll()).Returns((IEnumerable<User>)null);

            var controller = new AuthController(mockRepo.Object);

            // Act
            var result = await controller.GetAll();
            var statusResult = result as ObjectResult;

            // Assert
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(500, statusResult.StatusCode);
        }
        
            [TestMethod]
            public async Task GetById_ReturnsOk_WhenUserExists()
            {
                // Arrange
                var user = new User { Id = 1, Username = "Alice" };
                var mockRepo = new Mock<IRepository<User>>();
                mockRepo.Setup(r => r.GetById(1)).Returns(user);

                var controller = new AuthController(mockRepo.Object);

                // Act
                var result = await controller.GetById(1);
                var okResult = result as OkObjectResult;

                // Assert
                Assert.IsNotNull(okResult);
                Assert.AreEqual(200, okResult.StatusCode);
                Assert.AreSame(user, okResult.Value);
            }

            [TestMethod]
            public async Task GetById_ReturnsNotFound_WhenUserDoesNotExist()
            {
                // Arrange
                var mockRepo = new Mock<IRepository<User>>();
                mockRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns((User)null);

                var controller = new AuthController(mockRepo.Object);

                // Act
                var result = await controller.GetById(99);
                var notFoundResult = result as NotFoundObjectResult;

                // Assert
                Assert.IsNotNull(notFoundResult);
                Assert.AreEqual(404, notFoundResult.StatusCode);
                Assert.AreEqual("User with ID 99 not found", notFoundResult.Value);
            }

            [TestMethod]
            public async Task GetById_ReturnsInternalServerError_WhenRepositoryThrowsException()
            {
                // Arrange
                var mockRepo = new Mock<IRepository<User>>();
                mockRepo.Setup(r => r.GetById(It.IsAny<int>())).Throws(new Exception("Database error"));

                var controller = new AuthController(mockRepo.Object);

                // Act
                var result = await controller.GetById(1);
                var statusResult = result as ObjectResult;

                // Assert
                Assert.IsNotNull(statusResult);
                Assert.AreEqual(500, statusResult.StatusCode);
                Assert.AreEqual("Internal server error: Database error", statusResult.Value);
            }

            // Testing Add method

            // Testing successful addition
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

            // Testing Add with null user
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




            // Test for exception in Add
            [TestMethod]
            public void Add_ReturnsInternalServerError_WhenExceptionThrown()
            {
                // Arrange
                var mockRepo = new Mock<AuthController.IRepository<User>>();
                mockRepo.Setup(r => r.Add(It.IsAny<User>())).Throws(new Exception("Repository failed"));

                var controller = new AuthController(mockRepo.Object);
                var newUser = new User { Id = 4, Username = "David" };

                // Act
                var result = controller.Add(newUser);
                var statusResult = result as ObjectResult;

                // Assert
                Assert.IsNotNull(statusResult);
                Assert.AreEqual(500, statusResult.StatusCode);
                StringAssert.Contains(statusResult.Value.ToString(), "Repository failed");
            }

            // Optional: Test that CreatedAtAction returns correct route values
            [TestMethod]
            public void Add_ReturnedCreatedAtAction_HasCorrectRouteValues()
            {
                // Arrange
                var mockRepo = new Mock<AuthController.IRepository<User>>();
                var controller = new AuthController(mockRepo.Object);
                var newUser = new User { Id = 5, Username = "Eve" };

                // Act
                var result = controller.Add(newUser) as CreatedAtActionResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(nameof(AuthController.GetAll), result.ActionName);
                Assert.AreEqual(newUser.Id, ((dynamic)result.RouteValues["id"]));
            }

            // Edge case: Add a user with empty username
            [TestMethod]
            public void Add_AllowsUserWithEmptyUsername()
            {
                // Arrange
                var mockRepo = new Mock<AuthController.IRepository<User>>();
                var controller = new AuthController(mockRepo.Object);
                var newUser = new User { Id = 6, Username = "" };

                // Act
                var result = controller.Add(newUser);
                var createdResult = result as CreatedAtActionResult;

                // Assert
                Assert.IsNotNull(createdResult);
                Assert.AreEqual(201, createdResult.StatusCode);
                var returnedUser = createdResult.Value as User;
                Assert.IsNotNull(returnedUser);
                Assert.AreEqual("", returnedUser.Username);
                mockRepo.Verify(r => r.Add(It.Is<User>(u => u.Id == 6)), Times.Once);
            }

            // Test for the optional ThrowException endpoint                  

            [TestMethod]
            public void ThrowExceptionEndpoint_ThrowsInvalidOperationException()
            {
                var controller = new AuthController(new Mock<AuthController.IRepository<User>>().Object);

                Assert.ThrowsException<InvalidOperationException>(() => controller.ThrowException());
            }


        [TestMethod]
        public async Task Login_ReturnsUnauthorized_WhenUsernameDoesNotExist()
        {
            // Arrange
            var mockRepo = new Mock<IRepository<User>>();
            mockRepo.Setup(r => r.GetByUsername(It.IsAny<string>())).Returns((User)null);

            var controller = new AuthController(mockRepo.Object);

            // Act
            var result = await controller.Login("bob", "password123");
            var unauthorizedResult = result as UnauthorizedObjectResult;

            // Assert
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
            Assert.AreEqual("Invalid username or password", unauthorizedResult.Value);
        }

        [TestMethod]
        public async Task Login_ReturnsUnauthorized_WhenPasswordIsIncorrect()
        {
            // Arrange
            var user = new User { Id = 1, Username = "alice", Password = "password123" };
            var mockRepo = new Mock<IRepository<User>>();
            mockRepo.Setup(r => r.GetByUsername("alice")).Returns(user);

            var controller = new AuthController(mockRepo.Object);

            // Act
            var result = await controller.Login("alice", "wrongpassword");
            var unauthorizedResult = result as UnauthorizedObjectResult;

            // Assert
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
            Assert.AreEqual("Invalid username or password", unauthorizedResult.Value);
        }

        [TestMethod]
        public async Task Login_ReturnsInternalServerError_WhenRepositoryThrowsException()
        {
            // Arrange
            var mockRepo = new Mock<IRepository<User>>();
            mockRepo.Setup(r => r.GetByUsername(It.IsAny<string>()))
                    .Throws(new Exception("Database failure"));

            var controller = new AuthController(mockRepo.Object);

            // Act
            var result = await controller.Login("alice", "password123");
            var objectResult = result as ObjectResult;

            // Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("Internal server error: Database failure", objectResult.Value);
        }


        
    }
}


