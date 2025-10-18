using ECO.Integrations.Moq;
using Moq;
using Xunit;
using zerobudget.core.application.Handlers.Queries;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;

namespace zerobudget.core.application.tests;

public class BucketQueryHandlerTests
{
    [Fact]
    public async Task Handle_GetBucketByIdQuery_ShouldReturnBucketDto()
    {
        // Arrange
        var bucketRepository = new Mock<IBucketRepository>();
        var handler = new BucketQueryHandlers(bucketRepository.Object);
        var bucketResult = Bucket.Create("Test Bucket", "Test Description", 1000m);
        var bucket = bucketResult.Value!;

        var query = new GetBucketByIdQuery(1);

        bucketRepository
            .SetupRepository<IBucketRepository, Bucket, int>([bucket]);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Bucket", result.Name);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal(1000m, result.DefaultLimit);
    }

    [Fact]
    public async Task Handle_GetBucketByIdQuery_WithNonExistentBucket_ShouldReturnNull()
    {
        // Arrange
        var bucketRepository = new Mock<IBucketRepository>();
        var handler = new BucketQueryHandlers(bucketRepository.Object);
        var query = new GetBucketByIdQuery(999);

        bucketRepository
            .SetupRepository<IBucketRepository, Bucket, int>([]);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_GetBucketsByNameQuery_ShouldReturnBucketsWithMatchingName()
    {
        // Arrange
        var bucketRepository = new Mock<IBucketRepository>();
        var handler = new BucketQueryHandlers(bucketRepository.Object);

        var bucket1Result = Bucket.Create("Bucket 1", "Description 1", 1000m);
        var bucket2Result = Bucket.Create("Bucket 2", "Description 2", 2000m);
        var buckets = new[] { bucket1Result.Value!, bucket2Result.Value! };

        var query = new GetBucketsByNameQuery("Bucket", "", true);

        bucketRepository
            .SetupRepository<IBucketRepository, Bucket, int>(buckets);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, b => b.Name == "Bucket 1");
        Assert.Contains(result, b => b.Name == "Bucket 2");
    }
}
