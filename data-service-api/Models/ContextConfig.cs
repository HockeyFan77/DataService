public class ContextConfig
{
  public string Name { get; set; } = default!;
  public string ConnectionStringKey { get; set; } = default!;
  public List<DatabaseInfo> Databases { get; set; } = [];
}