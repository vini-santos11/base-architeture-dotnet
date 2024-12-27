using Application.Commands.Claim;
using FluentValidation;

namespace Application.Commands.Role;

public class CreateRoleCommand : BaseCommand
{
    public string Name { get; set; }
    public List<ClaimCommand> Claims { get; set; }
}

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters");

        RuleForEach(x => x.Claims).SetValidator(new ClaimCommandValidator());
    }
}