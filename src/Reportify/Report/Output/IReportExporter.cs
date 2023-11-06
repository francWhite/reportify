namespace Reportify.Report.Output;

internal interface IReportExporter
{
  void ExportToClipboard(OutputData outputData);
}