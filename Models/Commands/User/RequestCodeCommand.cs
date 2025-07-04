using FluentValidation;

namespace Application.Commands.User;

public class RequestCodeCommand : BaseCommand
{
    public string Email { get; set; }
}

public class RequestRecoveryPasswordCommandValidator : AbstractValidator<RequestCodeCommand>
{
    public RequestRecoveryPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");
    }
}