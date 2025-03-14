using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ParkingZone.Helpers
{
    public class RegexValidatorAttribute : ValidationAttribute
    {
        private readonly string Pattern = @"^\+?(998)?\d{9}$";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Telefon raqami kiritilishi kerak.");
            }

            var phone = value.ToString();
            if (Regex.IsMatch(phone!, Pattern))
            {
                return ValidationResult.Success!;
            }
            else
            {
                return new ValidationResult("Telefon raqami noto'g'ri formatda.");
            }
        }
    }
    public class RegexValidator
    {
        public static bool IsValidPhone(string phone)
        {
            string pattern = @"^\+?998?\s?\d{2}\s?\d{3}\s?\d{2}\s?\d{2}$|^\+?998?\d{9}$";
            return Regex.IsMatch(phone, pattern);
        }

        public static string NormalizePhoneNumber(string phoneNumber)
        {
            // Remove spaces, dashes, parentheses, or any non-numeric characters except '+'
            string cleanedNumber = Regex.Replace(phoneNumber, @"[^\d+]", "");

            // Handle different cases
            if (cleanedNumber.StartsWith("+998"))
            {
                // If it starts with +998 (country code for Uzbekistan), return as is
                return cleanedNumber;
            }
            else if (cleanedNumber.Length == 9 && cleanedNumber.StartsWith("9"))
            {
                // If it's a local number like 947030820, prepend +998
                return "+998" + cleanedNumber;
            }
            else if (cleanedNumber.Length == 12 && cleanedNumber.StartsWith("998"))
            {
                // If it's missing the '+' but has the country code 998, add the '+'
                return "+" + cleanedNumber;
            }
            else if (cleanedNumber.Length == 9)
            {
                // For short local numbers like 94 7030820, prepend +998
                return "+998" + cleanedNumber;
            }

            // If the number doesn't match any of the formats, return the cleaned number as is
            return cleanedNumber;
        }

        public static bool IsValidName(string input)
        {
            string pattern = @"^\D{2,}$";

            return Regex.IsMatch(input, pattern);
        }
    }
}
