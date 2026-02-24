using FluentValidation;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueMediaType;

namespace VoiceFirst_Admin.Utilities.Validators
{
    public class SysIssueMediaTypeCreateValidator : AbstractValidator<SysIssueMediaTypeCreateDTO>
    {
        public SysIssueMediaTypeCreateValidator() { RuleFor(x => x.IssueMediaType).NotEmpty().WithMessage("Issue media type is required.").MaximumLength(100).WithMessage("Issue media type cannot exceed 100 characters."); }
    }

    public class SysIssueMediaTypeUpdateValidator : AbstractValidator<SysIssueMediaTypeUpdateDTO>
    {
        public SysIssueMediaTypeUpdateValidator() { RuleFor(x => x.IssueMediaType).MaximumLength(100).WithMessage("Issue media type cannot exceed 100 characters.").When(x => !string.IsNullOrWhiteSpace(x.IssueMediaType)); }
    }
}
