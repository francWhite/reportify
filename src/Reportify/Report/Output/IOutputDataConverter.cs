namespace Reportify.Report.Output;

internal interface IOutputDataConverter
{
  OutputData Convert(Report report);
}