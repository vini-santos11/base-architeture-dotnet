using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class RoleClaim : IdentityRoleClaim<Guid>
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
}