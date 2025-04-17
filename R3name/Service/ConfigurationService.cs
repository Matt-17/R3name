using System;
using System.Collections.Generic;

namespace R3name.Service;

internal class ConfigurationService
{
    // method to list all configuration files inin the folder
    public List<string> ListConfigurationFiles()
    {
        var files = new List<string>();

        // folder MyDocuments
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        // folder R3name
        var r3nameFolder = System.IO.Path.Combine(folder, "R3name");
        // folder Configuration
        var configurationFolder = System.IO.Path.Combine(r3nameFolder, "Configuration");
        // all files in the folder as descripbed above   (*.yaml; *.yml, *.r3n, *.r3name) 
        var allFiles = System.IO.Directory.GetFiles(configurationFolder, "*.*", System.IO.SearchOption.TopDirectoryOnly);
        foreach (var file in allFiles)
        {
            var extension = System.IO.Path.GetExtension(file);



            if (extension is ".yaml" or ".yml" or ".r3n" or ".r3name")
            {
                files.Add(file);
            }
        }

        return files;
    }

    // method to read the configuration file
    public string ReadConfigurationFile(string fileName)
    {
        var content = string.Empty;
        if (System.IO.File.Exists(fileName))
        {
            content = System.IO.File.ReadAllText(fileName);
        }
        return content;
    }
    // method to write the configuration file


}