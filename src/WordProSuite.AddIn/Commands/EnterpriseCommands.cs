using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WordProSuite.Desktop.UI;

namespace WordProSuite.Desktop.Commands
{
    internal static class EnterpriseCommands
    {
        private static dynamic Range()
        {
            return WordContext.TargetRange;
        }

        private static dynamic Table()
        {
            dynamic selection = WordContext.Selection;
            if (selection == null || selection.Tables.Count == 0)
                throw new InvalidOperationException("ضع المؤشر داخل جدول أولاً.");
            return selection.Tables[1];
        }

        private static string RangeText(dynamic range)
        {
            return Convert.ToString(range.Text) ?? "";
        }

        private static void ReplaceRangeText(Func<string, string> transform)
        {
            dynamic range = Range();
            string source = RangeText(range);
            range.Text = transform(source);
        }

        // ---------------- Professional text cleanup ----------------
        internal static void NormalizeUnicodeSpaces()
        {
            ReplaceRangeText(delegate(string source)
            {
                string value = source.Replace('\u00A0', ' ')
                    .Replace('\u202F', ' ')
                    .Replace('\u2000', ' ')
                    .Replace('\u2001', ' ')
                    .Replace('\u2002', ' ')
                    .Replace('\u2003', ' ')
                    .Replace('\u2004', ' ')
                    .Replace('\u2005', ' ')
                    .Replace('\u2006', ' ')
                    .Replace('\u2007', ' ')
                    .Replace('\u2008', ' ')
                    .Replace('\u2009', ' ')
                    .Replace('\u200A', ' ');
                return Regex.Replace(value, @"[ \t]{2,}", " ");
            });
        }

        internal static void RemoveLeadingSpaces()
        {
            ReplaceRangeText(delegate(string source)
            {
                string[] lines = TextTransforms.Lines(source);
                return String.Join("\r", lines.Select((Func<string, string>)(x => x.TrimStart())));
            });
        }

        internal static void RemoveTrailingSpaces()
        {
            ReplaceRangeText(delegate(string source)
            {
                string[] lines = TextTransforms.Lines(source);
                return String.Join("\r", lines.Select((Func<string, string>)(x => x.TrimEnd())));
            });
        }

        internal static void RemoveAllBlankParagraphs()
        {
            ReplaceRangeText(delegate(string source)
            {
                string[] lines = TextTransforms.Lines(source);
                return String.Join("\r", lines.Where((Func<string, bool>)(x => !String.IsNullOrWhiteSpace(x))));
            });
        }

        internal static void SentencesToParagraphs()
        {
            ReplaceRangeText(delegate(string source)
            {
                return Regex.Replace(source.Trim(), @"(?<=[.!؟])\s+(?=\p{L})", "\r");
            });
        }

        internal static void ParagraphsToSemicolonList()
        {
            ReplaceRangeText(delegate(string source)
            {
                string[] items = TextTransforms.Lines(source)
                    .Where((Func<string, bool>)(x => !String.IsNullOrWhiteSpace(x)))
                    .Select((Func<string, string>)(x => x.Trim().TrimEnd('؛', ';', ',', '،')))
                    .ToArray();
                return String.Join("؛ ", items) + (items.Length > 0 ? "؛" : "");
            });
        }

        internal static void WrapArabicQuotes()
        {
            WrapSelection("«", "»");
        }

        internal static void WrapParentheses()
        {
            WrapSelection("(", ")");
        }

        internal static void WrapBrackets()
        {
            WrapSelection("[", "]");
        }

        private static void WrapSelection(string left, string right)
        {
            dynamic range = Range();
            string value = RangeText(range).TrimEnd('\r', '\a');
            range.Text = left + value + right;
        }

        internal static void StripHtmlTags()
        {
            ReplaceRangeText(delegate(string source)
            {
                string withoutTags = Regex.Replace(source, "<[^>]+>", " ");
                return WebUtility.HtmlDecode(Regex.Replace(withoutTags, @"[ \t]{2,}", " ")).Trim();
            });
        }

        internal static void ExtractNumbers()
        {
            ExtractWithRegex(new Regex(@"(?<!\p{L})[-+]?(?:\d+(?:[.,]\d+)?)(?!\p{L})"), "الأرقام المستخرجة");
        }

        internal static void ExtractHashtags()
        {
            ExtractWithRegex(new Regex(@"(?<!\w)#[\p{L}\p{N}_]+"), "الوسوم المستخرجة");
        }

