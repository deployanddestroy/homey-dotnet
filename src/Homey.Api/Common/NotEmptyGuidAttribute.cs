using System.ComponentModel.DataAnnotations;

namespace Homey.Api.Common;

/// <summary>
/// Ensures incoming GUIDs are not all 0s.
/// </summary>
public class NotEmptyGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is Guid guid) return guid != Guid.Empty;
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} cannot be an empty GUID.";
    }
}