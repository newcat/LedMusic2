using LedMusic2.Attributes;
using LedMusic2.Models;
using System;
using System.Windows;

namespace LedMusic2.Outputs
{

    [Output("TCP")]
    class TcpOutput : OutputBase
    {

        public override string DefaultName => "TCP";

        private FrameworkElement _settingsView = Activator.CreateInstance<TcpOutputView>();
        public override FrameworkElement SettingsView => _settingsView;

        public TcpOutput()
        {
            _settingsView.DataContext = this;
        }

        public override void CalculationDone(LedColor[] calculationResult)
        {
            //TODO
            return;
        }

    }
}
