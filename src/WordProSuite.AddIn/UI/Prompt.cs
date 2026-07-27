using System.Drawing;
using System.Windows.Forms;

namespace WordProSuite.Desktop.UI
{
    internal static class Prompt
    {
        internal static string Show(string title, string label, string initial = "", bool password = false)
        {
            using (var form = new Form
            {
                Text = title,
                RightToLeft = RightToLeft.Yes,
                RightToLeftLayout = true,
                StartPosition = FormStartPosition.CenterScreen,
                ClientSize = new Size(560, 190),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                Font = new Font("Segoe UI", 11f)
            })
            {
                var text = new TextBox
                {
                    Text = initial ?? "",
                    RightToLeft = password ? RightToLeft.No : RightToLeft.Yes,
                    UseSystemPasswordChar = password,
                    Dock = DockStyle.Top,
                    Height = 34,
                    Margin = new Padding(18)
                };
                var lbl = new Label { Text = label, Dock = DockStyle.Top, Height = 48, Padding = new Padding(12), TextAlign = ContentAlignment.MiddleRight };
                var panel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 68, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(12) };
                var ok = new Button { Text = "موافق", DialogResult = DialogResult.OK, Width = 120, Height = 40 };
                var cancel = new Button { Text = "إلغاء", DialogResult = DialogResult.Cancel, Width = 120, Height = 40 };
                panel.Controls.Add(ok); panel.Controls.Add(cancel);
                form.Controls.Add(panel); form.Controls.Add(text); form.Controls.Add(lbl);
                form.AcceptButton = ok; form.CancelButton = cancel;
                return form.ShowDialog() == DialogResult.OK ? text.Text : null;
            }
        }
    }
}
