using System.Net;
using Resulz;
using Resulz.Validation;

namespace zerobudget.core.domain;

public partial class Bucket
{
    #region Methods
    public static OperationResult Validate(string name, string description, decimal defaultLimit, bool enabled = true)
        => OperationResult.MakeSuccess()
            .With(name, nameof(name)).Required("Name is required.").StringLength(100)
            .With(description, nameof(description)).Required("Description is required.").StringLength(500)
            .With(defaultLimit, nameof(defaultLimit)).GreaterThenOrEqual(0, "Default limit must be positive.")
            .With(enabled, nameof(enabled)).EqualTo(true, "Bucket must be enabled.")
            .Result;
    public static OperationResult ValidateDefaultBalance(decimal defaultBalance)
        => OperationResult.MakeSuccess()
            .With(defaultBalance, nameof(defaultBalance)).GreaterThenOrEqual(0, "Default balance must be positive.")
            .Result;

    public static OperationResult ValidateMonthlyBucketCreation(short year, short month)
        => OperationResult.MakeSuccess()
            .With(year, nameof(year)).Between((short)2000, (short)2100, "Year must be between 2000 and 2100.")
            .With(month, nameof(month)).Between((short)1, (short)12, "Month must be between 1 and 12.")
            .Result;

    public static OperationResult ValidateStatusChange(bool oldStatus, bool newStatus)
        => OperationResult.MakeSuccess()
            .With(newStatus, nameof(newStatus)).Condition(val => val != oldStatus, "The new status must be different from the current one.")
            .Result;
    #endregion
}
