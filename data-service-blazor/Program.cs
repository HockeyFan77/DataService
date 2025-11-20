using MudBlazor.Services;
using DataServiceBlazor.Components;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
  .AddInteractiveServerComponents();

builder.Services.Configure<ApiSettings>(
  builder.Configuration.GetSection("Api")
);

builder.Services.AddHttpClient<IDataService, DataService>((sp, http) =>
{
  var api = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
  http.BaseAddress = new Uri(api.BaseUrl); // your API URL
});

// simpler for WebAssembly
// builder.Services.AddHttpClient<DataService>("Api", client =>
// {
//   client.BaseAddress = new Uri("http://localhost:5253/api/"); // your API URL
// });

// Register HttpClient for server-side use
// builder.Services.AddScoped<HttpClient>(sp => new HttpClient()
// {
//   BaseAddress = new Uri("http://localhost:5253/api/")
// });
// // Now IDataService can use the registered HttpClient
// builder.Services.AddScoped<IDataService>(sp =>
// {
//   var http = sp.GetRequiredService<HttpClient>();
//   //http.BaseAddress = new Uri("http://localhost:5253/api/");
//   return new DataService(http);
// });

builder.Services.ConfigureHttpJsonOptions(options =>
{
  options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(policy =>
  {
    policy.AllowAnyOrigin()
      .AllowAnyHeader()
      .AllowAnyMethod();
  });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
  .AddInteractiveServerRenderMode();

app.UseCors();

app.Run();
