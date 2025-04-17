using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

using R3name.Models;
using R3name.Models.Interfaces;
using R3name.Modules;
using R3name.Modules.FileSources;
using R3name.Modules.Filters;
using R3name.Modules.Groupers;
using R3name.Modules.Modificators;
using R3name.Modules.Sorters;
using R3name.Serialization;
using R3name.Views;

using YamlDotNet.Serialization;

using Settings = R3name.Properties.Settings;

namespace R3name.ViewModels;

public class MainViewModel : BaseViewModel, IModuleObserver
{
    private FileSource _fileSource;

    public MainViewModel()
    {
        // right now, LocalFolderSource is the only file source and should be used as default
        FileSource = new LocalFolderSource();

        RefreshCommand = Command(RefreshFiles);
        RefreshNamesCommand = Command(Refresh);
        RenameCommand = Command(Rename);

        //SaveConfigurationCommand = Command(SaveConfiguration);
        //LoadConfigurationCommand = Command(LoadConfiguration);
        AboutCommand = Command(About);
        ExitCommand = Command(Exit);
        ChangeFileSourceCommand = Command(ChangeFileSource);
        AddProcessorCommand = Command(AddProcessor);
        AddModificatorCommand = Command(AddModificator);
        ManageConfigurationsCommand = Command(ManageConfigurations);

        ClearCommand = Command(Clear);
        SaveConfigurationCommand = Command(SaveConfiguration);
        LoadConfigurationCommand = Command<string>(LoadConfiguration);

        Observer.Manager.Subscribe(this);

        LoadAvailableConfigurations();
    }

    private void Clear()
    {
        Modificators.Clear();
    }

    public ICommand ClearCommand { get; }

