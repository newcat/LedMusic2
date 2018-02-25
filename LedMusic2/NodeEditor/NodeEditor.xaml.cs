using LedMusic2.Nodes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LedMusic2.NodeEditor
{
    /// <summary>
    /// Interaction logic for NodeEditor.xaml
    /// </summary>
    public partial class NodeEditor : UserControl
    {

        private NodeEditorViewModel vm = null;
        private Point oldMousePosition = new Point();
        private bool isDragging = false;
        private bool wasDragged = false;

        public NodeEditor()
        {
            DataContextChanged += NodeEditor_DataContextChanged;
            InitializeComponent();
        }

        private void NodeEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is NodeEditorViewModel)
                vm = (NodeEditorViewModel)e.NewValue;
        }

        private void NodePanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            if (vm == null)
                return;

            vm.Scale += e.Delta > 0 ? 0.1 : -0.1;
            vm.ScaleCenterX = e.GetPosition(nodeIC).X;
            vm.ScaleCenterY = e.GetPosition(nodeIC).Y;
        }

        private void NodePanel_MouseMove(object sender, MouseEventArgs e)
        {

            if (vm == null)
                return;

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

        private void NodePanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (vm.IsAddNodePanelOpen)
            {
                if (addNodePanel.IsMouseOver)
                    return;
                else
                    vm.IsAddNodePanelOpen = false;
            }

            FocusManager.SetFocusedElement(this, this);

            oldMousePosition = e.GetPosition(nodeIC);
            isDragging = true;

        }

        private void NodePanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (vm == null)
                return;

            if (!wasDragged)
                NodeBase.InvokeUnselectAllNodes(this);

            if (vm.TemporaryConnection != null)
                vm.FinalizeTemporaryConnection();

            wasDragged = false;
            isDragging = false;

            vm.RecalculateAllConnectionPoints();

        }

        private void NodePanel_KeyDown(object sender, KeyEventArgs e)
        {

            if (vm == null)
                return;

            if (e.Key == Key.Delete)
                vm.DeleteSelectedNode();

            if (e.Key == Key.A && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                vm.IsAddNodePanelOpen = !vm.IsAddNodePanelOpen;

        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (vm == null || !(sender is ListBox))
                return;

            var lb = (ListBox)sender;

            if (!(lb.SelectedItem is NodeType))
                return;

            vm.AddNode((NodeType)lb.SelectedItem);
            vm.IsAddNodePanelOpen = false;

        }

    }
}
