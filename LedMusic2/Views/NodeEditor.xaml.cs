using LedMusic2.Nodes;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LedMusic2.Views
{
    /// <summary>
    /// Interaction logic for NodeEditor.xaml
    /// </summary>
    public partial class NodeEditor : UserControl
    {

        private NodeEditorViewModel vm = null; //TODO
        private Point oldMousePosition = new Point();
        private bool isDragging = false;
        private bool wasDragged = false;

        public NodeEditor()
        {
            InitializeComponent();
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
            vm.MousePosX = newPos.X;
            vm.MousePosY = newPos.Y;

            if (e.LeftButton == MouseButtonState.Pressed && isDragging)
            {
                vm.TranslateX += newPos.X - oldMousePosition.X;
                vm.TranslateY += newPos.Y - oldMousePosition.Y;
                oldMousePosition = newPos;
                wasDragged = true;
            }
            else
            {
                isDragging = false;
            }

        }

        private void nodePanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            //TODO
            /*if (vm.IsAddNodePanelOpen)
            {
                if (addNodePanel.IsMouseOver)
                    return;
                else
                    vm.IsAddNodePanelOpen = false;
            }*/

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

        private void nodePanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                vm.DeleteSelectedNode();
        }

    }
}
