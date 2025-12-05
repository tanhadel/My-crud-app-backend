using Microsoft.EntityFrameworkCore;
using BookApi.Data;
using BookApi.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Tests.Helpers
{
    public static class TestHelpers
    {
        // Skapar en test-databas i minnet
        public static ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        // Skapar test-användare
        public static User CreateTestUser(int id = 1, string username = "testuser")
        {
            return new User
            {
                Id = id,
                Username = username,
                Email = $"{username}@test.com",
                PasswordHash = "hashedpassword123",
                CreatedAt = DateTime.UtcNow
            };
        }

        // Skapar test-bok
        public static Book CreateTestBook(int id = 1, int userId = 1)
        {
            return new Book
            {
                Id = id,
                Title = "Test Book",
                Author = "Test Author",
                Description = "Test Description",
                Genre = "Test Genre",
                PublishedYear = 2024,
                ISBN = "123-456-789",
                UserId = userId
            };
        }

        // Skapar test-citat
        public static Quote CreateTestQuote(int id = 1, int bookId = 1, int userId = 1)
        {
            return new Quote
            {
                Id = id,
                Text = "Test quote text",
                Author = "Test Author",
                PageNumber = 42,
                CreatedAt = DateTime.UtcNow,
                BookId = bookId,
                UserId = userId
            };
        }

        // Mockar HttpContext med en användare (för [Authorize])
        public static void MockUserInController(ControllerBase controller, int userId, string username = "testuser")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, $"{username}@test.com")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
        }
    }
}