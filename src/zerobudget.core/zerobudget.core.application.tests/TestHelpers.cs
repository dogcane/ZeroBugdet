using ECO;
using Moq;

namespace zerobudget.core.application.tests;

/// <summary>
/// Helper methods for setting up test mocks
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Setup a repository mock to act as an IQueryable with the given data
    /// Since IRepository implements IQueryable, we need to setup the IQueryable interface methods
    /// </summary>
    public static void SetupAsQueryable<TRepository, TEntity, TKey>(this Mock<TRepository> repositoryMock, IEnumerable<TEntity> data)
        where TRepository : class, IRepository<TEntity, TKey>
        where TEntity : class, IAggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {
        var queryable = data.AsQueryable();
        repositoryMock.As<IQueryable<TEntity>>()
            .Setup(m => m.Provider).Returns(queryable.Provider);
        repositoryMock.As<IQueryable<TEntity>>()
            .Setup(m => m.Expression).Returns(queryable.Expression);
        repositoryMock.As<IQueryable<TEntity>>()
            .Setup(m => m.ElementType).Returns(queryable.ElementType);
        repositoryMock.As<IQueryable<TEntity>>()
            .Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
    }
}
