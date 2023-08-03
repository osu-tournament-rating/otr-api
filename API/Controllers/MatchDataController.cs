using API.Entities;
using API.Services.Interfaces;

namespace API.Controllers;

public class MatchDataController : CrudController<MatchData>
{
	public MatchDataController(ILogger<MatchDataController> logger, IMatchDataService service) : base(logger, service)
	{
		
	}
}