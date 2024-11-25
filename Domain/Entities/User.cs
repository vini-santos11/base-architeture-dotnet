using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string Name { get; set; }
    public string Document { get; set; }
    public string RegisterCode { get; set; }
    public DateTime ExpirationRegisterCode { get; set; }
    public string RecoveryCode { get; set; }
    public DateTime ExpirationRecoveryCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
}