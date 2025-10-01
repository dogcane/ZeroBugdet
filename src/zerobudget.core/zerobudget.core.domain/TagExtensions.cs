using System;

namespace zerobudget.core.domain;

public static class TagExtensions
{
    public static string[] ToTagNames(this Tag[] tags)
        => [.. tags
            .Select(t => t.Name.ToLowerInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(t => t, StringComparer.OrdinalIgnoreCase)
        ];
        
    public static string[] NormalizeTagNames(this string[] tagNames)
        => [.. tagNames
            .Where(tn => !string.IsNullOrWhiteSpace(tn))
            .Select(tn => tn.Trim().ToLowerInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(tn => tn, StringComparer.OrdinalIgnoreCase)
        ];
}
