using API.Configurations;
using API.Osu.Multiplayer;
using API.Services.Implementations;
using API.Services.Interfaces;
using Dapper;
using Serilog;
using Serilog.Events;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
       .AddJsonOptions(o => { o.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals; });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSerilog(configuration =>
{
	string connString = builder.Configuration.GetConnectionString("DefaultConnection") ??
	                    throw new InvalidOperationException("Missing connection string!");

	configuration.MinimumLevel.Debug()
	             .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
	             .Enrich.FromLogContext()
	             .WriteTo.Console()
	             .WriteTo.File("logs\\log.log", rollingInterval: RollingInterval.Day)
	             .WriteTo.PostgreSQL(connString, "Logs", needAutoCreateTable: true);
});

DefaultTypeMap.MatchNamesWithUnderscores = true;
SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);

builder.Services.AddLogging();

builder.Services.AddHostedService<MultiplayerLobbyDataWorker>();

builder.Services.AddScoped<IMatchDataService, MatchDataService>();
builder.Services.AddScoped<IRatingsService, RatingsService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IMultiplayerLinkService, MultiplayerLinkService>();
builder.Services.AddScoped<IRatingHistoryService, RatingHistoryService>();

builder.Services.AddSingleton<IOsuApiService, OsuApiService>();
builder.Services.AddSingleton<ICredentials, Credentials>(serviceProvider =>
{
	string? connString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");
	string? osuApiKey = serviceProvider.GetRequiredService<IConfiguration>().GetSection("Osu").GetValue<string>("ApiKey");
	
	if (connString == null)
	{
		throw new InvalidOperationException("Missing connection string!");
	}
	
	if (osuApiKey == null)
	{
		throw new InvalidOperationException("Missing osu! API key!");
	}

	return new Credentials(connString, osuApiKey);
});

builder.Services.AddSingleton<OsuApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();