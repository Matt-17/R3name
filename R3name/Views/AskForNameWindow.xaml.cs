using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace R3name.Views
{
    /// <summary>
    /// Interaction logic for AskForNameWindow.xaml
    /// </summary>
    public partial class AskForNameWindow : Window
    {
        public AskForNameWindow()
        {
            InitializeComponent();
        }

        public string ConfigurationName => NameTextBox.Text;

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
