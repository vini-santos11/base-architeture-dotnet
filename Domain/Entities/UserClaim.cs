using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class UserClaim : IdentityUserClaim<Guid>
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
}