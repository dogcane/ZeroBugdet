namespace zerobudget.core.application.Commands;

public record CreateTagCommand(
    string Name
);

public record DeleteTagCommand(
    int Id
);

public record CleanupUnusedTagsCommand();