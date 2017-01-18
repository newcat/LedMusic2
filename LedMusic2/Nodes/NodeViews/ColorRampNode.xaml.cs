using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace LedMusic2.Nodes.NodeViews
{
    /// <summary>
    /// Interaktionslogik für ColorRampNode.xaml
    /// </summary>
    public partial class ColorRampNode : UserControl
    {
        public ColorRampNode()
        {
            InitializeComponent();
        }


        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {

            if (!(sender is Thumb))
                return;

            var thumb = (Thumb)sender;

            if (thumb.DataContext == null || !(thumb.DataContext is ColorStopViewModel))
                return;

            var vm = (ColorStopViewModel)thumb.DataContext;
            vm.AddToPosition(e.HorizontalChange);
            e.Handled = true;

        }

        private void Thumb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement))
                return;

            var thumb = (FrameworkElement)sender;

            if (thumb.DataContext == null || !(thumb.DataContext is ColorStopViewModel))
                return;

            var vm = (ColorStopViewModel)thumb.DataContext;
            vm.Parent.SelectStop(vm);

        }

    }
}
