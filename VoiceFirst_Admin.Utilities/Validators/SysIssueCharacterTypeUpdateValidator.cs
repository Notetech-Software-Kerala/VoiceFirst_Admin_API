using FluentValidation;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueCharacterType;

namespace VoiceFirst_Admin.Utilities.Validators
{
    public class SysIssueCharacterTypeUpdateValidator : AbstractValidator<SysIssueCharacterTypeUpdateDTO>
    {
        public SysIssueCharacterTypeUpdateValidator()
        {
            RuleFor(x => x.IssueCharacterType)
                .MaximumLength(100).WithMessage("Issue character type cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.IssueCharacterType));
        }
    }
}
