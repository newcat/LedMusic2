using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace LedMusic2.Nodes
{
    /// <summary>
    /// Interaction logic for NodeInterfaceView.xaml
    /// </summary>
    public partial class NodeInterfaceView : UserControl
    {

        Ellipse ell;

        public NodeInterfaceView()
        {
            InitializeComponent();
            DataContextChanged += NodeInterfaceView_DataContextChanged;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            object obj = Template.FindName("ell", this);
            if (obj == null || !(obj is Ellipse))
                return;

            ell = (Ellipse)obj;
            ell.IsMouseDirectlyOverChanged += Ell_IsMouseDirectlyOverChanged;

        }

        private void Ell_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            if (DataContext == null || !(DataContext is NodeInterface) || ell == null)
                return;

            var ni = (NodeInterface)DataContext;

            if (ell.IsMouseDirectlyOver)
            {
                ni.NodeEditorVM.MouseOverInterface(ni);
            } else
            {
                ni.NodeEditorVM.MouseLeftInterface();
            }
            
        }

        private void NodeInterfaceView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext != null && DataContext is NodeInterface)
                ((NodeInterface)DataContext).View = this;
        }

    }
}
