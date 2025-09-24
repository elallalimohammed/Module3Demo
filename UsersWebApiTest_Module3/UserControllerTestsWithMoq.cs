using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using UsersWebApi_Module3.Controllers;
using UsersWebApi_Module3.Data;
using UsersWebApi_Module3.Models;

namespace UsersWebApiTest_Module3
{
    public class UserControllerTestsWithMoq
    {
    private Mock<DbSet<User>> _mockUserSet;
    private Mock<AppDbContext> _mockContext;
    private AuthController _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockUserSet = new Mock<DbSet<User>>();

        _mockContext = new Mock<AppDbContext>();
        _mockContext.Setup(c => c.Users).Returns(_mockUserSet.Object);

        _controller = new AuthController(_mockContext.Object);
    }

    
    [TestMethod]
    public void Register_ShouldAddUserAndReturnOk_WhenUsernameIsNew()
    {
        // Arrange
        var emptyUsers = new List<User>().AsQueryable();

        _mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(emptyUsers.Provider);
        _mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(emptyUsers.Expression);
        _mockUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(emptyUsers.ElementType);
        _mockUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(emptyUsers.GetEnumerator());

        var dto = new UserDto { Username = "newuser", Password = "pass" };

        // Act
        var result = _controller.Register(dto);

        // Assert
        _mockUserSet.Verify(m => m.Add(It.Is<User>(u => u.Username == "newuser")), Times.Once);
        _mockContext.Verify(c => c.SaveChanges(), Times.Once);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual("User registered successfully", okResult.Value);
    }
    }
}