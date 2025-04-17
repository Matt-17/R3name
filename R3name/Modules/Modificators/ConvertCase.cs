using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using R3name.Modules.Attributes;

namespace R3name.Modules.Modificators;

[Modificator("Convert case", "Converts case first to UPPERCASE, lowercase, iNVERT cASE, AlTeRnAtInG CaSe and then to First case only or English Style Case.")]
class ConvertCase : Modificator
{
    public enum SecondaryCaseType
    {
        [Display(Name = "Leave case")]
        LeaveCase,
        [Display(Name = "First letter only")]
        FirstLetterOnly,
        [Display(Name = "English Style")]
        EnglishStyle,
    }

    public enum PrimaryCaseType
    {
        [Display(Name = "Leave case")]
        LeaveCase,
        [Display(Name = "lower case")]
        LowerCase,
        [Display(Name = "UPPER CASE")]
        UpperCase,
        [Display(Name = "iNVERT cASE")]
        InvertCase,
        [Display(Name = "AlTeRnAtInG CaSe")]
        AlternatingCase,
    }

    [Display(Name ="First change case to ...")]
    [UseTwoLines]
    public PrimaryCaseType PrimaryCase { get; set; }

    [Display(Name = "Then change case to ...")]
    [UseTwoLines]
    public SecondaryCaseType SecondaryCase { get; set; }

    public override string ProcessFile(ModificatorContext context)
    {
        var filename = context.Filename;
        filename = DoPrimaryCasing(filename);
        filename = DoSecondaryCasing(filename);
        return filename;
    }

    private string DoPrimaryCasing(string filename)
    {
        switch (PrimaryCase)
        {
            case PrimaryCaseType.LeaveCase:
                // Do nothing
                break;
            case PrimaryCaseType.LowerCase:
                filename = filename.ToLower();
                break;
            case PrimaryCaseType.UpperCase:
                filename = filename.ToUpper();
                break;
            case PrimaryCaseType.InvertCase:
                var invertResult = new StringBuilder(filename.Length);
                for (int i = 0; i < filename.Length; i++)
                {
                    var s = filename.Substring(i, 1);
                    if (s.ToLower() == s)
                        invertResult.Append(s.ToUpper());
                    else if (s.ToUpper() == s)
                        invertResult.Append(s.ToLower());
                }
                filename = invertResult.ToString();
                break;
            case PrimaryCaseType.AlternatingCase:
                var alternatingResult = new StringBuilder(filename.Length);
                for (int i = 0; i < filename.Length; i++)
                {
                    var s = filename.Substring(i, 1);
                    alternatingResult.Append(i % 2 == 0 ? s.ToUpper() : s.ToLower());
                }

                filename = alternatingResult.ToString();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return filename;
    }

    private string DoSecondaryCasing(string filename)
    {
        switch (SecondaryCase)
        {
            case SecondaryCaseType.LeaveCase:
                // Do nothing
                break;
            case SecondaryCaseType.FirstLetterOnly:
                var flResult = new StringBuilder(filename.Length);
                flResult.Append(filename.Substring(0, 1).ToUpper());
                flResult.Append(filename.Substring(1));
                filename = flResult.ToString();
                break;
            case SecondaryCaseType.EnglishStyle:
                var englishResult = new StringBuilder(filename.Length);
                var shouldBeUpper = true;
                for (int i = 0; i < filename.Length; i++)
                {
                    if (filename[i] == ' ')
                    {
                        englishResult.Append(' ');
                        shouldBeUpper = true;
                        continue;
                    }
                    var s = filename.Substring(i, 1);
                    if (shouldBeUpper)
                    {
                        englishResult.Append(s.ToUpper());
                        shouldBeUpper = false;
                    }
                    else
                    {
                        englishResult.Append(s);
                    }
                }
                filename = englishResult.ToString();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return filename;
    }
}