using Microsoft.IdentityModel.Tokens.Experimental;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Common;



public static class Validate
{
    // ---------- Core helpers ----------
    public static ValidationResult New() => new();

    public static bool HasText(string? value) => !string.IsNullOrWhiteSpace(value);

    public static void Required(ValidationResult r, string field, string? value, string message = "Required", string? code = "VAL_REQUIRED")
    {
        if (string.IsNullOrWhiteSpace(value)) r.Add(field, message, code);
    }

    public static void Required<T>(ValidationResult r, string field, T? value, string message = "Required", string? code = "VAL_REQUIRED")
        where T : struct
    {
        if (!value.HasValue) r.Add(field, message, code);
    }

    public static void MinLength(ValidationResult r, string field, string? value, int min, string? code = "VAL_MINLEN")
    {
        if (value is null) return;
        if (value.Length < min) r.Add(field, $"Must be at least {min} characters.", code);
    }

    public static void MaxLength(ValidationResult r, string field, string? value, int max, string? code = "VAL_MAXLEN")
    {
        if (value is null) return;
        if (value.Length > max) r.Add(field, $"Must be at most {max} characters.", code);
    }

    public static void LengthBetween(ValidationResult r, string field, string? value, int min, int max, string? code = "VAL_LEN")
    {
        if (value is null) return;
        if (value.Length < min || value.Length > max) r.Add(field, $"Must be {min}-{max} characters.", code);
    }

    public static void Range(ValidationResult r, string field, int value, int min, int max, string? code = "VAL_RANGE")
    {
        if (value < min || value > max) r.Add(field, $"Must be between {min} and {max}.", code);
    }

    public static void Range(ValidationResult r, string field, decimal value, decimal min, decimal max, string? code = "VAL_RANGE")
    {
        if (value < min || value > max) r.Add(field, $"Must be between {min} and {max}.", code);
    }

    public static void GreaterThan(ValidationResult r, string field, int value, int minExclusive, string? code = "VAL_GT")
    {
        if (value <= minExclusive) r.Add(field, $"Must be greater than {minExclusive}.", code);
    }

    public static void RegexMatch(ValidationResult r, string field, string? value, string pattern, string message = "Invalid format", string? code = "VAL_REGEX")
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant)) r.Add(field, message, code);
    }

    public static void In(ValidationResult r, string field, string? value, params string[] allowed)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        if (!allowed.Contains(value, StringComparer.OrdinalIgnoreCase))
            r.Add(field, $"Must be one of: {string.Join(", ", allowed)}.", "VAL_IN");
    }

    // ---------- Common types ----------
    public static void Email(ValidationResult r, string field, string? value, string? code = "VAL_EMAIL")
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        try { _ = new MailAddress(value); }
        catch { r.Add(field, "Invalid email address.", code); }
    }

    // E.164-ish: + and 8-15 digits (simple, not country-specific)
    public static void Phone(ValidationResult r, string field, string? value, string? code = "VAL_PHONE")
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        if (!Regex.IsMatch(value, @"^\+?[1-9]\d{7,14}$")) r.Add(field, "Invalid phone number.", code);
    }

    public static void Guid(ValidationResult r, string field, string? value, string? code = "VAL_GUID")
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        if (!System.Guid.TryParse(value, out _)) r.Add(field, "Invalid GUID.", code);
    }

    public static void EnumValue<TEnum>(ValidationResult r, string field, string? value, bool ignoreCase = true, string? code = "VAL_ENUM")
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        if (!Enum.TryParse<TEnum>(value, ignoreCase, out _)) r.Add(field, "Invalid value.", code);
    }

    // ---------- Numbers & dates ----------
    public static void Int(ValidationResult r, string field, string? value, string? code = "VAL_INT")
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            r.Add(field, "Must be an integer.", code);
    }

    public static void Decimal(ValidationResult r, string field, string? value, string? code = "VAL_DECIMAL")
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out _))
            r.Add(field, "Must be a decimal number.", code);
    }

    public static void DateIso(ValidationResult r, string field, string? value, string? code = "VAL_DATE")
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        if (!DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            r.Add(field, "Must be ISO date (yyyy-MM-dd).", code);
    }

    public static void DateRange(ValidationResult r, string startField, DateTime start, string endField, DateTime end, string? code = "VAL_DATERANGE")
    {
        if (end < start) r.Add(endField, $"Must be >= {startField}.", code);
    }

    // ---------- Password (customizable) ----------
    public static void PasswordStrong(
        ValidationResult r,
        string field,
        string? value,
        int minLen = 8,
        bool requireUpper = true,
        bool requireLower = true,
        bool requireDigit = true,
        bool requireSymbol = true,
        string? code = "VAL_PASSWORD")
    {
        if (string.IsNullOrWhiteSpace(value)) return;

        if (value.Length < minLen) r.Add(field, $"Password must be at least {minLen} characters.", code);
        if (requireUpper && !value.Any(char.IsUpper)) r.Add(field, "Password must include an uppercase letter.", code);
        if (requireLower && !value.Any(char.IsLower)) r.Add(field, "Password must include a lowercase letter.", code);
        if (requireDigit && !value.Any(char.IsDigit)) r.Add(field, "Password must include a digit.", code);
        if (requireSymbol && !value.Any(ch => !char.IsLetterOrDigit(ch))) r.Add(field, "Password must include a symbol.", code);
    }
}
