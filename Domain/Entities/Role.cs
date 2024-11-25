using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class Role : IdentityRole<Guid>
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
}