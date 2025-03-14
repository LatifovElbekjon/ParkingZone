using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

public class DateGreaterThanOrEqualToNowAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        
        DateTime startTime;
        var formats = new[] { "dd.MM.yyyy H:mm", "dd.MM.yyyy HH:mm" };  // Support both "8:00" and "08:00" formats

        // Handle if value is DateTime
        if (value is string stringValue)
        {
            var culture = new CultureInfo("de-DE");
            if (!DateTime.TryParseExact(stringValue, formats, culture, DateTimeStyles.None, out startTime))
            {
                return new ValidationResult("Noto'g'ri sana formati. Iltimos, 'dd.MM.yyyy HH:mm' formatini kiriting.");
            }
        }
        else if (value is DateTime startTime2) // value ni to'g'ridan-to'g'ri DateTime sifatida tekshirish
        {
            // Hozirgi vaqt bilan solishtirish
            if (startTime2 < DateTime.Now)
            {
                return new ValidationResult("Start time must be greater than or equal to the current time.");
            }

            return ValidationResult.Success!;
        }
        else
        {
            return new ValidationResult("Noto'g'ri ma'lumot turi.");
        }

        // Check if the start time is in the future or at least equal to the current time
        if (startTime < DateTime.Now)
        {
            return new ValidationResult("Start time must be greater than or equal to the current time.");
        }

        return ValidationResult.Success!;
    }
}
