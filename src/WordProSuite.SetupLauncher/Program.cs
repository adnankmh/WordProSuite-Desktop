using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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

            string command = args == null ? "" : String.Join(" ", args);
            bool silent = command.IndexOf("/silent", StringComparison.OrdinalIgnoreCase) >= 0;

            try
            {
                if (command.IndexOf("/install", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    SetupEngine.Install(null);
                    if (!silent) MessageBox.Show("تم التثبيت بنجاح.", "WordPro Suite");
                    return;
                }

                if (command.IndexOf("/repair", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    SetupEngine.Install(null);
                    if (!silent) MessageBox.Show("تم الإصلاح بنجاح.", "WordPro Suite");
                    return;
                }

                if (command.IndexOf("/uninstall", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    command.IndexOf("/remove", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    SetupEngine.Uninstall();
                    if (!silent) MessageBox.Show("تمت إزالة البرنامج.", "WordPro Suite");
                    return;
                }

                Application.Run(new SetupForm());
            }
            catch (Exception ex)
            {
                Environment.ExitCode = 1;
                if (!silent)
                    MessageBox.Show(ex.Message, "WordPro Suite Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    internal sealed class SetupForm : Form
    {
        private readonly Label Status = new Label();
        private readonly TextBox SerialBox = new TextBox();
        private readonly ProgressBar Progress = new ProgressBar();
        private readonly Button InstallTrialButton;
        private readonly Button InstallActivateButton;
        private readonly Button ActivateButton;
        private readonly Button RepairButton;
        private readonly Button RemoveButton;

        internal SetupForm()
        {
            Text = "WordPro Suite Desktop Ultimate 3.0 Setup";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(900, 680);
            MinimumSize = new Size(820, 620);
            Font = new Font("Segoe UI", 11f);
            BackColor = Color.FromArgb(242, 245, 248);

            var hero = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = Color.FromArgb(18, 50, 74),
                Padding = new Padding(30, 22, 30, 16)
            };

            var title = new Label
            {
                Text = "WordPro Suite Desktop Ultimate 3.0",
                Dock = DockStyle.Top,
                Height = 54,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 25f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var subtitle = new Label
            {
                Text = "مثبت واحد فقط — 500 أداة، تبويبتان احترافيتان، وتفعيل آمن دون ملفات إضافية.",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(221, 233, 241),
                Font = new Font("Segoe UI", 12f),
                TextAlign = ContentAlignment.MiddleCenter
            };

            hero.Controls.Add(subtitle);
            hero.Controls.Add(title);

            var body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(30) };

            Status.Dock = DockStyle.Top;
            Status.Height = 72;
            Status.BackColor = Color.White;
            Status.BorderStyle = BorderStyle.FixedSingle;
            Status.Font = new Font("Segoe UI", 12.5f, FontStyle.Bold);
            Status.TextAlign = ContentAlignment.MiddleCenter;

            var machinePanel = new Panel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(0, 14, 0, 8) };
            var machineText = new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Text = LicenseTools.MachineId,
                Font = new Font("Consolas", 11.5f),
                TextAlign = HorizontalAlignment.Center,
                RightToLeft = RightToLeft.No
            };
            var copyMachine = new Button { Text = "نسخ معرّف الجهاز", Dock = DockStyle.Left, Width = 170 };
            copyMachine.Click += delegate
            {
                Clipboard.SetText(LicenseTools.MachineId);
                MessageBox.Show("تم نسخ معرّف الجهاز.", "WordPro Suite");
            };
            machinePanel.Controls.Add(machineText);
            machinePanel.Controls.Add(copyMachine);

            var serialLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 32,
                Text = "Serial Number — اتركه فارغًا للتجربة، أو الصقه للتثبيت والتفعيل مباشرة",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };

            SerialBox.Dock = DockStyle.Top;
            SerialBox.Height = 116;
            SerialBox.Multiline = true;
            SerialBox.ScrollBars = ScrollBars.Vertical;
            SerialBox.RightToLeft = RightToLeft.No;
            SerialBox.Font = new Font("Consolas", 10f);

            Progress.Dock = DockStyle.Top;
            Progress.Height = 18;
            Progress.Style = ProgressBarStyle.Marquee;
            Progress.Visible = false;

            var firstButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 82,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 12, 0, 0),
                WrapContents = false
            };

            InstallActivateButton = MakeButton("تثبيت وتفعيل", Color.FromArgb(27, 124, 92), 180);
            InstallTrialButton = MakeButton("تثبيت تجريبي", Color.FromArgb(34, 126, 155), 170);
            ActivateButton = MakeButton("تفعيل فقط", Color.FromArgb(91, 65, 135), 150);

            InstallActivateButton.Click += delegate
            {
                RunOperation(delegate
                {
                    string serial = SerialBox.Text.Trim();
                    if (String.IsNullOrWhiteSpace(serial))
                        throw new InvalidOperationException("الصق Serial Number أولاً، أو استخدم «تثبيت تجريبي».");
                    SetupEngine.Install(serial);
                    MessageBox.Show("تم التثبيت والتفعيل بنجاح.\nيمكنك فتح Microsoft Word الآن.",
                        "WordPro Suite", MessageBoxButtons.OK, MessageBoxIcon.Information);
                });
            };

            InstallTrialButton.Click += delegate
            {
                RunOperation(delegate
                {
                    SetupEngine.Install(null);
                    MessageBox.Show("تم تثبيت النسخة التجريبية بنجاح.\nيمكنك فتح Microsoft Word الآن.",
                        "WordPro Suite", MessageBoxButtons.OK, MessageBoxIcon.Information);
                });
            };

            ActivateButton.Click += delegate
            {
                RunOperation(delegate
                {
                    string serial = SerialBox.Text.Trim();
                    if (String.IsNullOrWhiteSpace(serial))
                        throw new InvalidOperationException("الصق Serial Number أولاً.");
                    string message;
                    if (!LicenseTools.Activate(serial, out message))
                        throw new InvalidOperationException(message);
                    MessageBox.Show(message, "WordPro Suite", MessageBoxButtons.OK, MessageBoxIcon.Information);
                });
            };

            firstButtons.Controls.Add(InstallActivateButton);
            firstButtons.Controls.Add(InstallTrialButton);
            firstButtons.Controls.Add(ActivateButton);

            var secondButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 82,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 10, 0, 0),
                WrapContents = false
            };

            RepairButton = MakeButton("إصلاح", Color.FromArgb(70, 103, 123), 145);
            RemoveButton = MakeButton("إزالة", Color.FromArgb(173, 68, 66), 145);
            var openWord = MakeButton("فتح Word", Color.FromArgb(78, 88, 100), 145);

            RepairButton.Click += delegate
            {
                RunOperation(delegate
                {
                    SetupEngine.Install(null);
                    MessageBox.Show("تم إصلاح التسجيل والملفات بنجاح.", "WordPro Suite");
                });
            };

            RemoveButton.Click += delegate
            {
                RunOperation(delegate
                {
                    SetupEngine.Uninstall();
                    MessageBox.Show("تمت إزالة WordPro Suite Desktop Ultimate.", "WordPro Suite");
                });
            };

            openWord.Click += delegate { SetupEngine.StartWord(); };

            secondButtons.Controls.Add(RepairButton);
            secondButtons.Controls.Add(RemoveButton);
            secondButtons.Controls.Add(openWord);

            var note = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopCenter,
                Padding = new Padding(10, 16, 10, 0),
                ForeColor = Color.DimGray,
                Text = "التثبيت يتم للحساب الحالي ولا يحتاج صلاحيات Administrator.\n"
                     + "البرنامج ينسخ الإضافة إلى LocalAppData ويسجل COM تلقائيًا ثم يتحقق من إمكانية تشغيلها.
