using System.Text;

namespace API;

public class RequestLoggingMiddleware
{
	private readonly ILogger<RequestLoggingMiddleware> _logger;
	private readonly RequestDelegate _next;

	public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task Invoke(HttpContext context)
	{
		await LogRequest(context.Request);

		var originalBodyStream = context.Response.Body;
		
		using (var responseBody = new MemoryStream())
		{
			context.Response.Body = responseBody;
			
			await _next(context);
			await LogResponse(context.Response);

			await responseBody.CopyToAsync(originalBodyStream);
		}
	}

	private async Task LogRequest(HttpRequest request)
	{
		var body = request.Body;

		//This line allows us to set the reader for the request back at the beginning of its stream.
		request.EnableBuffering();

		//We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
		var buffer = new byte[Convert.ToInt32(request.ContentLength)];

		//...Then we copy the entire request stream into the new buffer.
		await request.Body.ReadAsync(buffer, 0, buffer.Length);

		//We convert the byte[] into a string using UTF8 encoding...
		var bodyAsText = Encoding.UTF8.GetString(buffer);

		//..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
		request.Body = body;

		_logger.LogInformation("User with claims {@Claims} on scheme {Scheme} requests {Method} {Host}{Path}{QueryString} with body '{Body}'",
			request.HttpContext.User.Claims, request.Scheme, request.Method, request.Host, request.Path, request.QueryString, bodyAsText);
	}
	
	private async Task LogResponse(HttpResponse response)
	{
		//We need to read the response stream from the beginning...
		response.Body.Seek(0, SeekOrigin.Begin);

		//...and copy it into a string
		string text = await new StreamReader(response.Body).ReadToEndAsync();

		//We need to reset the reader for the response so that the client can read it.
		response.Body.Seek(0, SeekOrigin.Begin);

		//Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
		_logger.LogInformation("Returned status {StatusCode} to {Scheme} {Method} {Host}{Path}{QueryString} with body '{Body}'", response.StatusCode, 
			response.HttpContext.Request.Scheme, response.HttpContext.Request.Method, response.HttpContext.Request.Host, response.HttpContext.Request.Path, response.HttpContext.Request.QueryString, text);
	}
}