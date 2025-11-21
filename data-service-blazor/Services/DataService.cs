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
    string context,
    DatabaseObjectsListRequestDto request,
    CancellationToken cancellationToken = default)
  {
    // Build query parameters (omit null/empty)
    var query = new Dictionary<string, string?>()
    {
      ["ctx"] = context,
      ["dbaliases"] = request.DatabaseSearch,
      ["objsch"] = request.SchemaSearch,
      ["objname"] = request.NameSearch,
      ["objtypes"] = request.TypeSearch,
      ["colname"] = request.ColumnSearch,
      ["objtext"] = request.TextSearch,
      ["exact"] = (request.UseExactSearch ?? false).ToString(),
      ["page"] = request.Page?.ToString(),
      ["pagesize"] = request.PageSize?.ToString()
    };

    // Build query string
    var pairs = query
      .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
      .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}");

    var qs = pairs.Any() ? "?" + string.Join("&", pairs) : string.Empty;

    var url = $"dbobjects{qs}"; // relative to BaseAddress registered in Program.cs

    return await _http.GetFromJsonAsync<DatabaseObjectsListResponseDto?>(url, cancellationToken: cancellationToken);
  }

  public async Task<DatabaseObjectResponseDto?> GetDatabaseObjectAsync(
    string context,
    string databaseName,
    int objectId,
    CancellationToken cancellationToken = default)
  {
    // Build query parameters (omit null/empty)
    var query = new Dictionary<string, string?>()
    {
      ["ctx"] = context,
      ["dbname"] = databaseName,
      ["objid"] = objectId.ToString()
    };

    // Build query string
    var pairs = query
      .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
      .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}");

    var qs = pairs.Any() ? "?" + string.Join("&", pairs) : string.Empty;

    var url = $"dbobject{qs}";

    return await _http.GetFromJsonAsync<DatabaseObjectResponseDto?>(url, cancellationToken: cancellationToken);
  }

}