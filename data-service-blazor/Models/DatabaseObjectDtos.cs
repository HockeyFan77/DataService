using System.Text.Json.Serialization;

public class DatabaseObjectsListInputsDto : DatabaseObjectsListRequestDto
{
  [JsonPropertyName("totalrows")]
  public int? TotalRows { get; set; }
}

public class DatabaseObjectsListItemDto
{
  [JsonPropertyName("db")]
  public string? Database { get; set; } = "";
  [JsonPropertyName("sch")]
  public string? Schema { get; set; } = "";
  [JsonPropertyName("name")]
  public string? Name { get; set; } = "";
  [JsonPropertyName("id")]
  public int? Id { get; set;  }
  [JsonPropertyName("type")]
  public string? Type { get; set; } = "";
  [JsonPropertyName("crdate")]
  public DateTime? Created { get; set; }
  [JsonPropertyName("moddate")]
  public DateTime? Modified { get; set; }
  [JsonPropertyName("bdb")]
  public string? BaseDatabase { get; set; } = "";
  [JsonPropertyName("bsch")]
  public string? BaseSchema { get; set; } = "";
  [JsonPropertyName("bname")]
  public string? BaseName { get; set; } = "";
}

public class DatabaseObjectsListRequestDto
{
  [JsonPropertyName("dbaliases")]
  public string? DatabaseSearch { get; set; } = "";
  [JsonPropertyName("objsch")]
  public string? SchemaSearch { get; set; } = "";
  [JsonPropertyName("objname")]
  public string? NameSearch { get; set; } = "";
  [JsonPropertyName("objtypes")]
  public string? TypeSearch { get; set; } = "";
  [JsonPropertyName("colname")]
  public string? ColumnSearch { get; set; } = "";
  [JsonPropertyName("objtext")]
  public string? TextSearch { get; set; } = "";
  [JsonPropertyName("exact")]
  public bool? UseExactSearch { get; set; }
  [JsonPropertyName("page")]
  public int? Page { get; set; }
  [JsonPropertyName("pagesize")]
  public int? PageSize { get; set; }
}

public class DatabaseObjectsListResponseDto
{
  [JsonPropertyName("inputs")]
  public DatabaseObjectsListInputsDto Inputs { get; set; } = new();
  [JsonPropertyName("dbobjects")]
  public List<DatabaseObjectsListItemDto> Objects { get; set; } = [];
}

public class DatabaseObjectResponseDto
{
  [JsonPropertyName("db")]
  public string? Database { get; set; } = "";
  [JsonPropertyName("sch")]
  public string? Schema { get; set; } = "";
  [JsonPropertyName("name")]
  public string? Name { get; set; } = "";
  [JsonPropertyName("id")]
  public int? Id { get; set; }
  [JsonPropertyName("type")]
  public string? Type { get; set; } = "";
  [JsonPropertyName("crdate")]
  public DateTime? Created { get; set; }
  [JsonPropertyName("moddate")]
  public DateTime? Modified { get; set; }
  [JsonPropertyName("rowcnt")]
  public int? RowCount { get; set; }
  [JsonPropertyName("bdb")]
  public string? BaseDatabase { get; set; } = "";
  [JsonPropertyName("bsch")]
  public string? BaseSchema { get; set; } = "";
  [JsonPropertyName("bname")]
  public string? BaseName { get; set; } = "";
}