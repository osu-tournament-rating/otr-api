namespace API.DTOs;

public class UserDTO
{
    public int PlayerId { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime Created { get; set; }
    public string[] Scopes { get; set; } = [];
}
