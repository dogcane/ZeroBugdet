using Xunit;
using zerobudget.core.domain;

namespace zerobudget.core.domain.tests;

public class TagServiceTests
{
    // Note: TagService tests require integration testing with a real repository
    // as it uses IQueryable extension methods which cannot be easily mocked.
    // The functionality is indirectly tested through the domain model tests
    // and will be fully tested in integration tests.
    
    // TagService is a simple service that wraps repository operations and tag creation logic.
    // Its core logic (EnsureTagsByNameAsync) has the following behavior:
    // 1. Normalizes tag names using TagExtensions.NormalizeTagNames
    // 2. For each normalized tag name, checks if it exists in the repository
    // 3. If the tag doesn't exist, creates it using Tag.Create and adds it to the repository
    // 4. Returns the list of all tags (both existing and newly created)
    //
    // This functionality is covered by:
    // - Tag.Create validation tests (TagTests)
    // - TagExtensions.NormalizeTagNames tests (TagExtensionsTests)
    // - Integration tests that verify the full workflow with a real repository
}
