public class DataService : IDataService
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

  public async Task<DatabaseObjectsListResponseDto?> GetDatabaseObjectsAsync(
    string? databaseSearch = null,
    string? nameSearch = null,
    string? typeSearch = null,
    string? columnSearch = null,
    string? textSearch = null,
    bool? exactSearch = null,
    int? pageSize = null,
    int? pageNum = null,
    CancellationToken cancellationToken = default)
  {

    // Build query parameters (omit null/empty)
    var query = new Dictionary<string, string?>()
    {
      ["ctx"] = "int",                  // per your example
      ["searchdbs"] = databaseSearch,
      ["objnamesearch"] = nameSearch,
      ["objtypes"] = typeSearch,
      ["objcolumnsearch"] = columnSearch,
      ["objtextsearch"] = textSearch,
      ["objexactsearch"] = exactSearch.HasValue ? (exactSearch.Value ? "1" : "0") : null,
      ["pagesize"] = pageSize?.ToString(),
      ["pagenum"] = pageNum?.ToString()
    };

    // Build query string
    var pairs = query
      .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
      .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}");

    var qs = pairs.Any() ? "?" + string.Join("&", pairs) : string.Empty;

    var url = $"dbobjects{qs}"; // relative to BaseAddress registered in Program.cs

    url = "dbobjects?ctx=int&searchdbs=prs&objnamesearch=loc&objtypes=p;t";
    //return GetAsync<DatabaseObjectsListResponseDto>(url);
    return await _http.GetFromJsonAsync<DatabaseObjectsListResponseDto?>(url, cancellationToken: cancellationToken);
  }

}