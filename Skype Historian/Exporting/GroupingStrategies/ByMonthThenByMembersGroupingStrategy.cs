using System;
using System.IO;
using SkypeHistorian.Helpers;

namespace SkypeHistorian.Exporting.GroupingStrategies
{
    internal class ByMonthThenByMembersGroupingStrategy : IGroupingStrategy
    {
        #region Implementation of IGroupingStrategy

        public string GetChatPathForMessage(string members, DateTime timeStamp)
        {
            return Path.Combine(timeStamp.ToString("MMM yyyy"),
                PathExtensions.FixPath(members));
        }

        #endregion
    }
}
