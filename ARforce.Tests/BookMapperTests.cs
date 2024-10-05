using ARforce.Common;
using ARforce.Models;
using FluentAssertions;

namespace ARforce.Tests
{
    public class BookMapperTests
    {
        private readonly IBookMapper _bookMapper;
        private readonly byte[] _rowVersion = { 0, 0, 0, 1 };

        public BookMapperTests()
        {
            _bookMapper = new BookMapper();
        }

        [Fact]
        public void MapToUpdateBook_ShouldMapCorrectly()
        {
            // Arrange
            var updateBookDto = new UpdateBookDto
            {
                Title = "Updated Title",
                Author = "Updated Author",
                Status = BookStatus.Borrowed
            };

            // Act
            var updateBook = _bookMapper.MapToUpdateBook(updateBookDto);

            // Assert
            updateBook.Should().NotBeNull();
            updateBook.Title.Should().Be(updateBookDto.Title);
            updateBook.Author.Should().Be(updateBookDto.Author);
            updateBook.Status.Should().Be(updateBookDto.Status);
            updateBook.RowVersion.Should().BeEquivalentTo(_rowVersion);
        }

        [Fact]
        public void MapToBookFromUpdateBook_ShouldUpdateExistingBook()
        {
            // Arrange
            var existingBook = new Book
            {
                Id = 1,
                Title = "Original Title",
                Author = "Original Author",
                ISBN = "1234567890",
                Status = BookStatus.OnShelf,
                RowVersion = _rowVersion
            };

            var updateBook = new UpdateBook
            {
                Title = "Updated Title",
                Author = "Updated Author",
                Status = BookStatus.Borrowed,
                RowVersion = _rowVersion
            };

            // Act
            var updatedBook = _bookMapper.MapToBookFromUpdateBook(existingBook, updateBook);

            // Assert
            updatedBook.Should().NotBeNull();
            updatedBook.Should().BeSameAs(existingBook); 
            updatedBook.Title.Should().Be(updateBook.Title);
            updatedBook.Author.Should().Be(updateBook.Author);
            updatedBook.Status.Should().Be(updateBook.Status);
            updatedBook.RowVersion.Should().BeEquivalentTo(_rowVersion); 
        }

        [Fact]
        public void MapToBookFromCreateBook_ShouldMapCorrectly()
        {
            // Arrange
            var createBookDto = new CreateBookDto
            {
                Title = "New Book",
                Author = "New Author",
                ISBN = "0987654321",
                Status = BookStatus.OnShelf
            };

            // Act
            var newBook = _bookMapper.MapToBookFromCreateBook(createBookDto);

            // Assert
            newBook.Should().NotBeNull();
            newBook.Title.Should().Be(createBookDto.Title);
            newBook.Author.Should().Be(createBookDto.Author);
            newBook.ISBN.Should().Be(createBookDto.ISBN);
            newBook.Status.Should().Be(createBookDto.Status);
            newBook.RowVersion.Should().BeEquivalentTo(_rowVersion);
        }

        [Fact]
        public void MapToReturnBookDto_ShouldMapBookToReturnBookDto()
        {
            // Arrange
            var book = new Book
            {
                Id = 1,
                Title = "Example Title",
                Author = "Author Name",
                ISBN = "1234567890",
                Status = BookStatus.OnShelf,
                RowVersion = _rowVersion
            };

            // Act
            var dto = _bookMapper.MapToReturnBookDto(book);

            // Assert
            dto.Should().NotBeNull();
            dto.Id.Should().Be(book.Id);
            dto.Title.Should().Be(book.Title);
            dto.Author.Should().Be(book.Author);
            dto.ISBN.Should().Be(book.ISBN);
            dto.Status.Should().Be(book.Status);
        }
    }
}
