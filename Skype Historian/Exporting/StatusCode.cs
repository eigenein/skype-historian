using System;

namespace SkypeHistorian.Exporting
{
    public enum StatusCode
    {
        NoError = 0,
        Finished,
        SkypeInitializationFailed,
        SkypeStartFailed,
        SkypeCannotGetChatCollection,
        SkypeCannotGetChatProperties,
        SkypeCannotGetChat,
        SkypeCannotGetMessage,
        SkypeCannotGetMessageTimeStamp,
        MessageStoringFailed,
        CannotGetMessageCollection
    }
}
