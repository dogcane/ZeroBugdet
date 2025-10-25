using Resulz;
using Resulz.Validation;

namespace zerobudget.core.domain;

public partial class MonthlyBucket
{
    #region Methods
    public static OperationResult Validate(short year, short month, decimal balance, Bucket bucket) 
        => OperationResult.MakeSuccess()
            .With(year, nameof(year)).GreaterThenOrEqual((short)2000, "Year must be 2000 or later.")
            .With(month, nameof(month)).GreaterThenOrEqual((short)1, "Month must be between 1 and 12.").LessThenOrEqual((short)12, "Month must be between 1 and 12.")
            .With(bucket, nameof(bucket)).Required("Bucket is required.")
            .Result;

    public static OperationResult ValidateBalance(decimal balance)
        => OperationResult.MakeSuccess();
    #endregion
}
