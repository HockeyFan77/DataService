using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using DataServiceApi.Utility;

namespace DataServiceApi.Controllers
{

  [ApiController]
  [Route("api")]
  public class DataServiceController(IConfiguration configuration, IWebHostEnvironment environment) : ControllerBase
  {
    private const string MEDIA_TYPE_XML = "application/xml";
    private const string MEDIA_TYPE_JSON = "application/json";
    private readonly IConfiguration _configuration = configuration;
    private readonly IWebHostEnvironment _environment = environment;
    private readonly string _dataSourcePath = Path.Combine(environment.ContentRootPath, "data-sources");

    [HttpGet("dbdatabases")]
    public async Task<IActionResult> GetDbDatabases()
    {
      var contextConfig = GetContextConfig(GetRequestContext());
      if ( contextConfig == null )
        return BadRequest("Missing or invalid context. Use ?ctx=... or 'X-API-Context' header.");

      var databases = await GetDatabasesFromContextConfigAsync(contextConfig).ConfigureAwait(false);
      return Ok(new { databases = databases });
    }
    [HttpGet("dbobjects")]
    public async Task<IActionResult> GetDbObjects([FromQuery(Name = "searchdbs")] string searchdbs = "")
    {
      var contextConfig = GetContextConfig(GetRequestContext());
      if ( contextConfig == null )
        return BadRequest("Missing or invalid context. Use ?ctx=... or 'X-API-Context' header.");

      string? connectionString;
      if ( (connectionString = GetConnectionString(contextConfig!)).IsBlank() )
        return BadRequest();

      var dbAbbrs = searchdbs.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
      if ( dbAbbrs.Length == 0 )
        return BadRequest("Missing or invalid searchdbs.");

      // convert "searchdbs" parameter as a list of database abbreviations to their actual database names
      var databases = await GetDatabasesFromContextConfigAsync(contextConfig).ConfigureAwait(false);
      string dbNames = string.Join(";", databases
        .Where(ctxdb => dbAbbrs.Contains(ctxdb.Abbr, StringComparer.OrdinalIgnoreCase))
        .Select(ctxdb => ctxdb.Name)
      );
      if ( dbNames.IsBlank() )
        return BadRequest("Missing or invalid searchdbs.");

      var dataSourceParameters = CreateSqlParametersDictionary(Request.Query);
      dataSourceParameters["searchdbs"] = dbNames;

      string? content = null, dataSourceFileName = GetContentFileName("dbobjects.jsonds");
      if ( dataSourceFileName != null )
      {
        content = await GetDataSourceContentAsync(connectionString!, dataSourceFileName, dataSourceParameters).ConfigureAwait(false);
      }

      if ( content.IsBlank() )
        return NotFound();

      return Content(content!, MEDIA_TYPE_JSON);
    }
    [HttpGet("dbobject")]
    public async Task<IActionResult> GetDbObject([FromQuery(Name = "objdb")] string objdb = "")
    {
      var contextConfig = GetContextConfig(GetRequestContext());
      if ( contextConfig == null )
        return BadRequest("Missing or invalid context. Use ?ctx=... or 'X-API-Context' header.");

      string? connectionString;
      if ( (connectionString = GetConnectionString(contextConfig!)).IsBlank() )
        return BadRequest();

      // convert "objdb" parameter as a database abbreviations to its actual database name
      var databases = await GetDatabasesFromContextConfigAsync(contextConfig).ConfigureAwait(false);
      string? dbName = databases.FirstOrDefault(ctxdb => ctxdb.Abbr.EqualsAnyOrdinalNoCase(objdb))?.Name;
      if ( dbName.IsBlank() )
        return BadRequest("Missing or invalid objdb.");

      var builder = new SqlConnectionStringBuilder(connectionString)
      {
        InitialCatalog = dbName
      };
      connectionString = builder.ConnectionString;

      var dataSourceParameters = CreateSqlParametersDictionary(Request.Query);
      dataSourceParameters.Remove("objdb"); // leave in place if dbabbr is needed

      string? content = null, dataSourceFileName = GetContentFileName("dbobject.jsonds");
      if ( dataSourceFileName != null )
      {
        content = await GetDataSourceContentAsync(connectionString!, dataSourceFileName, dataSourceParameters).ConfigureAwait(false);
      }

      if ( content.IsBlank() )
        return NotFound();

      return Content(content!, MEDIA_TYPE_JSON);
    }

