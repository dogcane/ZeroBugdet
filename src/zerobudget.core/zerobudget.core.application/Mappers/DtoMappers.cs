using zerobudget.core.domain;
using zerobudget.core.application.DTOs;
using Riok.Mapperly.Abstractions;

namespace zerobudget.core.application.Mappers;

[Mapper(ThrowOnMappingNullMismatch = false)]
public partial class SpendingMapper
{
    public SpendingDto ToDto(Spending spending) => new(
            spending.Identity,
            spending.BucketId,
            spending.Description,
            spending.Amount,
            spending.Owner,
            spending.Tags,
            spending.Enabled
        );
}


[Mapper(ThrowOnMappingNullMismatch = false)]
public partial class BucketMapper
{
    public BucketDto ToDto(Bucket bucket) => new(
            bucket.Identity,
            bucket.Name,
            bucket.Description,
            bucket.DefaultLimit,
            bucket.DefaultBalance,
            bucket.Enabled
        );
}


[Mapper(ThrowOnMappingNullMismatch = false)]
public partial class MonthlySpendingMapper
{
    public MonthlySpendingDto ToDto(MonthlySpending monthlySpending) => new(
            monthlySpending.Identity,
            monthlySpending.Date,
            monthlySpending.MonthlyBucketId,
            monthlySpending.Description,
            monthlySpending.Amount,
            monthlySpending.Owner,
            monthlySpending.Tags
        );
}

[Mapper(ThrowOnMappingNullMismatch = false)]
public partial class TagMapper
{
    public TagDto ToDto(Tag tag) => new(
            tag.Identity,
            tag.Name
        );
}

[Mapper(ThrowOnMappingNullMismatch = false)]
public partial class MonthlyBucketMapper
{
    public MonthlyBucketDto ToDto(MonthlyBucket monthlyBucket) => new MonthlyBucketDto(
            monthlyBucket.Identity,
            monthlyBucket.Year,
            monthlyBucket.Month,
            monthlyBucket.Balance,
            monthlyBucket.Description,
            monthlyBucket.Limit,
            monthlyBucket.BucketId
        );
}
