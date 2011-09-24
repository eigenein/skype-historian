using System;

namespace SkypeHistorian.Exporting
{
    public interface IGroupingStrategy
    {
        string GetChatPathForMessage(string members, DateTime timeStamp);
    }
}
