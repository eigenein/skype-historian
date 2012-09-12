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
        private const int TextFormattingModeDisplay = 1;

        private static readonly string[] TextOptionsTypeNames = new string[]
        {
            "System.Windows.Media.TextOptions, " +
            "PresentationFramework, Version=4.0.0.0, " +
            "Culture=neutral, PublicKeyToken=31bf3856ad364e35"
        };

        private static readonly Logger Logger = 
            LogManager.GetCurrentClassLogger();

        private static readonly MethodInfo SetTextFormattingModeMethodInfo;

        static TextFormattingModeHelper()
        {
            Logger.Info("CLR version: {0}.", Environment.Version);
            Logger.Info("Looking for System.Windows.Media.TextOptions ...");
            SetTextFormattingModeMethodInfo = null;
            foreach (string textOptionsTypeName in TextOptionsTypeNames)
            {
                Type textOptionsType = Type.GetType(textOptionsTypeName);
                if (textOptionsType != null)
                {
                    Logger.Info(
                        "Looking for SetTextFormattingMode as '{0}' ...",
                        textOptionsTypeName);
                    SetTextFormattingModeMethodInfo =
                        textOptionsType.GetMethod("SetTextFormattingMode");
                    if (SetTextFormattingModeMethodInfo != null)
                    {
                        Logger.Info(
                            "Found SetTextFormattingMode: {0}.",
                            SetTextFormattingModeMethodInfo.ToString());
                        return;
                    }
                }
            }
            Logger.Info("SetTextFormattingMode is not found.");
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
