using R3name.Models;
using R3name.Modules.FileSources;
using R3name.Modules.Filters;

namespace R3name.Tests.Filters;

[TestClass]
public class RegexFilterTests
{
    [TestMethod]
    public void TestFilter_ReturnsTrue_WhenFileNameMatchesRegexPattern()
    {
        // Arrange
        var file = new FileDescriptionInternal("file.txt", 0);
        var args = new ModuleArgs();
        var filter = new RegexFilter
        {
            Pattern = @".*\.txt$",
            IgnoreCase = false
        };

        // Act
        var result = filter.Filter(file, args);

        // Assert
        Assert.IsTrue(result);
    }
    [TestMethod]
    public void TestFilter_ReturnsFalse_WhenPatternIsEmpty()
    {
        // Arrange
        var file = new FileDescriptionInternal("file.txt", 0);
        var args = new ModuleArgs();

        var filter = new RegexFilter();
        filter.Pattern = "";
        filter.IgnoreCase = false;

        // Act
        var result = filter.Filter(file, args);

        // Assert
        Assert.IsFalse(result);
    }
    [TestMethod]
    public void TestFilter_ReturnsTrue_WhenIgnoreCaseIsTrueAndFileNameMatchesRegexPattern()
    {
        // Arrange
        var file = new FileDescriptionInternal("file.txt", 0);
        var args = new ModuleArgs();

        var filter = new RegexFilter();
        filter.Pattern = @"\.txt$";
        filter.IgnoreCase = true;

        // Act
        var result = filter.Filter(file, args);

        // Assert
        Assert.IsTrue(result);
    }
    [TestMethod]
    public void TestFilter_ReturnsTrue_WhenIgnoreCaseIsTrueAndFileNameDoesNotMatchRegexPatternDueToCase()
    {
        // Arrange
        var file = new FileDescriptionInternal("FILE.TXT", 0);
        var args = new ModuleArgs();

        var filter = new RegexFilter();
        filter.Pattern = @"\.txt$";
        filter.IgnoreCase = true;

        // Act
        var result = filter.Filter(file, args);

        // Assert
        Assert.IsTrue(result);
    }
    [TestMethod]
    public void TestFilter_ReturnsFalse_WhenIgnoreCaseIsFalseAndFileNameDoesNotMatchRegexPatternDueToCase()
    {
        // Arrange
        var file = new FileDescriptionInternal("FILE.TXT", 0);
        var args = new ModuleArgs();

        var filter = new RegexFilter();
        filter.Pattern = @"\.txt$";
        filter.IgnoreCase = false;

        // Act
        var result = filter.Filter(file, args);

        // Assert
        Assert.IsFalse(result);
    }

}