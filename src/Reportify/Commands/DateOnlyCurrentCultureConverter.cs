using System.ComponentModel;
using System.Globalization;

namespace Reportify.Commands;

//workaround because Spectre.Console.Cli does always use InvariantCulture
internal class DateOnlyCurrentCultureConverter : DateOnlyConverter
{
  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    return base.ConvertFrom(context, CultureInfo.CurrentCulture, value);
  }
}