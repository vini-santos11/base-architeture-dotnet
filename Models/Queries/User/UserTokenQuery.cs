namespace Models.Queries.User;

public class UserTokenQuery : BaseQuery
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Document { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public string Role { get; set; }
}