using LedMusic2.Models;
using LedMusic2.Nodes;
using LedMusic2.Outputs;
using LedMusic2.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            Helpers.TypeConverter.Initialize();

            vm.Outputs.Add(new DummyOutput());
            //TODO: vm.Nodes.Add(new OutputNode(new Point(200, 200)));

            vm.Initialize();

        }

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO
            /*foreach (Connection c in MainViewModel.Instance.Connections)
            {
                c.CalculatePoints();
            }*/
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                vm.IsAddNodePanelOpen = !vm.IsAddNodePanelOpen;

        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ListBox))
                return;

            var lb = (ListBox)sender;

            if (!(lb.SelectedItem is NodeType))
                return;

            //TODO
            //MainViewModel.Instance.AddNode((NodeType)lb.SelectedItem);
            MainViewModel.Instance.IsAddNodePanelOpen = false;

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MainViewModel.Instance.End();
        }

    }
}
