namespace zerobudget.core.application.Commands;

public record GenerateMonthlyDataCommand(
    short Year,
    short Month
);