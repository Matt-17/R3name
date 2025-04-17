using R3name.Modules.Modificators;

namespace R3name.Tests.Modificators;

[TestClass]
public class AddLeadingNumbersTests
{
    private AddLeadingNumbers _modificator = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        // Arrange
        _modificator = new AddLeadingNumbers
        {
            LeadingChar = '0',
            LeadingCharsCount = 2,
            TextBefore = "",
            TextAfter = " - ",
            DoUppercaseNumbering = false,
        };
        _modificator.Initialize();
    }

    [TestMethod]
    [DataRow("test.txt", "01 - test.txt", DisplayName = "Starts with 1, leading char 0, text before and after")]
    public void ProcessFile_ReturnsExpectedResult(string input, string expectedOutput)
    {
        // Act
        var result = _modificator.ProcessFile(new ModificatorContext(input, null));

        // Assert
        Assert.AreEqual(expectedOutput, result);
    }
}
