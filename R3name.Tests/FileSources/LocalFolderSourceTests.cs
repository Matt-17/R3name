using R3name.Modules.FileSources;

namespace R3name.Tests.FileSources;

[TestClass]
public class LocalFolderSourceTests
{
    private LocalFolderSource _localFolderSource = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _localFolderSource = new LocalFolderSource();
    }

    [TestMethod]
    public void TestIsValidFilename()
    {
        Assert.IsTrue(_localFolderSource.IsValidFilename("valid.txt"));
        Assert.IsFalse(_localFolderSource.IsValidFilename("invalid:file.txt"));
        Assert.IsFalse(_localFolderSource.IsValidFilename("invalid/file.txt"));
        Assert.IsFalse(_localFolderSource.IsValidFilename("invalid\\file.txt"));
        Assert.IsFalse(_localFolderSource.IsValidFilename("invalid?file.txt"));
        Assert.IsFalse(_localFolderSource.IsValidFilename(""));
        Assert.IsFalse(_localFolderSource.IsValidFilename(null));
    }

    [TestMethod]
    public void TestFindDuplicates()
    {
        var filenames = new[]
        {
            "file1.txt",
            "file2.txt",
            "file1.txt",
            "file3.txt"
        };
        var expectedDuplicates = new[]
        {
            "file1.txt"
        };

        var duplicates = _localFolderSource.FindDuplicates(filenames);
        CollectionAssert.AreEqual(expectedDuplicates, duplicates);
    }

}