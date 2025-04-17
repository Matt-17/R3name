using R3name.Modules.Modificators;

namespace R3name.Tests.Modificators;

[TestClass]
public class SwapTests
{
    [TestMethod]
    public void TestSwapWithSeperator()
    {
        // Arrange
        var swap = new Swap
        {
            Seperator = "-"
        };

        // Act
        var result = swap.ProcessFile(new ModificatorContext("abc-def", null));

        // Assert
        Assert.AreEqual("def-abc", result);
    }

    [TestMethod]
    public void TestSwapWithoutSeperator()
    {
        // Arrange
        var swap = new Swap
        {
            Seperator = ""
        };

        // Act
        var result = swap.ProcessFile(new ModificatorContext("abc-def", null));

        // Assert
        Assert.AreEqual("abc-def", result);
    }
    [TestMethod]
    public void TestSwapWithNullSeperator()
    {
        // Arrange
        var swap = new Swap
        {
            Seperator = null
        };

        // Act
        var result = swap.ProcessFile(new ModificatorContext("abc-def", null));

        // Assert
        Assert.AreEqual("abc-def", result);
    }
    [DataTestMethod]
    [DataRow("test.txt", ".", 1, "txt.test")]
    [DataRow("test-file", "-", 1, "file-test")]
    [DataRow("test_file", "_", 1, "file_test")]
    [DataRow("test file", " ", 1, "file test")]
    [DataRow("test-file-1", "-", 2, "1-test-file")]
    public void ProcessFile_WithValidInput_ReturnsExpectedOutput(string filename, string seperator, int occurrence, string expected)
    {
        // Arrange
        var swap = new Swap { Seperator = seperator, Occurrence = occurrence };

        // Act
        var result = swap.ProcessFile(new ModificatorContext(filename, null));

        // Assert
        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow("test.txt", "", "test.txt")]
    [DataRow("test.txt", null, "test.txt")]
    [DataRow("test.txt", ",", "test.txt")]
    [DataRow("test-file-1.txt", ":", "test-file-1.txt")]
    public void ProcessFile_WithInvalidSeperator_ReturnsOriginalFilename(string filename, string seperator, string expected)
    {
        // Arrange
        var swap = new Swap { Seperator = seperator };

        // Act
        var result = swap.ProcessFile(new ModificatorContext(filename, null));

        // Assert
        Assert.AreEqual(expected, result);
    }

    // swaps partial 
    [DataTestMethod]
    [DataRow("test-file-1", "-", 2, true, "test-1-file")]        
    [DataRow("test-file-1", "-", 2, false, "1-test-file")]
    public void ProcessFile_WithPartialSwap_ReturnsExpectedOutput(string filename, string seperator, int occurrence, bool enablePartialSwap, string expected)
    {
        // Arrange
        var swap = new Swap { Seperator = seperator, Occurrence = occurrence, EnablePartialSwap = enablePartialSwap };

        // Act
        var result = swap.ProcessFile(new ModificatorContext(filename, null));

        // Assert
        Assert.AreEqual(expected, result);
    }
}