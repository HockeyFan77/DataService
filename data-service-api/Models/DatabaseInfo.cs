using System.Text.Json.Serialization;
public class DatabaseInfo
{
  [JsonPropertyName("id")]
  public int ID { get; set; } = default!;
  [JsonPropertyName("alias")]
  public string Alias { get; set; } = default!;
  [JsonPropertyName("name")]
  public string Name { get; set; } = default!;
  [JsonPropertyName("crdate")]
  public string CreateDate { get; set; } = default!;
  [JsonPropertyName("compatlevel")]
  public int CompatLevel { get; set; } = default!;
}