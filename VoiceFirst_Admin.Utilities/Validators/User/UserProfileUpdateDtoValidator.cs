using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.User;

namespace VoiceFirst_Admin.Utilities.Validators.User
{
    public class UserProfileUpdateDtoValidator : AbstractValidator<UserProfileUpdateDto>
    {
        public UserProfileUpdateDtoValidator()
        {
            // -------------------------
            // First Name
            // -------------------------
            When(x => x.FirstName != null, () =>
            {
                RuleFor(x => x.FirstName)
                    .MaximumLength(50)
                    .Matches(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$")
                    .WithMessage("First name must contain only letters");
            });

            // -------------------------
            // Last Name
            // -------------------------
            When(x => x.LastName != null, () =>
            {
                RuleFor(x => x.LastName)
                    .MaximumLength(50)
                    .Matches(@"^[a-zA-Z]*$")
                    .WithMessage("Last name must contain only letters");
            });

            // -------------------------
            // Gender (matches DB CHECK constraint)
            // -------------------------
            When(x => x.Gender != null, () =>
            {
                RuleFor(x => x.Gender)
                    .Must(g =>
                        g!.Equals("Male", StringComparison.OrdinalIgnoreCase) ||
                        g.Equals("Female", StringComparison.OrdinalIgnoreCase) ||
                        g.Equals("Other", StringComparison.OrdinalIgnoreCase))
                    .WithMessage("Gender must be Male, Female or Other");
            });

            // -------------------------
            // Mobile Number (basic + advanced)
            // -------------------------
            When(x => x.MobileNo != null, () =>
            {
                RuleFor(x => x.MobileNo)
                    .NotEmpty()
                    .Matches(@"^[0-9+]+$")
                    .WithMessage("Mobile number can contain only digits and '+'")
                    .MinimumLength(7)
                    .MaximumLength(15);
            });

            // -------------------------
            // Dial Code (Dial Code FK)
            // -------------------------
            When(x => x.DialCodeId.HasValue, () =>
            {
                RuleFor(x => x.DialCodeId)
                    .GreaterThan(0)
                    .WithMessage("Invalid DialCode selection");
            });

            // -------------------------
            // Mobile + Dial Code Dependency
            // -------------------------
            RuleFor(x => x)
                .Must(x =>
                {
                    // If mobile is provided → dial code must be provided
                    if (!string.IsNullOrWhiteSpace(x.MobileNo))
                        return x.DialCodeId.HasValue;

                    return true;
                })
                .WithMessage("DialCodeId is required when MobileNo is provided");

            // -------------------------
            // Birth Year
            // -------------------------
            When(x => x.BirthYear.HasValue, () =>
            {
                RuleFor(x => x.BirthYear!.Value)
                    .InclusiveBetween(1900, DateTime.UtcNow.Year)
                    .WithMessage("Birth year must be between 1900 and current year.");
            });
        }
    }
}
