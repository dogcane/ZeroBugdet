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
        => OperationResult.MakeSuccess();

    public static OperationResult ValidateMonthlyBucketCreation(short year, short month)
        => OperationResult.MakeSuccess()
            .With(year, nameof(year)).GreaterThenOrEqual((short)2000, "Year must be 2000 or later.").LessThenOrEqual((short)2100, "Year must be 2100 or earlier.")
            .With(month, nameof(month)).GreaterThenOrEqual((short)1, "Month must be 1 or later.").LessThenOrEqual((short)12, "Month must be 12 or earlier.")
            .Result;

    public static OperationResult ValidateStatusChange(bool oldStatus, bool newStatus)
        => OperationResult.MakeSuccess()
            .With(newStatus, nameof(newStatus)).Condition(val => val != oldStatus, "The new status must be different from the current one.")
            .Result;
    #endregion
}
