using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataServiceApi.Utility;

namespace DataServiceApi.Tests;

[TestClass]
public sealed class StringExtensionsTests
{

  [TestMethod]
  public void IsEmpty_ShouldReturnTrueForNullAndEmpty()
  {
    // Arrange (No setup needed)
    string? nullString = null;
    string emptyString = "";
    string whiteSpaceString = " ";
    string actualString = "abc";

    // Act & Assert
    Assert.IsTrue(nullString.IsEmpty(), "Null string should be Empty.");
    Assert.IsTrue(emptyString.IsEmpty(), "Empty string should be Empty.");
    Assert.IsFalse(whiteSpaceString.IsEmpty(), "Whitespace should NOT be Empty.");
    Assert.IsFalse(actualString.IsEmpty(), "Actual string should NOT be Empty.");
  }

  [TestMethod]
  public void IsBlank_ShouldReturnTrueForNullEmptyAndWhitespace()
  {
    // Arrange
    string? nullString = null;
    string emptyString = "";
    string whiteSpaceString = "   \t\n";
    string actualString = "abc";

    // Act & Assert
    Assert.IsTrue(nullString.IsBlank(), "Null string should be Blank.");
    Assert.IsTrue(emptyString.IsBlank(), "Empty string should be Blank.");
    Assert.IsTrue(whiteSpaceString.IsBlank(), "Whitespace string should be Blank.");
    Assert.IsFalse(actualString.IsBlank(), "Actual string should NOT be Blank.");
  }

  [DataTestMethod]
  [DataRow("Hello", "hello", false, DisplayName = "Case-Sensitive Mismatch")]
  [DataRow("Hello", "Hello", true, DisplayName = "Case-Sensitive Match")]
  [DataRow(null, null, true, DisplayName = "Both Null")]
  [DataRow("A", null, false, DisplayName = "One Null")]
  public void EqualsOrdinal_ShouldBeCaseSensitiveAndHandleNulls(string? s1, string? s2, bool expected)
  {
    Assert.AreEqual(expected, s1.EqualsOrdinal(s2));
  }

  [DataTestMethod]
  [DataRow("Hello", "hello", true, DisplayName = "Case-Insensitive Match")]
  [DataRow("FILE.pdf", "file.PDF", true, DisplayName = "Mixed Case Match")]
  [DataRow(null, "Test", false, DisplayName = "Input Null")]
  [DataRow(null, null, true, DisplayName = "Both Null")]
  public void EqualsOrdinalNoCase_ShouldBeCaseInsensitiveAndHandleNulls(string? s1, string? s2, bool expected)
  {
    Assert.AreEqual(expected, s1.EqualsOrdinalNoCase(s2));
  }

  [TestMethod]
  public void EqualsAny_ShouldReturnCorrectly()
  {
    string s = "Apple";
    string?[] matches = { "Banana", "apple", "Orange", null };

    // Ordinal (Case-Sensitive) - 'apple' is lowercase, 'Apple' != 'apple'
    Assert.IsFalse(s.EqualsAnyOrdinal(matches), "EqualsAnyOrdinal should fail case-sensitively.");

    // Ordinal No Case (Case-Insensitive) - 'Apple' == 'apple'
    Assert.IsTrue(s.EqualsAnyOrdinalNoCase(matches), "EqualsAnyOrdinalNoCase should succeed case-insensitively.");

    // Test null matches
    Assert.IsTrue(((string?)null).EqualsAnyOrdinal(matches), "Null input should match null in the list.");
    Assert.IsFalse("Test".EqualsAnyOrdinal(Array.Empty<string?>()), "Empty list should return false.");
  }

  [DataTestMethod]
  [DataRow("File.pdf", ".pdf", 4, "IndexOfOrdinal", DisplayName = "Found Ordinal")]
  [DataRow("FILE.pdf", ".PDF", -1, "IndexOfOrdinal", DisplayName = "Not Found Ordinal Case")]
  [DataRow("FILE.pdf", ".PDF", 4, "IndexOfOrdinalNoCase", DisplayName = "Found Ordinal No Case")]
  [DataRow(null, "test", -1, "IndexOfOrdinal", DisplayName = "Input Null")]
  [DataRow("test", null, -1, "IndexOfOrdinal", DisplayName = "Search Null")]
  public void IndexOf_ShouldHandleCasesAndNulls(string? s, string? other, int expectedIndex, string method)
  {
    int actual = (method == "IndexOfOrdinal") ? s.IndexOfOrdinal(other) : s.IndexOfOrdinalNoCase(other);
    Assert.AreEqual(expectedIndex, actual);
  }

  [DataTestMethod]
  [DataRow("Document.docx", ".docx", true, "ContainsOrdinal", DisplayName = "Found Ordinal")]
  [DataRow("Document.DOCX", ".docx", false, "ContainsOrdinal", DisplayName = "Not Found Ordinal Case")]
  [DataRow("Document.DOCX", ".docx", true, "ContainsOrdinalNoCase", DisplayName = "Found Ordinal No Case")]
  [DataRow(null, "test", false, "ContainsOrdinal", DisplayName = "Input Null")]
  [DataRow("test", null, false, "ContainsOrdinal", DisplayName = "Search Null")]
  public void Contains_ShouldHandleCasesAndNulls(string? s, string? other, bool expected, string method)
  {
    bool actual = (method == "ContainsOrdinal") ? s.ContainsOrdinal(other) : s.ContainsOrdinalNoCase(other);
    Assert.AreEqual(expected, actual);
  }

