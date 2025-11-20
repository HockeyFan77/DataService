using System.Data;
using Microsoft.Data.SqlClient;
using System.Xml.Linq;

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
      if ( value == null )
      {
        if ( sqlParameter.IsNullable )
        {
          sqlParameter.Value = DBNull.Value;
        }

        // not much else we can do
        return;
      }

      if ( __SqlDbTypeValueTypeMap.TryGetValue(sqlParameter.SqlDbType, out var valueType) )
      {
        string valueText = value.ToString() ?? "";

        // when assigning to a SqlString parameter, do direct assignment
        if ( valueType == typeof(string) )
          sqlParameter.Value = valueText;
        // otherwise, let the default conversion take place
        else
          sqlParameter.Value = valueText.ConvertToDefault(valueType);
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

      var timeout = ((string?)element.Attribute("timeout")).ConvertToDefault(0);
      var type = ((string?)element.Attribute("type")).ConvertToDefault(CommandType.Text);
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
          SqlDbType = ((string?)parameterElement.Attribute("type")).ConvertTo<SqlDbType>(),
          Direction = ((string?)parameterElement.Attribute("direction")).ConvertToDefault(ParameterDirection.Input),
          IsNullable = ((string?)parameterElement.Attribute("isNullable")).ConvertToDefault(true)
        };

        if ( sqlParameter.IsNullable )
          sqlParameter.Value = DBNull.Value;

        var size = ((string?)parameterElement.Attribute("size")).ConvertToDefault(0);
        if ( size > 0 )
          sqlParameter.Size = size;
        var precision = ((string?)parameterElement.Attribute("precision")).ConvertToDefault<byte>(0);
        if ( precision > 0 )
          sqlParameter.Precision = precision;
        var scale = ((string?)parameterElement.Attribute("scale")).ConvertToDefault<byte>(0);
        if ( scale > 0 )
          sqlParameter.Scale = scale;

        command.Parameters.Add(sqlParameter);
      }

      AssignParameterValues(command, parameterValues);

      return command;
    }

    private static readonly Dictionary<SqlDbType, Type> __SqlDbTypeValueTypeMap = new()
    {
      [SqlDbType.BigInt] = typeof(long),
      //[SqlDbType.Binary] = typeof(byte[]),
      [SqlDbType.Bit] = typeof(bool),
      [SqlDbType.Char] = typeof(string),
      [SqlDbType.Date] = typeof(DateTime),
      [SqlDbType.DateTime] = typeof(DateTime),
      [SqlDbType.DateTime2] = typeof(DateTime),
      [SqlDbType.DateTimeOffset] = typeof(DateTimeOffset),
      [SqlDbType.Decimal] = typeof(decimal),
      [SqlDbType.Float] = typeof(double),
      //[SqlDbType.Image] = typeof(byte[]),
      [SqlDbType.Int] = typeof(int),
      [SqlDbType.Money] = typeof(decimal),
      [SqlDbType.NChar] = typeof(string),
      [SqlDbType.NText] = typeof(string),
      [SqlDbType.NVarChar] = typeof(string),
      [SqlDbType.Real] = typeof(float),
      [SqlDbType.SmallDateTime] = typeof(DateTime),
      [SqlDbType.SmallInt] = typeof(short),
      [SqlDbType.SmallMoney] = typeof(decimal),
      //[SqlDbType.Structured] = typeof(object),
      [SqlDbType.Text] = typeof(string),
      [SqlDbType.Time] = typeof(TimeSpan),
      //[SqlDbType.Timestamp] = typeof(byte[]),
      [SqlDbType.TinyInt] = typeof(byte),
      //[SqlDbType.Udt] = typeof(object),
      [SqlDbType.UniqueIdentifier] = typeof(Guid),
      //[SqlDbType.VarBinary] = typeof(byte[]),
      [SqlDbType.VarChar] = typeof(string),
      //[SqlDbType.Variant] = typeof(object),
      [SqlDbType.Xml] = typeof(string)
    };
  }

}