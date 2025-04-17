using System.ComponentModel.DataAnnotations;
using R3name.Models;
using R3name.Modules.Attributes;
using R3name.Modules.FileSources;

namespace R3name.Modules.Filters;

[Modificator("File limiting", "File limiting")]
public class LimiterFilter : FilterProcessor
{
    private int _count;

    [Display(Name = "Skip file count")]
    [Numeric(Minimum = 0)]
    public int SkipCount { get; set; }

    [Display(Name = "Limit file count")]
    [Numeric(Minimum = 0)]
    public int LimitCount { get; set; }

    [Display(Name = "Take every # count")]
    [Numeric(Minimum = 1)]
    public int TakeEveryCount { get; set; }

    protected override void OnBeforeProcess()
    {
        _count = 0;
    }

    public override bool Filter(IFileDescription file, ModuleArgs args)
    {
        // ReSharper disable once ReplaceWithSingleAssignment.True
        var result = true;

        if (result && SkipCount > 0 && _count < SkipCount)
        {
            result = false;
        }

        if (result && LimitCount > 0 && _count >= LimitCount + SkipCount)
        {
            result = false;
        }

        if (result && TakeEveryCount > 1)
        {
            if ((_count - SkipCount) % TakeEveryCount != 0)
                result = false;
        }

        _count++;
        return result;
    }
}