  [DataTestMethod]
  [DataRow("abcdef", 3, "abc", DisplayName = "Normal Left")]
  [DataRow("abcdef", 10, "abcdef", DisplayName = "Count > Length Left")]
  [DataRow("abcdef", 0, "", DisplayName = "Count Zero Left")]
  [DataRow("", 5, "", DisplayName = "Empty String Left")]
  [DataRow(null, 5, null, DisplayName = "Null String Left")]
  public void TakeLeft_Count_ShouldHandleBoundaries(string? s, int count, string? expected)
  {
    Assert.AreEqual(expected, s.TakeLeft(count));
  }

  [DataTestMethod]
  [DataRow("path/to/file.txt", '/', false, "path", DisplayName = "TakeLeft Char (Exclude)")]
  [DataRow("path/to/file.txt", '/', true, "path/", DisplayName = "TakeLeft Char (Include)")]
  [DataRow("name=value&other=...", "=", false, "name", StringComparison.Ordinal, DisplayName = "TakeLeft String (Exclude)")]
  [DataRow("name=value&other=...", "=", true, "name=", StringComparison.Ordinal, DisplayName = "TakeLeft String (Include)")]
  [DataRow("nodels", ':', false, "", DisplayName = "Delimiter Not Found")]
  public void TakeLeft_Delimiter_ShouldWork(string? s, string delimiter, bool include, StringComparison comparisonType, string? expected)
  {
    // Use the string overload for simplicity in DataRow
    Assert.AreEqual(expected, s.TakeLeft(delimiter, include, comparisonType));
  }

  [DataTestMethod]
  [DataRow("abcdef", 3, "def", DisplayName = "Normal Right")]
  [DataRow("abcdef", 10, "abcdef", DisplayName = "Count > Length Right")]
  [DataRow("abcdef", 0, "", DisplayName = "Count Zero Right")]
  public void TakeRight_Count_ShouldHandleBoundaries(string? s, int count, string? expected)
  {
    Assert.AreEqual(expected, s.TakeRight(count));
  }

  [DataTestMethod]
  [DataRow("path.to.file", ".", false, "file", DisplayName = "TakeRight Last Delimiter (Exclude)")]
  [DataRow("path.to.file", ".", true, ".file", DisplayName = "TakeRight Last Delimiter (Include)")]
  [DataRow("path/to/file", "/", false, "file", DisplayName = "TakeRight Char (Exclude)")]
  [DataRow("path/to/file", "/", true, "/file", DisplayName = "TakeRight Char (Include)")]
  [DataRow("nodels", ':', false, "", DisplayName = "Delimiter Not Found")]
  public void TakeRight_Delimiter_ShouldWork(string? s, string delimiter, bool include, string? expected)
  {
    // Note: Must use the string overload with last delimiter logic
    Assert.AreEqual(expected, s.TakeRight(delimiter, include, StringComparison.Ordinal));
  }

  [DataTestMethod]
  [DataRow("{key}", "{", "}", false, "key", DisplayName = "TakeBetween Exclude")]
  [DataRow("{key}", "{", "}", true, "{key}", DisplayName = "TakeBetween Include")]
  [DataRow("prefix{key}suffix", "{", "}", false, "key", DisplayName = "TakeBetween with Prefix/Suffix")]
  [DataRow("key", "{", "}", false, "", DisplayName = "Delimiters Not Found")]
  [DataRow(null, "{", "}", false, null, DisplayName = "Input Null (Returns Null)")]
  public void TakeBetween_ShouldWork(string? s, string? left, string? right, bool include, string? expected)
  {
    Assert.AreEqual(expected, s.TakeBetween(left, right, include));
  }

  [DataTestMethod]
  [DataRow("test.email@example.com", ".,@", false, "testemailexamplecom", DisplayName = "Remove Any Ordinal")]
  [DataRow("test.Email@example.com", "E", true, "tst.mail@xampl.com", DisplayName = "Remove Any IgnoreCase")]
  [DataRow(null, ".,@", false, null, DisplayName = "Null String Input")]
  [DataRow("test", "", false, "test", DisplayName = "Empty Chars To Remove")]
  public void RemoveAny_ShouldRemoveCharacters(string? s, string charsToRemove, bool ignoreCase, string? expected)
  {
    // Testing the string overload that uses the ReadOnlySpan core method
    Assert.AreEqual(expected, s.RemoveAny(ignoreCase, charsToRemove));
  }

  [DataTestMethod]
  [DataRow("1", true, DisplayName = "Numeric True")]
  [DataRow("Y", true, DisplayName = "Case-Insensitive Y")]
  [DataRow("yes", true, DisplayName = "Lowercase Yes")]
  [DataRow("False", false, DisplayName = "False Keyword")]
  [DataRow("0", false, DisplayName = "Numeric False")]
  [DataRow(null, false, DisplayName = "Null Input")]
  public void IsTruthy_ShouldUseOrdinalNoCase(string? s, bool expected)
  {
    Assert.AreEqual(expected, s.IsTruthy());
  }

  [DataTestMethod]
  [DataRow("firstName", "First Name", DisplayName = "Camel Case")]
  [DataRow("PascalCase", "Pascal Case", DisplayName = "Pascal Case")]
  [DataRow("URLAddress", "URL Address", DisplayName = "Acronyms")]
  [DataRow("first NAME", "First NAME", DisplayName = "Partial Spacing")]
  [DataRow("myURLIsGood", "My URL Is Good", DisplayName = "Complex Mixed Case")]
  [DataRow(null, null, DisplayName = "Null Input")]
  [DataRow("", "", DisplayName = "Empty Input")]
  public void ToProperCaseSpaced_ShouldFormatCorrectly(string? s, string? expected)
  {
    Assert.AreEqual(expected, s.ToProperCaseSpaced());
  }

}
