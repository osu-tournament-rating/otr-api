using API;
using API.BackgroundWorkers;
using API.Configurations;
using API.ModelBinders.Providers;
using API.Osu.Multiplayer;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using API.Services.Implementations;
using API.Services.Interfaces;
using API.Utilities;
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
using API.Handlers.Implementations;
using API.Handlers.Interfaces;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

var builder = WebApplication.CreateBuilder(args);

// Configurations
builder.Services.AddOptionsWithValidateOnStart<ConnectionStringsConfiguration>()
	.Bind(builder.Configuration.GetSection(ConnectionStringsConfiguration.Position))
	.ValidateDataAnnotations();
builder.Services.AddOptionsWithValidateOnStart<OsuConfiguration>()
    .Bind(builder.Configuration.GetSection(OsuConfiguration.Position))
    .ValidateDataAnnotations();
builder.Services.AddOptionsWithValidateOnStart<JwtConfiguration>()
    .Bind(builder.Configuration.GetSection(JwtConfiguration.Position))
	.ValidateDataAnnotations();

// Add services to the container.

builder.Services.AddControllers(options => { options.ModelBinderProviders.Insert(0, new LeaderboardFilterModelBinderProvider()); })
       .AddJsonOptions(o =>
       {
	       o.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
	       o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
       })
       .AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSerilog(configuration =>
{
    string connString = builder.Configuration.BindAndValidate<ConnectionStringsConfiguration>(ConnectionStringsConfiguration.Position).DefaultConnection;

#if DEBUG
	configuration.MinimumLevel.Debug();
#else
	configuration.MinimumLevel.Information();
#endif

	configuration
		.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
		.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
		.MinimumLevel.Override("OsuSharp", LogEventLevel.Fatal)
		.Enrich.FromLogContext()
		.WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
		.WriteTo.File(Path.Join("logs", "log.log"), rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information)
		.WriteTo.PostgreSQL(connString, "Logs", needAutoCreateTable: true, restrictedToMinimumLevel: LogEventLevel.Warning);
});

DefaultTypeMap.MatchNamesWithUnderscores = true;

var configuration = new MapperConfiguration(cfg => { cfg.AddProfile<MapperProfile>(); });

// only during development, validate your mappings; remove it before release
#if DEBUG
configuration.AssertConfigurationIsValid();
#endif

builder.Services.AddSingleton(configuration.CreateMapper());

builder.Services.AddLogging();

// Hosted services
builder.Services.AddHostedService<MatchDuplicateDataWorker>();
builder.Services.AddHostedService<OsuPlayerDataWorker>();
builder.Services.AddHostedService<OsuMatchDataWorker>();
builder.Services.AddHostedService<OsuTrackApiWorker>();

// Handlers
builder.Services.AddScoped<IOAuthHandler, OAuthHandler>();

// Database context
builder.Services.AddDbContext<OtrContext>(o =>
{
	o.UseNpgsql(builder.Configuration.BindAndValidate<ConnectionStringsConfiguration>(ConnectionStringsConfiguration.Position).DefaultConnection);
});

builder.Services.AddDistributedMemoryCache();

// Repositories
builder.Services.AddScoped<IApiMatchRepository, ApiMatchRepository>();
builder.Services.AddScoped<IBaseStatsRepository, BaseStatsRepository>();
builder.Services.AddScoped<IBeatmapRepository, BeatmapRepository>();
builder.Services.AddScoped<IGamesRepository, GamesRepository>();
builder.Services.AddScoped<IGameWinRecordsRepository, GameWinRecordsRepository>();
builder.Services.AddScoped<IMatchesRepository, MatchesRepository>();
builder.Services.AddScoped<IMatchDuplicateRepository, MatchDuplicateRepository>();
builder.Services.AddScoped<IMatchRatingStatsRepository, MatchRatingStatsRepository>();
builder.Services.AddScoped<IMatchScoresRepository, MatchScoresRepository>();
builder.Services.AddScoped<IMatchWinRecordRepository, MatchWinRecordRepository>();
builder.Services.AddScoped<IOAuthClientRepository, OAuthClientRepository>();
builder.Services.AddScoped<IPlayerMatchStatsRepository, PlayerMatchStatsRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IRatingAdjustmentsRepository, RatingAdjustmentsRepository>();
builder.Services.AddScoped<ITournamentsRepository, TournamentsRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IBaseStatsService, BaseStatsService>();
builder.Services.AddScoped<IBeatmapService, BeatmapService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<IMatchesService, MatchesService>();
builder.Services.AddScoped<IOAuthClientService, OAuthClientService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IPlayerStatsService, PlayerStatsService>();
builder.Services.AddScoped<ITournamentsService, TournamentsService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddOsuSharp(options =>
{
	var osuConfiguration = builder.Configuration.BindAndValidate<OsuConfiguration>(OsuConfiguration.Position);
    options.Configuration = new OsuClientConfiguration
	{
		ClientId = osuConfiguration.ClientId,
		ClientSecret = osuConfiguration.ClientSecret
    };
});

builder.Services.AddSingleton<IOsuApiService, OsuApiService>();

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(corsPolicyBuilder =>
	{
		corsPolicyBuilder
			.WithOrigins("https://staging.otr.stagec.xyz", "https://otr.stagec.xyz")
			.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowCredentials();
	});
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Host.ConfigureOsuSharp((ctx, options) =>
{
    var osuConfiguration = builder.Configuration.BindAndValidate<OsuConfiguration>(OsuConfiguration.Position);
    options.Configuration = new OsuClientConfiguration
	{
		ClientId = osuConfiguration.ClientId,
		ClientSecret = osuConfiguration.ClientSecret
    };
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
	       options.TokenValidationParameters = new TokenValidationParameters
	       {
		       ValidateIssuer = false,
		       ValidateAudience = true,
		       ValidateLifetime = true,
		       ValidateIssuerSigningKey = true,
		       ClockSkew = TimeSpan.Zero,
		       ValidAudience = builder.Configuration["Jwt:Audience"],
		       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ??
		                                                                          throw new Exception("Missing Jwt:Key in configuration!"))),
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

app.UseRouting(); // UseRouting must come first before UseCors

app.UseCors(); // Placed after UseRouting and before UseAuthentication and UseAuthorization

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
	app.MapControllers().AllowAnonymous();
}
else
{
	app.MapControllers();
}

app.Logger.LogInformation("Running!");

// Migrations
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<OtrContext>();

int count = context.Database.GetPendingMigrations().Count();
if (count > 0)
{
	await context.Database.MigrateAsync();
	app.Logger.LogInformation($"Applied {count} pending migrations.");
}

app.Run();