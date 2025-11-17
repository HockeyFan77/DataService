public class DataService
{
  private readonly HttpClient _http;

  public DataService(HttpClient http) => _http = http;

  // IDataService implementation
  public async Task<T?> GetAsync<T>(string url)
  {
    return await _http.GetFromJsonAsync<T>(url);
  }
  public async Task<T?> PostAsync<T>(string url, object payload)
  {
    var response = await _http.PostAsJsonAsync(url, payload);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadFromJsonAsync<T>();
  }

  public Task<PagedResult<DatabaseObjectDto>> GetDatabaseObjectsAsync(
    string? name,
    string? type,
    int page,
    int pageSize)
  {
    string url = $"dbobjects?ctx={{dev}}&searchdbs=a&objnamesearch=person&objtypes=p;t";
    return GetAsync<PagedResult<DatabaseObjectDto>>(url);
  }
}