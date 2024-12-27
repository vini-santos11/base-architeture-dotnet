using Enumerations.Enums;
using Enumerations.Helpers;
using FluentValidation;

namespace Application.Commands.Claim;

public class ClaimCommand : BaseCommand
{
    public string Model { get; set; }
    public string Value { get; set; }
}

public class ClaimCommandValidator : AbstractValidator<ClaimCommand>
{
    public ClaimCommandValidator()
    {
        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required")
            .MaximumLength(50).WithMessage("Model must not exceed 50 characters");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required")
            .MaximumLength(50).WithMessage("Value must not exceed 50 characters");
        
        RuleFor(x => x).Must(BeValidClaim).WithMessage("Invalid claim - Does not exist");
    }

    private bool BeValidClaim(ClaimCommand result)
    {
        var claims = Enum.GetValues(typeof(ClaimsEnum)).Cast<ClaimsEnum>().ToList();
        foreach(var claim in claims)
        {
            var description = Utilities.GetEnumDescription(claim);
            var model = description.Split("-")[0];
            var value = description.Split("-")[1].ToLower();
            if (result.Model == model && result.Value == value)
                return true;
        };
        return false;
    }
}