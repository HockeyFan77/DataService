public interface IDataService
{
  // Generic calls
  Task<T?> GetAsync<T>(string url);
  Task<T?> PostAsync<T>(string url, object payload);

  // Page-specific calls
  Task<DatabaseObjectsListResponseDto?> GetDatabaseObjectsAsync(
    string? databaseSearch = null,
    string? nameSearch = null,
    string? typeSearch = null,
    string? columnSearch = null,
    string? textSearch = null,
    bool? exactSearch = null,
    int? pageSize = null,
    int? pageNum = null,
    CancellationToken cancellationToken = default);
}