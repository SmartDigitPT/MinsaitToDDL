using System.Text.RegularExpressions;

namespace MinsaitToDDL.Lib.Utilities
{
    public static class Utilities
    {
        public static string ExtractPostalCode(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Pattern: optional whitespace, postal code (NNNN or NNNN-NNN), whitespace or end of string
            var match = Regex.Match(input, @"^\s*(\d{4}(?:-\d{3})?)\b");
            if (match.Success)
                return match.Groups[1].Value;

            return string.Empty;
        }

        public static string ExtractTextAfterPostalCode(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            // Pattern: optional whitespace, postal code (NNNN or NNNN-NNN), whitespace, then capture the rest
            var match = Regex.Match(input, @"^\s*(\d{4}(?:-\d{3})?)\s+(.*)$");
            if (match.Success)
                return match.Groups[2].Value.Trim();

            // If no postal code, return the original string trimmed
            return input.Trim();
        }
    }
}
