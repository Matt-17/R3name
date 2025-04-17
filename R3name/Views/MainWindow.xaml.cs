using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

using R3name.Controls.Adorners;
using R3name.Helper;
using R3name.Models;
using R3name.ViewModels;

namespace R3name.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;


    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        DataContext = _viewModel;
        AssemblyHelper.Compose();
    }

    private void PasteCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        _viewModel.Insert();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _viewModel.LoadSettings();
        _viewModel.RefreshFiles();        
    }

    private void Module_DragStarted(object sender, ModuleViewModel e)
    {
        DragDrop.DoDragDrop((DependencyObject)sender, e, DragDropEffects.Move);
    }

    private void ItemsControl_DragEnter(object sender, DragEventArgs e)
    {
        e.Handled = true;
        var dropInfo = GetDropInfo(e);
        if (dropInfo == null)
        {
            e.Effects = DragDropEffects.None;
            return;
        }

        e.Effects = DragDropEffects.Move;
        var sender1 = sender as UIElement;
        var adorner = dropInfo.HasMoved ? new InsertAdorner(sender1, dropInfo.Index) : null;
        SetAdorner(sender1, adorner);
    }

    private static void SetAdorner<TAdorner>(UIElement sender, TAdorner adorner = null) where TAdorner : Adorner
    {
        if (sender == null)
            return;

        var adornerLayer = AdornerLayer.GetAdornerLayer(sender);
        if (adornerLayer == null)
            return;

        var adorners = adornerLayer.GetAdorners(sender);
        var existingAdorner = adorners?.SingleOrDefault(x => x is TAdorner);

        if (existingAdorner != null)
        {
            adornerLayer.Remove(existingAdorner);
        }

        if (adorner != null)
        {
            adornerLayer.Add(adorner);
        }
    }

    private static DropInfo GetDropInfo(DragEventArgs e)
    {
        if (e.Data.GetData(typeof(ModuleViewModel)) is not ModuleViewModel data)
            return null;

        var itemsControl = (ItemsControl)e.Source;
        if (itemsControl.ItemsSource is not ObservableCollection<ModuleViewModel> list)
            return null;

        var containsData = list.Contains(data);
        if (!containsData)
            return null;

        var index = GetInsertIndex(e, itemsControl);

        var indexOf = list.IndexOf(data);
        if (indexOf == index || indexOf + 1 == index)
        {
            return DropInfo.NoMoving;
        }

        return new DropInfo(index); ;
    }

    private static int GetInsertIndex(DragEventArgs e, ItemsControl itemsControl)
    {
        var generator = itemsControl.ItemContainerGenerator;
        var items = generator.Items.Select(x => generator.ContainerFromItem(x));

        var position = e.GetPosition(itemsControl);

        UIElement closest = null;
        var closestDistance = double.MaxValue;
        var isBottom = false;

        foreach (var element in items.OfType<UIElement>())
        {
            var p = element.TransformToAncestor(itemsControl).Transform(new Point(0, 0));
            var distance = position.Y <= p.Y ? p.Y - position.Y : position.Y - element.RenderSize.Height - p.Y;

            if (!(distance < closestDistance)) 
                continue;

            closest = element;
            closestDistance = distance;

            isBottom = position.Y - p.Y > element.RenderSize.Height / 2;
        }

        if (closest == null)
            return 0;

        var indexFromContainer = generator.IndexFromContainer(closest);
        if (isBottom)
            indexFromContainer++;
        return indexFromContainer;
    }

    private void ItemsControl_Drop(object sender, DragEventArgs e)
    {
        e.Handled = true;
        SetAdorner<InsertAdorner>(sender as ItemsControl);

        var dropInfo = GetDropInfo(e);
        if (dropInfo == null)
            return;

        if (e.Data.GetData(typeof(ModuleViewModel)) is not ModuleViewModel data)
            return;

        var itemsControl = (ItemsControl)e.Source;
        if (itemsControl.ItemsSource is not ObservableCollection<ModuleViewModel> list)
            return;

        if (!dropInfo.HasMoved)
            return;

        var oldIndex = list.IndexOf(data);

        var index = dropInfo.Index;
        if (index > oldIndex)
            index--;

        list.RemoveAt(oldIndex);
        list.Insert(index, data);

        (DataContext as MainViewModel)?.Refresh();

    }

    private void ItemsControl_DragLeave(object sender, DragEventArgs e)
    {
        UIElement sender1 = sender as ItemsControl;
        SetAdorner<InsertAdorner>(sender1);
    }

    private void FileSourceDragEnter(object sender, DragEventArgs e)
    {
        SetBorder(e, AcceptsValue(GetDragData(e)));
        e.Handled = true;
    }

    private void FileSourceDragLeave(object sender, DragEventArgs e)
    {
        SetAdorner<OutlineAdorner>(AdornerPanel);
        e.Handled = true;
    }

    private void FileSourceDrop(object sender, DragEventArgs e)
    {
        var firstData = GetDragData(e);
        if (firstData == null)
            return;

        ((MainViewModel)FileSourceBorder.DataContext).FileSource.SetValue(firstData);
        SetAdorner<OutlineAdorner>(AdornerPanel);
        e.Handled = true;
    }

    private bool AcceptsValue(string data)
    {
        return data != null && ((MainViewModel)FileSourceBorder.DataContext).FileSource.AcceptsValue(data);
    }

    private static string GetDragData(DragEventArgs e) => e.Data.GetData(DataFormats.FileDrop) is not string[] data ? null : data.FirstOrDefault();

    private void SetBorder(DragEventArgs e, bool allowDrop)
    {
        if (e.OriginalSource is Adorner)     
            return;

        if (!allowDrop)
        {
            e.Effects = DragDropEffects.None;
            SetAdorner<OutlineAdorner>(AdornerPanel);
            return;
        }

        e.Effects = DragDropEffects.Copy;

        SetAdorner(AdornerPanel, new OutlineAdorner(AdornerPanel));
    }

    private void Window_Unloaded(object sender, RoutedEventArgs e)
    {

    }
}