using LedMusic2.Attributes;
using LedMusic2.Models;
using System;
using System.Windows;
using System.Xml.Linq;

namespace LedMusic2.Outputs
{

    [Output("TCP")]
    class TcpOutput : OutputBase
    {

        public override string DefaultName => "TCP";

        private FrameworkElement _settingsView = Activator.CreateInstance<TcpOutputView>();
        public override FrameworkElement SettingsView => _settingsView;

        private int _port = 4444;
        public int Port
        {
            get { return _port; }
            set
            {
                _port = value;
                NotifyPropertyChanged();
            }
        }

        public TcpOutput()
        {
            _settingsView.DataContext = this;
        }

        public override void CalculationDone(LedColor[] calculationResult)
        {
            //TODO
            return;
        }

        protected override void SaveAdditionalXmlData(XElement x)
        {
            x.Add(new XElement("port", Port));
        }

        protected override void LoadAdditionalXmlData(XElement x)
        {
            foreach (var el in x.Elements())
            {
                switch (el.Name.LocalName)
                {
                    case "port":
                        Port = int.Parse(el.Value);
                        break;
                }
            }
        }

    }
}
