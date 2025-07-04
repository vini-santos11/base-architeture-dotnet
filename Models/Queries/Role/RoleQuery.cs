namespace Models.Queries.Role;

public class RoleQuery : BaseQuery
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}