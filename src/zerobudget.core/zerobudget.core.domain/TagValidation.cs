using Resulz;
using Resulz.Validation;
using System.Text.RegularExpressions;

namespace zerobudget.core.domain;

public partial class Tag
{
    #region Methods
    public static OperationResult Validate(string name, string description) => OperationResult.MakeSuccess()
            .With(name, nameof(name)).StringMatch("^[a-zA-Z0-9]{4,50}$", "Name must contain only letters and numbers and be between 4 and 50 characters long.").Required("Name is required.")
            .With(description, nameof(description)).Required("Description is required.").StringLength(500)
            .Result;
    #endregion
}
