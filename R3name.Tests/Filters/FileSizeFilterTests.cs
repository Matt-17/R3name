using R3name.Models;
using R3name.Modules.FileSources;
using R3name.Modules.Filters;

namespace R3name.Tests.Filters;

[TestClass]
public class FileSizeFilterTests
{
    [TestMethod]
    public void TestFilter_ReturnsTrue_WhenFileSizeIsWithinRange()
    {
        // Arrange
        var file = new FileDescriptionInternal("file.txt", 100);

        var args = new ModuleArgs();

        var filter = new FileSizeFilter
        {
            MinSize = 0,
            MaxSize = 1000
        };

        // Act
        var result = filter.Filter(file, args);

        // Assert
        Assert.IsTrue(result);
    }
    [TestMethod]
    public void TestFilter_ReturnsFalse_WhenFileSizeIsBelowMinimum()
    {
        // Arrange
        var file = new FileDescriptionInternal("file.txt", 99);

        var args = new ModuleArgs();

        var filter = new FileSizeFilter
        {
            MinSize = 100,
            MaxSize = 1000
        };

        // Act
        var result = filter.Filter(file, args);

        // Assert
        Assert.IsFalse(result);
    }
    [TestMethod]
    public void TestFilter_ReturnsFalse_WhenFileSizeIsAboveMaximum()
    {
        // Arrange
        var file = new FileDescriptionInternal("file.txt", 1001);

        var args = new ModuleArgs();

        var filter = new FileSizeFilter
        {
            MinSize = 0,
            MaxSize = 1000
        };

        // Act
        var result = filter.Filter(file, args);

        // Assert
        Assert.IsFalse(result);
    }
    [TestMethod]
    public void TestFilter_ReturnsTrue_WhenFileSizeIsEqualToMinimumAndMaximum()
    {
        // Arrange
        var file = new FileDescriptionInternal("file.txt", 100);

        var args = new ModuleArgs();

        var filter = new FileSizeFilter();
        filter.MinSize = 100;
        filter.MaxSize = 100;

        // Act
        var result = filter.Filter(file, args);

        // Assert
        Assert.IsTrue(result);
    }
    [TestMethod]
    public void TestFilter_IgnoreSize()
    {
        // Arrange
        var file = new FileDescriptionInternal("file.txt", 100);

        var args = new ModuleArgs();

        var filter = new FileSizeFilter();
        filter.MinSize = 0;
        filter.MaxSize = 0;

        // Act
        var result = filter.Filter(file, args);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestFilter_IgnoreMaxSizeButMinSize()
    {
        // Arrange
        var file = new FileDescriptionInternal("file.txt", 100);

        var args = new ModuleArgs();

        var filter = new FileSizeFilter();
        filter.MinSize = 200;
        filter.MaxSize = 0;

        // Act
        var result = filter.Filter(file, args);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void TestFilter_IgnoreMaxSize()
    {
        // Arrange
        var file = new FileDescriptionInternal("file.txt", 100);

        var args = new ModuleArgs();

        var filter = new FileSizeFilter();
        filter.MinSize = 50;
        filter.MaxSize = 0;

        // Act
        var result = filter.Filter(file, args);

        // Assert
        Assert.IsTrue(result);
    }
}