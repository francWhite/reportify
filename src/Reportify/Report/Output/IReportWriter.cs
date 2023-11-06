namespace Reportify.Report.Output;

internal interface IReportWriter
{
  void Write(OutputData outputData);
}