using FluentValidation;

namespace Application.Commands.User;

public class CheckRecoveryCodeCommand : BaseCommand
{
    public string Email { get; set; }
    public string RecoveryCode { get; set; }
}

public class CheckRecoveryCodeCommandValidator : AbstractValidator<CheckRecoveryCodeCommand>
{
    public CheckRecoveryCodeCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(x => x.RecoveryCode)
            .NotEmpty().WithMessage("RecoveryCode is required")
            .Length(6).WithMessage("RecoveryCode must be 6 characters");
    }
}