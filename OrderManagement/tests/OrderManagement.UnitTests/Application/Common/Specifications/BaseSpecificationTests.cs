using OrderManagement.Application.Common.Specifications;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.ValueObjects;
using Xunit;

namespace OrderManagement.UnitTests.Application.Common.Specifications;

public class BaseSpecificationTests
{
    [Fact]
    public void Constructor_WithNoCriteria_ShouldNotHaveCriteria()
    {
        // Arrange & Act
        var specification = new TestSpecification();

        // Assert
        Assert.Null(specification.Criteria);
        Assert.Empty(specification.Includes);
        Assert.False(specification.IsPagingEnabled);
    }

    [Fact]
    public void Constructor_WithCriteria_ShouldSetCriteria()
    {
        // Arrange & Act
        var specification = new TestSpecification(p => p.IsActive);

        // Assert
        Assert.NotNull(specification.Criteria);
        Assert.Empty(specification.Includes);
    }

    [Fact]
    public void ApplyPaging_ShouldSetPagingProperties()
    {
        // Arrange
        var specification = new TestSpecification();

        // Act
        specification.TestApplyPaging(10, 20);

        // Assert
        Assert.True(specification.IsPagingEnabled);
        Assert.Equal(10, specification.Skip);
        Assert.Equal(20, specification.Take);
    }

    [Fact]
    public void ApplyOrderBy_ShouldSetOrderByExpression()
    {
        // Arrange
        var specification = new TestSpecification();

        // Act
        specification.TestApplyOrderBy(p => p.Name);

        // Assert
        Assert.NotNull(specification.OrderBy);
        Assert.Null(specification.OrderByDescending);
    }

    [Fact]
    public void ApplyOrderByDescending_ShouldSetOrderByDescendingExpression()
    {
        // Arrange
        var specification = new TestSpecification();

        // Act
        specification.TestApplyOrderByDescending(p => p.CreatedAt);

        // Assert
        Assert.NotNull(specification.OrderByDescending);
        Assert.Null(specification.OrderBy);
    }

    [Fact]
    public void AddInclude_ShouldAddToIncludesList()
    {
        // Arrange
        var specification = new TestSpecification();

        // Act
        specification.TestAddInclude(p => p.Price);

        // Assert
        Assert.Single(specification.Includes);
    }

    [Fact]
    public void AddIncludeString_ShouldAddToIncludeStringsList()
    {
        // Arrange
        var specification = new TestSpecification();

        // Act
        specification.TestAddInclude("Category");

        // Assert
        Assert.Single(specification.IncludeStrings);
        Assert.Equal("Category", specification.IncludeStrings.First());
    }

    // Test specification class to expose protected methods
    private class TestSpecification : BaseSpecification<Product>
    {
        public TestSpecification() : base() { }

        public TestSpecification(System.Linq.Expressions.Expression<System.Func<Product, bool>> criteria)
            : base(criteria) { }

        public void TestApplyPaging(int skip, int take) => ApplyPaging(skip, take);
        public void TestApplyOrderBy(System.Linq.Expressions.Expression<System.Func<Product, object>> orderBy) => ApplyOrderBy(orderBy);
        public void TestApplyOrderByDescending(System.Linq.Expressions.Expression<System.Func<Product, object>> orderByDesc) => ApplyOrderByDescending(orderByDesc);
        public void TestAddInclude(System.Linq.Expressions.Expression<System.Func<Product, object>> include) => AddInclude(include);
        public void TestAddInclude(string include) => AddInclude(include);
    }
}
