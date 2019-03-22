using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GraphicEditor
{
    /// <summary>
    /// Interaction logic for Serialization.xaml
    /// </summary>
    public partial class SerializationWindow : Window
    {
        public string TypeSerialization 
            => RadioGrid.Children.Cast<RadioButton>().Where(c => c.IsChecked == true).Select(c => c).First()
                .Name.Replace("Formatter", "");

        public SerializationWindow()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    }
}