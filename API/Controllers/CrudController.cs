using API.Entities.Bases;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CrudController<T> : Controller where T : class, IEntity
{
	private readonly ILogger _logger;
	private readonly IService<T> _service;

	public CrudController()
	{
		// Used by DI, but not by inheriting classes.
	}
	
	public CrudController(ILogger logger, IService<T> service)
	{
		// This generic logger will be provided by inheriting classes, which 
		// then get their loggers from DI.
		_logger = logger;
		_service = service;
	}

	[HttpGet("all")]
	public virtual async Task<ActionResult<IEnumerable<T>?>> GetAll()
	{
		_logger.LogInformation("Fetching all entities of type {Type}", typeof(T).Name);
		var entities = await _service.GetAllAsync();
		if(entities == null)
		{
			_logger.LogWarning("No entities of type {Type} found", typeof(T).Name);
			return NotFound();
		}
		
		_logger.LogInformation("Successfully fetched all entities of type {Type}", typeof(T).Name);
		return Ok(entities);
	}
}