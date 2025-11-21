using System.Data;
using System.Collections.Concurrent;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;

namespace DataServiceApi.Utility
{

  // Note you can pass an instance of SqlParametersDictionary to SqlCommandUtils.AssignParameterValues()
  // or SqlCommandUtils.CreateSqlCommandFromXml() directly.
  public class SqlParametersDictionary : Dictionary<string, object?>
  {
    public SqlParametersDictionary()
      : base(StringComparer.OrdinalIgnoreCase) { }
    public SqlParametersDictionary(IDictionary<string, object?> dictionary)
      : base(dictionary, StringComparer.OrdinalIgnoreCase) { }
    public SqlParametersDictionary(IEnumerable<KeyValuePair<string, object?>> items)
      : base(StringComparer.OrdinalIgnoreCase)
    {
      foreach ( var kvp in items )
      {
        this[kvp.Key] = kvp.Value;
      }
    }
    public SqlParametersDictionary(int capacity)
      : base(capacity, StringComparer.OrdinalIgnoreCase) { }
  }

  public static class SqlCommandUtils
  {
    public static void AssignParameterValue(SqlParameter sqlParameter, object? value)
    {
      // --- Step 1: Handle Null to DBNull.Value ---
      if ( value == null )
      {
        if ( sqlParameter.IsNullable )
        {
          sqlParameter.Value = DBNull.Value;
          return;
        }

        // If the value is null but IsNullable is false, we let the strict conversion
        // logic below handle the error *if* the target type is a non-nullable value type.
        // For reference types, leaving value as null will also be handled by the conversion.
      }

      // --- Step 2: Determine Target CLR Type using the Map ---
      Type targetType;

      // Use your static map lookup to get the target C# Type
      if ( !__SqlDbTypeValueTypeMap.TryGetValue(sqlParameter.SqlDbType, out targetType!) )
      {
        // Throw if the SqlDbType is not found in your map, fulfilling your requirement
        throw new InvalidOperationException($"SQL type '{sqlParameter.SqlDbType}' is not mapped to a CLR type.");
      }

      // --- Step 3: Get Cached Method and Invoke (Cacher code needed above) ---
      try
      {
        // Fetch the MethodInfo from the cache (or create it once)
        MethodInfo convertMethod = GetConverterMethodInfo(targetType);

        // Invoke the cached, closed generic method
        // This is the call to value.ConvertTo<T>() using reflection.
        object? convertedValue = convertMethod.Invoke(null, new object?[] { value });

        sqlParameter.Value = convertedValue;
      }
      catch ( TargetInvocationException ex ) when ( ex.InnerException != null )
      {
        // Unwrap and re-throw the inner exception from the conversion logic
        // This handles all failed conversions (e.g., FormatException, ArgumentNullException from null to int).
        throw ex.InnerException;
      }
      catch ( Exception ex )
      {
        // Handle other possible reflection errors or unexpected issues
        throw new InvalidOperationException($"Error during value assignment for parameter '{sqlParameter.ParameterName}'.", ex);
      }
    }

    public static void AssignParameterValues(SqlCommand sqlCommand, params KeyValuePair<string, object?>[] parameterValues)
      => AssignParameterValues(sqlCommand, parameterValues as IEnumerable<KeyValuePair<string, object?>>);
    public static void AssignParameterValues(SqlCommand sqlCommand, IEnumerable<KeyValuePair<string, object?>> parameterValues)
    {
      if ( parameterValues is null ) return;
      var parameterValueMap = parameterValues
        .SelectMany(kvp => sqlCommand.Parameters.Cast<SqlParameter>()
        .Where(param => param.ParameterName.TrimStart('@').EqualsOrdinalNoCase(kvp.Key.TrimStart('@')))
        .Select(param => new
        {
          Parameter = param,
          kvp.Value
        }));
      foreach ( var parameterValue in parameterValueMap )
        AssignParameterValue(parameterValue.Parameter, parameterValue.Value);
    }

