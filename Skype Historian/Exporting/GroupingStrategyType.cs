using System;

namespace SkypeHistorian.Exporting
{
    internal enum GroupingStrategyType
    {
        ByMembers,
        ByMembersThenByMonth,
        ByMonthThenByMembers,
        ByMonthThenByDayThenThenByMembers
    }
}