    //[HttpGet("{*path}")]
    public async Task<IActionResult> GetFallback(/*string? path*/)
    {
      // possible routes to data sources or static files are (in prioritized order)
      // replace api with data-sources folder to find corresponding file
      //
      //  XML: accept header has application/xml, but not application/json
      //    api/{context}/{endpoint}.xmlds
      //    api/{endpoint}[{context}].xml
      //    api/{endpoint}.xml
      //    not found
      //  JSON: accept header */*
      //    api/{context}/{endpoint}.jsonds
      //    api/{endpoint}[{context}].json
      //    api/{endpoint}.json
      //    not found

      // All XML results should be wrapped in a root element named according to the API (e.g., <Products> ... </Products>)
      // All JSON results should be wrapped in { ... }
      // If no XML/JSON is returned (empty string), the API will return NotFound().
      //
      // JSON results should always be:
      //
      //  // empty collection
      //  {
      //    "products": []
      //  }
      //  // non-empty collection
      //  {
      //    "products": [
      //      { "id": 1, "name": "Product One" },
      //      { "id": 2, "name": "Product Two" },
      //      { "id": 3, "name": "Product Three" }
      //    ]
      //  }
      //  // single object (found)
      //  {
      //    "product": { "id": 1, "name": "Product One" }
      //  }
      //  // single object (not found - use only when not an error condition)
      //  {
      //    "product": null
      //  }
      //  // single object (not found - use when not found is a true error condition e.g., /products/573 is not found)
      //  // return HTTP 404 i.e., NotFound()
      //  {
      //    "error": "Product not found"
      //  }

      // string? path = HttpContext.Request.Path;
      // var segments = (path ?? "").Split('/', StringSplitOptions.RemoveEmptyEntries);
      // if ( segments.Length == 0 || !segments[0].EqualsOrdinalNoCase("api") )
      //   return NotFound();

      // string? endpoint = null, context = null;
      // if ( segments.Length == 2 )
      //   endpoint = segments[1];
      // else if ( segments.Length >= 3 )
      // {
      //   context = segments[1];
      //   endpoint = segments[2];
      // }

      // if ( segments.Length == 1 )
      //   endpoint = segments[0];
      // else if ( segments.Length >= 2 )
      // {
      //   context = segments[0];
      //   endpoint = segments[1];
      // }

      string? path = HttpContext.Request.Path;
      var segments = path?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? [];
      if ( segments.Length == 0 || !segments[0].EqualsOrdinalNoCase("api") )
        return NotFound();

      string? endpoint = segments.Length >= 2 ? segments[1] : null;
      string? context = GetRequestContext();
      var contextConfig = GetContextConfig(context);
      bool hasContext = contextConfig != null;
      var dataSourceParameters = CreateSqlParametersDictionary(Request.Query);

      var mediaTypes = Request.Headers.Accept.ToString().ToLowerInvariant()
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(item => item.Split(';', 2)[0].Trim())
        .ToArray();
      bool wantXml = mediaTypes.Contains(MEDIA_TYPE_XML);
      bool wantJson = mediaTypes.Contains(MEDIA_TYPE_JSON);

      string? dataSourceFileName = null, connectionString = null, staticContentFileName = null, content = null;
      if ( wantXml && !wantJson )
      {
        if ( hasContext && (dataSourceFileName = GetContentFileName($"{endpoint}.xmlds")) != null )
        {
          if ( (connectionString = GetConnectionString(contextConfig!)).IsBlank() )
            return BadRequest();
          content = await GetDataSourceContentAsync(connectionString!, dataSourceFileName, dataSourceParameters).ConfigureAwait(false);
        }
        else if ( (staticContentFileName = GetContentFileName($"{endpoint}{(hasContext ? ".[" + context! + "]" : "")}.xml")) != null )
          content = await GetStaticContentAsync(staticContentFileName).ConfigureAwait(false);

        if ( content.IsBlank() )
          return NotFound();

        return Content(content!, MEDIA_TYPE_XML);
      }

      if ( hasContext && (dataSourceFileName = GetContentFileName($"{endpoint}.jsonds")) != null )
      {
        if ( (connectionString = GetConnectionString(contextConfig!)).IsBlank() )
          return BadRequest();
        content = await GetDataSourceContentAsync(connectionString!, dataSourceFileName, dataSourceParameters).ConfigureAwait(false);
      }
      else if ( (staticContentFileName = GetContentFileName($"{endpoint}{(hasContext ? ".[" + context! + "]" : "")}.json")) != null )
        content = await GetStaticContentAsync(staticContentFileName).ConfigureAwait(false);

      if ( content.IsBlank() )
        return NotFound();

      return Content(content!, MEDIA_TYPE_JSON);
    }

