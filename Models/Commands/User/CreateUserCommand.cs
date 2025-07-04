using Enumerations.Helpers;
using FluentValidation;

namespace Models.Commands.User;

public class CreateUserCommand : BaseCommand
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Document { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must be less than 100 characters");
        
        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("Document is required")
            .Must(Utilities.ValidateDocument).WithMessage("Invalid document")
            .MaximumLength(14).WithMessage("Document must be less than 14 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match");
    }
} 