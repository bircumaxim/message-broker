using System;
using System.Xml;
using Serialization.WireProtocol;

namespace Data.Configuration.FileConfiguration
{
    public static class WireProtocolConfigHelper
    {
        public static IWireProtocol GetWireProtocolByName(XmlNode wireConfigXmlNode)
        {
            IWireProtocol wireProtocol = null;
            if (wireConfigXmlNode.Attributes != null)
            {
                var wireProtocolName = wireConfigXmlNode.Attributes.GetNamedItem("WireProtocol")?.Value;
                var enableCrypting = Convert.ToBoolean(wireConfigXmlNode.Attributes.GetNamedItem("EnableCrypting")?.Value);
                switch (wireProtocolName)
                {
                    default:
                        wireProtocol = new DefaultWireProtocol(enableCrypting);
                        break;
                }
            }
            return wireProtocol;
        }
    }
}