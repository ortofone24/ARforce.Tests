using ARforce.Common;
using ARforce.Models;
using FluentAssertions;

namespace ARforce.Tests
{
    public class SortingAndPaginationTests
    {
        private readonly ISortingAndPagination _sortingAndPagination;
        private readonly IQueryable<Book> _booksQuery;

        public SortingAndPaginationTests()
        {
            _sortingAndPagination = new SortingAndPagination();

            var books = new List<Book>
            {
                new Book { Id = 1, Title = "C# Programming", Author = "John Doe", ISBN = "111", Status = BookStatus.OnShelf },
                new Book { Id = 2, Title = "ASP.NET Core", Author = "Jane Smith", ISBN = "222", Status = BookStatus.Borrowed },
                new Book { Id = 3, Title = "Entity Framework", Author = "Bob Johnson", ISBN = "333", Status = BookStatus.Returned },
                new Book { Id = 4, Title = "LINQ in Action", Author = "Alice Williams", ISBN = "444", Status = BookStatus.Damaged }
            };

            _booksQuery = books.AsQueryable();
        }

        [Fact]
        public void SortingBy_ShouldSortByTitle()
        {
            // Arrange
            var sortBy = "title";

            // Act
            var result = _sortingAndPagination.SortingBy(sortBy, _booksQuery).ToList();

            // Assert
            result.Should().BeInAscendingOrder(b => b.Title);
        }

        [Fact]
        public void SortingBy_ShouldSortByAuthor()
        {
            // Arrange
            var sortBy = "author";

            // Act
            var result = _sortingAndPagination.SortingBy(sortBy, _booksQuery).ToList();

            // Assert
            result.Should().BeInAscendingOrder(b => b.Author);
        }

        [Fact]
        public void SortingBy_ShouldSortByISBN()
        {
            // Arrange
            var sortBy = "isbn";

            // Act
            var result = _sortingAndPagination.SortingBy(sortBy, _booksQuery).ToList();

            // Assert
            result.Should().BeInAscendingOrder(b => b.ISBN);
        }

        [Fact]
        public void SortingBy_ShouldSortByStatus()
        {
            // Arrange
            var sortBy = "status";

            // Act
            var result = _sortingAndPagination.SortingBy(sortBy, _booksQuery).ToList();

            // Assert
            result.Should().BeInAscendingOrder(b => b.Status);
        }

        [Fact]
        public void SortingBy_ShouldSortById_WhenInvalidSortByProvided()
        {
            // Arrange
            var sortBy = "invalid";

            // Act
            var result = _sortingAndPagination.SortingBy(sortBy, _booksQuery).ToList();

            // Assert
            result.Should().BeInAscendingOrder(b => b.Id);
        }

        [Fact]
        public async Task Items_ShouldReturnCorrectPage()
        {
            // Arrange
            var page = 2;
            var pageSize = 2;

            // Act
            var result = await _sortingAndPagination.Items(page, pageSize, _booksQuery);

            // Assert
            result.Should().HaveCount(2);
            result[0].Id.Should().Be(3);
            result[1].Id.Should().Be(4);
        }

        [Fact]
        public async Task Items_ShouldReturnEmptyList_WhenPageIsOutOfRange()
        {
            // Arrange
            var page = 3;
            var pageSize = 2;

            // Act
            var result = await _sortingAndPagination.Items(page, pageSize, _booksQuery);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Items_ShouldThrowException_WhenPageIsLessThanOne()
        {
            // Arrange
            var page = 0;
            var pageSize = 2;

            // Act
            Func<Task> act = async () => await _sortingAndPagination.Items(page, pageSize, _booksQuery);

            // Assert
            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithMessage("*Page must be greater than zero.*");
        }

        [Fact]
        public async Task Items_ShouldThrowException_WhenPageSizeIsLessThanOne()
        {
            // Arrange
            var page = 1;
            var pageSize = 0;

            // Act
            Func<Task> act = async () => await _sortingAndPagination.Items(page, pageSize, _booksQuery);

            // Assert
            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithMessage("*Page size must be greater than zero.*");
        }
    }
}
