using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class UserRole : IdentityUserRole<Guid>
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
}