    private string? GetRequestContext()
    {
      var ctx = HttpContext.Request.Query["ctx"].FirstOrDefault();
      if ( !ctx.IsBlank() )
        return ctx;

      ctx = HttpContext.Request.Headers["X-API-Context"].FirstOrDefault();
      return ctx.IsBlank() ? null : ctx;
    }
    private ContextConfig? GetContextConfig(string? context)
    {
      if ( context.IsBlank() )
        return null;

      var section = _configuration.GetSection($"contexts:{context!.ToLowerInvariant()}");
      if ( !section.Exists() )
        return null;

      return section.Get<ContextConfig>();
    }
    private string? GetConnectionString(ContextConfig contextConfig)
    {
      string? connectionString = null;
      string? connectionStringKey = contextConfig.ConnectionStringKey;
      if ( !connectionStringKey.IsBlank() )
      {
        connectionString = _configuration.GetConnectionString(connectionStringKey!);
      }
      return connectionString;
    }
    private string? GetContentFileName(string testFileName)
    {
      string? path = Path.Combine(_dataSourcePath, testFileName);
      if ( !System.IO.File.Exists(path) )
        path = null;
      return path;
    }
    private static SqlParametersDictionary CreateSqlParametersDictionary(IQueryCollection query)
    {
      var result = new SqlParametersDictionary();
      foreach ( var kvp in query )
      {
        if ( kvp.Value.Count > 0 && !kvp.Value[0].IsBlank() )
        {
          result[kvp.Key] = (object?)kvp.Value[0];
        }
      }
      return result;
    }

    private async Task<List<DatabaseInfo>> GetDatabasesFromContextConfigAsync(ContextConfig contextConfig)
    {
      var result = new List<DatabaseInfo>();

      string? connectionString;
      if ( (connectionString = GetConnectionString(contextConfig!)).IsBlank() )
        return result;

      var dataSourceParameters = new SqlParametersDictionary()
      {
        ["dbmappings"] = string.Join(";", contextConfig.Databases.Select((db) => $"{db.Abbr}={db.Name}"))
      };

      string? dataSourceFileName = GetContentFileName("dbdatabases.sqlds");
      if ( dataSourceFileName == null )
        return result;

      var xdoc = await CreateXDocumentFromFileAsync(dataSourceFileName).ConfigureAwait(false);
      using var sqlCommand = SqlCommandUtils.CreateSqlCommandFromXml(xdoc, dataSourceParameters);
      using var sqlConnection = new SqlConnection(connectionString);
      await sqlConnection.OpenAsync().ConfigureAwait(false);
      sqlCommand.Connection = sqlConnection;
      await using var reader = await sqlCommand.ExecuteReaderAsync().ConfigureAwait(false);
      while ( await reader.ReadAsync().ConfigureAwait(false) )
      {
        result.Add(new DatabaseInfo
        {
          ID = reader.GetFieldValue<int>("ID"),
          Abbr = reader.GetFieldValue<string>("Abbr"),
          Name = reader.GetFieldValue<string>("Name"),
          CreateDate = reader.GetFieldValue<string>("CreateDate"),
          CompatLevel = reader.GetFieldValue<int>("CompatLevel")
        });
      }

      return result;
    }
    private static async Task<string> GetDataSourceContentAsync(string connectionString, string dataSourceFileName, SqlParametersDictionary dataSourceParameters)
    {
      var xdoc = await CreateXDocumentFromFileAsync(dataSourceFileName).ConfigureAwait(false);
      using var sqlCommand = SqlCommandUtils.CreateSqlCommandFromXml(xdoc, dataSourceParameters);
      using var sqlConnection = new SqlConnection(connectionString);
      await sqlConnection.OpenAsync().ConfigureAwait(false);
      sqlCommand.Connection = sqlConnection;
      await using var reader = await sqlCommand.ExecuteReaderAsync().ConfigureAwait(false);
      var result = new StringBuilder();
      while ( await reader.ReadAsync().ConfigureAwait(false) )
        result.Append(reader.GetString(0));

      return result.ToString();
    }
    private static async Task<string> GetStaticContentAsync(string staticContentFileName)
    {
      return await System.IO.File.ReadAllTextAsync(staticContentFileName).ConfigureAwait(false);
    }
    private static async Task<XDocument> CreateXDocumentFromFileAsync(string fileName)
    {
      using var stream = System.IO.File.OpenRead(fileName);
      return await XDocument.LoadAsync(stream, LoadOptions.PreserveWhitespace, default).ConfigureAwait(false);
    }
  }

}