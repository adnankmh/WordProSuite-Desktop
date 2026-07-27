using Microsoft.Win32;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace WordProSuite.Desktop.Licensing
{
    internal sealed class LicenseState
    {
        internal bool IsLicensed { get; set; }
        internal bool IsTrial { get; set; }
        internal int TrialDaysLeft { get; set; }
        internal string Customer { get; set; }
        internal string Edition { get; set; }
        internal DateTime? Expires { get; set; }
        internal string Message { get; set; }
    }

    internal static class LicenseManager
    {
        private const string Product = "WPSD";
        private const string Prefix = "WPS2";
        private const int TrialDays = 14;
        private const string RegistryPath = @"Software\WordProSuite\Desktop\License";
        private const string PublicKeyXml = @"<RSAKeyValue><Modulus>mMvHSxwL3pPu8NM8M1FlZiixGHiqL+SxTLChr+rPL5Tlxi0czQwr1YrhJR+1EyJq++VjClXCFy7M0fFnRViB4k1WRy08jhXEUuJh17Rq5KMyeaE6Q9UtgEOwMG2XrtFhMLB09WOw1V32+ZDiiR3Fhc9E+TNI/VBmJDU1AQUqzTFu1yaEPvlr5nW4XrY36o7jG5WefmM0bDruyRqZmc6GQ3SODi5X9JGnHOL1DgWOIlxA7jknioQ9MNy7TDJZbLG44ybWZk8VA851Aifoq6PhKmWFOAkUX+BgM4pn/p4MSNmZJ0lq2wrsDCbmiPXu+2Bgqz9JMSYhwu8Kp99oVw/r5Q==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        internal static void Initialize()
        {
            try { EnsureTrialStart(); } catch { }
        }

        internal static string MachineId => MachineFingerprint.Create();

        internal static LicenseState Current
        {
            get
            {
                try
                {
                    string serial = ReadProtected("Serial");
                    if (!String.IsNullOrWhiteSpace(serial))
                    {
                        LicenseState licensed;
                        if (TryValidate(serial, out licensed)) return licensed;
                    }
                }
                catch { }
                return TrialState();
            }
        }

        internal static string StatusText
        {
            get
            {
                var s = Current;
                if (s.IsLicensed)
                {
                    string expiry = s.Expires.HasValue ? s.Expires.Value.ToString("yyyy-MM-dd") : "دائم";
                    return "مفعّل — " + (String.IsNullOrWhiteSpace(s.Customer) ? "مستخدم مرخّص" : s.Customer) + " — " + expiry;
                }
                if (s.IsTrial) return "نسخة تجريبية — متبقّي " + s.TrialDaysLeft + " يوم";
                return "غير مفعّل";
            }
        }

        internal static bool EnsureLicensedWithUi()
        {
            var state = Current;
            if (state.IsLicensed || state.IsTrial) return true;
            using (var form = new ActivationForm())
            {
                form.ShowDialog();
            }
            state = Current;
            return state.IsLicensed || state.IsTrial;
        }

        internal static bool Activate(string serial, out string message)
        {
            LicenseState state;
            if (!TryValidate(serial, out state))
            {
                message = state == null ? "رقم التفعيل غير صالح." : state.Message;
                return false;
            }
            WriteProtected("Serial", serial.Trim());
            message = "تم التفعيل بنجاح باسم: " + state.Customer;
            return true;
        }

        internal static void Deactivate()
        {
            using (var key = Registry.CurrentUser.CreateSubKey(RegistryPath))
                key.DeleteValue("Serial", false);
        }

        internal static bool TryValidate(string serial, out LicenseState state)
        {
            state = new LicenseState { IsLicensed = false, IsTrial = false, Message = "رقم التفعيل غير صالح." };
            try
            {
                string[] parts = (serial ?? "").Trim().Split('.');
                if (parts.Length != 3 || !String.Equals(parts[0], Prefix, StringComparison.Ordinal))
                {
                    state.Message = "صيغة رقم التفعيل غير صحيحة.";
                    return false;
                }

                byte[] payloadBytes = Base64UrlDecode(parts[1]);
                byte[] signature = Base64UrlDecode(parts[2]);
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.PersistKeyInCsp = false;
                    rsa.FromXmlString(PublicKeyXml);
                    if (!rsa.VerifyData(payloadBytes, CryptoConfig.MapNameToOID("SHA256"), signature))
                    {
                        state.Message = "توقيع الترخيص غير صحيح.";
                        return false;
                    }
                }

                string payload = Encoding.UTF8.GetString(payloadBytes);
                string[] fields = payload.Split('|');
                if (fields.Length != 7 || fields[0] != "2" || fields[1] != Product)
                {
                    state.Message = "بيانات الترخيص غير متوافقة مع المنتج.";
                    return false;
                }

                string customer = Encoding.UTF8.GetString(Base64UrlDecode(fields[2]));
                string machine = fields[3];
                string expiryRaw = fields[4];
                string edition = fields[5];

                if (!String.Equals(machine, "ANY", StringComparison.OrdinalIgnoreCase) &&
                    !String.Equals(machine, MachineId, StringComparison.OrdinalIgnoreCase))
                {
                    state.Message = "هذا الترخيص مخصص لجهاز آخر.\nمعرّف هذا الجهاز: " + MachineId;
                    return false;
                }

                DateTime? expiry = null;
                if (!String.Equals(expiryRaw, "NEVER", StringComparison.OrdinalIgnoreCase))
                {
                    DateTime parsed;
                    if (!DateTime.TryParseExact(expiryRaw, "yyyyMMdd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out parsed))
                    {
                        state.Message = "تاريخ انتهاء الترخيص غير صالح.";
                        return false;
                    }
                    expiry = parsed.Date.AddDays(1).AddTicks(-1);
                    if (DateTime.Now > expiry.Value)
                    {
                        state.Message = "انتهت صلاحية الترخيص بتاريخ " + parsed.ToString("yyyy-MM-dd") + ".";
                        return false;
                    }
                }

                state = new LicenseState
                {
                    IsLicensed = true,
                    IsTrial = false,
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

        private static LicenseState TrialState()
        {
            DateTime start = EnsureTrialStart();
            int used = Math.Max(0, (int)Math.Floor((DateTime.UtcNow.Date - start.Date).TotalDays));
            int left = TrialDays - used;
            if (left > 0)
            {
                return new LicenseState
                {
                    IsLicensed = false,
                    IsTrial = true,
                    TrialDaysLeft = left,
                    Edition = "TRIAL",
                    Message = "الفترة التجريبية فعالة."
                };
            }
            return new LicenseState
            {
                IsLicensed = false,
                IsTrial = false,
                TrialDaysLeft = 0,
                Edition = "UNLICENSED",
                Message = "انتهت الفترة التجريبية."
            };
        }

        private static DateTime EnsureTrialStart()
        {
            string saved = ReadProtected("TrialStart");
            DateTime dt;
            if (DateTime.TryParseExact(saved, "yyyyMMdd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out dt)) return dt;
            dt = DateTime.UtcNow.Date;
            WriteProtected("TrialStart", dt.ToString("yyyyMMdd"));
            return dt;
        }

        private static string ReadProtected(string name)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath))
            {
                string encoded = key == null ? null : key.GetValue(name) as string;
                if (String.IsNullOrWhiteSpace(encoded)) return null;
                byte[] protectedBytes = Convert.FromBase64String(encoded);
                byte[] bytes = ProtectedData.Unprotect(protectedBytes, Entropy(), DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(bytes);
            }
        }

        private static void WriteProtected(string name, string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value ?? "");
            byte[] protectedBytes = ProtectedData.Protect(bytes, Entropy(), DataProtectionScope.CurrentUser);
            using (var key = Registry.CurrentUser.CreateSubKey(RegistryPath))
                key.SetValue(name, Convert.ToBase64String(protectedBytes), RegistryValueKind.String);
        }

        private static byte[] Entropy() => Encoding.UTF8.GetBytes("WordProSuite.Desktop.License.v2");

        internal static string Base64UrlEncode(byte[] bytes) =>
            Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        internal static byte[] Base64UrlDecode(string value)
        {
            string s = (value ?? "").Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4) { case 2: s += "=="; break; case 3: s += "="; break; }
            return Convert.FromBase64String(s);
        }
    }

    internal static class MachineFingerprint
    {
        internal static string Create()
        {
            string machineGuid = "";
            try
            {
                using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                    .OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
                    machineGuid = Convert.ToString(key == null ? null : key.GetValue("MachineGuid"));
            }
            catch { }
            string source = (String.IsNullOrWhiteSpace(machineGuid) ? Environment.MachineName : machineGuid) + "|WPSD2";
            using (var sha = SHA256.Create())
            {
                string hex = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(source))).Replace("-", "");
                return hex.Substring(0, 6) + "-" + hex.Substring(6, 6) + "-" + hex.Substring(12, 6) + "-" + hex.Substring(18, 6);
            }
        }
    }
}
