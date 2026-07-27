using System;
using System.IO;
using System.Text;

namespace WordProSuite.Desktop.Infrastructure
{
    internal static class Logger
    {
        private static readonly object Gate = new object();
        internal static string LogDirectory => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WordProSuite", "Logs");
        internal static string CurrentLog => Path.Combine(LogDirectory, "WordProSuite.log");
        internal static void Info(string message) => Write("INFO", message);
        internal static void Error(string message, Exception ex=null) => Write("ERROR", ex == null ? message : message + Environment.NewLine + ex);
        private static void Write(string level, string message)
        {
            try
            {
                lock (Gate)
                {
                    Directory.CreateDirectory(LogDirectory);
                    File.AppendAllText(CurrentLog,
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}{Environment.NewLine}", Encoding.UTF8);
                }
            }
            catch { }
        }
    }
}
