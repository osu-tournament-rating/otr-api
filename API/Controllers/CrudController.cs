using API.Entities.Bases;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CrudController<T> : Controller where T : class, IEntity
{
	public CrudController(ILogger logger, IService<T> service)
	{
		// This generic logger will be provided by inheriting classes, which 
		// then get their loggers from DI.
		Logger = logger;
		Service = service;
	}

	public ILogger Logger { get; }
	public IService<T> Service { get; }

	[HttpGet("{id:int}")]
	public virtual async Task<ActionResult<T?>> Get(int id)
	{
		Logger.LogInformation("Fetching entity of type {Type} with id {Id}", typeof(T).Name, id);
		var entity = await Service.GetAsync(id);
		if (entity == null)
		{
			Logger.LogWarning("No entity of type {Type} with id {Id} found", typeof(T).Name, id);
			return NotFound();
		}

		Logger.LogInformation("Successfully fetched entity of type {Type} with id {Id}", typeof(T).Name, id);
		return Ok(entity);
	}
	
	// public virtual async Task<ActionResult<IEnumerable<T>?>> GetAll()
	// {
	// 	Logger.LogInformation("Fetching all entities of type {Type}", typeof(T).Name);
	// 	var entities = await Service.GetAllAsync();
	// 	if (entities == null)
	// 	{
	// 		Logger.LogWarning("No entities of type {Type} found", typeof(T).Name);
	// 		return NotFound();
	// 	}
	//
	// 	Logger.LogInformation("Successfully fetched all entities of type {Type}", typeof(T).Name);
	// 	return Ok(entities);
	// }
}