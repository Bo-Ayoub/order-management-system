using OrderManagement.Application.Common.Models;
using Xunit;

namespace OrderManagement.UnitTests.Application.Common.Models;

public class PaginatedListTests
{
    [Fact]
    public void Constructor_ShouldCalculatePropertiesCorrectly()
    {
        // Arrange
        var items = new List<string> { "item1", "item2", "item3" };
        var totalCount = 10;
        var pageNumber = 2;
        var pageSize = 3;

        // Act
        var paginatedList = new PaginatedList<string>(items, totalCount, pageNumber, pageSize);

        // Assert
        Assert.Equal(items, paginatedList.Items);
        Assert.Equal(pageNumber, paginatedList.PageNumber);
        Assert.Equal(totalCount, paginatedList.TotalCount);
        Assert.Equal(4, paginatedList.TotalPages); // Ceiling(10/3) = 4
    }

    [Fact]
    public void HasPreviousPage_WhenPageNumberGreaterThanOne_ShouldReturnTrue()
    {
        // Arrange
        var items = new List<string>();
        var paginatedList = new PaginatedList<string>(items, 10, 2, 5);

        // Act & Assert
        Assert.True(paginatedList.HasPreviousPage);
    }

    [Fact]
    public void HasPreviousPage_WhenPageNumberIsOne_ShouldReturnFalse()
    {
        // Arrange
        var items = new List<string>();
        var paginatedList = new PaginatedList<string>(items, 10, 1, 5);

        // Act & Assert
        Assert.False(paginatedList.HasPreviousPage);
    }

    [Fact]
    public void HasNextPage_WhenPageNumberLessThanTotalPages_ShouldReturnTrue()
    {
        // Arrange
        var items = new List<string>();
        var paginatedList = new PaginatedList<string>(items, 10, 1, 5); // 2 total pages

        // Act & Assert
        Assert.True(paginatedList.HasNextPage);
    }

    [Fact]
    public void HasNextPage_WhenPageNumberEqualsToTotalPages_ShouldReturnFalse()
    {
        // Arrange
        var items = new List<string>();
        var paginatedList = new PaginatedList<string>(items, 10, 2, 5); // 2 total pages

        // Act & Assert
        Assert.False(paginatedList.HasNextPage);
    }

    [Fact]
    public void Create_ShouldCreatePaginatedListCorrectly()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3 };

        // Act
        var paginatedList = PaginatedList<int>.Create(items, 2, 3, 10);

        // Assert
        Assert.Equal(items, paginatedList.Items);
        Assert.Equal(2, paginatedList.PageNumber);
        Assert.Equal(10, paginatedList.TotalCount);
        Assert.Equal(4, paginatedList.TotalPages);
    }
}