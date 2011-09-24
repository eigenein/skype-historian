using System;
using Newtonsoft.Json;

namespace SkypeHistorian.Helpers
{
    internal static class JsonTextWriterExtensions
    {
        private static void WriteProperty(this JsonTextWriter writer,
            string name, string value)
        {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        public static void WriteProperty(this JsonTextWriter writer, 
            string name, DateTime value)
        {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        public static void WriteProperty(this JsonTextWriter writer, 
            string name, object value)
        {
            WriteProperty(writer, name, value.ToString());
        }
    }
}
