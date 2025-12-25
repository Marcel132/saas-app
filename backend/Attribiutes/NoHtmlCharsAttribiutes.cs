using System.ComponentModel.DataAnnotations;

public class NoHtmlCharsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is not IEnumerable<string> list)
            return ValidationResult.Success;

        foreach (var item in list)
        {
            if (item.Contains('<') || item.Contains('>') || item.Contains('%'))
                return new ValidationResult("Invalid characters in list.");
        }

        return ValidationResult.Success;
    }
}