بعد التثبيت تظهر تبويبتا WordPro Suite Pro وWordPro Enterprise داخل Word."
            };

            body.Controls.Add(note);
            body.Controls.Add(secondButtons);
            body.Controls.Add(firstButtons);
            body.Controls.Add(Progress);
            body.Controls.Add(SerialBox);
            body.Controls.Add(serialLabel);
            body.Controls.Add(machinePanel);
            body.Controls.Add(Status);

            Controls.Add(body);
            Controls.Add(hero);
            Shown += delegate { RefreshStatus(); };
        }

        private static Button MakeButton(string text, Color color, int width)
        {
            var button = new Button
            {
                Text = text,
                Width = width,
                Height = 52,
                Margin = new Padding(8),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold)
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void RunOperation(Action action)
        {
            try
            {
                ToggleBusy(true);
                action();
                RefreshStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "WordPro Suite Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ToggleBusy(false);
            }
        }

        private void ToggleBusy(bool busy)
        {
            Progress.Visible = busy;
            InstallTrialButton.Enabled = !busy;
            InstallActivateButton.Enabled = !busy;
            ActivateButton.Enabled = !busy;
            RepairButton.Enabled = !busy;
            RemoveButton.Enabled = !busy;
            Application.DoEvents();
        }

        private void RefreshStatus()
        {
            bool installed = SetupEngine.IsInstalled;
            string license = LicenseTools.StatusText;
            Status.Text = installed
                ? "مثبت وجاهز — " + license
                : "غير مثبت — " + license;
        }
    }

    internal static class SetupEngine
    {
        internal const string ProgId = "WordProSuite.Desktop.AddIn";
        internal const string Clsid = "{79D9E91D-88D5-4C41-B805-82D64D1348B2}";

        private static string InstalledDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "WordProSuite", "Desktop");
            }
        }

        internal static string InstalledDll
        {
            get { return Path.Combine(InstalledDirectory, "WordProSuite.AddIn.dll"); }
        }

        private static RegistryView[] Views
        {
            get
            {
                return Environment.Is64BitOperatingSystem
                    ? new[] { RegistryView.Registry64, RegistryView.Registry32 }
                    : new[] { RegistryView.Registry32 };
            }
        }

        internal static bool IsInstalled
        {
            get
            {
                return File.Exists(InstalledDll) && Views.Any(IsRegistered);
            }
        }

        internal static void Install(string serial)
        {
            StopWord();
            Directory.CreateDirectory(InstalledDirectory);
            ExtractEmbeddedAddIn(InstalledDll);

            foreach (RegistryView view in Views)
            {
                RegisterView(view);
                ClearOfficeDisableState(view);
            }

            if (!Views.Any(IsRegistered))
                throw new InvalidOperationException("فشل تسجيل COM.");

            VerifyComActivation();

            if (!String.IsNullOrWhiteSpace(serial))
            {
                string message;
                if (!LicenseTools.Activate(serial, out message))
                    throw new InvalidOperationException(message);
            }
        }

        internal static void Uninstall()
        {
            StopWord();
            foreach (RegistryView view in Views) UnregisterView(view);
            try
            {
                if (Directory.Exists(InstalledDirectory))
                    Directory.Delete(InstalledDirectory, true);
            }
            catch { }
        }

        private static void ExtractEmbeddedAddIn(string output)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("WordProSuite.AddIn.dll"))
            {
                if (stream == null)
                    throw new InvalidOperationException("ملف الإضافة غير مضمّن داخل Setup.exe. أعد بناء الإصدار.");
                using (FileStream file = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None))
                    stream.CopyTo(file);
            }
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
                    clsid.SetValue(null, "WordPro Suite Desktop Ultimate Add-in");
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

                    using (RegistryKey progId = clsid.CreateSubKey("ProgId"))
                        progId.SetValue(null, ProgId);
                }

                using (RegistryKey prog = root.CreateSubKey(@"Software\Classes\" + ProgId))
                {
                    prog.SetValue(null, "WordPro Suite Desktop Ultimate Add-in");
                    using (RegistryKey c = prog.CreateSubKey("CLSID"))
                        c.SetValue(null, Clsid);
                }

                using (RegistryKey addin = root.CreateSubKey(@"Software\Microsoft\Office\Word\Addins\" + ProgId))
                {
                    addin.SetValue("FriendlyName", "WordPro Suite Desktop Ultimate 3.0");
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
                {
                    return key != null &&
                        String.Equals(Convert.ToString(key.GetValue(null)), "mscoree.dll",
                            StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                return false;
            }
        }

        private static void VerifyComActivation()
        {
            Type type = Type.GetTypeFromProgID(ProgId, true);
            object instance = Activator.CreateInstance(type);
            if (instance == null)
                throw new InvalidOperationException("فشل اختبار تشغيل COM.");
            try
            {
                if (Marshal.IsComObject(instance))
                    Marshal.FinalReleaseComObject(instance);
            }
            catch { }
        }

        private static void DeleteTree(RegistryKey root, string path)
        {
            try { root.DeleteSubKeyTree(path, false); } catch { }
        }

        private static void StopWord()
        {
            foreach (Process process in Process.GetProcessesByName("WINWORD"))
            {
                try
                {
                    process.Kill();
                    process.WaitForExit(3000);
                }
                catch { }
            }
        }

        internal static void StartWord()
        {
            try { Process.Start("winword.exe"); }
            catch { MessageBox.Show("تعذر فتح Word. افتحه من قائمة ابدأ."); }
        }
    }

    internal sealed class SetupLicenseState
    {
        internal bool IsLicensed;
        internal string Customer;
        internal string Edition;
        internal DateTime? Expires;
        internal string Message;
    }

    internal static class LicenseTools
    {
        private const string Prefix = "WPS2";
        private const string Product = "WPSD";
        private const string RegistryPath = @"Software\WordProSuite\Desktop\License";
        private const string PublicKeyXml = @"<RSAKeyValue><Modulus>mMvHSxwL3pPu8NM8M1FlZiixGHiqL+SxTLChr+rPL5Tlxi0czQwr1YrhJR+1EyJq++VjClXCFy7M0fFnRViB4k1WRy08jhXEUuJh17Rq5KMyeaE6Q9UtgEOwMG2XrtFhMLB09WOw1V32+ZDiiR3Fhc9E+TNI/VBmJDU1AQUqzTFu1yaEPvlr5nW4XrY36o7jG5WefmM0bDruyRqZmc6GQ3SODi5X9JGnHOL1DgWOIlxA7jknioQ9MNy7TDJZbLG44ybWZk8VA851Aifoq6PhKmWFOAkUX+BgM4pn/p4MSNmZJ0lq2wrsDCbmiPXu+2Bgqz9JMSYhwu8Kp99oVw/r5Q==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        internal static string MachineId
        {
            get
            {
                string machineGuid = "";
                try
                {
                    using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                        .OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
                    {
                        machineGuid = Convert.ToString(key == null ? null : key.GetValue("MachineGuid"));
                    }
                }
                catch { }

                string source = (String.IsNullOrWhiteSpace(machineGuid)
                    ? Environment.MachineName
                    : machineGuid) + "|WPSD2";

                using (SHA256 sha = SHA256.Create())
                {
                    string hex = BitConverter.ToString(
                        sha.ComputeHash(Encoding.UTF8.GetBytes(source))).Replace("-", "");
                    return hex.Substring(0, 6) + "-" + hex.Substring(6, 6) + "-"
                         + hex.Substring(12, 6) + "-" + hex.Substring(18, 6);
                }
            }
        }

        internal static string StatusText
        {
            get
            {
                string serial = ReadProtected("Serial");
                SetupLicenseState state;
                if (!String.IsNullOrWhiteSpace(serial) && TryValidate(serial, out state))
                {
                    string expiry = state.Expires.HasValue
                        ? state.Expires.Value.ToString("yyyy-MM-dd")
                        : "دائم";
                    return "مفعّل — " + state.Customer + " — " + expiry;
                }
                return "غير مفعّل أو نسخة تجريبية";
            }
        }

        internal static bool Activate(string serial, out string message)
        {
            SetupLicenseState state;
            if (!TryValidate(serial, out state))
            {
                message = state == null ? "رقم التفعيل غير صالح." : state.Message;
                return false;
            }

            WriteProtected("Serial", serial.Trim());
            message = "تم التفعيل بنجاح باسم: " + state.Customer;
            return true;
        }

        private static bool TryValidate(string serial, out SetupLicenseState state)
        {
            state = new SetupLicenseState { Message = "رقم التفعيل غير صالح." };

            try
            {
                string[] parts = (serial ?? "").Trim().Split('.');
                if (parts.Length != 3 || parts[0] != Prefix)
                {
                    state.Message = "صيغة Serial Number غير صحيحة.";
                    return false;
                }

                byte[] payloadBytes = Base64UrlDecode(parts[1]);
                byte[] signature = Base64UrlDecode(parts[2]);

                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.PersistKeyInCsp = false;
                    rsa.FromXmlString(PublicKeyXml);

                    if (!rsa.VerifyData(payloadBytes,
                        CryptoConfig.MapNameToOID("SHA256"), signature))
                    {
                        state.Message = "توقيع الترخيص غير صحيح.";
                        return false;
                    }
                }

                string payload = Encoding.UTF8.GetString(payloadBytes);
                string[] fields = payload.Split('|');

                if (fields.Length != 7 || fields[0] != "2" || fields[1] != Product)
                {
                    state.Message = "بيانات الترخيص غير متوافقة.";
                    return false;
                }

                string customer = Encoding.UTF8.GetString(Base64UrlDecode(fields[2]));
                string machine = fields[3];
                string expiryRaw = fields[4];
                string edition = fields[5];

                if (!String.Equals(machine, "ANY", StringComparison.OrdinalIgnoreCase) &&
                    !String.Equals(machine, MachineId, StringComparison.OrdinalIgnoreCase))
                {
                    state.Message = "هذا Serial Number مخصص لجهاز آخر.\nمعرّف هذا الجهاز:\n" + MachineId;
                    return false;
                }

                DateTime? expiry = null;
                if (!String.Equals(expiryRaw, "NEVER", StringComparison.OrdinalIgnoreCase))
                {
                    DateTime parsed;
                    if (!DateTime.TryParseExact(expiryRaw, "yyyyMMdd",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        state.Message = "تاريخ انتهاء الترخيص غير صالح.";
                        return false;
                    }

                    expiry = parsed.Date.AddDays(1).AddTicks(-1);

                    if (DateTime.Now > expiry.Value)
                    {
                        state.Message = "انتهت صلاحية الترخيص.";
                        return false;
                    }
                }

                state = new SetupLicenseState
                {
                    IsLicensed = true,
                    Customer = customer,
                    Edition = edition,
                    Expires = expiry,
                    Message = "الترخيص صالح."
                };
                return true;
            }
            catch (Exception ex)
            {
                state.Message = "تعذر قراءة الترخيص: " + ex.Message;
                return false;
            }
        }

        private static void WriteProtected(string name, string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value ?? "");
            byte[] protectedBytes = ProtectedData.Protect(
                data, Entropy(), DataProtectionScope.CurrentUser);

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryPath))
                key.SetValue(name, Convert.ToBase64String(protectedBytes),
                    RegistryValueKind.String);
        }

        private static string ReadProtected(string name)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath))
                {
                    string encoded = key == null ? null : key.GetValue(name) as string;
                    if (String.IsNullOrWhiteSpace(encoded))
                        return null;

                    byte[] protectedBytes = Convert.FromBase64String(encoded);
                    byte[] data = ProtectedData.Unprotect(
                        protectedBytes, Entropy(), DataProtectionScope.CurrentUser);
                    return Encoding.UTF8.GetString(data);
                }
            }
            catch
            {
                return null;
            }
        }

        private static byte[] Entropy()
        {
            return Encoding.UTF8.GetBytes("WordProSuite.Desktop.License.v2");
        }

        private static byte[] Base64UrlDecode(string value)
        {
            string data = (value ?? "").Replace('-', '+').Replace('_', '/');
            switch (data.Length % 4)
            {
                case 2: data += "=="; break;
                case 3: data += "="; break;
            }
            return Convert.FromBase64String(data);
        }
    }
}
