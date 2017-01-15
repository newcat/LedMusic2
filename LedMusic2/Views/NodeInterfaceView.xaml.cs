using LedMusic2.Models;
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

            if (ell.IsMouseDirectlyOver)
            {
                MainViewModel.Instance.MouseOverInterface((NodeInterface)DataContext);
            } else
            {
                MainViewModel.Instance.MouseLeftInterface();
            }
            
        }

        private void NodeInterfaceView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext != null && DataContext is NodeInterface)
                ((NodeInterface)DataContext).View = this;
        }

    }
}
