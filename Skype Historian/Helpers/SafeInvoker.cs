using System;
using NLog;

namespace SkypeHistorian.Helpers
{
    internal static class SafeInvoker
    {
        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        private static readonly string ExceptionMessage;

        static SafeInvoker()
        {
            ExceptionMessage = "The exception was raised at invocation target."
                + Environment.NewLine;
        }

        public static bool Repeat<TResult>(Func<TResult> target, out TResult result,
            int repeatCount)
        {
            result = default(TResult);
            while (repeatCount > 0)
            {
                if (Invoke(target, out result))
                {
                    return true;
                }
                repeatCount -= 1;
                Logger.Warn(repeatCount > 0 ? 
                    "Repeating Invoke ..." : "All repeats failed.");
            }
            return false;
        }

        public static bool Invoke(Action target)
        {
            try
            {
                target();
                return true;
            }
            catch (Exception ex)
            {
                Logger.WarnException(ExceptionMessage, ex);
                return false;
            }
        }

        public static bool Invoke<TResult>(Func<TResult> target, out TResult result)
        {
            try
            {
                result = target();
                return true;
            }
            catch (Exception ex)
            {
                Logger.WarnException(ExceptionMessage, ex);
                result = default(TResult);
                return false;
            }
        }
    }
}
