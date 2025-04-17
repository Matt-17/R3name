using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace R3name.Controls;

/// <summary>
/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
///
/// Step 1a) Using this custom control in a XAML file that exists in the current project.
/// Add this XmlNamespace attribute to the root element of the markup file where it is 
/// to be used:
///
///     xmlns:MyNamespace="clr-namespace:R3name"
///
///
/// Step 1b) Using this custom control in a XAML file that exists in a different project.
/// Add this XmlNamespace attribute to the root element of the markup file where it is 
/// to be used:
///
///     xmlns:MyNamespace="clr-namespace:R3name;assembly=R3name"
///
/// You will also need to add a project reference from the project where the XAML file lives
/// to this project and Rebuild to avoid compilation errors:
///
///     Right click on the target project in the Solution Explorer and
///     "Add Reference"->"Projects"->[Browse to and select this project]
///
///
/// Step 2)
/// Go ahead and use your control in the XAML file.
///
///     <MyNamespace:NumericUpDown/>
///
/// </summary>
public class NumericUpDown : Control
{
    static NumericUpDown()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
    }
    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum), typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(int.MaxValue, CheckValueStatic));

    private static void CheckValueStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var numericUpDown = (NumericUpDown)d;
        numericUpDown.CheckValue();
        numericUpDown.RaiseValueChangedEvent(e);
    }


    public int Minimum
    {
        get => (int)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
        nameof(Minimum), typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(int.MinValue, CheckValueStatic));


    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetCurrentValue(ValueProperty, value);
    }
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value), typeof(int), typeof(NumericUpDown), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (o, e) =>
        {
            var tb = (NumericUpDown)o;
            tb.CheckValue();
            tb.RaiseValueChangedEvent(e);
        }));

    private void CheckValue()
    {
        if (Value > Maximum)
            Value = Maximum;
        if (Value < Minimum)
            Value = Minimum;
    }

    public event EventHandler<DependencyPropertyChangedEventArgs> ValueChanged;
    private void RaiseValueChangedEvent(DependencyPropertyChangedEventArgs e)
    {
        ValueChanged?.Invoke(this, e);
    }


    public int Step
    {
        get => (int)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }
    public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
        nameof(Step), typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(1));


    private RepeatButton _upButton;
    private RepeatButton _downButton;
    private TextBox _tbMain;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _tbMain = (TextBox)Template.FindName("PART_TbMain", this);
        _upButton = (RepeatButton)Template.FindName("PART_UpButton", this);
        _downButton = (RepeatButton)Template.FindName("PART_DownButton", this);
        _upButton.Click += btup_Click;
        _downButton.Click += btdown_Click;
        PreviewKeyDown += NumericUpDown_KeyDown;
        PreviewMouseWheel += NumericUpDown_MouseWheel;
    }

    private void NumericUpDown_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Up)
        {
            SetValue(Value + Step);
            e.Handled = true;
        }
        if (e.Key == Key.Down)
        {
            SetValue(Value - Step);
            e.Handled = true;
        }
    }

    private void NumericUpDown_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (!_tbMain.IsFocused)
        {
            return;
        }

        var step = Math.Sign(e.Delta);
        SetValue(Value + step);
        e.Handled = true;
    }

    private void btup_Click(object sender, RoutedEventArgs e)
    {
        SetValue(Value + Step);
    }

    private void SetValue(int value)
    {
        if (value > Maximum)
            value = Maximum;
        if (value < Minimum)
            value = Minimum;
        Value = value;

        _tbMain.Focus();
        _tbMain.Select(_tbMain.Text.Length, 0);
    }

    private void btdown_Click(object sender, RoutedEventArgs e)
    {
        SetValue(Value - Step);
    }
}