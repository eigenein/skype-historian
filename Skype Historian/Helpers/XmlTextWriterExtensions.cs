using System;
using System.Xml;

namespace SkypeHistorian.Helpers
{
    internal static class XmlTextWriterExtensions
    {
        public static void WriteAttribute(this XmlTextWriter writer,
            string localName, object value)
        {
            writer.WriteStartAttribute(localName);
            writer.WriteValue(value.ToString());
            writer.WriteEndAttribute();
        }

        public static void WriteAttribute(this XmlTextWriter writer,
            string localName, DateTime value)
        {
            writer.WriteStartAttribute(localName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }
    }
}
