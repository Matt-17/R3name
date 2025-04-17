using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using R3name.Modules.Attributes;

namespace R3name.Modules.Modificators;

[Modificator("Add leading numbers", "Adds leading numbers.")]
public class AddLeadingNumbers : Modificator
{
    public enum NumberBase
    {
        [Description("Binary")]
        Binary = 2,

        [Description("Octal")]
        Octal = 8,

        [Description("Decimal")]
        Decimal = 10,

        [Description("Hexadecimal")]
        Hexadecimal = 16
    }
    private int _count;

    [Display(Name = "Start with")]
    public int StartsWith { get; set; } = 1;

    [Display(Name = "Leading chars #")]
    [Numeric(Minimum = 1)]
    public int LeadingCharsCount { get; set; } = 1;

    [Display(Name = "Leading char")]
    public char LeadingChar { get; set; } = '0';

    [Display(Name = "Text before")]
    public string TextBefore { get; set; } = string.Empty;

    [Display(Name = "Text after")]
    public string TextAfter { get; set; } = " - ";

    [Display(Name = "Number base type")]
    [UseTwoLines]
    public NumberBase NumberBaseType { get; set; } = NumberBase.Decimal;


    [Display(Name = "Uppercase number")]
    public bool DoUppercaseNumbering { get; set; }

    public override void Initialize()
    {
        _count = StartsWith;
    }
        
    public override string ProcessFile(ModificatorContext context)
    {
        var formattedNumber = Convert.ToString(_count, (int)NumberBaseType);
        if (DoUppercaseNumbering)
            formattedNumber = formattedNumber.ToUpper();

        while (formattedNumber.Length < LeadingCharsCount)
            formattedNumber = LeadingChar + formattedNumber;

        _count++;
        return $"{TextBefore}{formattedNumber}{TextAfter}{context.Filename}";
    }
}