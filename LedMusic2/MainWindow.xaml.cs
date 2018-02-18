﻿using LedMusic2.Models;
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

            vm.Initialize();

        }
        
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MainViewModel.Instance.End();
        }

    }
}
