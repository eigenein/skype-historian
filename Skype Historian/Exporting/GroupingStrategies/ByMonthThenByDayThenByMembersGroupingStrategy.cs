using System;
using System.IO;
using SkypeHistorian.Helpers;

namespace SkypeHistorian.Exporting.GroupingStrategies
{
    internal class ByMonthThenByDayThenByMembersGroupingStrategy
        : IGroupingStrategy
    {
        #region Implementation of IGroupingStrategy

        public string GetChatPathForMessage(string members, DateTime timeStamp)
        {
            return PathCombine(timeStamp.ToString("MMM yyyy"),
                timeStamp.ToString("dd"),
                PathExtensions.FixPath(members));
        }

        #endregion

        private static string PathCombine(string path1, string path2,
            string path3)
        {
            return Path.Combine(Path.Combine(path1, path2), path3);
        }
    }
}
