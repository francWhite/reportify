namespace Reportify.Report;

internal interface IReportExporter
{
  void ExportToClipboard(Report report);
}