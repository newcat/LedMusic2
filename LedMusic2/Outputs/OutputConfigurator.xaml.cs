using LedMusic2.Models;
using LedMusic2.Outputs;
using LedMusic2.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

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
            if (outputsList.SelectedIndex >= 0 && e.AddedItems.Count > 0)
            {
                var t = (OutputType)e.AddedItems[0];
                var i = outputsList.SelectedIndex;
                if (MainViewModel.Instance.Outputs[i].GetType() != t.T)
                {

                    var oldName = "";
                    var oldOutput = MainViewModel.Instance.Outputs[i];
                    if (oldOutput.Name != oldOutput.DefaultName) oldName = oldOutput.Name;

                    var newOutput = (OutputBase)t.T.GetConstructor(Type.EmptyTypes).Invoke(null);
                    if (oldName != "") newOutput.Name = oldName;
                    MainViewModel.Instance.Outputs[i] = newOutput;
                    outputsList.SelectedIndex = i;    
                    
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.Outputs.Add(new DummyOutput());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (outputsList.SelectedIndex >= 0)
                MainViewModel.Instance.Outputs.RemoveAt(outputsList.SelectedIndex);
        }

    }
}
