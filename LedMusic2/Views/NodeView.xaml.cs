using LedMusic2.Models;
using LedMusic2.Nodes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;

namespace LedMusic2.Views
{
    /// <summary>
    /// Interaction logic for NodeView.xaml
    /// </summary>
    public partial class NodeView : UserControl
    {

        private NodeBase _vm;
        private NodeBase vm => _vm ?? (DataContext is NodeBase ? (_vm = (NodeBase)DataContext) : null);

        public NodeView()
        {
            InitializeComponent();
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (vm == null)
                return;

            if (e.OriginalSource is Thumb t)
            {
                if (t.Tag != null && t.Tag is string && (string)t.Tag == "MainThumb")
                {
                    vm.PosX += e.HorizontalChange - vm.NodeEditorVM.TranslateX;
                    vm.PosY += e.VerticalChange - vm.NodeEditorVM.TranslateY;
                }
            }
        }

        private void Thumb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (vm == null)
                return;

            NodeBase.InvokeUnselectAllNodes(this);
            vm.IsSelected = true;
        }

        private void NodeInterface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Ellipse) || vm == null)
                return;

            var ellipse = (Ellipse)sender;
            var nodeInterface = (NodeInterface)ellipse.DataContext;
            vm.NodeEditorVM.CreateTemporaryConnection(nodeInterface);
        }
    }
}
