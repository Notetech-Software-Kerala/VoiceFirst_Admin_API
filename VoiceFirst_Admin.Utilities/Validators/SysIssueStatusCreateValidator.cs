using FluentValidation;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueStatus;

namespace VoiceFirst_Admin.Utilities.Validators
{
    public class SysIssueStatusCreateValidator : AbstractValidator<SysIssueStatusCreateDTO>
    {
        public SysIssueStatusCreateValidator()
        {
            RuleFor(x => x.IssueStatus)
                .NotEmpty().WithMessage("Issue status is required.")
                .MaximumLength(100).WithMessage("Issue status cannot exceed 100 characters.");
        }
    }
}
