using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using R3name.Models;
using R3name.Models.Enums;
using R3name.Modules.Attributes;
using R3name.Modules.Modificators;

namespace R3name.Controls;

public class PropertyPanel : UserControl
{
    private readonly StackPanel _panel;

    public PropertyPanel()
    {
        Content = _panel = new StackPanel();
    }

    public object Module
    {
        get => GetValue(ModuleProperty);
        set => SetValue(ModuleProperty, value);
    }

    // Using a DependencyProperty as the backing store for Module.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ModuleProperty =
        DependencyProperty.Register(nameof(Module), typeof(object), typeof(PropertyPanel), new PropertyMetadata(null, SetModule));

    private static void SetModule(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var panel = (PropertyPanel)d;
        panel.SetModule(e.NewValue);
    }

    private void SetModule(object model)
    {
        _panel.Children.Clear();

        if (model == null)
            return;

        var type = model.GetType();
        BuildPropertyFields(type);
    }

    public void BuildPropertyFields(Type modelType)
    {
        var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (ShouldIgnoreProperty(property))
                continue;

            var element = BuildPropertyField(property);

            _panel.Children.Add(element);
        }
        if (_panel.Children.Count == 0)
        {
            Visibility = Visibility.Collapsed;
        }
    }

    private static bool ShouldIgnoreProperty(PropertyInfo property)
    {
        var ignore = property.GetCustomAttribute<IgnoreAttribute>();
        var b = ignore != null;
        return b;
    }

    private void NotifyObserver()
    {
        if (Module is Modificator)
        {
            Observer.Manager.NotifyModificatorChange();
        }
    }

    private UIElement BuildPropertyField(PropertyInfo property)
    {
        var type = property.PropertyType;

        if (type == typeof(int))
            return CreateNumericValue(property);

        if (type == typeof(string))
            return CreateStringValue(property);

        if (type == typeof(char))
            return CreateCharValue(property);

        if (type == typeof(bool))
            return CreateBoolValue(property);

        if (type.IsSubclassOf(typeof(Enum)))
            return CreateEnumValue(property);

        if (type == typeof(DateTime))
            return CreateDateValue(property);

        throw new ArgumentOutOfRangeException();
    }
    private UIElement CreateDateValue(PropertyInfo property)
    {
        var box = new TextBox();
        var value = property.GetValue(Module);
        if (value != null)
        {
            box.Text = value.ToString()!;
        }
        box.TextChanged += (sender, e) =>
        {
            var boxText = box.Text;
            var date = DateTime.Parse(boxText);
            property.SetValue(Module, date);
            NotifyObserver();
        };
        return BuildFieldContainer(property, box);
    }
    private UIElement CreateEnumValue(PropertyInfo property)
    {
        var box = new ComboBox
        {
            DisplayMemberPath = "Name",
            SelectedValuePath = "Value"
        };

        var enumTypes = GetValues(property.PropertyType);
        box.ItemsSource = enumTypes;

        var value = property.GetValue(Module);
        if (value != null)
            box.SelectedValue = value;
        box.SelectionChanged += (sender, e) =>
        {
            property.SetValue(Module, box.SelectedValue);
            NotifyObserver();
        };
        return BuildFieldContainer(property, box);
    }

    private UIElement BuildFieldContainer(PropertyInfo property, UIElement innerElement)
    {
        var propertyName = property.Name;

        var display = property.GetCustomAttribute<DisplayAttribute>();
        if (display != null)
        {
            if (display.Name != null)
            {
                propertyName = display.Name;
            }
        }

        var grid = new Grid();
        var useTwoLines = property.GetCustomAttribute<UseTwoLinesAttribute>();
        if (useTwoLines != null)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(innerElement, 1);
        }
        else
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetColumn(innerElement, 1);
        }


        var block = new TextBlock
        {
            Text = propertyName,
            FontSize = 13,
            Foreground = Brushes.White,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(2)
        };

        grid.Children.Add(block);
        grid.Children.Add(innerElement);


        return grid;
    }

    private UIElement CreateBoolValue(PropertyInfo property)
    {
        var box = new CheckBox
        {
            Foreground = Brushes.White
        };
        var propertyName = property.Name;

        var display = property.GetCustomAttribute<DisplayAttribute>();
        if (display?.Name != null)
        {
            propertyName = display.Name;
        }


        box.Content = propertyName;

        var value = property.GetValue(Module);
        if (value != null)
            box.IsChecked = (bool)value;

        void Handler(object sender, RoutedEventArgs e)
        {
            property.SetValue(Module, box.IsChecked);
            NotifyObserver();
        }

        box.Checked += Handler;
        box.Unchecked += Handler;
        return box;
    }
    private UIElement CreateCharValue(PropertyInfo property)
    {
        var box = new TextBox
        {
            MaxLength = 1,
            FontSize = 15,
            TextAlignment = TextAlignment.Right,
            IsReadOnly = true,
            FontWeight = FontWeights.SemiBold,
            Width = 30,
            Padding = new Thickness(4, 0, 4, 0),
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right

        };
        box.PreviewTextInput += TextBox_PreviewTextInput;
        box.SelectionChanged += TextBox_SelectionChanged;

        var value = property.GetValue(Module);
        if (value != null)
            box.Text = value.ToString() ?? " ";
        box.TextChanged += (sender, e) =>
        {
            property.SetValue(Module, box.Text[0]);
            NotifyObserver();
        };
        return BuildFieldContainer(property, box);
    }

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        ((TextBox)sender).Text = e.Text;
    }

    private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        if (textBox.SelectionLength > 0)
            textBox.Select(0, 0);
    }

    private UIElement CreateStringValue(PropertyInfo property)
    {
        var box = new TextBox();
        var value = property.GetValue(Module);
        if (value != null)
            box.Text = (string)value;
        box.TextChanged += (sender, e) =>
        {
            property.SetValue(Module, box.Text);
            NotifyObserver();
        };
        return BuildFieldContainer(property, box);
    }

    private UIElement CreateNumericValue(PropertyInfo property)
    {
        var box = new NumericUpDown();

        var range = property.GetCustomAttribute<NumericAttribute>();
        if (range != null)
        {
            box.Minimum = range.Minimum;
            box.Maximum = range.Maximum;
            box.Step = range.Step;
        }

        var value = property.GetValue(Module);
        if (value != null)
            box.Value = (int)value;

        box.ValueChanged += (sender, e) =>
        {
            property.SetValue(Module, box.Value);
            NotifyObserver();
        };
        property.SetValue(Module, box.Value);

        return BuildFieldContainer(property, box);
    }
    public static Array GetValues(Type enumeration)
    {
        var result = new ArrayList();

        var values = Enum.GetValues(enumeration);
        foreach (Enum value in values)
        {
            var fi = enumeration.GetField(value.ToString());
            if (null == fi)
                continue;

            if (fi.GetCustomAttribute(typeof(BrowsableAttribute), true) is BrowsableAttribute browsable && browsable.Browsable == false)
                continue;

            if (fi.GetCustomAttribute(typeof(DisplayAttribute), true) is DisplayAttribute displayAttribute)
                result.Add(new EnumWrapper(value, displayAttribute.Name));
            else
                result.Add(new EnumWrapper(value, value.ToString()));
        }

        return result.ToArray();
    }
}