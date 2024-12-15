using System.ComponentModel.DataAnnotations;
using System.Text;

namespace API.Utilities;

/// <summary>
/// Specifies the minimum and maximum length of a string property in bits.
/// </summary>
public class BitLengthAttribute : ValidationAttribute
{
    public int Minimum { get; set; }
    public int Maximum { get; set; } = int.MaxValue;

    public override bool IsValid(object? value)
    {
        var strValue = value as string;
        if (string.IsNullOrEmpty(strValue))
        {
            return true;
        }

        // Multiply n bytes by 8 to get n bits
        var bits = Encoding.UTF8.GetBytes(strValue).Length * 8;
        return bits >= Minimum && bits <= Maximum;
    }
}
