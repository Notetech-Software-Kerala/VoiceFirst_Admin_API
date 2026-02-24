using FluentValidation;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType;

namespace VoiceFirst_Admin.Utilities.Validators
{
    public class SysIssueTypeCreateValidator : AbstractValidator<SysIssueTypeCreateDTO>
    {
        public SysIssueTypeCreateValidator()
        {
            RuleFor(x => x.IssueType)
                .NotEmpty().WithMessage("Issue type name is required.")
                .MaximumLength(100).WithMessage("Issue type name cannot exceed 100 characters.");
        }
    }
}
