namespace Reportify.Report;

public record Report(IEnumerable<DailyReport> DailyReports);

public record DailyReport(DateOnly Date, IEnumerable<Position> Positions);

public record Position(string Name, TimeSpan Duration, int? ErpContract, int? ErpPosition);