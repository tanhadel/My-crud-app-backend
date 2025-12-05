using Xunit;
using BookApi.Controllers;
using BookApi.Tests.Helpers;
using BookApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Tests.Controllers
{
    public class BooksControllerTests
    {
        [Fact]
        public async Task GetBooks_ReturnsOnlyUserBooks()
        {
            // ARRANGE (Förbered)
            // Skapa databas och controller
            var dbContext = TestHelpers.GetInMemoryDbContext();
            var controller = new BooksController(dbContext);
            
            // Skapa två användare
            var user1 = TestHelpers.CreateTestUser(1, "user1");
            var user2 = TestHelpers.CreateTestUser(2, "user2");
            dbContext.Users.AddRange(user1, user2);
            
            // User1 har 2 böcker, User2 har 1 bok
            dbContext.Books.Add(TestHelpers.CreateTestBook(1, userId: 1));
            dbContext.Books.Add(TestHelpers.CreateTestBook(2, userId: 1));
            dbContext.Books.Add(TestHelpers.CreateTestBook(3, userId: 2));
            await dbContext.SaveChangesAsync();
            
            // Mocka att User1 är inloggad
            TestHelpers.MockUserInController(controller, userId: 1, username: "user1");

            // ACT (Utför)
            // Kalla GetBooks
            var result = await controller.GetBooks();

            // ASSERT (Kontrollera)
            // Ska returnera OkObjectResult
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var books = Assert.IsAssignableFrom<IEnumerable<BookDto>>(okResult.Value);
            
            // User1 ska bara se sina 2 böcker (inte User2's bok)
            Assert.Equal(2, books.Count());
        }

        [Fact]
        public async Task CreateBook_AddsBookWithCorrectUserId()
        {
            // ARRANGE
            var dbContext = TestHelpers.GetInMemoryDbContext();
            var controller = new BooksController(dbContext);
            
            var user = TestHelpers.CreateTestUser(1, "testuser");
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            
            TestHelpers.MockUserInController(controller, userId: 1, username: "testuser");
            
            var newBookDto = new CreateBookDto
            {
                Title = "New Book",
                Author = "New Author",
                Genre = "Fiction"
            };

            // ACT
            var result = await controller.CreateBook(newBookDto);

            // ASSERT
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var bookDto = Assert.IsType<BookDto>(createdResult.Value);
            
            Assert.Equal("New Book", bookDto.Title);
            // BookDto doesn't expose UserId for security reasons
        }

        [Fact]
        public async Task GetBook_ReturnsNotFound_WhenBookNotOwned()
        {
            // ARRANGE
            var dbContext = TestHelpers.GetInMemoryDbContext();
            var controller = new BooksController(dbContext);
            
            var user1 = TestHelpers.CreateTestUser(1, "user1");
            var user2 = TestHelpers.CreateTestUser(2, "user2");
            dbContext.Users.AddRange(user1, user2);
            
            // Bok ägs av User2
            var book = TestHelpers.CreateTestBook(1, userId: 2);
            dbContext.Books.Add(book);
            await dbContext.SaveChangesAsync();
            
            // User1 försöker hämta User2's bok
            TestHelpers.MockUserInController(controller, userId: 1, username: "user1");

            // ACT
            var result = await controller.GetBook(1);

            // ASSERT
            // Ska returnera NotFound (404)
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
    }
}