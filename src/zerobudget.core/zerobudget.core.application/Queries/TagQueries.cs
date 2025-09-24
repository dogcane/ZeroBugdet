namespace zerobudget.core.application.Queries;

public record GetTagByIdQuery(int Id);

public record GetAllTagsQuery();

public record GetTagsByNameQuery(string Name);