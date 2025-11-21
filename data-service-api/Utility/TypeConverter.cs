using System.ComponentModel;
using System.Globalization;

namespace DataServiceApi.Utility
{

  public static class TypeConverter
  {
    /// <summary>
    /// Converts an object? to a target type T.
    /// THROWS an exception if the input is null (for non-nullable value types) or if conversion is impossible/fails.
    /// </summary>
    public static T? ConvertTo<T>(object? input)
    {
      // Calls the internal helper, requiring a default of null and telling it to THROW on failure.
      return ConvertInternal(input, (T?)default, throwOnFail: true);
    }

    /// <summary>
    /// Converts an object? to a target type T.
    /// Returns the specified defaultValue if conversion is impossible or fails.
    /// </summary>
    /// <param name="input">The object to convert.</param>
    /// <param name="defaultValue">The value to return if conversion fails or input is null.</param>
    public static T? ConvertToDefault<T>(object? input, T? defaultValue)
    {
      // Calls the internal helper, passing the user-supplied default and telling it NOT to throw.
      return ConvertInternal(input, defaultValue, throwOnFail: false);
    }

    // --- PRIVATE HELPER METHOD CONTAINING ALL LOGIC ---

    /// <summary>
    /// Contains the core conversion logic with options for fallback value and throwing exceptions.
    /// </summary>
    private static T? ConvertInternal<T>(object? input, T? fallbackValue, bool throwOnFail)
    {
      var targetType = typeof(T);
      var underlyingType = Nullable.GetUnderlyingType(targetType);

      // --- Step 1: Strict Null Check for Non-Nullable Value Types ---
      if ( input == null )
      {
        // If throwOnFail is TRUE AND T is a non-nullable value type (underlyingType == null)
        if ( throwOnFail && (underlyingType == null) && targetType.IsValueType )
        {
          // Throwing here ensures ConvertTo<int>(null) fails strictly.
          throw new ArgumentNullException(nameof(input), $"Cannot convert null input to non-nullable value type '{targetType.Name}'.");
        }

        // Otherwise (safe to be null: reference type, nullable value type, or throwOnFail is false)
        return fallbackValue;
      }

      // --- Step 2 & 3: Handle Direct Compatibility and Empty Strings ---
      var effectiveTargetType = underlyingType ?? targetType;

      // If the input is already the target type or directly compatible, return it immediately
      if ( effectiveTargetType.IsInstanceOfType(input) )
      {
        return (T)input;
      }

      // Handle null/empty strings gracefully if target is nullable (already checked input == null above)
      if ( string.IsNullOrEmpty(input.ToString()) && underlyingType != null )
      {
        return fallbackValue;
      }

      // --- Step 4 & 5: Use TypeConverter Fallback ---
      try
      {
        var converter = TypeDescriptor.GetConverter(effectiveTargetType);

        // Attempt conversion from the original type
        if ( converter.CanConvertFrom(input.GetType()) )
        {
          return (T)converter.ConvertFrom(null, CultureInfo.InvariantCulture, input)!;
        }

        // Fallback to converting FROM a string representation
        var inputAsString = input.ToString();
        if ( converter.CanConvertFrom(typeof(string)) )
        {
          return (T)converter.ConvertFrom(null, CultureInfo.InvariantCulture, inputAsString!)!;
        }
      }
      catch ( Exception ex ) when ( !throwOnFail )
      {
        // SAFE EXIT: If throwOnFail is FALSE, catch the conversion exception (e.g., FormatException)
        // and return the safe fallback value.
        return fallbackValue;
      }
      catch ( Exception )
      {
        // STRICT EXIT: If throwOnFail is TRUE, the exception is allowed to propagate.
        throw;
      }

      // --- Final Failure Point (Reached if CanConvertFrom was false for both paths) ---
      if ( throwOnFail )
      {
        // If no conversion path was possible and we must throw, indicate conversion failure.
        throw new InvalidCastException($"Cannot convert value '{input}' of type '{input.GetType().Name}' to target type '{typeof(T).Name}'. No suitable TypeConverter was found.");
      }

      // Safe return for ConvertToDefault where no conversion path was found.
      return fallbackValue;
    }
  }

}
