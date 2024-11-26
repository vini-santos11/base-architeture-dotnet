using FluentValidation;

namespace Application.Commands.User;

public class RecoveryPasswordCommand
{
    public string Email { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}

public class RecoveryPasswordCommandValidator : AbstractValidator<RecoveryPasswordCommand>
{
    public RecoveryPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.ConfirmPassword).Equal(x => x.NewPassword).WithMessage("Passwords do not match");
    }
}