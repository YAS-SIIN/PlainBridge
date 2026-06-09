
using System.ComponentModel.DataAnnotations;
using System.Reflection; 
 

namespace PlainBridge.SharedApplication.Extensions;

public static class EnumExtensions
{
    public static string ToDisplayName(this Enum enumValue)
    {
        return enumValue.GetType()
          .GetMember(enumValue.ToString())
          .First()
          .GetCustomAttribute<DisplayAttribute>()
          ?.GetName() ?? string.Empty;
    }
}
