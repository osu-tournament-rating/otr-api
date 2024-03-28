using API.Entities;
using AutoMapper;

namespace API.DTOs;

[AutoMap(typeof(User))]
public class UserDTO
{
    public int PlayerId { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime Created { get; set; }
    public string[] Scopes { get; set; } = [];
}
