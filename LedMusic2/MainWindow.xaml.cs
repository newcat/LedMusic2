using LedMusic2.Helpers;
using LedMusic2.Models;
using LedMusic2.Nodes;
using LedMusic2.Sound;
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

            vm.Nodes.Add(new OutputNode(new Point(200, 200)));

            vm.Initialize();

        }

        private void nodePanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            vm.Scale += e.Delta > 0 ? 0.1 : -0.1;
            vm.ScaleCenterX = e.GetPosition(nodeIC).X;
            vm.ScaleCenterY = e.GetPosition(nodeIC).Y;
        }

        private void nodePanel_MouseMove(object sender, MouseEventArgs e)
        {
            Point newPos = e.GetPosition(nodeIC);
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

        private void nodePanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (vm.IsAddNodePanelOpen) {
                if (addNodePanel.IsMouseOver)
                    return;
                else
                    vm.IsAddNodePanelOpen = false;
            }

            FocusManager.SetFocusedElement(this, this);

            oldMousePosition = e.GetPosition(nodeIC);
            isDragging = true;
        }

        private void nodePanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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

            if (e.Key == Key.Delete)
                vm.DeleteSelectedNode();

            if (e.Key == Key.Space)
            {
                if (SoundEngine.Instance.CanPlay)
                    SoundEngine.Instance.Play();
                else if (SoundEngine.Instance.CanPause)
                    SoundEngine.Instance.Pause();
            }
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ListBox))
                return;

            var lb = (ListBox)sender;

            if (!(lb.SelectedItem is NodeType))
                return;

            MainViewModel.Instance.AddNode((NodeType)lb.SelectedItem);
            MainViewModel.Instance.IsAddNodePanelOpen = false;

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MainViewModel.Instance.End();
        }

        private void waveform_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double width = MainViewModel.Instance.TrackWidth;
            double trackDuration = SoundEngine.Instance.Length.TotalSeconds;
            if (width == 0)
                return;

            double perc = e.GetPosition(waveform).X / width;
            SoundEngine.Instance.Position = TimeSpan.FromSeconds(perc * trackDuration);
        }
    }
}
