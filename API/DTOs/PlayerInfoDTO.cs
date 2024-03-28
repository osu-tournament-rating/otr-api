using API.Entities;
using AutoMapper;

namespace API.DTOs;

[AutoMap(typeof(Player))]
public class PlayerInfoDTO
{
    public int Id { get; set; }
    public long OsuId { get; set; }
    public string? Username { get; set; }
    public string? Country { get; set; }
}