    public static SqlCommand CreateSqlCommandFromXml(XDocument? document, params KeyValuePair<string, object?>[] parameterValues)
      => CreateSqlCommandFromXml(document, parameterValues as IEnumerable<KeyValuePair<string, object?>>);
    public static SqlCommand CreateSqlCommandFromXml(XDocument? document, IEnumerable<KeyValuePair<string, object?>> parameterValues)
      => CreateSqlCommandFromXml(document?.Root, parameterValues);
    public static SqlCommand CreateSqlCommandFromXml(XElement? element, params KeyValuePair<string, object?>[] parameterValues)
      => CreateSqlCommandFromXml(element, parameterValues as IEnumerable<KeyValuePair<string, object?>>);
    public static SqlCommand CreateSqlCommandFromXml(XElement? element, IEnumerable<KeyValuePair<string, object?>> parameterValues)
    {
      ArgumentNullException.ThrowIfNull(element);
      if ( element.Name != "SqlCommand" )
        throw new ArgumentException($"{nameof(element)}.Name must be \"SqlCommand\".", nameof(element));

      /*
        <SqlCommand
          timeout="{int}" default=30 (seconds)
          type="{Text|StoredProcedure|TableDirect}" default=Text
        >
          <Parameters>
            <Parameter
              name="{string}"
              type="{SqlDbType}" default=NVarChar
              direction="{Input|Output|InputOutput|ReturnValue}" default=Input
              isNullable="{bool}" default=true
              size="{int}" default=not-set
              precision="{byte}" default=not-set
              scale="{byte}" default=not-set
              value="{string}" default=null/DbNull.Value
            />
          </Parameters>
          <CommandText>
            <![CDATA[
            ]]>
          </CommandText>
        </SqlCommand>
      */

      var timeout = TypeConverter.ConvertToDefault((string?)element.Attribute("timeout"), 0);
      var type = TypeConverter.ConvertToDefault((string?)element.Attribute("type"), CommandType.Text);
      var text = (string?)element.Element("CommandText") ?? "";

      var command = new SqlCommand
      {
        CommandTimeout = timeout > 0 ? timeout : 30,
        CommandType = type,
        CommandText = type != CommandType.Text ? text.Trim() : text
      };

      var parameterElements = element.Element("Parameters")?.Elements("Parameter") ?? [];
      foreach ( var parameterElement in parameterElements )
      {
        var sqlParameter = new SqlParameter()
        {
          ParameterName = (string?)parameterElement.Attribute("name") ?? "",
          SqlDbType = TypeConverter.ConvertTo<SqlDbType>((string?)parameterElement.Attribute("type")),
          Direction = TypeConverter.ConvertToDefault((string?)parameterElement.Attribute("direction"), ParameterDirection.Input),
          IsNullable = TypeConverter.ConvertToDefault((string?)parameterElement.Attribute("isNullable"), true)
        };

        if ( sqlParameter.IsNullable )
        {
          sqlParameter.Value = DBNull.Value;
        }

        var size = TypeConverter.ConvertToDefault((string?)parameterElement.Attribute("size"), 0);
        if ( size > 0 )
        {
          sqlParameter.Size = size;
        }
        var precision = TypeConverter.ConvertToDefault<byte>((string?)parameterElement.Attribute("precision"), 0);
        if ( precision > 0 )
        {
          sqlParameter.Precision = precision;
        }
        var scale = TypeConverter.ConvertToDefault<byte>((string?)parameterElement.Attribute("scale"), 0);
        if ( scale > 0 )
        {
          sqlParameter.Scale = scale;
        }

        command.Parameters.Add(sqlParameter);
      }

      AssignParameterValues(command, parameterValues);

      return command;
    }

    // Caches the closed generic MethodInfo for ConversionExtensions.ConvertTo<T>
    private static readonly ConcurrentDictionary<Type, MethodInfo> ConvertToMethodCache = new();

    // Get the MethodInfo for the non-generic definition once
    private static readonly MethodInfo GenericConvertToMethod = typeof(TypeConverter).GetMethod(nameof(TypeConverter.ConvertTo), new[] { typeof(object) })!;

    /// <summary>
    /// Gets or creates the closed generic MethodInfo for the specified target type T.
    /// </summary>
    private static MethodInfo GetConverterMethodInfo(Type targetType)
    {
      // Use GetOrAdd for thread-safe access and creation
      return ConvertToMethodCache.GetOrAdd(targetType, t =>
      {
        // The slow reflection operation runs only once per Type
        return GenericConvertToMethod.MakeGenericMethod(t);
      });
    }

    private static readonly Dictionary<SqlDbType, Type> __SqlDbTypeValueTypeMap = new()
    {
      { SqlDbType.BigInt, typeof(long) },
      // { SqlDbType.Binary, typeof(byte { ]) },
      { SqlDbType.Bit, typeof(bool) },
      { SqlDbType.Char, typeof(string) },
      { SqlDbType.Date, typeof(DateTime) },
      { SqlDbType.DateTime, typeof(DateTime) },
      { SqlDbType.DateTime2, typeof(DateTime) },
      { SqlDbType.DateTimeOffset, typeof(DateTimeOffset) },
      { SqlDbType.Decimal, typeof(decimal) },
      { SqlDbType.Float, typeof(double) },
      // { SqlDbType.Image, typeof(byte { ]) },
      { SqlDbType.Int, typeof(int) },
      { SqlDbType.Money, typeof(decimal) },
      { SqlDbType.NChar, typeof(string) },
      { SqlDbType.NText, typeof(string) },
      { SqlDbType.NVarChar, typeof(string) },
      { SqlDbType.Real, typeof(float) },
      { SqlDbType.SmallDateTime, typeof(DateTime) },
      { SqlDbType.SmallInt, typeof(short) },
      { SqlDbType.SmallMoney, typeof(decimal) },
      // { SqlDbType.Structured, typeof(object) },
      { SqlDbType.Text, typeof(string) },
      { SqlDbType.Time, typeof(TimeSpan) },
      // { SqlDbType.Timestamp, typeof(byte { ]) },
      { SqlDbType.TinyInt, typeof(byte) },
      // { SqlDbType.Udt, typeof(object) },
      { SqlDbType.UniqueIdentifier, typeof(Guid) },
      // { SqlDbType.VarBinary, typeof(byte { ]) },
      { SqlDbType.VarChar, typeof(string) },
      // { SqlDbType.Variant, typeof(object) },
      { SqlDbType.Xml, typeof(string) }
    };
  }

}