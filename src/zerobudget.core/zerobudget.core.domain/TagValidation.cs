using Resulz;
using Resulz.Validation;

namespace zerobudget.core.domain;

public partial class Tag
{
    #region Methods
    public static OperationResult Validate(string name)
    {
        return OperationResult.MakeSuccess()
            .With(name, nameof(name)).Required("Name is required.").StringLength(100)
            .Result;
    }
    #endregion
}
