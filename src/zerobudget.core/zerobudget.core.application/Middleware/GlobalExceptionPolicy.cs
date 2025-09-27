using Wolverine;
using Wolverine.Configuration;

namespace zerobudget.core.application.Middleware;

/// <summary>
/// Wolverine policy to apply global exception middleware to all message handlers
/// </summary>
public class GlobalExceptionPolicy : IHandlerPolicy
{
    public void Apply(IReadOnlyList<HandlerChain> chains, GenerationRules rules, IServiceContainer container)
    {
        // Apply the global exception middleware to all handler chains
        foreach (var chain in chains)
        {
            // Add the middleware to wrap all handlers
            chain.Middleware.Add(typeof(GlobalExceptionMiddleware));
        }
    }
}
