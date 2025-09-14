using System.Text.Json.Serialization;
using MiniMediaMetadataAPI.Application.Configurations;
using MiniMediaMetadataAPI.Application.Repositories;
using MiniMediaMetadataAPI.Application.Services;
using MiniMediaMetadataAPI.Middlewares;
using MiniMediaMetadataAPI.Options;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => options.ModelValidatorProviders.Clear())
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection("DatabaseConfiguration"));
builder.Services.Configure<PrometheusOptions>(builder.Configuration.GetSection("Prometheus"));

builder.Services.AddScoped<JobRepository>();
builder.Services.AddScoped<MusicBrainzRepository>();
builder.Services.AddScoped<SpotifyRepository>();
builder.Services.AddScoped<TidalRepository>();
builder.Services.AddScoped<DeezerRepository>();
builder.Services.AddScoped<DiscogsRepository>();
builder.Services.AddScoped<SearchArtistService>();
builder.Services.AddScoped<SearchAlbumService>();
builder.Services.AddScoped<SearchTrackService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseRequestMiddleware();

//app.UseHttpsRedirection();

app.UseRouting();
app.UseCors();
app.UseEndpoints(endpoints => endpoints.MapControllers());
app.UseMetricServer(url: builder.Configuration.GetSection("Prometheus")["MetricsUrl"]);


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}