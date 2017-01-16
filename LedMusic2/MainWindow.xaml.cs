using LedMusic2.Helpers;
using LedMusic2.Models;
using LedMusic2.Nodes;
using LedMusic2.ViewModels;
using LedMusic2.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LedMusic2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MainViewModel vm = MainViewModel.Instance;
        private Point oldMousePosition = new Point();
        private bool isDragging = false;
        private bool wasDragged = false;

        public MainWindow()
        {

            InitializeComponent();
            DataContext = MainViewModel.Instance;

            Helpers.TypeConverter.Initialize();
            NodeBase.FireOutputChangedEvents = true; //TODO: Change to false when playing back while settings keyframes

            Dot d1 = new Dot(new Point(10, 10));
            Dot d2 = new Dot(new Point(210, 10));
            MathNode m1 = new MathNode(new Point(410, 10));
            MathNode m2 = new MathNode(new Point(610, 10));
            MathNode m3 = new MathNode(new Point(810, 10));

            vm.Nodes.Add(d1);
            vm.Nodes.Add(d2);
            vm.Nodes.Add(m1);
            vm.Nodes.Add(m2);
            vm.Nodes.Add(m3);

            vm.Initialize();

        }

        private void window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            vm.Scale += e.Delta > 0 ? 0.1 : -0.1;
            vm.ScaleCenterX = e.GetPosition(this).X;
            vm.ScaleCenterY = e.GetPosition(this).Y;
        }

        private void window_MouseMove(object sender, MouseEventArgs e)
        {
            Point newPos = e.GetPosition(this);
            MainViewModel.Instance.MousePosX = newPos.X;
            MainViewModel.Instance.MousePosY = newPos.Y;

            if (e.LeftButton == MouseButtonState.Pressed && isDragging)
            {
                
                vm.TranslateX += newPos.X - oldMousePosition.X;
                vm.TranslateY += newPos.Y - oldMousePosition.Y;
                oldMousePosition = newPos;
                wasDragged = true;
            } else
            {
                isDragging = false;
            }

        }

        private void window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (vm.IsAddNodePanelOpen) {
                if (addNodePanel.IsMouseOver)
                    return;
                else
                    vm.IsAddNodePanelOpen = false;
            }                

            oldMousePosition = e.GetPosition(this);
            isDragging = true;
        }

        private void window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!wasDragged)
                NodeBase.InvokeUnselectAllNodes(this);

            if (vm.TemporaryConnection != null)
                vm.FinalizeTemporaryConnection();

            wasDragged = false;
            isDragging = false;

            vm.RecalculateAllConnectionPoints();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Connection c in MainViewModel.Instance.Connections)
            {
                c.CalculatePoints();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                vm.IsAddNodePanelOpen = !vm.IsAddNodePanelOpen;
        }
    }
}
