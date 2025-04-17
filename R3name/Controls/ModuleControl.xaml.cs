using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using R3name.Helper;
using R3name.ViewModels;

namespace R3name.Controls;

/// <summary>
/// Interaction logic for Module.xaml
/// </summary>
public partial class ModuleControl : UserControl
{
    public const double DragOpacity = 0.2;
    private Point _startPoint;
    private bool _isDragging;
    private UIElement _dragSource;
    private ItemContainerGenerator _itemsGenerator;

    public event EventHandler<ModuleViewModel> DragStarted;

    #region ctor
    public ModuleControl()
    {
        InitializeComponent();

        Loaded += Module_Loaded;
    }

    private void Module_Loaded(object sender, RoutedEventArgs e)
    {
        var itemsControl = this.FindVisualParent<ItemsControl>();
        _itemsGenerator = itemsControl.ItemContainerGenerator;
    }
    #endregion

    #region Fill Property
    public SolidColorBrush Fill
    {
        get => (SolidColorBrush)GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    public static readonly DependencyProperty FillProperty = DependencyProperty.Register(nameof(Fill), typeof(SolidColorBrush), typeof(ModuleControl), new PropertyMetadata(Brushes.Silver, FillBackground));

    private static void FillBackground(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var module = (ModuleControl)d;
        module.FillBackground(e.NewValue as SolidColorBrush);
    }

    private void FillBackground(SolidColorBrush brush)
    {
        BORDER_Title.Background = brush;
        BORDER_Modificator.Background = brush;
    }
    #endregion

    #region Drag'n'Drop
    private void DragIconMouseMove(object sender, MouseEventArgs e)
    {
        if (Mouse.LeftButton != MouseButtonState.Pressed)
            return;
        if (_isDragging)
            return;

        if (Math.Abs(e.GetPosition(null).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance || Math.Abs(e.GetPosition(null).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance)
        {
            _dragSource?.ReleaseMouseCapture();
            OnDragStarted();
        }
    }

    private void SetDragMode(bool isDragging)
    {
        _isDragging = isDragging;

        if (!isDragging)
        {
            Opacity = 1;
        }

        for (var index = 0; index < _itemsGenerator.Items.Count; index++)
        {
            var item = (UIElement)_itemsGenerator.ContainerFromIndex(index);
            item.IsHitTestVisible = !isDragging;
        }
    }

    private void DragIconMouseDown(object sender, MouseButtonEventArgs e)
    {
        _startPoint = e.GetPosition(null);
        _dragSource = (UIElement)e.Source;
        _dragSource.CaptureMouse();
        Opacity = DragOpacity;
    }

    private void DragIconMouseUp(object sender, MouseButtonEventArgs e)
    {
        _dragSource?.ReleaseMouseCapture();
        SetDragMode(false);
    }

    private void OnDragStarted()
    {
        if (DataContext is not ModuleViewModel moduleContainer)
            return;
        SetDragMode(true);
        DragStarted?.Invoke(this, moduleContainer);
        SetDragMode(false);
    }
    #endregion

    #region Context menu
    private void ShowContextMenuClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var menu = button.ContextMenu;
        if (menu == null)
            return;

        menu.PlacementTarget = button;
        menu.Placement = PlacementMode.Bottom;
        menu.DataContext = DataContext;
        menu.IsOpen = true;
    }
    #endregion
}