namespace zerobudget.core.application.Commands;

public record CreateTagCommand(
    string Name
);

public record UpdateTagCommand(
    int Id,
    string Name
);

public record DeleteTagCommand(
    int Id
);