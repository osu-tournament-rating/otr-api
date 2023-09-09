using API;
using API.Configurations;
using API.DTOs;
using API.Models;
using API.Osu;
using API.Osu.Multiplayer;
using API.Services.Implementations;
using API.Services.Interfaces;
using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
       .AddJsonOptions(o =>
       {
	       o.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
	       o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
       });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSerilog(configuration =>
{
	string connString = builder.Configuration.GetConnectionString("DefaultConnection") ??
	                    throw new InvalidOperationException("Missing connection string!");

	configuration.MinimumLevel.Debug()
	             .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
	             .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
	             .Enrich.FromLogContext()
	             .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
	             .WriteTo.File("logs\\log.log", rollingInterval: RollingInterval.Day)
	             .WriteTo.PostgreSQL(connString, "Logs", needAutoCreateTable: true);
});

DefaultTypeMap.MatchNamesWithUnderscores = true;

var configuration = new MapperConfiguration(cfg => 
{
	cfg.CreateMap<Beatmap, BeatmapDTO>();
	cfg.CreateMap<API.Models.Game, GameDTO>();
	cfg.CreateMap<API.Models.Match, MatchDTO>();
	cfg.CreateMap<MatchScore, MatchScoreDTO>();
	cfg.CreateMap<Player, PlayerDTO>();
	cfg.CreateMap<Player, PlayerRanksDTO>();
	cfg.CreateMap<Rating, RatingDTO>();
	cfg.CreateMap<RatingHistory, RatingHistoryDTO>();
	cfg.CreateMap<User, UserDTO>();
});

// only during development, validate your mappings; remove it before release
#if DEBUG
configuration.AssertConfigurationIsValid();
#endif

builder.Services.AddSingleton(configuration.CreateMapper());

builder.Services.AddLogging();

#if !DEBUG
builder.Services.AddHostedService<OsuPlayerDataWorker>();
builder.Services.AddHostedService<OsuMatchDataWorker>();
#endif

builder.Services.AddDbContext<OtrContext>(o =>
{
	o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ??
	            throw new InvalidOperationException("Missing connection string!"));
});

builder.Services.AddScoped<IRatingsService, RatingsService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IMatchesService, MatchesService>();
builder.Services.AddScoped<IRatingHistoryService, RatingHistoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGamesService, GamesService>();
builder.Services.AddScoped<IMatchScoresService, MatchScoresService>();
builder.Services.AddScoped<IBeatmapService, BeatmapService>();

builder.Services.AddSingleton<IOsuApiService, OsuApiService>();
builder.Services.AddSingleton<ICredentials, Credentials>(serviceProvider =>
{
	string? connString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");
	string? osuApiKey = serviceProvider.GetRequiredService<IConfiguration>().GetSection("Osu").GetValue<string>("ApiKey");

	if (connString == null)
	{
		throw new InvalidOperationException("Missing connection string!");
	}

	return new Credentials(connString, osuApiKey);
});

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