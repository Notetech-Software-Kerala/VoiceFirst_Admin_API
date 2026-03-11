namespace VoiceFirst_Admin.Utilities.Models.Common;

public class BulkValidationResult
{
    public bool IdNotFound { get; init; }
    public bool IsDeleted { get; init; }
    public bool IsInactive { get; init; }
}
