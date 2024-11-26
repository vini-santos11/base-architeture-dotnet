using Enumerations.Helpers;
using FluentValidation;

namespace Application.Commands.User;

public class LoginCommand : BaseCommand
{
    public string Document { get; set; }
    public string Password { get; set; }
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("Document is required")
            .Must(Utilities.ValidateDocument).WithMessage("Invalid document")
            .MaximumLength(14).WithMessage("Document must be less than 14 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");
    }
}