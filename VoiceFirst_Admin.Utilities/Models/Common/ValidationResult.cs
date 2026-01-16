using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Models.Common;

public sealed class ValidationResult
{
    private readonly List<ValidationError> _errors = new();

    public IReadOnlyList<ValidationError> Errors => _errors;
    public bool IsValid => _errors.Count == 0;

    public void Add(string field, string message, string? code = null)
        => _errors.Add(new ValidationError { Field = field, Message = message, Code = code });

    public override string ToString()
        => IsValid ? "Valid" : string.Join("; ", _errors.Select(e => $"{e.Field}: {e.Message}"));
}
