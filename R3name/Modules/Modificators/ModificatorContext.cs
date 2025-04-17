namespace R3name.Modules.Modificators;

public class ModificatorContext
{
    public ModificatorContext(string filename, string folder)
    {
        Filename = filename;
        Folder = folder;
    }

    public string Filename { get; set; }
    public string Folder { get; }
}