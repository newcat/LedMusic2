using System.Xml.Linq;

namespace LedMusic2.Interfaces
{
    interface IExportable
    {

        XElement GetXmlElement();

        void LoadFromXml(XElement element);

    }
}
