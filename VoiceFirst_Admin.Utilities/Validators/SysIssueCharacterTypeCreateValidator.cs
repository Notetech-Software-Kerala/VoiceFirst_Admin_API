using FluentValidation;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueCharacterType;

namespace VoiceFirst_Admin.Utilities.Validators
{
    public class SysIssueCharacterTypeCreateValidator : AbstractValidator<SysIssueCharacterTypeCreateDTO>
    {
        public SysIssueCharacterTypeCreateValidator()
        {
            RuleFor(x => x.IssueCharacterType)
                .NotEmpty().WithMessage("Issue character type is required.")
                .MaximumLength(100).WithMessage("Issue character type cannot exceed 100 characters.");
        }
    }
}