        internal static void ExtractMentions()
        {
            ExtractWithRegex(new Regex(@"(?<!\w)@[\p{L}\p{N}_.-]+"), "الإشارات المستخرجة");
        }

        private static void ExtractWithRegex(Regex regex, string title)
        {
            string source = RangeText(Range());
            List<string> items = regex.Matches(source).Cast<Match>()
                .Select((Func<Match, string>)(x => x.Value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (items.Count == 0)
            {
                MessageBox.Show("لم يتم العثور على نتائج.", title);
                return;
            }
            dynamic document = WordContext.Application.Documents.Add();
            document.Content.Text = title + "\r\r" + String.Join("\r", items);
        }

        internal static void WordFrequencyTop()
        {
            string raw = Prompt.Show("تكرار الكلمات", "عدد الكلمات الأعلى تكرارًا:", "25");
            if (raw == null) return;
            int count;
            if (!Int32.TryParse(raw, out count)) count = 25;
            count = Math.Max(5, Math.Min(200, count));

            string source = RangeText(Range()).ToLower(CultureInfo.CurrentCulture);
            var frequencies = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
            foreach (Match match in Regex.Matches(source, @"[\p{L}\p{N}]{2,}"))
            {
                string word = match.Value.Trim();
                int current;
                frequencies.TryGetValue(word, out current);
                frequencies[word] = current + 1;
            }

            var ordered = frequencies.OrderByDescending((Func<KeyValuePair<string, int>, int>)(x => x.Value))
                .ThenBy((Func<KeyValuePair<string, int>, string>)(x => x.Key))
                .Take(count)
                .ToList();

            var output = new StringBuilder();
            output.AppendLine("الكلمة\tالتكرار");
            foreach (KeyValuePair<string, int> item in ordered)
                output.AppendLine(item.Key + "\t" + item.Value);

            dynamic document = WordContext.Application.Documents.Add();
            document.Content.Text = output.ToString();
        }

        internal static void FindReplacePrompt()
        {
            string find = Prompt.Show("بحث واستبدال", "النص المطلوب البحث عنه:");
            if (find == null) return;
            string replace = Prompt.Show("بحث واستبدال", "النص البديل:", "");
            if (replace == null) return;
            WordContext.ReplaceAll(Range(), find, replace);
        }

        internal static void HighlightTerm()
        {
            string term = Prompt.Show("تمييز كلمة", "الكلمة أو العبارة:");
            if (String.IsNullOrWhiteSpace(term)) return;
            dynamic range = Range();
            dynamic find = range.Find;
            find.ClearFormatting();
            find.Replacement.ClearFormatting();
            find.Text = term;
            find.Replacement.Text = "^&";
            find.Replacement.Highlight = 1;
            find.Forward = true;
            find.Wrap = 1;
            find.Format = true;
            find.MatchWildcards = false;
            find.Execute(Replace: 2);
        }

        internal static void ClearHighlight()
        {
            Range().HighlightColorIndex = 0;
        }

        internal static void DuplicateSelection()
        {
            dynamic selection = WordContext.Selection;
            string source = Convert.ToString(selection.Text) ?? "";
            if (String.IsNullOrEmpty(source))
                throw new InvalidOperationException("حدد نصًا أولاً.");
            selection.Collapse(0);
            selection.TypeText(source);
        }

        internal static void SortParagraphsByLength()
        {
            ReplaceRangeText(delegate(string source)
            {
                string[] paragraphs = TextTransforms.Lines(source)
                    .Where((Func<string, bool>)(x => !String.IsNullOrWhiteSpace(x)))
                    .OrderBy((Func<string, int>)(x => x.Trim().Length))
                    .ToArray();
                return String.Join("\r", paragraphs);
            });
        }

        internal static void RemoveShortParagraphs()
        {
            string raw = Prompt.Show("حذف الفقرات القصيرة", "الحد الأدنى لعدد الأحرف:", "20");
            if (raw == null) return;
            int min;
            if (!Int32.TryParse(raw, out min)) min = 20;
            min = Math.Max(1, Math.Min(5000, min));
            ReplaceRangeText(delegate(string source)
            {
                string[] paragraphs = TextTransforms.Lines(source)
                    .Where((Func<string, bool>)(x => x.Trim().Length >= min))
                    .ToArray();
                return String.Join("\r", paragraphs);
            });
        }

        internal static void UniqueWordsOnly()
        {
            ReplaceRangeText(delegate(string source)
            {
                var seen = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
                var output = new List<string>();
                foreach (Match match in Regex.Matches(source, @"[\p{L}\p{N}_]+"))
                {
                    string word = match.Value;
                    if (seen.Add(word)) output.Add(word);
                }
                return String.Join(" ", output);
            });
        }

        internal static void CommaListToBullets()
        {
            dynamic range = Range();
            string source = RangeText(range);
            string[] items = Regex.Split(source, @"[,،;\r\n]+")
                .Where((Func<string, bool>)(x => !String.IsNullOrWhiteSpace(x)))
                .Select((Func<string, string>)(x => x.Trim()))
                .ToArray();
            range.Text = String.Join("\r", items);
            range.ListFormat.ApplyBulletDefault();
        }

        // ---------------- Inserts and fields ----------------
        internal static void InsertCurrentTime()
        {
            WordContext.Selection.TypeText(DateTime.Now.ToString("HH:mm"));
        }

        internal static void InsertIsoDate()
        {
            WordContext.Selection.TypeText(DateTime.Today.ToString("yyyy-MM-dd"));
        }

        internal static void InsertTimestamp()
        {
            WordContext.Selection.TypeText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        internal static void InsertPageXOfY()
        {
            dynamic document = WordContext.Document;
            foreach (dynamic section in document.Sections)
            {
                dynamic footer = section.Footers[1];
                footer.Range.Text = "صفحة ";
                dynamic end = footer.Range;
                end.Collapse(0);
                footer.Range.Fields.Add(end, 33);
                end = footer.Range;
                end.Collapse(0);
                end.InsertAfter(" من ");
                end = footer.Range;
                end.Collapse(0);
                footer.Range.Fields.Add(end, 26);
                footer.Range.ParagraphFormat.Alignment = 1;
            }
        }

        internal static void InsertCheckboxSymbols()
        {
            string raw = Prompt.Show("مربعات اختيار", "عدد المربعات:", "5");
            if (raw == null) return;
            int count;
            if (!Int32.TryParse(raw, out count)) count = 5;
            count = Math.Max(1, Math.Min(100, count));
            WordContext.Selection.TypeText(String.Join("\r", Enumerable.Repeat("☐ ................................................", count)));
        }

        internal static void InsertDocumentInfoBlock()
        {
            dynamic document = WordContext.Document;
            string title = Convert.ToString(document.BuiltInDocumentProperties["Title"].Value);
            string author = Convert.ToString(document.BuiltInDocumentProperties["Author"].Value);
            string text = "معلومات المستند\r"
                + "العنوان: " + title + "\r"
                + "المؤلف: " + author + "\r"
                + "اسم الملف: " + Convert.ToString(document.Name) + "\r"
                + "تاريخ الإنشاء: " + DateTime.Now.ToString("yyyy-MM-dd") + "\r";
            WordContext.Selection.TypeText(text);
        }

        // ---------------- Professional tables and frameworks ----------------
        internal static void InsertDecisionLog()
        {
            InsertFrameworkTable("سجل القرارات", new[] { "م", "القرار", "الخلفية", "المسؤول", "التاريخ", "الحالة" }, 6);
        }

        internal static void InsertRaciMatrix()
        {
            InsertFrameworkTable("مصفوفة RACI", new[] { "النشاط", "Responsible", "Accountable", "Consulted", "Informed" }, 7);
        }

        internal static void InsertSwotMatrix()
        {
            InsertFrameworkTable("تحليل SWOT", new[] { "نقاط القوة", "نقاط الضعف", "الفرص", "التهديدات" }, 5);
        }

        internal static void InsertKpiTable()
        {
            InsertFrameworkTable("جدول مؤشرات الأداء", new[] { "المؤشر", "خط الأساس", "المستهدف", "النتيجة", "مصدر التحقق", "المسؤول" }, 7);
        }

        internal static void InsertBudgetTable()
        {
            InsertFrameworkTable("جدول الميزانية", new[] { "البند", "الوصف", "الكمية", "سعر الوحدة", "الإجمالي", "ملاحظات" }, 8);
        }

        internal static void InsertTimelineTable()
        {
            InsertFrameworkTable("الجدول الزمني", new[] { "النشاط", "البداية", "النهاية", "المدة", "المسؤول", "الحالة" }, 8);
        }

        internal static void InsertYesNoTable()
        {
            InsertFrameworkTable("قائمة تحقق", new[] { "البند", "نعم", "لا", "غير منطبق", "ملاحظات" }, 10);
        }

        private static void InsertFrameworkTable(string title, string[] headers, int dataRows)
        {
            dynamic selection = WordContext.Selection;
            selection.TypeText(title + "\r");
            dynamic document = WordContext.Document;
            dynamic table = document.Tables.Add(selection.Range, dataRows + 1, headers.Length);
            table.Direction = 0;
            table.Borders.Enable = 1;
            table.AutoFitBehavior(2);
            for (int column = 1; column <= headers.Length; column++)
            {
                table.Cell(1, column).Range.Text = headers[column - 1];
                table.Cell(1, column).Range.Bold = 1;
            }
            table.Rows[1].HeadingFormat = -1;
            table.Range.ParagraphFormat.ReadingOrder = 0;
            table.Range.ParagraphFormat.Alignment = 1;
            table.Range.Cells.VerticalAlignment = 1;
        }

        internal static void TableCurrencyFormat()
        {
            dynamic table = Table();
            string symbol = Prompt.Show("تنسيق العملة", "رمز العملة:", "$");
            if (symbol == null) return;
            foreach (dynamic cell in table.Range.Cells)
            {
                string raw = WordContext.CleanCell(Convert.ToString(cell.Range.Text));
                decimal value;
                if (Decimal.TryParse(TextTransforms.Western(raw).Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    cell.Range.Text = value.ToString("#,##0.00", CultureInfo.InvariantCulture) + " " + symbol;
            }
        }

        internal static void TablePercentageFormat()
        {
            dynamic table = Table();
            foreach (dynamic cell in table.Range.Cells)
            {
                string raw = WordContext.CleanCell(Convert.ToString(cell.Range.Text));
                decimal value;
                if (Decimal.TryParse(TextTransforms.Western(raw).Replace("%", "").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    cell.Range.Text = value.ToString("0.##", CultureInfo.InvariantCulture) + "%";
            }
        }

        internal static void TableDigitsEastern()
        {
            TransformTableCells(TextTransforms.Eastern);
        }

        internal static void TableDigitsWestern()
        {
            TransformTableCells(TextTransforms.Western);
        }

        internal static void TableRemoveCellBreaks()
        {
            TransformTableCells(delegate(string value)
            {
                return value.Replace("\r", " ").Replace("\n", " ").Replace("\v", " ");
            });
        }

        private static void TransformTableCells(Func<string, string> transform)
        {
            dynamic table = Table();
            foreach (dynamic cell in table.Range.Cells)
            {
                string value = WordContext.CleanCell(Convert.ToString(cell.Range.Text));
                cell.Range.Text = transform(value);
            }
        }

        internal static void TableBoldFirstRow()
        {
            dynamic table = Table();
            table.Rows[1].Range.Bold = 1;
            table.Rows[1].HeadingFormat = -1;
        }

        internal static void TableAutoRowHeight()
        {
            dynamic table = Table();
            table.Rows.HeightRule = 0;
        }

        internal static void TableColumnWidthPrompt()
        {
            string raw = Prompt.Show("عرض الأعمدة", "العرض بالسنتيمتر:", "3");
            if (raw == null) return;
            double cm;
            if (!Double.TryParse(TextTransforms.Western(raw), NumberStyles.Any, CultureInfo.InvariantCulture, out cm))
                throw new InvalidOperationException("أدخل رقمًا صحيحًا.");
            float points = (float)(cm * 28.3464567);
            dynamic table = Table();
            table.Columns.Width = points;
        }

        // ---------------- Document analytics and exports ----------------
        internal static void DocumentDashboard()
        {
            dynamic document = WordContext.Document;
            int pages = Convert.ToInt32(document.ComputeStatistics(2));
            int words = Convert.ToInt32(document.ComputeStatistics(0));
            int characters = Convert.ToInt32(document.ComputeStatistics(3));
            int paragraphs = document.Paragraphs.Count;
            int tables = document.Tables.Count;
            int images = document.InlineShapes.Count + document.Shapes.Count;
            int links = document.Hyperlinks.Count;
            int comments = document.Comments.Count;
            int revisions = document.Revisions.Count;

            MessageBox.Show(
                "لوحة المستند\r\n\r\n"
                + "الصفحات: " + pages + "\r\n"
                + "الكلمات: " + words + "\r\n"
                + "الأحرف: " + characters + "\r\n"
                + "الفقرات: " + paragraphs + "\r\n"
                + "الجداول: " + tables + "\r\n"
                + "الصور والأشكال: " + images + "\r\n"
                + "الروابط: " + links + "\r\n"
                + "التعليقات: " + comments + "\r\n"
                + "التعديلات: " + revisions,
                "WordPro Suite — لوحة المستند");
        }

        internal static void ListDocumentHeadings()
        {
            dynamic document = WordContext.Document;
            var rows = new List<string>();
            int index = 1;
            foreach (dynamic paragraph in document.Paragraphs)
            {
                string text = Convert.ToString(paragraph.Range.Text).Trim();
                string style = "";
                try { style = Convert.ToString(paragraph.Range.get_Style().NameLocal); } catch { }
                if (!String.IsNullOrWhiteSpace(text) &&
                    (style.IndexOf("Heading", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     style.IndexOf("عنوان", StringComparison.CurrentCultureIgnoreCase) >= 0))
                {
                    rows.Add(index + "\t" + style + "\t" + text);
                    index++;
                }
            }
            if (rows.Count == 0)
            {
                MessageBox.Show("لم يتم العثور على عناوين.", "WordPro Suite");
                return;
            }
            dynamic result = WordContext.Application.Documents.Add();
            result.Content.Text = "م\tالنمط\tالعنوان\r" + String.Join("\r", rows);
        }

        internal static void ListBookmarks()
        {
            dynamic document = WordContext.Document;
            var rows = new List<string>();
            foreach (dynamic bookmark in document.Bookmarks)
                rows.Add(Convert.ToString(bookmark.Name));
            if (rows.Count == 0)
            {
                MessageBox.Show("لا توجد إشارات مرجعية.", "WordPro Suite");
                return;
            }
            dynamic result = WordContext.Application.Documents.Add();
            result.Content.Text = "الإشارات المرجعية\r\r" + String.Join("\r", rows);
        }

        internal static void ExportPlainText()
        {
            dynamic document = WordContext.Document;
            string path = WordContext.SavePath("تصدير نص", "Text File (*.txt)|*.txt", WordContext.BaseName() + ".txt");
            if (String.IsNullOrWhiteSpace(path)) return;
            File.WriteAllText(path, Convert.ToString(document.Content.Text), Encoding.UTF8);
            MessageBox.Show("تم التصدير:\r\n" + path, "WordPro Suite");
        }

        internal static void ExportSelectionText()
        {
            string path = WordContext.SavePath("تصدير التحديد", "Text File (*.txt)|*.txt", WordContext.BaseName() + "_Selection.txt");
            if (String.IsNullOrWhiteSpace(path)) return;
            File.WriteAllText(path, Convert.ToString(WordContext.Selection.Text), Encoding.UTF8);
            MessageBox.Show("تم التصدير:\r\n" + path, "WordPro Suite");
        }

        internal static void CopyAsMarkdown()
        {
            string source = Convert.ToString(WordContext.Selection.Text) ?? "";
            if (String.IsNullOrWhiteSpace(source))
                throw new InvalidOperationException("حدد نصًا أولاً.");
            string[] lines = TextTransforms.Lines(source);
            string markdown = String.Join(Environment.NewLine,
                lines.Select((Func<string, string>)(x => x.TrimEnd())));
            Clipboard.SetText(markdown);
            MessageBox.Show("تم نسخ النص بصيغة Markdown بسيطة.", "WordPro Suite");
        }

        internal static void UnlinkSelectionFields()
        {
            dynamic range = Range();
            for (int index = range.Fields.Count; index >= 1; index--)
                range.Fields[index].Unlink();
        }

        internal static void RemoveFootnotes()
        {
            dynamic document = WordContext.Document;
            while (document.Footnotes.Count > 0) document.Footnotes[1].Delete();
        }

        internal static void RemoveEndnotes()
        {
            dynamic document = WordContext.Document;
            while (document.Endnotes.Count > 0) document.Endnotes[1].Delete();
        }

        internal static void MoveSelectionToNewDocument()
        {
            string source = Convert.ToString(WordContext.Selection.Text) ?? "";
            if (String.IsNullOrWhiteSpace(source))
                throw new InvalidOperationException("حدد نصًا أولاً.");
            dynamic document = WordContext.Application.Documents.Add();
            document.Content.FormattedText = WordContext.Selection.Range.FormattedText;
        }
    }
}
