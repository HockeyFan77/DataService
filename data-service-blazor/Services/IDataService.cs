public interface IDataService
{
  // Generic calls
  Task<T?> GetAsync<T>(string url);
  Task<T?> PostAsync<T>(string url, object payload);

  // Page-specific calls
  Task<PagedResult<DatabaseObjectDto>> GetDatabaseObjectsAsync(string? name, string? type, int page, int pageSize);
}