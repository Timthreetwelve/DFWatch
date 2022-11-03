// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;

public class FileValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult(false, "Cannot be blank");
        }

        if (Directory.Exists(value.ToString()))
        {
            return new ValidationResult(false, "Supply a file name");
        }

        if (!File.Exists(value.ToString()))
        {
            return new ValidationResult(false, "File not found");
        }

        return new ValidationResult(true, null);
    }
}
