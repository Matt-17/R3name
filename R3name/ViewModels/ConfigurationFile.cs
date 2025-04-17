namespace R3name.ViewModels;

public class ConfigurationFile(string file, string name)
{
    public string File { get; } = file;
    public string Name { get; } = name;
}