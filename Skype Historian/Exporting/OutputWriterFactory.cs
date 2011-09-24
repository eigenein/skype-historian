using System;
using SkypeHistorian.Exporting.OutputWriters;

namespace SkypeHistorian.Exporting
{
    internal static class OutputWriterFactory
    {
        public static OutputWriter Create(OutputType outputType,
            Storage storage)
        {
            switch (outputType)
            {
                case OutputType.TextFiles:
                    return new TextFilesOutputWriter(storage);
                case OutputType.Csv:
                    return new CsvOutputWriter(storage);
                case OutputType.Json:
                    return new JsonOutputWriter(storage);
                case OutputType.Xml:
                    return new XmlOutputWriter(storage);
                default:
                    return null;
            }
        }
    }
}
