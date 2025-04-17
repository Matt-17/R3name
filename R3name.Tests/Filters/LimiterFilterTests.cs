using R3name.Models;
using R3name.Modules.FileSources;
using R3name.Modules.Filters;

namespace R3name.Tests.Filters;

[TestClass]
public class LimiterFilterTests
{
    private ModuleArgs args;
    private FileDescriptionInternal file;

    [TestInitialize]
    public void TestInitialize()
    {
        file = new FileDescriptionInternal("file.txt", 0);
        args = new ModuleArgs();
    }

    [TestMethod]
    public void TestFilter_ReturnsTrue_WhenFileIsNotFiltered()
    {
        var filter = new LimiterFilter
        {
            SkipCount = 0,
            LimitCount = 0,
            TakeEveryCount = 1
        };

        // Act
        var result = filter.Filter(file, args);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestFilter_ReturnsFalse_WhenFileIsFiltered()
    {
        var filter = new LimiterFilter
        {
            SkipCount = 1,
            LimitCount = 0,
            TakeEveryCount = 1
        };

        // Act
        var result = filter.Filter(file, args);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void TestFilter_ReturnsTrueForEveryNTimes_WhenTakeEveryCountIsNAndSkipCountIs0()
    {
        var filter = new LimiterFilter
        {
            SkipCount = 0,
            LimitCount = 0,
            TakeEveryCount = 2
        };

        // Act
        var results = new[]
        {
            filter.Filter(file, args),
            filter.Filter(file, args),
            filter.Filter(file, args),
            filter.Filter(file, args)
        };

        // Assert
        CollectionAssert.AreEqual(new[]
        {
            true,
            false,
            true,
            false
        }, results);
    }

    [TestMethod]
    public void TestFilter_ReturnsFalseForEveryNTimes_WhenTakeEveryCountIsNAndSkipCountIsGreaterThan0()
    {
        var filter = new LimiterFilter
        {
            SkipCount = 1,
            LimitCount = 0,
            TakeEveryCount = 2
        };

        // Act
        var results = new[]
        {
            filter.Filter(file, args),
            filter.Filter(file, args),
            filter.Filter(file, args),
            filter.Filter(file, args)
        };

        // Assert
        CollectionAssert.AreEqual(new[]
        {
            false,
            true,
            false,
            true
        }, results);
    }

    [TestMethod]
    public void TestFilter_ReturnsTrueForFirstNFiles_WhenTakeEveryCountIs1AndLimitCountIsN()
    {
        var filter = new LimiterFilter
        {
            SkipCount = 0,
            LimitCount = 2,
            TakeEveryCount = 1
        };

        // Act
        var results = new[]
        {
            filter.Filter(file, args),
            filter.Filter(file, args),
            filter.Filter(file, args),
            filter.Filter(file, args)
        };

        // Assert
        CollectionAssert.AreEqual(new[]
        {
            true,
            true,
            false,
            false
        }, results);
    }

    [TestMethod]
    public void TestFilter_ReturnsTrueForAllFiles_WhenTakeEveryCountIs1AndLimitCountIs0()
    {
        var filter = new LimiterFilter
        {
            SkipCount = 0,
            LimitCount = 0,
            TakeEveryCount = 1
        };

        // Act
        var results = new[]
        {
            filter.Filter(file, args),
            filter.Filter(file, args),
            filter.Filter(file, args),
            filter.Filter(file, args)
        };

        // Assert
        CollectionAssert.AreEqual(new[]
        {
            true,
            true,
            true,
            true
        }, results);
    }
}