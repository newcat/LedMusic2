using LedMusic2.Models;
using LedMusic2.Outputs;
using LedMusic2.ViewModels;
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

namespace LedMusic2
{
    /// <summary>
    /// Interaktionslogik für OutputConfigurator.xaml
    /// </summary>
    public partial class OutputConfigurator : Window
    {
        public OutputConfigurator()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (outputsList.SelectedIndex >= 0)
            {
                var t = (OutputType)e.AddedItems[0];
                var constructor = t.T.GetConstructor(Type.EmptyTypes);
                MainViewModel.Instance.Outputs[outputsList.SelectedIndex] = (OutputBase)constructor.Invoke(null);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.Outputs.Add(new DummyOutput());
        }



    }
}
