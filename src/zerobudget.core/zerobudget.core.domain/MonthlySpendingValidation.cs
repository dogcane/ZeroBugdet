using Resulz;
using Resulz.Validation;

namespace zerobudget.core.domain;

public partial class MonthlySpending
{
    #region Methods
    public static OperationResult Validate(DateOnly date, string description, decimal amount, string owner, string[] tags)
    {
        return OperationResult.MakeSuccess()
            .With(date, nameof(date)).Required("Date is required.")
            .With(description, nameof(description)).Required("Description is required.").StringLength(500)
            .With(amount, nameof(amount)).GreaterThenOrEqual(0, "Amount must be positive.")
            .With(owner, nameof(owner)).Required("Owner is required.").StringLength(100)
            .With(tags, nameof(tags)).Condition(t => t?.Length <= 3, "Maximum three tags supported.")
            .Result;
    }
    #endregion
}
