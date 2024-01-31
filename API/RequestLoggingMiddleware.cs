using System.IdentityModel.Tokens.Jwt;
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

		try
		{
			using (var responseBody = new MemoryStream())
			{
				context.Response.Body = responseBody;

				await _next(context);

				await LogResponse(context.Response);

				// Reset the position to the beginning of the stream before copying.
				responseBody.Position = 0;

				await responseBody.CopyToAsync(originalBodyStream);
			}
		}
		finally
		{
			// Ensure the Response.Body is set back to the original stream in case of an error.
			context.Response.Body = originalBodyStream;
		}
	}

	private async Task LogRequest(HttpRequest request)
	{
		request.EnableBuffering();

		var body = request.Body;
		try
		{
			// Create a new memory stream.
			var memStream = new MemoryStream();
			await body.CopyToAsync(memStream);

			// Reset the position of the memory stream to be read from the beginning.
			memStream.Position = 0;

			// Read from the memory stream and convert to string for logging.
			byte[] buffer = new byte[memStream.Length];
			// ReSharper disable once MustUseReturnValue
			await memStream.ReadAsync(buffer, 0, buffer.Length);
			string bodyAsText = Encoding.UTF8.GetString(buffer);

			if (bodyAsText.Length > 500)
			{
				bodyAsText = bodyAsText[..500] + "...";
			}

			string? ident = request.HttpContext.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value;

			_logger.LogInformation("User with identity {Identity} on scheme {Scheme} requests {Method} {Host}{Path}{QueryString} with body '{Body}'",
				ident, request.Scheme, request.Method, request.Host, request.Path,
				request.QueryString, bodyAsText);

			// Reset the memory stream position again before assigning it back to the request body to be read by subsequent middlewares.
			memStream.Position = 0;
			request.Body = memStream;
		}
		finally
		{
			// If there is any exception, make sure the original inaccessible body is still assigned back to the request.
			if (request.Body != body)
			{
				body.Dispose();
			}
		}
	}

	private async Task LogResponse(HttpResponse response)
	{
		//We need to read the response stream from the beginning...
		response.Body.Seek(0, SeekOrigin.Begin);

		//...and copy it into a string
		char[] buff = new char[1000];
		await new StreamReader(response.Body).ReadAsync(buff, 0, (int)Math.Min(1000, response.Body.Length));

		//We need to reset the reader for the response so that the client can read it.
		response.Body.Seek(0, SeekOrigin.Begin);

		string text = new(buff);
		if (text.Length == 1000)
		{
			text += "...";
		}

		string? ident = response.HttpContext.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value;

		if (response.StatusCode >= 400)
		{
			_logger.LogError("User with identity {Identity} on scheme {Scheme} {Method} {Host}{Path}{QueryString} with body '{Body}' returned status {StatusCode}",
				ident, response.HttpContext.Request.Scheme, response.HttpContext.Request.Method, response.HttpContext.Request.Host, response.HttpContext.Request.Path,
				response.HttpContext.Request.QueryString, text, response.StatusCode);

			return;
		}

		//Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
		_logger.LogInformation("Returned status {StatusCode} to {Scheme} {Method} {Host}{Path}{QueryString} with body '{Body}'", response.StatusCode,
			response.HttpContext.Request.Scheme, response.HttpContext.Request.Method, response.HttpContext.Request.Host, response.HttpContext.Request.Path,
			response.HttpContext.Request.QueryString, text);
	}
}