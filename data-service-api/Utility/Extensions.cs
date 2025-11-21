using System.Data.Common;

namespace DataServiceApi.Utility
{

  public static class DbDataReaderExtensions
  {
    public static T GetFieldValue<T>(this DbDataReader reader, string columnName, T defaultValue = default!)
    {
      var ordinal = reader.GetOrdinal(columnName);
      return reader.IsDBNull(ordinal) ? defaultValue : reader.GetFieldValue<T>(ordinal);
    }
  }

  public static class StringExtensions
  {

    /// <summary>
    /// Returns <c>true</c> if <paramref name="s"/> is <c>null</c> or empty (via the
    /// <see cref="String.IsNullOrEmpty(string)"/> method).
    /// </summary>
    public static bool IsEmpty(this string? s)
      => string.IsNullOrEmpty(s);
    /// <summary>
    /// Returns <c>true</c> if <paramref name="s"/> is <c>null</c> or white space (via the
    /// <see cref="String.IsNullOrWhiteSpace(string)"/> method).
    /// </summary>
    public static bool IsBlank(this string? s)
      => string.IsNullOrWhiteSpace(s);

    /// <summary>
    /// Returns <c>true</c> if <paramref name="s"/> equals <paramref name="other"/> (via the
    /// <see cref="String.Equals(string, string, StringComparison)"/> method) using the
    /// <see cref="StringComparison.Ordinal"/> comparison type.
    /// </summary>
    public static bool EqualsOrdinal(this string? s, string? other)
      => string.Equals(s, other, StringComparison.Ordinal);
    /// <summary>
    /// Returns <c>true</c> if <paramref name="s"/> equals <paramref name="other"/> (via the
    /// <see cref="String.Equals(string, string, StringComparison)"/> method) using the
    /// <see cref="StringComparison.OrdinalIgnoreCase"/> comparison type.
    /// </summary>
    public static bool EqualsOrdinalNoCase(this string? s, string? other)
      => string.Equals(s, other, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Returns <c>true</c> if <paramref name="s"/> equals any string in <paramref name="strings"/> (via the
    /// <see cref="String.Equals(string, string, StringComparison)"/> method) using the given
    /// <paramref name="comparisonType"/>.
    /// </summary>
    public static bool EqualsAny(this string? s, StringComparison comparisonType, params string?[] strings)
      => EqualsAny(s, comparisonType, (IEnumerable<string?>?) strings);
    /// <summary>
    /// Returns <c>true</c> if <paramref name="s"/> equals any string in <paramref name="strings"/> (via the
    /// <see cref="String.Equals(string, string, StringComparison)"/> method) using the given
    /// <paramref name="comparisonType"/>.
    /// </summary>
    public static bool EqualsAny(this string? s, StringComparison comparisonType, IEnumerable<string?>? strings)
    {
      if ( strings != null )
        foreach ( string? test in strings )
          if ( string.Equals(s, test, comparisonType) )
            return true;
      return false;
    }
    /// <summary>
    /// Returns <c>true</c> if <paramref name="s"/> equals any string in <paramref name="strings"/> (via the
    /// <see cref="String.Equals(string, string, StringComparison)"/> method) using the
    /// <see cref="StringComparison.Ordinal"/> comparison type.
    /// </summary>
    public static bool EqualsAnyOrdinal(this string? s, params string?[] strings)
      => EqualsAny(s, StringComparison.Ordinal, (IEnumerable<string?>?) strings);
    /// <summary>
    /// Returns <c>true</c> if <paramref name="s"/> equals any string in <paramref name="strings"/> (via the
    /// <see cref="String.Equals(string, string, StringComparison)"/> method) using the
    /// <see cref="StringComparison.Ordinal"/> comparison type.
    /// </summary>
    public static bool EqualsAnyOrdinal(this string? s, IEnumerable<string?>? strings)
      => EqualsAny(s, StringComparison.Ordinal, strings);
    /// <summary>
    /// Returns <c>true</c> if <paramref name="s"/> equals any string in <paramref name="strings"/> (via the
    /// <see cref="String.Equals(string, string, StringComparison)"/> method) using the
    /// <see cref="StringComparison.OrdinalIgnoreCase"/> comparison type.
    /// </summary>
    public static bool EqualsAnyOrdinalNoCase(this string? s, params string?[] strings)
      => EqualsAny(s, StringComparison.OrdinalIgnoreCase, (IEnumerable<string?>?) strings);
    /// <summary>
    /// Returns <c>true</c> if <paramref name="s"/> equals any string in <paramref name="strings"/> (via the
    /// <see cref="String.Equals(string, string, StringComparison)"/> method) using the
    /// <see cref="StringComparison.OrdinalIgnoreCase"/> comparison type.
    /// </summary>
    public static bool EqualsAnyOrdinalNoCase(this string? s, IEnumerable<string?>? strings)
      => EqualsAny(s, StringComparison.OrdinalIgnoreCase, strings);

    /// <summary>
    /// Returns the index of <paramref name="other"/> in <paramref name="s"/> (via the
    /// <see cref="String.IndexOf(string, StringComparison)"/> method) using the
    /// <see cref="StringComparison.Ordinal"/> comparison type.
    /// </summary>
    public static int IndexOfOrdinal(this string? s, string? other)
      => s == null || other == null ? -1 : s.IndexOf(other, StringComparison.Ordinal);
    /// <summary>
    /// Returns the index of <paramref name="other"/> in <paramref name="s"/> (via the
    /// <see cref="String.IndexOf(string, StringComparison)"/> method) using the
    /// <see cref="StringComparison.OrdinalIgnoreCase"/> comparison type.
    /// </summary>
    public static int IndexOfOrdinalNoCase(this string? s, string? other)
      => s == null || other == null ? -1 : s.IndexOf(other, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Returns <c>true</c> if <paramref name="other"/> is found in <paramref name="s"/> (via the
    /// <see cref="String.IndexOf(string, StringComparison)"/> method) using the
    /// <see cref="StringComparison.Ordinal"/> comparison type.
    /// </summary>
    public static bool ContainsOrdinal(this string? s, string? other)
      => s != null && other != null && s.Contains(other, StringComparison.Ordinal);
    /// <summary>
    /// Returns <c>true</c> if <paramref name="other"/> is found in <paramref name="s"/> (via the
    /// <see cref="String.IndexOf(string, StringComparison)"/> method) using the
    /// <see cref="StringComparison.OrdinalIgnoreCase"/> comparison type.
    /// </summary>
    public static bool ContainsOrdinalNoCase(this string? s, string? other)
      => s != null && other != null && s.Contains(other, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Returns <c>true</c> if <paramref name="s"/> is found in any string in <paramref name="strings"/> (via the
    /// <see cref="String.Contains(string, StringComparison)"/> method) using the given
    /// <paramref name="comparisonType"/>.
    /// </summary>
    public static bool ContainsAny(this string? s, StringComparison comparisonType, params string?[] strings)
      => ContainsAny(s, comparisonType, (IEnumerable<string?>?) strings);
    /// <summary>
    /// Returns <c>true</c> if <paramref name="s"/> is found in any string in <paramref name="strings"/> (via the
    /// <see cref="String.Contains(string, StringComparison)"/> method) using the given
    /// <paramref name="comparisonType"/>.
    /// </summary>
    public static bool ContainsAny(this string? s, StringComparison comparisonType, IEnumerable<string?>? strings)
    {
      if ( s != null && strings != null )
        foreach ( string? test in strings )
          if ( test != null && s.Contains(test, comparisonType) )
            return true;
      return false;
    }
    /// <summary>
    /// Returns <c>true</c> if <paramref name="s"/> is found in any string in <paramref name="strings"/> (via the
    /// <see cref="String.Contains(string, StringComparison)"/> method) using the
    /// <see cref="StringComparison.Ordinal"/> comparison type.
    /// </summary>
    public static bool ContainsAnyOrdinal(this string? s, params string?[] strings)
      => ContainsAny(s, StringComparison.Ordinal, (IEnumerable<string?>?) strings);
    /// <summary>
    /// Returns <c>true</c> if <paramref name="s"/> is found in any string in <paramref name="strings"/> (via the
    /// <see cref="String.Contains(string, StringComparison)"/> method) using the
    /// <see cref="StringComparison.Ordinal"/> comparison type.
    /// </summary>
    public static bool ContainsAnyOrdinal(this string? s, IEnumerable<string?>? strings)
      => ContainsAny(s, StringComparison.Ordinal, strings);
    /// <summary>
    /// Returns <c>true</c> if <paramref name="s"/> is found in any string in <paramref name="strings"/> (via the
    /// <see cref="String.Contains(string, StringComparison)"/> method) using the
    /// <see cref="StringComparison.OrdinalIgnoreCase"/> comparison type.
    /// </summary>
    public static bool ContainsAnyOrdinalNoCase(this string? s, params string?[] strings)
      => ContainsAny(s, StringComparison.OrdinalIgnoreCase, (IEnumerable<string?>?) strings);
    /// <summary>
    /// Returns <c>true</c> if <paramref name="s"/> is found in any string in <paramref name="strings"/> (via the
    /// <see cref="String.Contains(string, StringComparison)"/> method) using the
    /// <see cref="StringComparison.OrdinalIgnoreCase"/> comparison type.
    /// </summary>
    public static bool ContainsAnyOrdinalNoCase(this string? s, IEnumerable<string?>? strings)
      => ContainsAny(s, StringComparison.OrdinalIgnoreCase, strings);

    /// <summary>
    /// Returns the left <paramref name="count"/> characters of <paramref name="s"/>.
    /// Returns <c>null</c> when <paramref name="s"/> is <c>null</c>.
    /// </summary>
    public static string? TakeLeft(this string? s, int count)
    {
      if ( s == null ) return null;
      if ( s == "" || count <= 0 ) return "";
      if ( count >= s.Length ) return s;
      return s[..count];
    }
    /// <summary>
    /// Returns the left characters of <paramref name="s"/> up to but not including <paramref name="delimiter"/>.
    /// Returns <c>null</c> when <paramref name="s"/> is <c>null</c>.
    /// </summary>
    public static string? TakeLeft(this string? s, char delimiter, StringComparison comparisonType = StringComparison.Ordinal)
      => TakeLeft(s, delimiter.ToString(), false, comparisonType);
    /// <summary>
    /// Returns the left characters of <paramref name="s"/> up to and including <paramref name="delimiter"/>
    /// when <paramref name="includeDelimiter"/> is <c>true</c>.
    /// Returns <c>null</c> when <paramref name="s"/> is <c>null</c>.
    /// </summary>
    public static string? TakeLeft(this string? s, char delimiter, bool includeDelimiter, StringComparison comparisonType = StringComparison.Ordinal)
      => TakeLeft(s, delimiter.ToString(), includeDelimiter, comparisonType);
    /// <summary>
    /// Returns the left characters of <paramref name="s"/> up to but not including <paramref name="delimiter"/>.
    /// Returns <c>null</c> when <paramref name="s"/> is <c>null</c>.
    /// </summary>
    public static string? TakeLeft(this string? s, string? delimiter, StringComparison comparisonType = StringComparison.Ordinal)
      => TakeLeft(s, delimiter, false, comparisonType);
    /// <summary>
    /// Returns the left characters of <paramref name="s"/> up to and including <paramref name="delimiter"/>
    /// when <paramref name="includeDelimiter"/> is <c>true</c>.
    /// Returns <c>null</c> when <paramref name="s"/> is <c>null</c>.
    /// </summary>
    public static string? TakeLeft(this string? s, string? delimiter, bool includeDelimiter, StringComparison comparisonType = StringComparison.Ordinal)
    {
      if ( s == null || delimiter == null ) return null;
      if ( s == "" || delimiter == "" ) return "";
      int delimiterIndex = s.IndexOf(delimiter, comparisonType);
      int count = (delimiterIndex < 0)
        ? 0
        : includeDelimiter
          ? delimiterIndex + delimiter.Length
          : delimiterIndex;
      return TakeLeft(s, count);
    }

    /// <summary>
    /// Returns the right <paramref name="count"/> characters of <paramref name="s"/>.
    /// Returns <c>null</c> when <paramref name="s"/> is <c>null</c>.
    /// </summary>
    public static string? TakeRight(this string? s, int count)
    {
      if ( s == null ) return null;
      if ( s == "" || count <= 0 ) return "";
      if ( count >= s.Length ) return s;
      return s.Substring(s.Length - count, count);
    }
    /// <summary>
    /// Returns the right characters of <paramref name="s"/> up to but not including the last <paramref name="delimiter"/>.
    /// Returns <c>null</c> when <paramref name="s"/> is <c>null</c>.
    /// <para>Note: the search for <paramref name="delimiter"/> starts at the end of <paramref name="s"/>
    /// and goes right-to-left.</para>
    /// </summary>
    public static string? TakeRight(this string? s, char delimiter, StringComparison comparisonType = StringComparison.Ordinal)
      => TakeRight(s, delimiter.ToString(), false, comparisonType);
    /// <summary>
    /// Returns the right characters of <paramref name="s"/> up to and including the last <paramref name="delimiter"/>
    /// when <paramref name="includeDelimiter"/> is <c>true</c>.
    /// Returns <c>null</c> when <paramref name="s"/> is <c>null</c>.
    /// <para>Note: the search for <paramref name="delimiter"/> starts at the end of <paramref name="s"/>
    /// and goes right-to-left.</para>
    /// </summary>
    public static string? TakeRight(this string? s, char delimiter, bool includeDelimiter, StringComparison comparisonType = StringComparison.Ordinal)
      => TakeRight(s, delimiter.ToString(), includeDelimiter, comparisonType);
    /// <summary>
    /// Returns the right characters of <paramref name="s"/> up to but not including the last <paramref name="delimiter"/>.
    /// Returns <c>null</c> when <paramref name="s"/> is <c>null</c>.
    /// <para>Note: the search for <paramref name="delimiter"/> starts at the end of <paramref name="s"/>
    /// and goes right-to-left.</para>
    /// </summary>
    public static string? TakeRight(this string? s, string? delimiter, StringComparison comparisonType = StringComparison.Ordinal)
      => TakeRight(s, delimiter, false, comparisonType);
    /// <summary>
    /// Returns the right characters of <paramref name="s"/> up to and including the last <paramref name="delimiter"/>
    /// when <paramref name="includeDelimiter"/> is <c>true</c>.
    /// Returns <c>null</c> when <paramref name="s"/> is <c>null</c>.
    /// <para>Note: the search for <paramref name="delimiter"/> starts at the end of <paramref name="s"/>
    /// and goes right-to-left.</para>
    /// </summary>
    public static string? TakeRight(this string? s, string? delimiter, bool includeDelimiter, StringComparison comparisonType = StringComparison.Ordinal)
    {
      if ( s == null || delimiter == null ) return null;
      if ( s == "" || delimiter == "" ) return "";
      int delimiterIndex = s.LastIndexOf(delimiter, comparisonType);
      int count = (delimiterIndex < 0)
        ? 0
        : includeDelimiter
          ? s.Length - delimiterIndex
          : s.Length - delimiterIndex - delimiter.Length;
      return TakeRight(s, count);
    }

    public static string? TakeBetween(this string? s, char leftDelimiter, char rightDelimiter, StringComparison comparisonType = StringComparison.Ordinal)
      => TakeBetween(s, leftDelimiter.ToString(), rightDelimiter.ToString(), false, comparisonType);
    public static string? TakeBetween(this string? s, char leftDelimiter, char rightDelimiter, bool includeDelimiters, StringComparison comparisonType = StringComparison.Ordinal)
      => TakeBetween(s, leftDelimiter.ToString(), rightDelimiter.ToString(), includeDelimiters, comparisonType);
    public static string? TakeBetween(this string? s, string? leftDelimiter, string? rightDelimiter, StringComparison comparisonType = StringComparison.Ordinal)
      => TakeBetween(s, leftDelimiter, rightDelimiter, false, comparisonType);
    public static string? TakeBetween(this string? s, string? leftDelimiter, string? rightDelimiter, bool includeDelimiters, StringComparison comparisonType = StringComparison.Ordinal)
    {
      if ( s == null || leftDelimiter == null || rightDelimiter == null ) return s;

      int leftIndex = s.IndexOf(leftDelimiter, comparisonType);
      if ( leftIndex < 0 ) return "";

      int rightIndex = s.IndexOf(rightDelimiter, leftIndex + leftDelimiter.Length, comparisonType);
      if ( rightIndex < 0 ) return "";

      if ( leftIndex < 0 || rightIndex < leftIndex + leftDelimiter.Length ) return "";

      int startIndex = includeDelimiters ? leftIndex : leftIndex + leftDelimiter.Length;
      int endIndex = includeDelimiters ? rightIndex + rightDelimiter.Length : rightIndex;
      if ( endIndex < startIndex ) return "";

      return s[startIndex..endIndex];
    }

    public static string? RemoveAny(this string? s, bool ignoreCase, ReadOnlySpan<char> charsToRemove)
    {
      if ( string.IsNullOrEmpty(s) || charsToRemove.IsEmpty ) return s;

      char[]? lowerChars = null;
      ReadOnlySpan<char> compareChars = charsToRemove;
      if ( ignoreCase )
      {
        lowerChars = new char[charsToRemove.Length];
        for ( int i = 0; i < charsToRemove.Length; i++ )
          lowerChars[i] = char.ToLowerInvariant(charsToRemove[i]);
        compareChars = lowerChars;
      }

      Span<char> buffer = s.Length <= 256 ? stackalloc char[s.Length] : new char[s.Length];

      int index = 0;
      foreach ( char c in s )
      {
        char compareChar = ignoreCase ? char.ToLowerInvariant(c) : c;
        bool found = false;
        for ( int i = 0; i < compareChars.Length; i++ )
        {
          if ( compareChar == compareChars[i] )
          {
            found = true;
            break;
          }
        }
        if ( !found )
        {
          buffer[index++] = c;
        }
      }

      return new string(buffer.Slice(0, index));
    }
    public static string? RemoveAny(this string? s, bool ignoreCase, char[]? charsToRemove)
      => RemoveAny(s, ignoreCase, charsToRemove == null ? [] : new ReadOnlySpan<char>(charsToRemove));
    public static string? RemoveAny(this string? s, bool ignoreCase, string? charsToRemove)
      => RemoveAny(s, ignoreCase, charsToRemove == null ? [] : charsToRemove.AsSpan());
    public static string? RemoveAny(this string? s, char[] removeChars)
      => RemoveAny(s, false, removeChars);
    public static string? RemoveAny(this string? s, string? removeChars)
      => RemoveAny(s, false, removeChars);
    public static string? RemoveAnyIgnoreCase(this string? s, char[] removeChars)
      => RemoveAny(s, true, removeChars);
    public static string? RemoveAnyIgnoreCase(this string? s, string? removeChars)
      => RemoveAny(s, true, removeChars);

    /// <summary>
    /// Returns <c>true</c> if <paramref name="s"/> is contains a "true" string
    /// (e.g. "1", "y", "yes", "t", "true").
    /// </summary>
    public static bool IsTruthy(this string? s)
      => EqualsAnyOrdinalNoCase(s, "1", "y", "yes", "t", "true");

    public static string? ToProperCaseSpaced(this string? s)
    {
      if ( IsBlank(s) ) return s;

      Func<string, int, bool> hasCharAt = (w, cx) => { return cx >= 0 && cx < w.Length; };
      Func<string, int, bool> isCharLower = (w, cx) => { return hasCharAt(w, cx) && Char.IsLower(w[cx]); };
      Func<string, int, bool> isCharUpper = (w, cx) => { return hasCharAt(w, cx) && Char.IsUpper(w[cx]); };

      string[] words = s!.Split([' '], StringSplitOptions.RemoveEmptyEntries);

      for ( int wx = 0; wx < words.Length; wx++ )
      {
        string w = words[wx];
        int cx = w.Length - 1;
        while ( cx >= 1 )
        {
          if ( isCharUpper(w, cx) && isCharLower(w, cx - 1) )
            w = w.Insert(cx, " ");
          else if ( isCharUpper(w, cx) && isCharUpper(w, cx - 1) && isCharLower(w, cx + 1) )
          {
            w = w.Insert(cx, " ");
            while ( isCharUpper(w, cx - 1) )
              cx--;
            if ( hasCharAt(w, cx) && hasCharAt(w, cx - 1) )
              w = w.Insert(cx, " ");
          }
          cx--;
        }
        if ( isCharLower(w, 0) )
        {
          var chars = w.ToCharArray();
          chars[0] = Char.ToUpper(chars[0]);
          w = new String(chars);
        }
        words[wx] = w;
      }

      return string.Join(" ", words);
    }

  }

}