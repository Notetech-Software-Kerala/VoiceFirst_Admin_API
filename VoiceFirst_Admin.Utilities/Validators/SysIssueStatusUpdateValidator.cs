using FluentValidation;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueStatus;

namespace VoiceFirst_Admin.Utilities.Validators
{
    public class SysIssueStatusUpdateValidator : AbstractValidator<SysIssueStatusUpdateDTO>
    {
        public SysIssueStatusUpdateValidator()
        {
            RuleFor(x => x.IssueStatus)
                .MaximumLength(100).WithMessage("Issue status cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.IssueStatus));
        }
    }
}
