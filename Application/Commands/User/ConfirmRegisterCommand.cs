using FluentValidation;

namespace Application.Commands.User;

public class ConfirmRegisterCommand : BaseCommand
{
    public string Email { get; set; }
    public string Code { get; set; }
}

public class ConfirmRegisterCommandValidator : AbstractValidator<ConfirmRegisterCommand>
{
    public ConfirmRegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .Length(6).WithMessage("Code must be 6 characters");
    }
}