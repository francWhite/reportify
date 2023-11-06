using Reportify.Commands;
using Reportify.Configuration.Validation;
using Reportify.Extensions;
using Reportify.Report;
using Reportify.Report.Output;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Reportify.Tests.Commands;

public class ReportCommandTest
{
  private readonly ReportCommand _sut;

  private readonly IReportBuilder _reportBuilder;
  private readonly IReportWriter _reportWriter;
  private readonly IReportExporter _reportExporter;
  private readonly IConfigurationValidator _configurationValidator;
  private readonly IValidationErrorWriter _validationErrorWriter;
  private readonly IOutputDataConverter _outputDataConverter;
  private readonly CommandContext _commandContext;
  private readonly TestConsole _testConsole;

  public ReportCommandTest()
  {
    _reportBuilder = A.Fake<IReportBuilder>();
    _reportWriter = A.Fake<IReportWriter>();
    _reportExporter = A.Fake<IReportExporter>();
    _configurationValidator = A.Fake<IConfigurationValidator>();
    _validationErrorWriter = A.Fake<IValidationErrorWriter>();
    _outputDataConverter = A.Fake<IOutputDataConverter>();
    _commandContext = A.Dummy<CommandContext>();

    _testConsole = new TestConsole();
    AnsiConsole.Console = _testConsole;

    _sut = new ReportCommand(
      _reportBuilder,
      _reportWriter,
      _reportExporter,
      _configurationValidator,
      _validationErrorWriter,
      _outputDataConverter);
  }

  [Fact]
  public async Task ExecuteAsync_WhenPrintVersionIsSet_ShouldPrintVersion()
  {
    var settings = new ReportCommandSettings { PrintVersion = true };

    var result = await _sut.ExecuteAsync(_commandContext, settings);

    result.Should().Be(0);
    _testConsole.Output.Should().Contain("reportify version");
  }

  [Fact]
  public async Task ExecuteAsync_WhenConfigurationIsInvalid_ShouldReturnFailureAndWriteValidationErrors()
  {
    var settings = new ReportCommandSettings();
    var validationErrors = new List<ValidationError> { A.Dummy<ValidationError>() };
    A.CallTo(() => _configurationValidator.ValidateAsync())
      .Returns(validationErrors);

    var result = await _sut.ExecuteAsync(_commandContext, settings);
    result.Should().Be(1);

    A.CallTo(() => _validationErrorWriter.Write(validationErrors))
      .MustHaveHappenedOnceExactly();
  }

  [Fact]
  public async Task ExecuteAsync_WhenConfigurationIsValid_ShouldReturnSuccess()
  {
    var settings = new ReportCommandSettings();

    A.CallTo(() => _configurationValidator.ValidateAsync())
      .Returns(new List<ValidationError>());

    var result = await _sut.ExecuteAsync(_commandContext, settings);
    result.Should().Be(0);

    A.CallTo(() => _validationErrorWriter.Write(A<IReadOnlyList<ValidationError>>._))
      .MustNotHaveHappened();
  }

  [Fact]
  public async Task ExecuteAsync_WhenConfigurationIsValid_ShouldBuildAndWriteReport()
  {
    var settings = new ReportCommandSettings();

    A.CallTo(() => _configurationValidator.ValidateAsync())
      .Returns(new List<ValidationError>());

    var result = await _sut.ExecuteAsync(_commandContext, settings);
    result.Should().Be(0);

    A.CallTo(() => _reportBuilder.BuildAsync(A<DateOnly>._, A<CancellationToken>._))
      .MustHaveHappenedOnceExactly();
  }

  [Fact]
  public async Task ExecuteAsync_WhenCopyToClipboardIsSet_ShouldExportReportToClipboard()
  {
    var settings = new ReportCommandSettings { CopyToClipboard = true };

    A.CallTo(() => _configurationValidator.ValidateAsync())
      .Returns(new List<ValidationError>());

    var result = await _sut.ExecuteAsync(_commandContext, settings);
    result.Should().Be(0);

    A.CallTo(() => _reportExporter.ExportToClipboard(A<OutputData>._))
      .MustHaveHappenedOnceExactly();
  }

  [Fact]
  public async Task ExecuteAsync_WhenCopyToClipboardIsNotSet_ShouldNotExportReportToClipboard()
  {
    var settings = new ReportCommandSettings { CopyToClipboard = false };

    A.CallTo(() => _configurationValidator.ValidateAsync())
      .Returns(new List<ValidationError>());

    var result = await _sut.ExecuteAsync(_commandContext, settings);
    result.Should().Be(0);

    A.CallTo(() => _reportExporter.ExportToClipboard(A<OutputData>._))
      .MustNotHaveHappened();
  }

  [Fact]
  public async Task ExecuteAsync_WhenReportIsGenerated_ShouldWriteReport()
  {
    var settings = new ReportCommandSettings();

    A.CallTo(() => _configurationValidator.ValidateAsync())
      .Returns(new List<ValidationError>());

    var report = A.Dummy<Reportify.Report.Report>();
    A.CallTo(() => _reportBuilder.BuildAsync(A<DateOnly>._, A<CancellationToken>._))
      .Returns(report);

    var outputData = A.Dummy<OutputData>();
    A.CallTo(() => _outputDataConverter.Convert(report))
      .Returns(outputData);

    var result = await _sut.ExecuteAsync(_commandContext, settings);
    result.Should().Be(0);

    A.CallTo(() => _reportWriter.Write(outputData))
      .MustHaveHappenedOnceExactly();
  }

  [Fact]
  public async Task ExecuteAsync_WhenEntireWeekIsSet_ShouldBuildReportForEntireWeek()
  {
    var settings = new ReportCommandSettings { EntireWeek = true };

    A.CallTo(() => _configurationValidator.ValidateAsync())
      .Returns(new List<ValidationError>());

    // TODO: Use some kind of faked date provider
    var today = DateOnly.FromDateTime(DateTime.Now);

    var result = await _sut.ExecuteAsync(_commandContext, settings);
    result.Should().Be(0);

    A.CallTo(
        () => _reportBuilder.BuildAsync(today.GetFirstDayOfWeek(), today.GetLastDayOfWeek(), A<CancellationToken>._))
      .MustHaveHappenedOnceExactly();
  }
}