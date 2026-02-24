using FluentValidation;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType;

namespace VoiceFirst_Admin.Utilities.Validators
{
    public class SysIssueTypeUpdateValidator : AbstractValidator<SysIssueTypeUpdateDTO>
    {
        public SysIssueTypeUpdateValidator()
        {
            RuleFor(x => x.IssueType)
                .MaximumLength(100).WithMessage("Issue type name cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.IssueType));
        }
    }
}
