using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;

namespace LedMusic2.Nodes
{
    /// <summary>
    /// Interaction logic for NodeView.xaml
    /// </summary>
    public partial class NodeView : UserControl
    {

        private NodeBase _vm;
        private NodeBase Vm => _vm ?? (DataContext is NodeBase ? (_vm = (NodeBase)DataContext) : null);

        public NodeView()
        {
            InitializeComponent();
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Vm == null)
                return;

            if (e.OriginalSource is Thumb t)
            {
                if (t.Tag != null && t.Tag is string && (string)t.Tag == "MainThumb")
                {
                    Vm.PosX += e.HorizontalChange - Vm.NodeEditorVM.TranslateX;
                    Vm.PosY += e.VerticalChange - Vm.NodeEditorVM.TranslateY;
                }
            }
        }

        private void Thumb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Vm == null)
                return;

            NodeBase.InvokeUnselectAllNodes(this);
            Vm.IsSelected = true;
        }

        private void NodeInterface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Ellipse) || Vm == null)
                return;

            var ellipse = (Ellipse)sender;
            var nodeInterface = (NodeInterface)ellipse.DataContext;
            Vm.NodeEditorVM.CreateTemporaryConnection(nodeInterface);
        }
    }
}
