using LedMusic2.LedColors;
using System.Xml.Linq;

namespace LedMusic2.Outputs.OutputModels
{

    [Output("TCP")]
    class TcpOutput : OutputBase
    {

        public override string DefaultName => "TCP";
        
        public int Port { get; set; } = 4444;

        public override void CalculationDone(LedColorArray calculationResult)
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
