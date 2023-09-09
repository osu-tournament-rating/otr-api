using API.Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : Controller
{
	public GamesController(ILogger<GamesController> logger) { }
}