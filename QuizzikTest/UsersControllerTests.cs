using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using Quizzik_Project.Controllers;
using Quizzik_Project.DTO;
using Quizzik_Project.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quizzik_Project.Tests.Controllers
{
    [TestFixture]
    public class UsersControllerTests
    {
        private EFCoreDbContext _context;
        private Mock<IMapper> _mockMapper;
        private Mock<IConfiguration> _mockConfiguration;
        private UsersController _controller;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<EFCoreDbContext>()
                .UseInMemoryDatabase(databaseName: "QuizzikTest")
                .Options;

            _context = new EFCoreDbContext(options);
            _mockMapper = new Mock<IMapper>();
            _mockConfiguration = new Mock<IConfiguration>();
            _controller = new UsersController(_mockMapper.Object, _context, _mockConfiguration.Object);

            ClearDatabase();
            SeedDatabase();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private void ClearDatabase()
        {
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();
        }

        private void SeedDatabase()
        {
            var user = new User
            {
                UserID = 1,
                Username = "testuser",
                Password = BCrypt.Net.BCrypt.HashPassword("password"),
                Email = "testuser@example.com",
                Role = "Student",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithListOfUsers()
        {
            // Arrange
            var users = new List<User> { new User { UserID = 1, Username = "testuser", Email = "testuser@example.com", Role = "Student", CreatedAt = DateTime.Now } };
            var userDTOs = new List<UserDTO> { new UserDTO { UserID = 1, Username = "testuser", Email = "testuser@example.com", Role = "Student", CreatedAt = DateTime.Now } };
            _mockMapper.Setup(m => m.Map<List<UserDTO>>(It.IsAny<List<User>>())).Returns(userDTOs);

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<List<UserDTO>>());
            var returnedUsers = okResult.Value as List<UserDTO>;
            Assert.That(returnedUsers.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetById_ReturnsOkResult_WithUser()
        {
            // Arrange
            var user = new User { UserID = 1, Username = "testuser", Email = "testuser@example.com", Role = "Student", CreatedAt = DateTime.Now };
            var userDTO = new UserDTO { UserID = 1, Username = "testuser", Email = "testuser@example.com", Role = "Student", CreatedAt = DateTime.Now };
            _mockMapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<UserDTO>());
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenUserNotFound()
        {
            // Act
            var result = await _controller.GetById(99);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Register_ReturnsCreatedAtActionResult_WithUser()
        {
            // Arrange
            var userDTO = new UserDTO { Username = "newuser", Password = "password", Email = "newuser@example.com", Role = "Student", CreatedAt = DateTime.Now };
            var user = new User { UserID = 2, Username = "newuser", Password = BCrypt.Net.BCrypt.HashPassword("password"), Email = "newuser@example.com", Role = "Student", CreatedAt = DateTime.Now };
            _mockMapper.Setup(m => m.Map<User>(It.IsAny<UserDTO>())).Returns(user);

            // Act
            var result = await _controller.Register(userDTO);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(createdResult.Value, Is.InstanceOf<UserDTO>());
        }

        [Test]
        public async Task Register_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await _controller.Register(new UserDTO());

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        

        [Test]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginDTO = new LoginDTO { Email = "wrong@example.com", Password = "wrongpassword" };

            // Act
            var result = await _controller.Login(loginDTO);

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        

        [Test]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var userDTO = new UserDTO { UserID = 1, Username = "updateduser", Email = "updateduser@example.com", Role = "Student", CreatedAt = DateTime.Now };

            // Act
            var result = await _controller.Update(2, userDTO);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenDeleteIsSuccessful()
        {
            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenUserNotFound()
        {
            // Act
            var result = await _controller.Delete(99);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}