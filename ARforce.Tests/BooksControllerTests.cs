using ARforce.Controllers;
using ARforce.Infrastructure;
using ARforce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace ARforce.Tests
{
    public class BooksControllerTests
    {
        private readonly BooksController _controller;
        private readonly LibraryContext _context;

        public BooksControllerTests()
        {
            // Configure InMemory database
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LibraryContext(options);

            // Seed initial data
            SeedDatabase();

            _controller = new BooksController(_context);
        }

        private void SeedDatabase()
        {
            _context.Books.AddRange(new List<Book>
            {
                new Book { Id = 1, Title = "Book A", Author = "Author A", ISBN = "ISBN-A", Status = BookStatus.OnShelf },
                new Book { Id = 2, Title = "Book B", Author = "Author B", ISBN = "ISBN-B", Status = BookStatus.Borrowed },
                new Book { Id = 3, Title = "Book C", Author = "Author C", ISBN = "ISBN-C", Status = BookStatus.Returned }
            });
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetBooks_ReturnsCorrectResult()
        {
            // Arrange
            var sortBy = "Title";
            var page = 1;
            var pageSize = 2;

            // Act
            var result = await _controller.GetBooks(sortBy, page, pageSize);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var data = okResult.Value as GetBooksResponse;
            data.Should().NotBeNull();

            data.TotalItems.Should().Be(3);
            data.Page.Should().Be(page);
            data.PageSize.Should().Be(pageSize);
            data.Items.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetBook_ReturnsBook_WhenBookExists()
        {
            // Arrange
            int bookId = 1;

            // Act
            var result = await _controller.GetBook(bookId);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var book = okResult.Value as Book;
            book.Should().NotBeNull();
            book.Id.Should().Be(bookId);
        }

        [Fact]
        public async Task GetBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            int bookId = 999;

            // Act
            var result = await _controller.GetBook(bookId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateBook_ReturnsCreatedBook_WhenDataIsValid()
        {
            // Arrange
            var newBookDto = new CreateBookDto
            {
                Title = "New Book",
                Author = "New Author",
                ISBN = "ISBN-NEW",
                Status = BookStatus.OnShelf
            };

            // Act
            var result = await _controller.CreateBook(newBookDto);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult;
            createdAtActionResult.Should().NotBeNull();

            var createdBook = createdAtActionResult.Value as Book;
            createdBook.Should().NotBeNull();
            createdBook.Id.Should().BeGreaterThan(0);
            createdBook.Title.Should().Be(newBookDto.Title);
            createdBook.ISBN.Should().Be(newBookDto.ISBN);
        }

        [Fact]
        public async Task CreateBook_ReturnsBadRequest_WhenISBNIsNotUnique()
        {
            // Arrange
            var newBookDto = new CreateBookDto
            {
                Title = "Duplicate ISBN Book",
                Author = "Author",
                ISBN = "ISBN-A", // ISBN that already exists
                Status = BookStatus.OnShelf
            };

            // Act
            var result = await _controller.CreateBook(newBookDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();

            var modelState = badRequestResult.Value as SerializableError;
            modelState.Should().ContainKey("ISBN");
        }

        [Fact]
        public async Task UpdateBook_ReturnsNoContent_WhenDataIsValid()
        {
            // Arrange
            int bookId = 1;
            var updatedBook = new UpdateBookDto()
            {
                Title = "Updated Title",
                Author = "Updated Author",
                Status = BookStatus.Damaged
            };

            // Act
            var result = await _controller.UpdateBook(bookId, updatedBook);

            // Assert
            result.Should().BeOfType<NoContentResult>();

            // Verify that the book was updated
            var book = await _context.Books.FindAsync(bookId);
            book.Title.Should().Be("Updated Title");
            book.Author.Should().Be("Updated Author");
            book.Status.Should().Be(BookStatus.Damaged);
        }

        [Fact]
        public async Task UpdateBook_ReturnsBadRequest_WhenInvalidStatusTransition()
        {
            // Arrange
            int bookId = 2; // Book with Status = Borrowed
            var updatedBook = new UpdateBookDto()
            {
                Title = "Book B",
                Author = "Author B",
                Status = BookStatus.Damaged // Invalid transition from Borrowed to Damaged
            };

            // Act
            var result = await _controller.UpdateBook(bookId, updatedBook);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();

            var errorMessage = badRequestResult.Value as string;
            errorMessage.Should().Contain("Invalid status change from");
        }

        [Fact]
        public async Task DeleteBook_ReturnsNoContent_WhenBookExists()
        {
            // Arrange
            int bookId = 1;

            // Act
            var result = await _controller.DeleteBook(bookId);

            // Assert
            result.Should().BeOfType<NoContentResult>();

            // Verify that the book was removed
            var book = await _context.Books.FindAsync(bookId);
            book.Should().BeNull();
        }

        [Fact]
        public async Task DeleteBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            int bookId = 999;

            // Act
            var result = await _controller.DeleteBook(bookId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}