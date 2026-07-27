using System;
using System.Drawing;
using System.Windows.Forms;

namespace WordProSuite.Desktop.Licensing
{
    internal sealed class ActivationForm : Form
    {
        private readonly TextBox SerialBox = new TextBox();
        private readonly Label Status = new Label();

        internal ActivationForm()
        {
            Text = "تفعيل WordPro Suite Desktop";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(760, 520);
            MinimumSize = new Size(660, 470);
            Font = new Font("Segoe UI", 11f);

            var header = new Panel { Dock = DockStyle.Top, Height = 92, Padding = new Padding(24, 16, 24, 8) };
            var title = new Label
            {
                Text = "تفعيل WordPro Suite Desktop Pro",
                Dock = DockStyle.Top,
                Height = 38,
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };
            var subtitle = new Label
            {
                Text = "أرسل معرّف الجهاز إلى مالك البرنامج للحصول على Serial Number موقّع.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight
            };
            header.Controls.Add(subtitle);
            header.Controls.Add(title);

            var body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24) };
            var machineLabel = new Label { Text = "معرّف الجهاز", Dock = DockStyle.Top, Height = 26, Font = new Font("Segoe UI", 11f, FontStyle.Bold) };
            var machinePanel = new Panel { Dock = DockStyle.Top, Height = 56 };
            var machine = new TextBox { Text = LicenseManager.MachineId, ReadOnly = true, Dock = DockStyle.Fill, Font = new Font("Consolas", 12f), TextAlign = HorizontalAlignment.Center };
            var copy = new Button { Text = "نسخ", Dock = DockStyle.Left, Width = 110 };
            copy.Click += (s, e) => { Clipboard.SetText(LicenseManager.MachineId); MessageBox.Show("تم نسخ معرّف الجهاز.", "WordPro Suite"); };
            machinePanel.Controls.Add(machine);
            machinePanel.Controls.Add(copy);

            var serialLabel = new Label { Text = "Serial Number", Dock = DockStyle.Top, Height = 32, Padding = new Padding(0, 8, 0, 0), Font = new Font("Segoe UI", 11f, FontStyle.Bold) };
            SerialBox.Dock = DockStyle.Top;
            SerialBox.Height = 150;
            SerialBox.Multiline = true;
            SerialBox.ScrollBars = ScrollBars.Vertical;
            SerialBox.Font = new Font("Consolas", 10.5f);
            SerialBox.RightToLeft = RightToLeft.No;

            Status.Dock = DockStyle.Top;
            Status.Height = 62;
            Status.Padding = new Padding(0, 12, 0, 0);
            Status.TextAlign = ContentAlignment.MiddleRight;
            Status.Text = LicenseManager.StatusText;

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 72, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 10, 0, 0) };
            var activate = new Button { Text = "تفعيل الآن", Width = 155, Height = 48, Font = new Font("Segoe UI", 12f, FontStyle.Bold) };
            var info = new Button { Text = "حالة الترخيص", Width = 150, Height = 48 };
            var close = new Button { Text = "إغلاق", Width = 120, Height = 48 };
            activate.Click += ActivateClick;
            info.Click += (s, e) => MessageBox.Show(LicenseManager.StatusText + "\n\nمعرّف الجهاز:\n" + LicenseManager.MachineId, "حالة الترخيص");
            close.Click += (s, e) => Close();
            buttons.Controls.Add(activate);
            buttons.Controls.Add(info);
            buttons.Controls.Add(close);

            body.Controls.Add(buttons);
            body.Controls.Add(Status);
            body.Controls.Add(SerialBox);
            body.Controls.Add(serialLabel);
            body.Controls.Add(machinePanel);
            body.Controls.Add(machineLabel);
            Controls.Add(body);
            Controls.Add(header);
        }

        private void ActivateClick(object sender, EventArgs e)
        {
            string message;
            if (LicenseManager.Activate(SerialBox.Text, out message))
            {
                Status.Text = LicenseManager.StatusText;
                MessageBox.Show(message, "WordPro Suite", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                Status.Text = message;
                MessageBox.Show(message, "تعذر التفعيل", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        internal static void ShowActivation()
        {
            using (var form = new ActivationForm()) form.ShowDialog();
        }
    }
}
