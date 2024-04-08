namespace API.DTOs;

/// <summary>
/// Represents a newly created resource
/// </summary>
public class CreatedResultBaseDTO
{
    /// <summary>
    /// Id of the resource
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// URL of where the new resource can be accessed
    /// </summary>
    public string Location { get; set; } = null!;
}
