namespace Reportify.Report.Output;

internal interface IReportExporter
{
  void ExportToClipboard(Report report);
}