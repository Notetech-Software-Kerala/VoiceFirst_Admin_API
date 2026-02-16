using FluentValidation;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;

namespace VoiceFirst_Admin.Utilities.Validators
{
    public class SysBusinessActivityCreateValidator : AbstractValidator<SysBusinessActivityCreateDTO>
    {
        public SysBusinessActivityCreateValidator()
        {
            RuleFor(x => x.ActivityName)
                .NotEmpty().WithMessage("Activity name is required.")
                .MaximumLength(150).WithMessage("Activity name cannot exceed 150 characters.");
        }
    }
}
