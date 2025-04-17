using System.Text;
using R3name.Modules.Attributes;

namespace R3name.Modules.Modificators;

[Modificator("Insert/Append text", "Useful to insert and append text")]
class InsertAppendText : Modificator
{
    public string Text { get; set; }

    public int Position { get; set; }

    public bool DoOverlay { get; set; }

    public string FillupText { get; set; }

    public override string ProcessFile(ModificatorContext context)
    {
        if (string.IsNullOrEmpty(Text))
            return context.Filename;

        var startIndex = Position;
        var sb = new StringBuilder();

        if (startIndex >= context.Filename.Length)
            context.Filename = Fillup(context.Filename, startIndex);

        if (startIndex > context.Filename.Length)
            startIndex = context.Filename.Length;

        if (startIndex < 0)
            startIndex = context.Filename.Length + startIndex + 1;

        if (startIndex < 0)
            startIndex = 0;

        sb.Append(context.Filename[..startIndex]);

        foreach (var t in Text)
        {
            sb.Append(t);
            if (DoOverlay)
            {
                startIndex++;
            }
        }

        for (var i = startIndex; i < context.Filename.Length; i++)
        {
            sb.Append(context.Filename[i]);
        }

        return sb.ToString();
    }

    private string Fillup(string s, int desiredLength)
    {
        if (string.IsNullOrEmpty(FillupText))
            return s;

        var sb = new StringBuilder(desiredLength);
        sb.Append(s);
        for (int i = 0; i < desiredLength - s.Length; i++)
        {
            sb.Append(FillupText[i % FillupText.Length]);
        }

        return sb.ToString();
    }
}