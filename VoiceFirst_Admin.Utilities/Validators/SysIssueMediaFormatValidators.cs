using FluentValidation;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueMediaFormat;

namespace VoiceFirst_Admin.Utilities.Validators
{
    public class SysIssueMediaFormatCreateValidator : AbstractValidator<SysIssueMediaFormatCreateDTO> { public SysIssueMediaFormatCreateValidator() { RuleFor(x => x.IssueMediaFormat).NotEmpty().WithMessage("Issue media format is required.").MaximumLength(100).WithMessage("Issue media format cannot exceed 100 characters."); } }
    public class SysIssueMediaFormatUpdateValidator : AbstractValidator<SysIssueMediaFormatUpdateDTO> { public SysIssueMediaFormatUpdateValidator() { RuleFor(x => x.IssueMediaFormat).MaximumLength(100).WithMessage("Issue media format cannot exceed 100 characters.").When(x => !string.IsNullOrWhiteSpace(x.IssueMediaFormat)); } }
}
