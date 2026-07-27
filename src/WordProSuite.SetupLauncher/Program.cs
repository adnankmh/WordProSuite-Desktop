using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace WordProSuite.SetupLauncher
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string command = args == null ? "" : String.Join(" ", args).ToLowerInvariant();
            try
            {
                if (command.Contains("/install") || command.Contains("/repair"))
                {
                    SetupForm.Install(true);
                    return;
                }
                if (command.Contains("/uninstall") || command.Contains("/remove"))
                {
                    SetupForm.Uninstall(true);
                    return;
                }
            }
            catch (Exception ex)
            {
                Environment.ExitCode = 1;
                if (!command.Contains("/silent")) MessageBox.Show(ex.Message, "WordPro Suite Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.Run(new SetupForm());
        }
    }

    internal sealed class SetupForm : Form
    {
        private const string ProgId = "WordProSuite.Desktop.AddIn";
        private const string Clsid = "{79D9E91D-88D5-4C41-B805-82D64D1348B2}";
        private readonly Label Status = new Label();
        private readonly ProgressBar Progress = new ProgressBar();
        private readonly Button InstallButton;
        private readonly Button RepairButton;
        private readonly Button RemoveButton;

        internal SetupForm()
        {
            Text = "WordPro Suite Desktop Pro Setup";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(760, 520);
            MinimumSize = new Size(700, 500);
            Font = new Font("Segoe UI", 11f);
            BackColor = Color.FromArgb(245, 247, 250);

            var hero = new Panel { Dock = DockStyle.Top, Height = 150, BackColor = Color.FromArgb(22, 53, 79), Padding = new Padding(28, 22, 28, 18) };
            var title = new Label
            {
                Text = "WordPro Suite Desktop Pro",
                Dock = DockStyle.Top,
                Height = 52,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 24f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            var subtitle = new Label
            {
                Text = "مثبت مكتبي مباشر لمختلف إصدارات Microsoft Word — لا يحتاج Node.js أو localhost.",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(220, 232, 241),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11.5f)
            };
            hero.Controls.Add(subtitle);
            hero.Controls.Add(title);

            var body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(28) };
            Status.Dock = DockStyle.Top;
            Status.Height = 84;
            Status.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
            Status.TextAlign = ContentAlignment.MiddleCenter;
            Status.BackColor = Color.White;
            Status.BorderStyle = BorderStyle.FixedSingle;

            var machinePanel = new Panel { Dock = DockStyle.Top, Height = 82, Padding = new Padding(0, 16, 0, 10) };
            var machineLabel = new Label { Text = "معرّف الجهاز: " + MachineId(), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Consolas", 11f) };
            var copy = new Button { Text = "نسخ المعرّف", Dock = DockStyle.Left, Width = 140 };
            copy.Click += (s, e) => { Clipboard.SetText(MachineId()); MessageBox.Show("تم نسخ معرّف الجهاز."); };
            machinePanel.Controls.Add(machineLabel);
            machinePanel.Controls.Add(copy);

            Progress.Dock = DockStyle.Top;
            Progress.Height = 18;
            Progress.Style = ProgressBarStyle.Marquee;
            Progress.Visible = false;

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 98, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 18, 0, 0) };
            InstallButton = MakeButton("تثبيت", Color.FromArgb(30, 137, 105));
            RepairButton = MakeButton("إصلاح", Color.FromArgb(34, 126, 155));
            RemoveButton = MakeButton("إزالة", Color.FromArgb(176, 72, 70));
            var openWord = MakeButton("فتح Word", Color.FromArgb(76, 89, 103));
            InstallButton.Click += (s, e) => RunOperation(() => Install(false));
            RepairButton.Click += (s, e) => RunOperation(() => Install(false));
            RemoveButton.Click += (s, e) => RunOperation(() => Uninstall(false));
            openWord.Click += (s, e) => StartWord();
            buttons.Controls.Add(InstallButton);
            buttons.Controls.Add(RepairButton);
            buttons.Controls.Add(RemoveButton);
            buttons.Controls.Add(openWord);

            var note = new Label
            {
                Text = "بعد التثبيت افتح Word ثم استخدم زر «تفعيل البرنامج» لإدخال Serial Number.\nالمثبت يعمل للحساب الحالي ولا يحتاج صلاحيات Administrator.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopCenter,
                ForeColor = Color.DimGray,
                Padding = new Padding(10)
            };

            body.Controls.Add(note);
            body.Controls.Add(buttons);
            body.Controls.Add(Progress);
            body.Controls.Add(machinePanel);
            body.Controls.Add(Status);
            Controls.Add(body);
            Controls.Add(hero);
            Shown += (s, e) => RefreshStatus();
        }

        private static Button MakeButton(string text, Color color)
        {
            var button = new Button
            {
                Text = text,
                Width = 155,
                Height = 54,
                Margin = new Padding(8),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold)
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void RunOperation(Action operation)
        {
            try
            {
                ToggleBusy(true);
                operation();
                RefreshStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "WordPro Suite Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { ToggleBusy(false); }
        }

        private void ToggleBusy(bool busy)
        {
            Progress.Visible = busy;
            InstallButton.Enabled = RepairButton.Enabled = RemoveButton.Enabled = !busy;
            Application.DoEvents();
        }

        private void RefreshStatus()
        {
            string dll = InstalledDll;
            bool file = File.Exists(dll);
            bool registry = Views.Any(IsRegistered);
            Status.Text = file && registry
                ? "البرنامج مثبت وجاهز — الإصدار " + FileVersionInfo.GetVersionInfo(dll).FileVersion
                : file ? "ملفات البرنامج موجودة، لكن التسجيل يحتاج إصلاحًا."
                : "WordPro Suite Desktop Pro غير مثبت على هذا الحساب.";
        }

        private static RegistryView[] Views => Environment.Is64BitOperatingSystem
            ? new[] { RegistryView.Registry64, RegistryView.Registry32 }
            : new[] { RegistryView.Registry32 };

        private static string InstalledDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WordProSuite", "Desktop");
        private static string InstalledDll => Path.Combine(InstalledDirectory, "WordProSuite.AddIn.dll");
        private static string PayloadDll => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Payload", "WordProSuite.AddIn.dll");

        internal static void Install(bool silent)
        {
            StopWord();
            if (!File.Exists(PayloadDll)) throw new FileNotFoundException("لم يتم العثور على Payload\\WordProSuite.AddIn.dll بجانب ملف Setup.", PayloadDll);
            Directory.CreateDirectory(InstalledDirectory);
            File.Copy(PayloadDll, InstalledDll, true);
            foreach (RegistryView view in Views)
            {
                RegisterView(view);
                ClearOfficeDisableState(view);
            }

            if (!Views.Any(IsRegistered))
                throw new InvalidOperationException("لم ينجح تسجيل COM.");

            if (!silent) MessageBox.Show("تم تثبيت WordPro Suite Desktop Pro بنجاح.\nافتح Word ثم فعّل البرنامج باستخدام Serial Number.", "تم التثبيت", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        internal static void Uninstall(bool silent)
        {
            StopWord();
            foreach (RegistryView view in Views) UnregisterView(view);
            try { if (Directory.Exists(InstalledDirectory)) Directory.Delete(InstalledDirectory, true); } catch { }
            if (!silent) MessageBox.Show("تمت إزالة WordPro Suite Desktop Pro من الحساب الحالي.", "تمت الإزالة", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void RegisterView(RegistryView view)
        {
            AssemblyName assembly = AssemblyName.GetAssemblyName(InstalledDll);
            string runtime = Assembly.ReflectionOnlyLoadFrom(InstalledDll).ImageRuntimeVersion;
            string codeBase = new Uri(InstalledDll).AbsoluteUri;

            using (RegistryKey root = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view))
            {
                using (RegistryKey clsid = root.CreateSubKey(@"Software\Classes\CLSID\" + Clsid))
                {
                    clsid.SetValue(null, "WordPro Suite Desktop Add-in");
                    using (RegistryKey inproc = clsid.CreateSubKey("InprocServer32"))
                    {
                        inproc.SetValue(null, "mscoree.dll");
                        inproc.SetValue("ThreadingModel", "Both");
                        inproc.SetValue("Class", "WordProSuite.Desktop.WordProAddIn");
                        inproc.SetValue("Assembly", assembly.FullName);
                        inproc.SetValue("RuntimeVersion", runtime);
                        inproc.SetValue("CodeBase", codeBase);
                        using (RegistryKey version = inproc.CreateSubKey(assembly.Version.ToString()))
                        {
                            version.SetValue("Class", "WordProSuite.Desktop.WordProAddIn");
                            version.SetValue("Assembly", assembly.FullName);
                            version.SetValue("RuntimeVersion", runtime);
                            version.SetValue("CodeBase", codeBase);
                        }
                    }
                    using (RegistryKey prog = clsid.CreateSubKey("ProgId")) prog.SetValue(null, ProgId);
                }

                using (RegistryKey progId = root.CreateSubKey(@"Software\Classes\" + ProgId))
                {
                    progId.SetValue(null, "WordPro Suite Desktop Add-in");
                    using (RegistryKey c = progId.CreateSubKey("CLSID")) c.SetValue(null, Clsid);
                }

                using (RegistryKey addin = root.CreateSubKey(@"Software\Microsoft\Office\Word\Addins\" + ProgId))
                {
                    addin.SetValue("FriendlyName", "WordPro Suite Desktop Pro");
                    addin.SetValue("Description", "Professional productivity suite for Microsoft Word.");
                    addin.SetValue("LoadBehavior", 3, RegistryValueKind.DWord);
                    addin.SetValue("CommandLineSafe", 0, RegistryValueKind.DWord);
                }
            }
        }

        private static void UnregisterView(RegistryView view)
        {
            using (RegistryKey root = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view))
            {
                DeleteTree(root, @"Software\Classes\CLSID\" + Clsid);
                DeleteTree(root, @"Software\Classes\" + ProgId);
                DeleteTree(root, @"Software\Microsoft\Office\Word\Addins\" + ProgId);
            }
        }

        private static void ClearOfficeDisableState(RegistryView view)
        {
            using (RegistryKey root = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view))
            {
                DeleteTree(root, @"Software\Microsoft\Office\16.0\Word\Resiliency\DisabledItems");
                DeleteTree(root, @"Software\Microsoft\Office\16.0\Word\Resiliency\CrashingAddinList");
                using (RegistryKey safe = root.CreateSubKey(@"Software\Microsoft\Office\16.0\Word\Resiliency\DoNotDisableAddinList"))
                    safe.SetValue(ProgId, 1, RegistryValueKind.DWord);
            }
        }

        private static bool IsRegistered(RegistryView view)
        {
            try
            {
                using (RegistryKey root = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view))
                using (RegistryKey key = root.OpenSubKey(@"Software\Classes\CLSID\" + Clsid + @"\InprocServer32"))
                    return key != null && String.Equals(Convert.ToString(key.GetValue(null)), "mscoree.dll", StringComparison.OrdinalIgnoreCase);
            }
            catch { return false; }
        }

        private static void DeleteTree(RegistryKey root, string path)
        {
            try { root.DeleteSubKeyTree(path, false); } catch { }
        }

        private static void StopWord()
        {
            foreach (Process process in Process.GetProcessesByName("WINWORD"))
                try { process.Kill(); process.WaitForExit(3000); } catch { }
        }

        private static void StartWord()
        {
            try { Process.Start("winword.exe"); }
            catch { MessageBox.Show("تعذر فتح Word. افتحه من قائمة ابدأ."); }
        }

        private static string MachineId()
        {
            string machineGuid = "";
            try
            {
                using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
                    machineGuid = Convert.ToString(key == null ? null : key.GetValue("MachineGuid"));
            }
            catch { }
            string source = (String.IsNullOrWhiteSpace(machineGuid) ? Environment.MachineName : machineGuid) + "|WPSD2";
            using (SHA256 sha = SHA256.Create())
            {
                string hex = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(source))).Replace("-", "");
                return hex.Substring(0, 6) + "-" + hex.Substring(6, 6) + "-" + hex.Substring(12, 6) + "-" + hex.Substring(18, 6);
            }
        }
    }
}
