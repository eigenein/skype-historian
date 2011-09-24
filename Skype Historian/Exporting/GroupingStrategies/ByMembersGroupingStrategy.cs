using System;
using SkypeHistorian.Helpers;

namespace SkypeHistorian.Exporting.GroupingStrategies
{
    internal class ByMembersGroupingStrategy : IGroupingStrategy
    {
        #region Implementation of IGroupingStrategy

        public string GetChatPathForMessage(string members, DateTime timeStamp)
        {
            return PathExtensions.FixPath(members);
        }

        #endregion
    }
}
