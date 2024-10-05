using ARforce.Common;
using ARforce.Models;
using FluentAssertions;

namespace ARforce.Tests
{
    public class BookStatusValidatorTests
    {
        private readonly IBookStatusValidator _validator;

        public BookStatusValidatorTests()
        {
            _validator = new BookStatusValidator();
        }

        [Theory]
        // Transistion to BookStatus.OnShelf
        [InlineData(BookStatus.Returned, BookStatus.OnShelf, true)]
        [InlineData(BookStatus.Damaged, BookStatus.OnShelf, true)]
        [InlineData(BookStatus.Borrowed, BookStatus.OnShelf, false)]
        [InlineData(BookStatus.OnShelf, BookStatus.OnShelf, false)]

        // Transistion to BookStatus.Borrowed
        [InlineData(BookStatus.OnShelf, BookStatus.Borrowed, true)]
        [InlineData(BookStatus.Returned, BookStatus.Borrowed, false)]
        [InlineData(BookStatus.Damaged, BookStatus.Borrowed, false)]
        [InlineData(BookStatus.Borrowed, BookStatus.Borrowed, false)]

        // Transistion to BookStatus.Returned
        [InlineData(BookStatus.Borrowed, BookStatus.Returned, true)]
        [InlineData(BookStatus.OnShelf, BookStatus.Returned, false)]
        [InlineData(BookStatus.Damaged, BookStatus.Returned, false)]
        [InlineData(BookStatus.Returned, BookStatus.Returned, false)]

        // Transistion to BookStatus.Damaged
        [InlineData(BookStatus.OnShelf, BookStatus.Damaged, true)]
        [InlineData(BookStatus.Returned, BookStatus.Damaged, true)]
        [InlineData(BookStatus.Borrowed, BookStatus.Damaged, false)]
        [InlineData(BookStatus.Damaged, BookStatus.Damaged, false)]

        // False Transistion 
        [InlineData(BookStatus.Borrowed, (BookStatus)999, false)] 
        public void IsValidTransition_ShouldReturnExpectedResult(BookStatus currentStatus, BookStatus newStatus, bool expectedResult)
        {
            // Act
            var result = _validator.IsValidTransition(currentStatus, newStatus);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void IsValidTransition_ShouldReturnFalse_WhenNewStatusIsInvalid()
        {
            // Arrange
            var currentStatus = BookStatus.OnShelf;
            var newStatus = (BookStatus)999; 

            // Act
            var result = _validator.IsValidTransition(currentStatus, newStatus);

            // Assert
            result.Should().BeFalse();
        }
    }
}
