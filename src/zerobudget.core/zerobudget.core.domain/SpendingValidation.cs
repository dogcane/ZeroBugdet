using Resulz;
using Resulz.Validation;

namespace zerobudget.core.domain;

public partial class Spending
{
    #region Methods
    public static OperationResult Validate(string description, decimal amount, string owner, Tag[] tags, bool enabled = true) 
        => OperationResult.MakeSuccess()
            .With(description, nameof(description)).Required("Description is required.").StringLength(500)
            .With(amount, nameof(amount)).GreaterThenOrEqual(0, "Amount must be positive.")
            .With(owner, nameof(owner)).Required("Owner is required.").StringLength(100)
            .With(enabled, nameof(enabled)).EqualTo(true, "Spending must be enabled.")
            .With(tags, nameof(tags)).Condition(t => t?.Length <= 3, "Maximum three tags supported.")
            .Result;

    public static OperationResult Validate(Bucket bucket, string description, decimal amount, string owner, Tag[] tags, bool enabled = true)
        => Validate(description, amount, owner, tags, enabled)
        .With(bucket, nameof(bucket)).Required("Bucket is required.")
        .Result;

    #endregion
}