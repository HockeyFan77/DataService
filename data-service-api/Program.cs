const bool useOpenApi = false;
const bool logMatchedEndpoints = true;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
if ( useOpenApi )
{
  builder.Services.AddOpenApi();
}
builder.Services.AddControllers();
// uncomment to enable CORS (if needed)
// builder.Services.AddCors(options =>
// {
//   options.AddDefaultPolicy(policy =>
//   {
//     policy.WithOrigins("http://localhost:5173")
//       .AllowAnyHeader()
//       .AllowAnyMethod();
//   });
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (useOpenApi && app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

// use this to log matched endpoints
if ( logMatchedEndpoints )
{
  app.Use(async (context, next) =>
  {
    Console.WriteLine($"[Routing] Matched endpoint: {context.GetEndpoint()?.DisplayName ?? "None"}");
    await next();
  });
}

// uncomment to redirect to https://
//app.UseHttpsRedirection();
// uncomment to enable CORS (if needed)
//app.UseCors();
app.MapControllers();

var summaries = new[]
{
  "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/api/weatherforecast", async () =>
{
  await Task.Delay(3000);
  var forecast =  Enumerable.Range(1, 5).Select(index =>
    new WeatherForecast
    (
      DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
      Random.Shared.Next(-20, 55),
      summaries[Random.Shared.Next(summaries.Length)]
    ))
    .ToArray();
  return forecast;
})
.WithName("GetWeatherForecast");

app.MapFallbackToController("GetFallback", "DataService");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}