using System.Xml.Serialization;

namespace MonkeySpeak.Backend.Core.Resources.Xml;

[XmlRoot("Manifest")]
public class Manifest()
{
    public int Version;
    public string VersionName;
}