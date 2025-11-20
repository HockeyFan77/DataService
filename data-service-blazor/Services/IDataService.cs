public interface IDataService
{
  // Generic calls
  Task<T?> GetAsync<T>(string url);
  Task<T?> PostAsync<T>(string url, object payload);

  // Page-specific calls
  Task<DatabaseObjectsListResponseDto?> GetDatabaseObjectsAsync(
    string context,
    DatabaseObjectsListRequestDto request,
    CancellationToken cancellationToken = default);

  Task<DatabaseObjectResponseDto?> GetDatabaseObjectAsync(
    string context,
    string databaseName,
    int objectId,
    CancellationToken cancellationToken = default);
}