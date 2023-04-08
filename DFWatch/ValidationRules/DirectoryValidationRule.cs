// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;

public class DirectoryValidationRule : ValidationRule
{
    /// <summary>
    /// Validation rule for folders
    /// </summary>
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (string.IsNullOrWhiteSpace(value?.ToString()))
        {
            return new ValidationResult(false, "A valid folder name is required");
        }

        if (!Directory.Exists(value.ToString()))
        {
            return new ValidationResult(false, "Folder not found");
        }

        return new ValidationResult(true, null);
    }
}
