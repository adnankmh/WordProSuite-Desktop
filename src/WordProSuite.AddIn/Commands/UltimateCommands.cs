using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WordProSuite.Desktop.Commands
{
    internal static class UltimateCommands
    {
        private const float PointsPerCentimeter = 28.3464567f;
        private static readonly Regex WordRegex = new Regex(@"[\p{L}\p{N}]+", RegexOptions.Compiled);
        private static readonly Regex SentenceRegex = new Regex(@"[^.!؟!?\r\n]+[.!؟!?]?", RegexOptions.Compiled);

        private static dynamic Target()
        {
            return WordContext.TargetRange;
        }

        private static dynamic Document()
        {
            return WordContext.Document;
        }

        private static float Cm(float value)
        {
            return value * PointsPerCentimeter;
        }

        // -----------------------------------------------------------------
        // Global typography presets
        // -----------------------------------------------------------------
        internal static void ApplyFontPreset(string fontName, float size)
        {
            dynamic range = Target();
            range.Font.Name = fontName;
            range.Font.NameBi = fontName;
            range.Font.Size = size;
            range.Font.SizeBi = size;
        }

        internal static void SetBold(bool enabled) { Target().Font.Bold = enabled ? -1 : 0; }
        internal static void SetItalic(bool enabled) { Target().Font.Italic = enabled ? -1 : 0; }
        internal static void SetUnderline(bool enabled) { Target().Font.Underline = enabled ? 1 : 0; }
        internal static void SetStrike(bool enabled) { Target().Font.StrikeThrough = enabled ? -1 : 0; }
        internal static void SetAllCaps(bool enabled) { Target().Font.AllCaps = enabled ? -1 : 0; }
        internal static void SetSmallCaps(bool enabled) { Target().Font.SmallCaps = enabled ? -1 : 0; }
        internal static void SetSuperscript(bool enabled)
        {
            dynamic font = Target().Font;
            font.Subscript = 0;
            font.Superscript = enabled ? -1 : 0;
        }
        internal static void SetSubscript(bool enabled)
        {
            dynamic font = Target().Font;
            font.Superscript = 0;
            font.Subscript = enabled ? -1 : 0;
        }

        internal static void SetFontColor(int red, int green, int blue)
        {
            Target().Font.Color = ColorTranslator.ToOle(Color.FromArgb(red, green, blue));
        }

        internal static void SetHighlight(int colorIndex)
        {
            Target().HighlightColorIndex = colorIndex;
        }

        internal static void ClearTextEffects()
        {
            dynamic font = Target().Font;
            font.Bold = 0;
            font.Italic = 0;
            font.Underline = 0;
            font.StrikeThrough = 0;
            font.AllCaps = 0;
            font.SmallCaps = 0;
            font.Superscript = 0;
            font.Subscript = 0;
            font.Color = -16777216;
            Target().HighlightColorIndex = 0;
        }

        // -----------------------------------------------------------------
        // Precise paragraph engineering
        // -----------------------------------------------------------------
        internal static void SetParagraphSpacing(float before, float after)
        {
            dynamic paragraph = Target().ParagraphFormat;
            paragraph.SpaceBefore = before;
            paragraph.SpaceAfter = after;
        }

        internal static void SetLeftIndentCm(float cm)
        {
            Target().ParagraphFormat.LeftIndent = Cm(cm);
        }

        internal static void SetRightIndentCm(float cm)
        {
            Target().ParagraphFormat.RightIndent = Cm(cm);
        }

        internal static void SetFirstLineIndentCm(float cm)
        {
            Target().ParagraphFormat.FirstLineIndent = Cm(cm);
        }

        internal static void SetHangingIndentCm(float cm)
        {
            Target().ParagraphFormat.FirstLineIndent = -Cm(cm);
        }

        internal static void SetExactLineSpacing(float points)
        {
            dynamic paragraph = Target().ParagraphFormat;
            paragraph.LineSpacingRule = 4;
            paragraph.LineSpacing = points;
        }

        internal static void SetKeepTogether(bool enabled)
        {
            Target().ParagraphFormat.KeepTogether = enabled ? -1 : 0;
        }

        // -----------------------------------------------------------------
        // Advanced page layout
        // -----------------------------------------------------------------
        internal static void SetUniformMarginsCm(float cm)
        {
            SetCustomMarginsCm(cm, cm, cm, cm);
        }

        internal static void SetCustomMarginsCm(float top, float bottom, float left, float right)
        {
            dynamic setup = Document().PageSetup;
            setup.TopMargin = Cm(top);
            setup.BottomMargin = Cm(bottom);
            setup.LeftMargin = Cm(left);
            setup.RightMargin = Cm(right);
        }

        internal static void SetPaperSize(int paperSize)
        {
            Document().PageSetup.PaperSize = paperSize;
        }

        internal static void SetOrientation(int orientation)
        {
            Document().PageSetup.Orientation = orientation;
        }

        internal static void SetColumns(int count)
        {
            if (count < 1 || count > 6) throw new ArgumentOutOfRangeException("count");
            Document().PageSetup.TextColumns.SetCount(count);
        }

        internal static void SetLineNumbering(int mode)
        {
            dynamic numbering = Document().PageSetup.LineNumbering;
            if (mode < 0)
            {
                numbering.Active = 0;
                return;
            }
            numbering.Active = -1;
            numbering.CountBy = 1;
            numbering.StartingNumber = 1;
            numbering.RestartMode = mode;
        }

        // -----------------------------------------------------------------
        // Dates, time and reference IDs
        // -----------------------------------------------------------------
        internal static void InsertDateFormat(string format, string cultureName)
        {
            CultureInfo culture = String.IsNullOrWhiteSpace(cultureName)
                ? CultureInfo.InvariantCulture
                : CultureInfo.GetCultureInfo(cultureName);
            WordContext.Selection.TypeText(DateTime.Now.ToString(format, culture));
        }

        internal static void InsertTimeFormat(string format)
        {
            WordContext.Selection.TypeText(DateTime.Now.ToString(format, CultureInfo.InvariantCulture));
        }

        internal static void InsertReferenceCode(string prefix)
        {
            string safePrefix = String.IsNullOrWhiteSpace(prefix) ? "REF" : prefix.Trim().ToUpperInvariant();
            string value = safePrefix + "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture) + "-" +
                           Guid.NewGuid().ToString("N").Substring(0, 6).ToUpperInvariant();
            WordContext.Selection.TypeText(value);
        }

        // -----------------------------------------------------------------
        // Lists, symbols and wrappers
        // -----------------------------------------------------------------
        internal static void PrefixParagraphs(string prefix)
        {
            dynamic range = Target();
            string text = Convert.ToString(range.Text) ?? "";
            string[] lines = TextTransforms.Lines(text);
            for (int i = 0; i < lines.Length; i++)
            {
                string clean = lines[i].Trim();
                lines[i] = clean.Length == 0 ? "" : prefix + clean;
            }
            range.Text = String.Join("\r", lines);
        }

        internal static void WrapSelection(string left, string right)
        {
            dynamic range = Target();
            string text = (Convert.ToString(range.Text) ?? "").TrimEnd('\r', '\a');
            range.Text = left + text + right;
        }

        internal static void FilterText(string mode)
        {
            dynamic range = Target();
            string text = Convert.ToString(range.Text) ?? "";
            string output;
            switch ((mode ?? "").ToLowerInvariant())
            {
                case "remove-digits": output = Regex.Replace(text, @"\p{N}+", ""); break;
                case "keep-digits": output = String.Concat(text.Where(delegate(char c) { return Char.IsDigit(c); })); break;
                case "remove-latin": output = Regex.Replace(text, @"[A-Za-z]+", ""); break;
                case "remove-arabic": output = Regex.Replace(text, @"[\u0600-\u06FF]+", ""); break;
                case "keep-letters": output = String.Concat(text.Where(delegate(char c) { return Char.IsLetter(c) || Char.IsWhiteSpace(c); })); break;
                case "keep-alphanumeric": output = String.Concat(text.Where(delegate(char c) { return Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c); })); break;
                case "remove-brackets": output = Regex.Replace(text, @"[\[\](){}<>«»﴿﴾]", ""); break;
                case "slashes-to-dashes": output = text.Replace("/", "-").Replace("\\", "-"); break;
                default: throw new InvalidOperationException("مرشح النص غير معروف: " + mode);
            }
            range.Text = output;
        }

        // -----------------------------------------------------------------
        // Enterprise-ready table templates
        // -----------------------------------------------------------------
        internal static void InsertBusinessTable(string title, string[] headers, int dataRows)
        {
            if (headers == null || headers.Length == 0) throw new ArgumentException("headers");
            dataRows = Math.Max(1, Math.Min(100, dataRows));

            dynamic selection = WordContext.Selection;
            selection.TypeText(title + "\r");
            dynamic range = selection.Range;
            dynamic table = Document().Tables.Add(range, dataRows + 1, headers.Length);
            table.Borders.Enable = -1;
            table.AutoFitBehavior(2);
            table.Range.ParagraphFormat.ReadingOrder = 0;
            table.Range.ParagraphFormat.Alignment = 1;
            table.Range.Font.Name = "Arial";
            table.Range.Font.NameBi = "Arial";
            table.Range.Font.Size = 10;
            table.Range.Font.SizeBi = 10;

            for (int i = 0; i < headers.Length; i++)
            {
                dynamic cell = table.Cell(1, i + 1);
                cell.Range.Text = headers[i];
                cell.Range.Font.Bold = -1;
                cell.Shading.BackgroundPatternColor = 14277081;
            }
            table.Rows[1].HeadingFormat = -1;
            selection.SetRange(table.Range.End, table.Range.End);
            selection.TypeParagraph();
        }

        // -----------------------------------------------------------------
        // Document intelligence and analytics
        // -----------------------------------------------------------------
        internal static void ShowMetric(string metric)
        {
            dynamic document = Document();
            string key = (metric ?? "").ToLowerInvariant();
            string title;
            string value;

            switch (key)
            {
                case "words": title = "عدد الكلمات"; value = Convert.ToString(document.ComputeStatistics(0)); break;
                case "characters": title = "عدد الأحرف"; value = Convert.ToString(document.ComputeStatistics(3)); break;
                case "paragraphs": title = "عدد الفقرات"; value = Convert.ToString(document.Paragraphs.Count); break;
                case "pages": title = "عدد الصفحات"; value = Convert.ToString(document.ComputeStatistics(2)); break;
                case "tables": title = "عدد الجداول"; value = Convert.ToString(document.Tables.Count); break;
                case "images": title = "عدد الصور والأشكال"; value = Convert.ToString(document.InlineShapes.Count + document.Shapes.Count); break;
                case "hyperlinks": title = "عدد الروابط"; value = Convert.ToString(document.Hyperlinks.Count); break;
                case "comments": title = "عدد التعليقات"; value = Convert.ToString(document.Comments.Count); break;
                case "revisions": title = "عدد التعديلات المتعقبة"; value = Convert.ToString(document.Revisions.Count); break;
                case "bookmarks": title = "عدد الإشارات المرجعية"; value = Convert.ToString(document.Bookmarks.Count); break;
                case "sections": title = "عدد المقاطع"; value = Convert.ToString(document.Sections.Count); break;
                case "footnotes": title = "الحواشي السفلية"; value = Convert.ToString(document.Footnotes.Count); break;
                case "endnotes": title = "الحواشي الختامية"; value = Convert.ToString(document.Endnotes.Count); break;
                case "fields": title = "عدد الحقول"; value = CountFields(document).ToString(CultureInfo.InvariantCulture); break;
                case "headings": title = "عدد العناوين"; value = CountHeadings(document).ToString(CultureInfo.InvariantCulture); break;
                case "readability": ShowReadability(); return;
                default: throw new InvalidOperationException("المؤشر غير معروف: " + metric);
            }

            MessageBox.Show(title + ": " + value, "تحليل المستند", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static int CountFields(dynamic document)
        {
            int count = 0;
            foreach (dynamic story in document.StoryRanges)
            {
                dynamic current = story;
                while (current != null)
                {
                    count += current.Fields.Count;
                    current = current.NextStoryRange;
                }
            }
            return count;
        }

        private static int CountHeadings(dynamic document)
        {
            int count = 0;
            foreach (dynamic paragraph in document.Paragraphs)
            {
                string style = "";
                try { style = Convert.ToString(paragraph.Range.Style); } catch { }
                if (style.IndexOf("Heading", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    style.IndexOf("عنوان", StringComparison.CurrentCultureIgnoreCase) >= 0)
                    count++;
            }
            return count;
        }

        private static void ShowReadability()
        {
            string text = Convert.ToString(Target().Text) ?? "";
            int words = WordRegex.Matches(text).Count;
            int sentences = Math.Max(1, SentenceRegex.Matches(text).Cast<Match>().Count(delegate(Match m) { return m.Value.Trim().Length > 0; }));
            int letters = text.Count(delegate(char c) { return Char.IsLetter(c); });
            double wordsPerSentence = words == 0 ? 0 : (double)words / sentences;
            double lettersPerWord = words == 0 ? 0 : (double)letters / words;
            var builder = new StringBuilder();
            builder.AppendLine("تحليل قابلية القراءة");
            builder.AppendLine("الكلمات: " + words);
            builder.AppendLine("الجمل: " + sentences);
            builder.AppendLine("متوسط الكلمات في الجملة: " + wordsPerSentence.ToString("0.0", CultureInfo.InvariantCulture));
            builder.AppendLine("متوسط الأحرف في الكلمة: " + lettersPerWord.ToString("0.0", CultureInfo.InvariantCulture));
            builder.AppendLine();
            builder.AppendLine(wordsPerSentence <= 18 ? "النتيجة: الجمل مناسبة نسبيًا للقراءة." : "النتيجة: يُفضّل تقصير الجمل الطويلة.");
            MessageBox.Show(builder.ToString(), "تحليل قابلية القراءة");
        }

        // -----------------------------------------------------------------
        // View and workspace control
        // -----------------------------------------------------------------
        internal static void SetZoom(int percent)
        {
            percent = Math.Max(10, Math.Min(500, percent));
            WordContext.Application.ActiveWindow.View.Zoom.Percentage = percent;
        }

        internal static void SetViewType(int viewType)
        {
            WordContext.Application.ActiveWindow.View.Type = viewType;
        }

        internal static void SetFormattingMarks(bool visible)
        {
            WordContext.Application.ActiveWindow.View.ShowAll = visible;
        }

        internal static void SetRulers(bool visible)
        {
            WordContext.Application.ActiveWindow.View.ShowRulers = visible;
        }
    }
}
