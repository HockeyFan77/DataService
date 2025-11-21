using DataServiceApi.Utility; // Reference your utility namespace

namespace DataServiceApi.Tests
{
  [TestClass]
  public class TypeConverterTests
  {
    // --- Test ConvertTo<T>() - Strict Mode (Throws on failure/nulls for value types) ---

    [DataTestMethod]
    [DataRow("123", 123)]
    [DataRow(456, 456)]
    public void ConvertTo_Int_ShouldWork(object input, int expected)
    {
      var result = TypeConverter.ConvertTo<int>(input);
      Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow("9223372036854775807", long.MaxValue)]
    [DataRow(12345L, 12345L)]
    public void ConvertTo_Long_ShouldWork(object input, long expected)
    {
      var result = TypeConverter.ConvertTo<long>(input);
      Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow("True", true)]
    [DataRow("true", true)]
    [DataRow("1", true)] // Custom handler for "1"
    [DataRow(true, true)]
    [DataRow("False", false)]
    [DataRow("false", false)]
    [DataRow("0", false)] // Custom handler for "0"
    [DataRow(false, false)]
    public void ConvertTo_Bool_ShouldWork(object input, bool expected)
    {
      var result = TypeConverter.ConvertTo<bool>(input);
      Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ConvertTo_DateTime_ShouldWork()
    {
      var expected = DateTime.UtcNow;
      object input = expected.ToString("o");

      var result = TypeConverter.ConvertTo<DateTime>(input).ToUniversalTime();
      Assert.AreEqual(expected, result);
      Assert.AreEqual(DateTimeKind.Utc, result.Kind);
    }

    [TestMethod]
    public void ConvertTo_String_ShouldWork()
    {
      object inputInt = 123;
      object inputDate = new DateTime(2025, 1, 1);

      Assert.AreEqual("123", TypeConverter.ConvertTo<string>(inputInt));
      // Default DateTime.ToString() formatting depends on culture, use Contains or specific format check
      var dateResult = TypeConverter.ConvertTo<string>(inputDate);
      StringAssert.Contains(dateResult, "2025");
    }

    [TestMethod]
    public void ConvertTo_String_ShouldReturnNullOnNullInput()
    {
       object? inputNull = null;
       // For string target type, null input returns default(string) which is null
       Assert.IsNull(TypeConverter.ConvertTo<string>(inputNull));
    }

    // --- Test Null Handling (Strict Mode) ---

    [TestMethod]
    public void ConvertTo_NonNullableInt_ShouldThrowOnNullInput()
    {
      object? input = null;
      // Strict mode should throw ArgumentNullException when converting null to int
      Assert.ThrowsException<ArgumentNullException>(() =>
      {
        TypeConverter.ConvertTo<int>(input);
      });
    }

    [TestMethod]
    public void ConvertTo_Int_ShouldThrowOnDouble()
    {
      object? input = 1.23;
      // Strict mode should throw ArgumentNullException when converting null to int
      Assert.ThrowsException<ArgumentException>(() =>
      {
        TypeConverter.ConvertTo<int>(input);
      });
    }

    [TestMethod]
    public void ConvertTo_NullableInt_ShouldReturnNullOnNullInput()
    {
      object? input = null;
      // Strict mode allows null for Nullable<T> targets
      int? result = TypeConverter.ConvertTo<int?>(input);
      Assert.IsNull(result);
    }

    // --- Test Invalid Inputs (Strict Mode throws InvalidCastException/FormatException) ---

    [TestMethod]
    public void ConvertTo_Int_ShouldThrowOnInvalidString()
    {
      object input = "hello";
      // TypeConverter uses internal parsers, which typically throw FormatException
      Assert.ThrowsException<ArgumentException>(() =>
      {
        TypeConverter.ConvertTo<int>(input);
      });
    }

    [TestMethod]
    public void ConvertTo_Bool_ShouldThrowOnInvalidString()
    {
      object input = "perhaps";
      // Check for FormatException
      Assert.ThrowsException<FormatException>(() =>
      {
        TypeConverter.ConvertTo<bool>(input);
      });
    }

    // --- Test ConvertToDefault<T>() - Safe Mode (Returns default value on failure) ---

    [DataTestMethod]
    [DataRow("hello", -1, -1)] // Invalid string input, returns default
    [DataRow(null, -1, -1)]   // Null input, returns default
    [DataRow("100", -1, 100)] // Valid input, returns converted value
    public void ConvertToDefault_Int_ShouldReturnDefaultOnFail(object input, int defaultValue, int expected)
    {
      var result = TypeConverter.ConvertToDefault(input, defaultValue);
      Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ConvertToDefault_NullableDateTime_ShouldReturnDefaultOnFail()
    {
      object input = "not a date";
      DateTime? defaultValue = new DateTime(2000, 1, 1);

      var result = TypeConverter.ConvertToDefault(input, defaultValue);

      Assert.AreEqual(defaultValue, result);
    }

    [TestMethod]
    public void ConvertToDefault_Bool_ShouldReturnDefaultOnFail()
    {
      object input = "perhaps";
      // Default(bool) is false.
      var result = TypeConverter.ConvertToDefault(input, default(bool));
      Assert.IsFalse(result);
    }
  }
}
