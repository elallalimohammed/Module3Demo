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
}
    }
