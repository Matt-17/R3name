using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using R3name.Modules.Attributes;

namespace R3name.Modules.Modificators;

[Modificator("Kill leading numbers", "Kills leading numbers. If a seperator is provided it kill only numbers if it's following.")]
class KillLeadingNumber : Modificator
{
    [Display(Name = "Only when followed by this")]
    [UseTwoLines]
    public string FollowedBy { get; set; }

    public override string ProcessFile(ModificatorContext context)
    {
        var followedBy = string.Empty;
        if (!string.IsNullOrEmpty(FollowedBy))
        {
            followedBy = Regex.Escape(FollowedBy);
        }

        return Regex.Replace(context.Filename, "^([0-9]*)" + followedBy, string.Empty);
    }
}