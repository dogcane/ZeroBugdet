namespace zerobudget.core.application.Commands;

public record CreateTagCommand(
    string Name,
    string Description
);

public record UpdateTagCommand(
    int Id,
    string Name,
    string Description
);

public record DeleteTagCommand(
    int Id
);