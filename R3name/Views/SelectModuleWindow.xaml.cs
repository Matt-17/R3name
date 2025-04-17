using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using R3name.Models;
using R3name.Models.Interfaces;

namespace R3name.Views;

/// <summary>
/// Interaction logic for SelectModuleWindow.xaml
/// </summary>
public partial class SelectModuleWindow : Window
{
    public SelectModuleWindow()
    {
        InitializeComponent();
        DataContextChanged += SelectModuleWindow_DataContextChanged;
    }

    private void SelectModuleWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is IDialogViewModel dvmOld)
        {
            dvmOld.DialogResultRequested -= DialogResultRequested;
        }

        if (e.NewValue is IDialogViewModel dvm)
        {
            dvm.DialogResultRequested += DialogResultRequested;
        }
        else
            throw new ArgumentException();
    }

    private void DialogResultRequested(bool dialogResult)
    {
        DialogResult = dialogResult;
        Close();
    }

    private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var listBox = (ListBox)sender;
        if (listBox.SelectedItem is ModuleDescription)
        {
            DialogResultRequested(true);
        }
    }
}