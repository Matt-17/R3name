using R3name.Models;
using R3name.Modules.FileSources;
using R3name.Modules.Filters;

namespace R3name.Tests.Filters;

[TestClass]
public class FileExtensionFilterTests
{
    [TestMethod]
    public void TestFilter_ReturnsTrue_WhenFileExtensionIsAllowed()
    {
        // Arrange
        var file = new FileDescriptionInternal("file.txt", 0);
        var args = new ModuleArgs();

        var filter = new FileExtensionFilter();
        filter.Extensions = ".txt;.doc;.docx";

        // Act
        var result = filter.Filter(file, args);

        // Assert
        Assert.IsTrue(result);
    }
    [TestMethod]
    public void TestFilter_ReturnsFalse_WhenFileExtensionIsNotAllowed()
    {
        // Arrange
        var file = new FileDescriptionInternal("file.exe", 0);
        var args = new ModuleArgs();

        var filter = new FileExtensionFilter();
        filter.Extensions = ".txt;.doc;.docx";

        // Act
        var result = filter.Filter(file, args);

        // Assert
        Assert.IsFalse(result);
    }

}