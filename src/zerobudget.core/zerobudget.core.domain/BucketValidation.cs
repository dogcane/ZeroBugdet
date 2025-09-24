using Resulz;
using Resulz.Validation;

namespace zerobudget.core.domain;

public partial class Bucket
{
    #region Methods
    public static OperationResult Validate(string name, string description, decimal defaultLimit, bool enabled = true)
    {
        return OperationResult.MakeSuccess()
            .With(name, nameof(name)).Required("Name is required.").StringLength(100)
            .With(description, nameof(description)).Required("Description is required.").StringLength(500)
            .With(defaultLimit, nameof(defaultLimit)).GreaterThenOrEqual(0, "Default limit must be positive.")
            .With(enabled, nameof(enabled)).EqualTo(true, "Bucket must be enabled.")
            .Result;
    }
    #endregion
}
