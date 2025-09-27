using System;

namespace zerobudget.core.domain;

public static class TagExtensions
{
    public static string[] ToTagNames(this Tag[] tags) 
        => [.. tags.Select(t => t.Name)];
}