    public ObservableCollection<ConfigurationFile> AvailableConfigurations { get; set; } = [];
    private void LoadAvailableConfigurations()
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        folder = Path.Combine(folder, "R3name");
        if (!Directory.Exists(folder))
            return;
        var files = Directory.GetFiles(folder, "*.yaml");
        AvailableConfigurations.Clear();
        foreach (var file in files)
        {
            var fileContent = File.ReadAllText(file);
            var context = GetNameFromYaml(fileContent);

            AvailableConfigurations.Add(new ConfigurationFile(file, context));
        }
    }

    private void LoadConfiguration(string obj)
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        folder = Path.Combine(folder, "R3name");
        var file = Path.Combine(folder, obj);
        if (!File.Exists(file))
            return;
        var yaml = File.ReadAllText(file);
        var mods = DeserializeFromYaml(yaml);
        Modificators.Clear();
        foreach (var mod in mods)
        {
            var viewModel = ModuleViewModel.Create(this, mod);
            Modificators.Add(viewModel);
        }

        Refresh();

    }

    private void SaveConfiguration()
    {
        var ask = new AskForNameWindow
        {
            Owner = Application.Current.MainWindow
        };

        if (!ask.ShowDialog() ?? false)
            return;

        var name = ask.ConfigurationName;
        if (string.IsNullOrEmpty(name))
            return;

        var yaml = Yaml(name);

        // environment special path for app data then R3name then clean lower case name of name
        var filename = name.ToLowerInvariant().Replace(" ", "_");
        filename = Path.ChangeExtension(filename, ".yaml");
        var configurationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "R3name", filename);
        Directory.CreateDirectory(Path.GetDirectoryName(configurationPath)!);

        File.WriteAllText(configurationPath, yaml);

        LoadAvailableConfigurations();
    }


    public FileViewModel Selected { get; set; }
    public ObservableCollection<FileViewModel> Files { get; } = [];
    public ObservableCollection<ModuleViewModel> Modificators { get; } = [];
    public ObservableCollection<ModuleViewModel> FileProcessors { get; } = [];

    public FileSource FileSource
    {
        get => _fileSource;
        set
        {
            if (Equals(value, _fileSource))
                return;

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (_fileSource != null)
                _fileSource.Changed -= FileSourceChanged;
            _fileSource = value;
            _fileSource.Changed += FileSourceChanged;
            OnPropertyChanged();
        }
    }

    private void FileSourceChanged(object sender, RefreshNeededEventHandlerArgs e)
    {
        Application.Current.Dispatcher.Invoke(RefreshFiles);
    }

    public ICommand AboutCommand { get; }
    public ICommand ExitCommand { get; }
    public ICommand ChangeFileSourceCommand { get; }
    public ICommand AddProcessorCommand { get; }
    public ICommand AddModificatorCommand { get; }
    public ICommand ManageConfigurationsCommand { get; }
    public ICommand SaveConfigurationCommand { get; }
    public ICommand LoadConfigurationCommand { get; }

    public ICommand RenameCommand { get; }

    public ICommand RefreshNamesCommand { get; }

    public ICommand RefreshCommand { get; }

    private void About() { /* Add code here */ }
    private void Exit() { /* Add code here */ }
    private void ManageConfigurations()
    {
    }

    public void Refresh()
    {
        var modificators = GetModificators().ToList();

        foreach (var mod in modificators)
        {
            mod.Initialize();
        }
        foreach (var file in Files)
        {
            file.FilenameNew = file.FilenameOld;
        }
        foreach (var file in Files)
        {
            foreach (var mod in modificators)
            {
                var input = Path.GetFileNameWithoutExtension(file.FilenameNew);
                var context = new ModificatorContext(input, file.Folder);
                var result = mod.ProcessFile(context);
                result = result.Replace("$text", file.Text);
                result = result.Replace("$folder", GetParentFolderName(file.Folder, 0));
                file.FilenameNew = result + Path.GetExtension(file.FilenameNew);
            }
        }
    }

    private string GetParentFolderName(string path, int level)
    {
        if (level == 0)
        {
            return Path.GetFileName(path);
        }
        return GetParentFolderName(Path.GetDirectoryName(path), level - 1);
    }

    private void AddModificator()
    {
        var vm = new SelectModuleViewModel();
        vm.LoadModules<Modificator>("Modificators");

        if (WindowService.Instance.ShowSelectModuleDialog(vm))
        {
            var module = vm.Selected.CreateModuleInstance<Modificator>();
            var viewModel = ModuleViewModel.Create(this, module);
            Modificators.Add(viewModel);
            Refresh();
        }
    }

    private void AddProcessor()
    {
        var vm = new SelectModuleViewModel();
        vm.LoadModules<SortProcessor>("Sort processors");
        vm.LoadModules<FilterProcessor>("Filter processors");
        vm.LoadModules<GroupProcessor>("Group processors");

        if (WindowService.Instance.ShowSelectModuleDialog(vm))
        {
            var module = vm.Selected.CreateModuleInstance<Processor>();
            var viewModel = ModuleViewModel.Create(this, module);
            FileProcessors.Add(viewModel);
            Refresh();
        }
    }

    private void ChangeFileSource()
    {
        var vm = new SelectModuleViewModel();
        vm.LoadModules<FileSource>("File sources");

        if (WindowService.Instance.ShowSelectModuleDialog(vm))
        {
            var module = vm.Selected.CreateModuleInstance<FileSource>();
            FileSource = module;
        }
    }


    private void Rename()
    {
        foreach (var file in Files)
        {
            Debug.WriteLine("Rename " + file.PathOriginal + " --> " + Path.Combine(file.Folder, file.FilenameNew));

            if (File.Exists(Path.Combine(file.Folder, file.FilenameNew)))
            {
                continue;
            }

            File.Move(file.PathOriginal, Path.Combine(file.Folder, file.FilenameNew));
        }

        RefreshFiles();
    }
    public void Insert()
    {
        const int spalte = 2;
        var text = Clipboard.GetText();
        if (string.IsNullOrEmpty(text))
            return;

        var zeilen = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var hatTabZeilen = zeilen.Any(z => z.Contains('\t'));

        var items = zeilen
            .Select(zeile => zeile.Split('\t'))
            .Where(spalten => spalten.Length > spalte || (!hatTabZeilen && spalten.Length > 0))
            .Select(spalten => spalten.Length > spalte ? spalten[spalte]?.Trim() : null)
            .Where(val => val != null)
            .ToArray();

        var current = 0;
        var go = false;
        foreach (var datei in Files)
        {
            if (Selected == datei)
                go = true;

            if (!go)
                continue;

            if (current >= items.Length)
                return;

            datei.Text = items[current];
            current++;
        }

        Refresh();
    }


    private IEnumerable<Modificator> GetModificators()
    {
        foreach (var modificator in Modificators)
        {
            if (modificator.IsDeactivated)
                continue;
            yield return (Modificator)modificator.Module;
        }
    }

    private IEnumerable<Processor> GetProcessors()
    {
        foreach (var modificator in FileProcessors)
        {
            if (modificator.IsDeactivated)
                continue;
            yield return (Processor)modificator.Module;
        }
    }

    public void Process()
    {
        foreach (var file in Files)
        {
            if (!file.IsChanged)
                continue;

            File.Move(file.PathOriginal, file.GetRequestedPath());
        }
    }

    public void RefreshFiles()
    {
        Files.Clear();

        var sourceFiles = FileSource.GetFiles();

        var list = new List<FileDescriptionInternal>();

        foreach (var sourceFile in sourceFiles)
        {
            var fileDescription = sourceFile.CreateDescriptor();

            // Add text input

            list.Add(fileDescription);
        }

        IEnumerable<IFileDescription> files = list;

        var args = new ModuleArgs();
        foreach (var processor in GetProcessors())
        {
            files = processor.Process(files, args);
        }

        Files.Clear();

        foreach (var fileDescription in files.Cast<FileDescriptionInternal>())
        {
            var fileViewModel = new FileViewModel(fileDescription);
            Files.Add(fileViewModel);
        }

        Refresh();
    }
    private string Yaml(string name = null)
    {
        var serializationContext = new SerializationContext()
        {
            Name = name,
            Modificators = Modificators
                .Select(m => m.Module)
                .OfType<Modificator>()
                .Select(ConvertToSerializable)
                .ToList()
        };

        var serializer = new SerializerBuilder()
            .Build();

        var serialize = serializer.Serialize(serializationContext);
        return serialize;
    }

    private SerializableModule ConvertToSerializable(Modificator modificator)
    {
        var type = modificator.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead);

        var settings = properties.ToDictionary(
            prop => prop.Name,
            prop => prop.GetValue(modificator)
        );

        return new SerializableModule
        {
            Type = type.Name,
            Deactivated = modificator.IsDeactivated ? true : null,
            Settings = settings
        };
    }
    private Modificator ConvertFromSerializable(SerializableModule module)
    {
        var type = Assembly.GetExecutingAssembly()
            .GetTypes()
            .FirstOrDefault(t => t.Name == module.Type && typeof(Modificator).IsAssignableFrom(t));

        if (type == null)
            throw new InvalidOperationException($"Unknown module type: {module.Type}");

        var instance = (Modificator)Activator.CreateInstance(type)!;

        foreach (var (key, value) in module.Settings)
        {
            var prop = type.GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
            {
                if (prop.PropertyType.IsEnum && value is not null)
                {
                    prop.SetValue(instance, Enum.Parse(prop.PropertyType, value.ToString()!));
                }
                else
                {
                    prop.SetValue(instance, Convert.ChangeType(value, prop.PropertyType));
                }

            }
        }

        instance.IsDeactivated = module.Deactivated ?? false;

        return instance;
    }
    private string GetNameFromYaml(string yaml)
    {
        var deserializer = new DeserializerBuilder()
            .WithTagMapping("!Modificator", typeof(Modificator))
            .WithAttemptingUnquotedStringTypeDeserialization()
            .Build();

        var serializationContext = deserializer.Deserialize<SerializationContext>(yaml);

        return serializationContext.Name;
    }
    private List<Modificator> DeserializeFromYaml(string yaml)
    {
        var deserializer = new DeserializerBuilder()
            .WithTagMapping("!Modificator", typeof(Modificator))
            .WithAttemptingUnquotedStringTypeDeserialization()
            .Build();

        var serializationContext = deserializer.Deserialize<SerializationContext>(yaml);

        return serializationContext.Modificators?
            .Select(ConvertFromSerializable)
            .ToList() ?? [];
    }


    public void LoadSettings()
    {
        var yaml = Settings.Default.CurrentSettings; // Angenommen, diese Einstellung liefert jetzt YAML statt JSON
        if (string.IsNullOrEmpty(yaml))
            return;

        var mods = DeserializeFromYaml(yaml);
        Modificators.Clear();
        foreach (var mod in mods)
        {
            var viewModel = ModuleViewModel.Create(this, mod);
            Modificators.Add(viewModel);
        }
    }

    public void SaveSettings()
    {
        Settings.Default.CurrentSettings = Yaml();
        Settings.Default.Save();
    }

    public void GoWithArguments(string[] args)
    {
        if (args.Length == 0)
            return;
        var file = args[0];
        if (!Directory.Exists(file))
            return;                     
        FileSource.SetValue(file);
    }
}