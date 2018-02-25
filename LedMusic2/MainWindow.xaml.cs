using LedMusic2.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace LedMusic2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MainViewModel vm = MainViewModel.Instance;

        public MainWindow()
        {

            InitializeComponent();
            DataContext = MainViewModel.Instance;

            NodeConnection.TypeConverter.Initialize();

            vm.Initialize();

        }
        
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MainViewModel.Instance.End();
        }

    }
}
