using System.Xml.Linq;

namespace LedMusic2
{
    interface IExportable
    {

        XElement GetXmlElement();

        void LoadFromXml(XElement element);

    }
}
