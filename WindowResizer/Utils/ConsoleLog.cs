using System;


namespace WindowResizer.Utils
{
    /// <summary>
    /// Provides console logging utility methods.
    /// </summary>
    internal static class ConsoleLog
    {
        /// <summary>
        /// Log datetime format.
        /// </summary>
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        /// <summary>
        /// Write console log.
        /// </summary>
        /// <param name="message">Log message.</param>
        public static void WriteLine(string message)
        {
            WriteLine(DateTime.Now, message);
        }

        /// <summary>
        /// Write console log.
        /// </summary>
        /// <param name="logAt">Log datetime.</param>
        /// <param name="message">Log message.</param>
        public static void WriteLine(DateTime logAt, string message)
        {
            Console.WriteLine($"[{logAt.ToString(DateTimeFormat)}] {message}");
        }
    }
}
