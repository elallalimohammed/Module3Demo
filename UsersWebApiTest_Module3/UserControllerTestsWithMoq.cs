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
        private Mock<DbSet<User>> _mockUserSet;
        private Mock<AppDbContext> _mockContext;
        private AuthController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Sample in-memory user list
            var users = new List<User>().AsQueryable();

            // Mock DbSet
            _mockUserSet = new Mock<DbSet<User>>();
            _mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _mockUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _mockUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // Mock AppDbContext
            _mockContext = new Mock<AppDbContext>();
            _mockContext.Setup(c => c.Users).Returns(_mockUserSet.Object);

            // Initialize controller
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