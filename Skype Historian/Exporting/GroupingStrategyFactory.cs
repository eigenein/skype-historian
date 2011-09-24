using System;
using SkypeHistorian.Exporting.GroupingStrategies;

namespace SkypeHistorian.Exporting
{
    internal static class GroupingStrategyFactory
    {
        public static IGroupingStrategy Create(GroupingStrategyType type)
        {
            switch (type)
            {
                case GroupingStrategyType.ByMembers:
                    return new ByMembersGroupingStrategy();
                case GroupingStrategyType.ByMembersThenByMonth:
                    return new ByMembersThenByMonthGroupingStrategy();
                case GroupingStrategyType.ByMonthThenByMembers:
                    return new ByMonthThenByMembersGroupingStrategy();
                case GroupingStrategyType.ByMonthThenByDayThenThenByMembers:
                    return new ByMonthThenByDayThenByMembersGroupingStrategy();
                default:
                    throw new InvalidOperationException("Strategy is not defined.");
            }
        }
    }
}
