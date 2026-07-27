using System;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using WordProSuite.Desktop.Infrastructure;
using WordProSuite.Desktop.Licensing;

namespace WordProSuite.Desktop.Commands
{
    internal static class SystemCommands
    {
        internal static void About()
        {
            MessageBox.Show(
                "WordPro Suite Desktop Pro\n" +
                "Version 2.2.0\n\n" +
                "إضافة COM مكتبية احترافية لـ Microsoft Word\n" +
                "تعمل دون Node.js أو localhost أو VBA.\n\n" +
                "الأدوات المسجلة: " + CommandRouter.All.Count().ToString() + "\n" +
                "الترخيص: " + LicenseManager.StatusText,
                "حول WordPro Suite");
        }

        internal static void HealthCheck()
        {
            var b = new StringBuilder();
            b.AppendLine("WordPro Suite Desktop Pro Health Check");
            b.AppendLine("Version: 2.2.0");
            b.AppendLine("Process: " + (Environment.Is64BitProcess ? "64-bit" : "32-bit"));
            b.AppendLine("OS: " + (Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit"));
            b.AppendLine("Connected: " + (WordContext.Application != null));
            b.AppendLine("License: " + LicenseManager.StatusText);
            b.AppendLine("Machine ID: " + LicenseManager.MachineId);
            b.AppendLine("Log: " + Logger.CurrentLog);
            try { b.AppendLine("Word version: " + Convert.ToString(WordContext.Application.Version)); }
            catch (Exception ex) { b.AppendLine("Word version error: " + ex.Message); }
            MessageBox.Show(b.ToString(), "فحص WordPro Suite");
        }
    }
}
