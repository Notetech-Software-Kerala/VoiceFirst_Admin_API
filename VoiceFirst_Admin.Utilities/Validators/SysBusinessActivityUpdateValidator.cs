using FluentValidation;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;

namespace VoiceFirst_Admin.Utilities.Validators
{
    public class SysBusinessActivityUpdateValidator : AbstractValidator<SysBusinessActivityUpdateDTO>
    {
        public SysBusinessActivityUpdateValidator()
        {
            RuleFor(x => x.ActivityName)
                .MaximumLength(150).WithMessage("Activity name cannot exceed 150 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.ActivityName));
        }
    }
}
