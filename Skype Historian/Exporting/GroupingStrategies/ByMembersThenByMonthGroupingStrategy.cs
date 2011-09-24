using System;
using System.IO;
using SkypeHistorian.Helpers;

namespace SkypeHistorian.Exporting.GroupingStrategies
{
    internal class ByMembersThenByMonthGroupingStrategy : IGroupingStrategy
    {
        #region Implementation of IGroupingStrategy

        public string GetChatPathForMessage(string members, DateTime timeStamp)
        {
            return Path.Combine(PathExtensions.FixPath(members), 
                timeStamp.ToString("MMM yyyy"));
        }

        #endregion
    }
}
