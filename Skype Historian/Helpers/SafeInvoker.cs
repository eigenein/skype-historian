using System;
using System.Threading;
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

        public static bool Invoke<TResult>(Func<TResult> target,
            out TResult result, TimeSpan timeout)
        {
            Thread threadToKill = null;
            Func<TResult> wrappedAction = () =>
            {
                threadToKill = Thread.CurrentThread;
                return target();
            };
            IAsyncResult ar = wrappedAction.BeginInvoke(null, null);
            if (ar.AsyncWaitHandle.WaitOne(timeout))
            {
                result = wrappedAction.EndInvoke(ar);
                return true;
            }
            threadToKill.Abort();
            result = default(TResult);
            return false;
        }

        public static bool Repeat<TResult>(Func<TResult> target, out TResult result,
            int repeatCount, TimeSpan timeout)
        {
            result = default(TResult);
            while (repeatCount > 0)
            {
                if (Invoke(target, out result, timeout))
                {
                    return true;
                }
                repeatCount -= 1;
                Logger.Warn(repeatCount > 0 ?
                    "Repeating Invoke ..." : "All repeats failed.");
            }
            return false;
        }
    }
}
