using System;

namespace SkypeHistorian.Helpers
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime BeginUtc = new DateTime(1970, 1, 1);

        public static long GetUnixTime(this DateTime value)
        {
            return (long)(value - BeginUtc).TotalSeconds;
        }
    }
}
