namespace Application.Queries.User;

public class UserQuery : BaseQuery
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Document { get; set; }
    public string RegisterCode { get; set; }
    public DateTime ExpirationRegisterCode { get; set; }
    public string RecoveryCode { get; set; }
    public DateTime ExpirationRecoveryCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
}