namespace zerobudget.core.application.DTOs;

public record TagDto(
    int Id,
    string Name
);

public record CreateTagDto(
    string Name
);

public record UpdateTagDto(
    int Id,
    string Name
);