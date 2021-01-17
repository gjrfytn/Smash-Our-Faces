using Gjrfytn.Dim;
using System.Xml.Linq;

namespace Sof.Auxiliary
{
    public abstract class XmlParser
    {
        protected static T ParseEnum<T>(string value) where T : System.Enum => (T)System.Enum.Parse(typeof(T), value);
        protected static Position ExtractPosition(XElement element) => new Position(int.Parse(element.Attribute("x").Value), int.Parse(element.Attribute("y").Value));
    }
}
