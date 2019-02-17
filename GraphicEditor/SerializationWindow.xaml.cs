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

namespace GraphicEditor
{
    /// <summary>
    /// Interaction logic for Serialization.xaml
    /// </summary>
    public partial class SerializationWindow : Window
    {
        public string TypeSerialization
        {
            get
            {
               return RadioGrid.Children.Cast<RadioButton>().Where(c => c.IsChecked == true).Select(c => c).First().Name.Replace("Formatter", "");
            }
        }

        public SerializationWindow()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
