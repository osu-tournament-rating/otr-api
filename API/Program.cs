using API;
using API.Configurations;
using API.DTOs;
using API.Entities;
using API.Osu;
using API.Osu.Multiplayer;
using API.Services.Implementations;
using API.Services.Interfaces;
using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OsuSharp;
using OsuSharp.Extensions;
using Serilog;
using Serilog.Events;
using System.Text;
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
	             .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
	             .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
	             .Enrich.FromLogContext()
	             .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
	             .WriteTo.File(Path.Join("logs", "log.log"), rollingInterval: RollingInterval.Day)
	             .WriteTo.PostgreSQL(connString, "Logs", needAutoCreateTable: true);
});

DefaultTypeMap.MatchNamesWithUnderscores = true;

var configuration = new MapperConfiguration(cfg =>
{
	cfg.CreateMap<Beatmap, BeatmapDTO>();
	cfg.CreateMap<API.Entities.Game, GameDTO>();
	cfg.CreateMap<API.Entities.Match, MatchDTO>();
	cfg.CreateMap<MatchScore, MatchScoreDTO>();
	cfg.CreateMap<Player, PlayerDTO>().ForMember(x => x.Statistics, opt => opt.Ignore());
	cfg.CreateMap<Player, PlayerRanksDTO>();
	cfg.CreateMap<Rating, RatingDTO>();
	cfg.CreateMap<RatingHistory, RatingHistoryDTO>()
	   .ForMember(x => x.MatchName, opt => opt.MapFrom(y => y.Match.Name))
	   .ForMember(x => x.OsuMatchId, opt => opt.MapFrom(y => y.Match.MatchId))
	   .ForMember(x => x.TournamentName, opt => opt.MapFrom(y => y.Match.TournamentName))
	   .ForMember(x => x.Abbreviation, opt => opt.MapFrom(y => y.Match.Abbreviation));
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
builder.Services.AddHostedService<OsuBeatmapSrDataWorker>();
builder.Services.AddHostedService<OsuTrackApiWorker>();
#endif


builder.Services.AddDbContext<OtrContext>(o =>
{
	o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ??
	            throw new InvalidOperationException("Missing connection string!"));
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddScoped<IGameSrCalculator, GameSrCalculator>();

builder.Services.AddScoped<IRatingsService, RatingsService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IMatchesService, MatchesService>();
builder.Services.AddScoped<IRatingHistoryService, RatingHistoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGamesService, GamesService>();
builder.Services.AddScoped<IMatchScoresService, MatchScoresService>();
builder.Services.AddScoped<IBeatmapService, BeatmapService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();

builder.Services.AddOsuSharp(options =>
{
	options.Configuration = new OsuClientConfiguration
	{
		ClientId = int.Parse(builder.Configuration["Osu:ClientId"]!),
		ClientSecret = builder.Configuration["Osu:ClientSecret"]!
	};
});

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

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigin", corsPolicyBuilder =>
	{
		corsPolicyBuilder
			.WithOrigins("http://localhost:3000", "https://staging.otr.stagec.xyz", "https://otr.stagec.xyz")
			.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowCredentials();
	});
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
	       options.TokenValidationParameters = new TokenValidationParameters
	       {
		       ValidateIssuer = true,
		       ValidateAudience = true,
		       ValidateLifetime = true,
		       ValidateIssuerSigningKey = true,
		       ValidIssuer = builder.Configuration["Jwt:Issuer"],
		       ValidAudience = builder.Configuration["Jwt:Issuer"],
		       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ??
		                                                                          throw new Exception("Missing Jwt:Key in configuration!")))
	       };

	       options.Events = new JwtBearerEvents();
	       options.Events.OnMessageReceived = context =>
	       {
		       if (context.Request.Cookies.ContainsKey("OTR-Access-Token"))
		       {
			       context.Token = context.Request.Cookies["OTR-Access-Token"];
		       }
		       else if (context.Request.Headers.ContainsKey("Authorization"))
		       {
			       context.Token = context.Request.Headers.Authorization;
		       }

		       return Task.CompletedTask;
	       };
       });

var app = builder.Build();

// Set switch for Npgsql
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting(); // UseRouting should come first before UseCors

app.UseCors("AllowSpecificOrigin"); // Placed after UseRouting and before UseAuthentication and UseAuthorization

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Logger.LogInformation("Running!");

app.Run();