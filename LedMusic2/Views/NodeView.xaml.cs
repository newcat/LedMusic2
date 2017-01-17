using LedMusic2.Helpers;
using LedMusic2.Models;
using LedMusic2.Nodes;
using LedMusic2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for NodeView.xaml
    /// </summary>
    public partial class NodeView : UserControl
    {

        private NodeBase _vm;
        private NodeBase vm {
            get { return _vm ?? (DataContext is NodeBase ? (_vm = (NodeBase)DataContext) : null); }
        }

        public NodeView()
        {
            InitializeComponent();
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (vm == null)
                return;

            if (e.OriginalSource is Thumb)
            {
                var t = (Thumb)e.OriginalSource;
                if (t.Tag != null && t.Tag is string && (string)t.Tag == "MainThumb")
                {
                    vm.PosX += e.HorizontalChange - MainViewModel.Instance.TranslateX;
                    vm.PosY += e.VerticalChange - MainViewModel.Instance.TranslateY;
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
            if (!(sender is Ellipse))
                return;

            var ellipse = (Ellipse)sender;
            var nodeInterface = (NodeInterface)ellipse.DataContext;
            MainViewModel.Instance.CreateTemporaryConnection(nodeInterface);
        }
    }
}
