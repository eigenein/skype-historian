using System;
using System.Reflection;

using NLog;

namespace SkypeHistorian.Helpers
{
    /// <summary>
    /// Helps to find <c>TextFormattingMode</c> enum.
    /// </summary>
    internal static class TextFormattingModeHelper
    {
        private const string TextOptionsTypeName =
            "System.Windows.Media.TextOptions, " +
            "PresentationFramework, Version=4.0.0.0, " +
            "Culture=neutral, PublicKeyToken=31bf3856ad364e35";

        private const int TextFormattingModeDisplay = 1;

        private static readonly Logger Logger = 
            LogManager.GetCurrentClassLogger();

        private static readonly MethodInfo SetTextFormattingModeMethodInfo;

        static TextFormattingModeHelper()
        {
            Logger.Info("CLR version: {0}.", Environment.Version);
            Logger.Info("Looking for System.Windows.Media.TextOptions ...");
            Type textOptionsType = Type.GetType(TextOptionsTypeName);
            if (textOptionsType != null)
            {
                Logger.Info("Looking for SetTextFormattingMode ...");
                SetTextFormattingModeMethodInfo = 
                    textOptionsType.GetMethod("SetTextFormattingMode");
            }
            else
            {
                Logger.Info("System.Windows.Media.TextOptions is not available.");
                SetTextFormattingModeMethodInfo = null;
            }
        }

        /// <summary>
        /// Applies the <c>TextFormattingMode.Display</c> on the specified
        /// object if available.
        /// </summary>
        public static void ApplyDisplayTextFormattingMode(object target)
        {
            if (SetTextFormattingModeMethodInfo != null)
            {
                Logger.Info("Setting TextFormattingMode.Display ...");
                SetTextFormattingModeMethodInfo.Invoke(
                    null, 
                    new object[] { target, TextFormattingModeDisplay });
            }
            else
            {
                Logger.Info("SetTextFormattingMode is not available.");
            }
        }
    }
}
