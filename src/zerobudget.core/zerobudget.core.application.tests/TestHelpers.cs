using ECO;
using Moq;
using System.Reflection;

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

    /// <summary>
    /// Sets the Identity property of an aggregate root using reflection.
    /// This is useful for test scenarios where entities need specific IDs.
    /// </summary>
    public static TEntity WithIdentity<TEntity, TKey>(this TEntity entity, TKey identity)
        where TEntity : class, IAggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {
        var identityProperty = typeof(TEntity).GetProperty("Identity", BindingFlags.Public | BindingFlags.Instance);
        if (identityProperty == null)
        {
            throw new InvalidOperationException($"Identity property not found on {typeof(TEntity).Name}");
        }

        // Check if property has a public setter
        if (identityProperty.CanWrite && identityProperty.SetMethod?.IsPublic == true)
        {
            identityProperty.SetValue(entity, identity);
            return entity;
        }

        // Try to find the backing field by searching all fields including inherited ones
        Type? currentType = typeof(TEntity);
        FieldInfo? backingField = null;
        
        while (currentType != null && backingField == null)
        {
            backingField = currentType
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .FirstOrDefault(f => 
                    f.Name.Contains("Identity", StringComparison.OrdinalIgnoreCase) || 
                    f.Name.Contains("<Identity>", StringComparison.Ordinal));
            
            currentType = currentType.BaseType;
        }

        if (backingField != null)
        {
            backingField.SetValue(entity, identity);
            return entity;
        }

        throw new InvalidOperationException($"Cannot set Identity on {typeof(TEntity).Name}. No public setter or accessible backing field found.");
    }
}