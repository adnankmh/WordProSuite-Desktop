using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WordProSuite.Desktop.Infrastructure;
using WordProSuite.Desktop.Licensing;
using WordProSuite.Desktop.UI;

namespace WordProSuite.Desktop.Commands
{
    internal static class AdvancedCommands
    {
        private static readonly Regex ArabicLetters = new Regex("[\\u0600-\\u06FF]", RegexOptions.Compiled);
        private static readonly Regex LatinLetters = new Regex("[A-Za-z]", RegexOptions.Compiled);
        private static readonly Regex EmailRegex = new Regex(@"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex UrlRegex = new Regex(@"(?:https?://|www\.)[^\s<>()]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new Regex(@"(?<!\d)(?:\+?\d[\d\s\-()]{6,}\d)(?!\d)", RegexOptions.Compiled);
        private static FormatSnapshot Snapshot;

        // ---------------- Arabic and language ----------------
        internal static void AutoDirection()
        {
            dynamic r = WordContext.TargetRange;
            string text = Convert.ToString(r.Text) ?? "";
            bool rtl = ArabicLetters.Matches(text).Count >= LatinLetters.Matches(text).Count;
            r.ParagraphFormat.ReadingOrder = rtl ? 0 : 1;
            r.ParagraphFormat.Alignment = rtl ? 2 : 0;
        }

        internal static void NormalizeArabicPunctuation()
        {
            dynamic r = WordContext.TargetRange;
            string t = Convert.ToString(r.Text) ?? "";
            t = t.Replace(",", "،").Replace(";", "؛").Replace("?", "؟");
            t = Regex.Replace(t, @"\s+([،؛؟.!:])", "$1");
            t = Regex.Replace(t, @"([،؛؟.!:])(?=[^\s\r\n])", "$1 ");
            r.Text = t;
        }

        internal static void ArabicQuotes()
        {
            dynamic r = WordContext.TargetRange;
            string t = Convert.ToString(r.Text) ?? "";
            t = t.Replace('“', '«').Replace('”', '»').Replace('„', '«');
            t = Regex.Replace(t, "\"([^\"]+)\"", "«$1»");
            r.Text = t;
        }

        internal static void RemoveZeroWidth()
        {
            dynamic r = WordContext.TargetRange;
            string t = Convert.ToString(r.Text) ?? "";
            r.Text = t.Replace("\u200B", "").Replace("\u200C", "").Replace("\u200D", "").Replace("\uFEFF", "");
        }

        internal static void KeyboardArabicToEnglish()
        {
            dynamic r = WordContext.TargetRange;
            string text = (Convert.ToString(r.Text) ?? "").Replace("لا", "b");
            const string ar = "ضصثقفغعهخحجدشسيبلاتنمكطئءؤرىةوزظ";
            const string en = "qwertyuiop[]asdfghjkl;'zxcvnm,./";
            var sb = new StringBuilder();
            foreach (char ch in text)
            {
                int i = ar.IndexOf(ch);
                sb.Append(i >= 0 && i < en.Length ? en[i] : ch);
            }
            r.Text = sb.ToString();
        }

        internal static void KeyboardEnglishToArabic()
        {
            dynamic r = WordContext.TargetRange;
            string text = Convert.ToString(r.Text) ?? "";
            const string en = "qwertyuiop[]asdfghjkl;'zxcvnm,./";
            const string ar = "ضصثقفغعهخحجدشسيبلاتنمكطئءؤرىةوزظ";
            var sb = new StringBuilder();
            foreach (char original in text)
            {
                char ch = Char.ToLowerInvariant(original);
                if (ch == 'b') { sb.Append("لا"); continue; }
                int i = en.IndexOf(ch);
                sb.Append(i >= 0 && i < ar.Length ? ar[i] : original);
            }
            r.Text = sb.ToString();
        }

        internal static void InsertArabicDate()
        {
            string[] months = { "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر" };
            DateTime d = DateTime.Today;
            WordContext.Selection.TypeText(d.Day + " " + months[d.Month - 1] + " " + d.Year);
        }

        internal static void InsertHijriDate()
        {
            var cal = new UmAlQuraCalendar();
            DateTime d = DateTime.Today;
            string[] months = { "محرم", "صفر", "ربيع الأول", "ربيع الآخر", "جمادى الأولى", "جمادى الآخرة", "رجب", "شعبان", "رمضان", "شوال", "ذو القعدة", "ذو الحجة" };
            WordContext.Selection.TypeText(cal.GetDayOfMonth(d) + " " + months[cal.GetMonth(d) - 1] + " " + cal.GetYear(d) + " هـ");
        }

        internal static void ReverseText()
        {
            dynamic r = WordContext.TargetRange;
            char[] c = (Convert.ToString(r.Text) ?? "").ToCharArray();
            Array.Reverse(c);
            r.Text = new string(c);
        }

        internal static void NumberToArabicWords()
        {
            dynamic r = WordContext.TargetRange;
            string raw = TextTransforms.Western((Convert.ToString(r.Text) ?? "").Trim()).Replace(",", "");
            long n;
            if (!Int64.TryParse(raw, out n) || n < 0 || n > 999999999)
                throw new InvalidOperationException("حدد رقمًا صحيحًا بين 0 و999,999,999.");
            r.Text = ArabicNumberWords(n);
        }

        private static string ArabicNumberWords(long n)
        {
            if (n == 0) return "صفر";
            var parts = new List<string>();
            int million = (int)(n / 1000000); n %= 1000000;
            int thousand = (int)(n / 1000); n %= 1000;
            if (million > 0) parts.Add(GroupName(million, "مليون", "مليونان", "ملايين"));
            if (thousand > 0) parts.Add(GroupName(thousand, "ألف", "ألفان", "آلاف"));
            if (n > 0) parts.Add(UnderThousand((int)n));
            return String.Join(" و", parts.Where(x => !String.IsNullOrWhiteSpace(x)));
        }

        private static string GroupName(int value, string one, string two, string plural)
        {
            if (value == 1) return one;
            if (value == 2) return two;
            if (value >= 3 && value <= 10) return UnderThousand(value) + " " + plural;
            return UnderThousand(value) + " " + one;
        }

        private static string UnderThousand(int n)
        {
            string[] ones = { "", "واحد", "اثنان", "ثلاثة", "أربعة", "خمسة", "ستة", "سبعة", "ثمانية", "تسعة", "عشرة", "أحد عشر", "اثنا عشر", "ثلاثة عشر", "أربعة عشر", "خمسة عشر", "ستة عشر", "سبعة عشر", "ثمانية عشر", "تسعة عشر" };
            string[] tens = { "", "", "عشرون", "ثلاثون", "أربعون", "خمسون", "ستون", "سبعون", "ثمانون", "تسعون" };
            string[] hundreds = { "", "مائة", "مائتان", "ثلاثمائة", "أربعمائة", "خمسمائة", "ستمائة", "سبعمائة", "ثمانمائة", "تسعمائة" };
            var p = new List<string>();
            if (n >= 100) { p.Add(hundreds[n / 100]); n %= 100; }
            if (n > 0)
            {
                if (n < 20) p.Add(ones[n]);
                else
                {
                    int o = n % 10, t = n / 10;
                    if (o > 0) p.Add(ones[o]);
                    p.Add(tens[t]);
                }
            }
            return String.Join(" و", p);
        }

        // ---------------- Text tools ----------------
        internal static void TrimLines()
        {
            dynamic r = WordContext.TargetRange;
            r.Text = String.Join("\r", TextTransforms.Lines(Convert.ToString(r.Text)).Select(x => x.Trim()));
        }

        internal static void JoinParagraphs()
        {
            dynamic r = WordContext.TargetRange;
            string t = Convert.ToString(r.Text) ?? "";
            t = Regex.Replace(t, @"\s*(?:\r\n|\r|\n)+\s*", " ");
            r.Text = Regex.Replace(t, @" {2,}", " ").Trim();
        }

        internal static void RemoveDuplicateParagraphs()
        {
            dynamic r = WordContext.TargetRange;
            var seen = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
            var output = new List<string>();
            foreach (string line in TextTransforms.Lines(Convert.ToString(r.Text)))
            {
                string key = Regex.Replace(line.Trim(), @"\s+", " ");
                if (key.Length == 0 || seen.Add(key)) output.Add(line.TrimEnd());
            }
            r.Text = String.Join("\r", output);
        }

        internal static void RemoveDuplicateConsecutiveWords()
        {
            dynamic r = WordContext.TargetRange;
            r.Text = Regex.Replace(Convert.ToString(r.Text) ?? "", @"\b([\p{L}\p{N}_]+)(\s+\1\b)+", "$1", RegexOptions.IgnoreCase);
        }

        internal static void SmartQuotes()
        {
            dynamic r = WordContext.TargetRange;
            string t = Convert.ToString(r.Text) ?? "";
            t = Regex.Replace(t, "\"([^\"]+)\"", "“$1”");
            t = Regex.Replace(t, "'([^']+)'", "‘$1’");
            r.Text = t;
        }

        internal static void NormalizeDashes()
        {
            dynamic r = WordContext.TargetRange;
            string t = Convert.ToString(r.Text) ?? "";
            r.Text = t.Replace("--", "—").Replace(" - ", " – ").Replace("−", "-");
        }

        internal static void PrefixLines()
        {
            string prefix = Prompt.Show("بادئة الأسطر", "اكتب النص الذي سيضاف قبل كل سطر:");
            if (prefix == null) return;
            dynamic r = WordContext.TargetRange;
            r.Text = String.Join("\r", TextTransforms.Lines(Convert.ToString(r.Text)).Select(x => prefix + x));
        }

        internal static void SuffixLines()
        {
            string suffix = Prompt.Show("لاحقة الأسطر", "اكتب النص الذي سيضاف بعد كل سطر:");
            if (suffix == null) return;
            dynamic r = WordContext.TargetRange;
            r.Text = String.Join("\r", TextTransforms.Lines(Convert.ToString(r.Text)).Select(x => x + suffix));
        }

        internal static void ParagraphsToBullets() => WordContext.TargetRange.ListFormat.ApplyBulletDefault();
        internal static void ParagraphsToNumbers() => WordContext.TargetRange.ListFormat.ApplyNumberDefault();
        internal static void ClearList() => WordContext.TargetRange.ListFormat.RemoveNumbers();

        internal static void SentenceCase()
        {
            dynamic r = WordContext.TargetRange;
            string t = (Convert.ToString(r.Text) ?? "").ToLower(CultureInfo.CurrentCulture);
            bool upper = true;
            char[] c = t.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (upper && Char.IsLetter(c[i])) { c[i] = Char.ToUpper(c[i], CultureInfo.CurrentCulture); upper = false; }
                if (c[i] == '.' || c[i] == '!' || c[i] == '?' || c[i] == '؟' || c[i] == '\r' || c[i] == '\n') upper = true;
            }
            r.Text = new string(c);
        }

        internal static void InvertCase()
        {
            dynamic r = WordContext.TargetRange;
            char[] c = (Convert.ToString(r.Text) ?? "").ToCharArray();
            for (int i = 0; i < c.Length; i++)
                c[i] = Char.IsUpper(c[i]) ? Char.ToLower(c[i], CultureInfo.CurrentCulture) : Char.ToUpper(c[i], CultureInfo.CurrentCulture);
            r.Text = new string(c);
        }

        internal static void ExtractEmails() => ExtractMatches(EmailRegex, "عناوين البريد الإلكتروني");
        internal static void ExtractUrls() => ExtractMatches(UrlRegex, "الروابط");
        internal static void ExtractPhones() => ExtractMatches(PhoneRegex, "أرقام الهواتف");

        private static void ExtractMatches(Regex regex, string title)
        {
            string text = Convert.ToString(WordContext.TargetRange.Text) ?? "";
            var items = regex.Matches(text).Cast<Match>().Select(x => x.Value.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (items.Count == 0) { MessageBox.Show("لم يتم العثور على نتائج.", title); return; }
            WordContext.Selection.TypeText(String.Join("\r", items));
        }

        internal static void TextStatistics()
        {
            string text = Convert.ToString(WordContext.TargetRange.Text) ?? "";
            int words = Regex.Matches(text, @"\b[\p{L}\p{N}_]+\b").Count;
            int chars = text.Length;
            int charsNoSpaces = text.Count(c => !Char.IsWhiteSpace(c));
            int paragraphs = TextTransforms.Lines(text).Count(x => x.Trim().Length > 0);
            int sentences = Regex.Matches(text, @"[.!؟?]+(?:\s|$)").Count;
            MessageBox.Show("الكلمات: " + words + "\nالأحرف: " + chars + "\nالأحرف دون مسافات: " + charsNoSpaces + "\nالفقرات: " + paragraphs + "\nالجمل: " + sentences, "إحصاءات النص");
        }

        internal static void PastePlainText()
        {
            if (!Clipboard.ContainsText()) throw new InvalidOperationException("الحافظة لا تحتوي نصًا.");
            WordContext.Selection.TypeText(Clipboard.GetText(TextDataFormat.UnicodeText));
        }

        internal static void SplitByDelimiter()
        {
            string delimiter = Prompt.Show("تقسيم النص", "اكتب الفاصل المطلوب استبداله بفاصل فقرة:", ",");
            if (delimiter == null || delimiter.Length == 0) return;
            dynamic r = WordContext.TargetRange;
            r.Text = (Convert.ToString(r.Text) ?? "").Replace(delimiter, "\r");
        }

        internal static void RemoveNonPrinting()
        {
            dynamic r = WordContext.TargetRange;
            string t = Convert.ToString(r.Text) ?? "";
            t = new string(t.Where(c => c == '\r' || c == '\n' || c == '\t' || !Char.IsControl(c)).ToArray());
            r.Text = t.Replace("\u00A0", " ").Replace("\u2028", "\r");
        }

        // ---------------- Formatting ----------------
        internal static void FontPreset(string name, float size)
        {
            dynamic f = WordContext.TargetRange.Font;
            f.Name = name; f.NameBi = name; f.Size = size; f.SizeBi = size;
        }

        internal static void LineSpacingRule(int rule) => WordContext.TargetRange.ParagraphFormat.LineSpacingRule = rule;
        internal static void SpaceAfter(float points) => WordContext.TargetRange.ParagraphFormat.SpaceAfter = points;

        internal static void FirstLineIndent(float points)
        {
            dynamic p = WordContext.TargetRange.ParagraphFormat;
            p.FirstLineIndent = points;
        }

        internal static void RemoveIndents()
        {
            dynamic p = WordContext.TargetRange.ParagraphFormat;
            p.LeftIndent = 0; p.RightIndent = 0; p.FirstLineIndent = 0;
        }

        internal static void KeepWithNext() => WordContext.TargetRange.ParagraphFormat.KeepWithNext = -1;
        internal static void PageBreakBefore() => WordContext.TargetRange.ParagraphFormat.PageBreakBefore = -1;
        internal static void HighlightYellow() => WordContext.TargetRange.HighlightColorIndex = 7;
        internal static void ClearHighlight() => WordContext.TargetRange.HighlightColorIndex = 0;
        internal static void BorderBox() => WordContext.TargetRange.Borders.Enable = 1;
        internal static void ClearBorders() => WordContext.TargetRange.Borders.Enable = 0;

        internal static void OfficialLetterFormat()
        {
            dynamic d = WordContext.Document;
            dynamic p = d.PageSetup;
            p.PaperSize = 7; p.TopMargin = 56.7f; p.BottomMargin = 56.7f; p.LeftMargin = 56.7f; p.RightMargin = 56.7f;
            dynamic r = WordContext.TargetRange;
            r.Font.Name = "Arial"; r.Font.NameBi = "Arial"; r.Font.Size = 14; r.Font.SizeBi = 14;
            r.ParagraphFormat.ReadingOrder = 0; r.ParagraphFormat.Alignment = 3; r.ParagraphFormat.LineSpacingRule = 0; r.ParagraphFormat.SpaceAfter = 6;
        }

        internal static void AcademicFormat()
        {
            dynamic d = WordContext.Document;
            dynamic p = d.PageSetup;
            p.PaperSize = 7; p.TopMargin = 72f; p.BottomMargin = 72f; p.LeftMargin = 90f; p.RightMargin = 72f;
            dynamic r = WordContext.TargetRange;
            r.Font.Name = "Times New Roman"; r.Font.NameBi = "Arial"; r.Font.Size = 12; r.Font.SizeBi = 14;
            r.ParagraphFormat.Alignment = 3; r.ParagraphFormat.LineSpacingRule = 1; r.ParagraphFormat.SpaceAfter = 6;
        }

        internal static void CopyFormatSnapshot()
        {
            dynamic r = WordContext.TargetRange;
            Snapshot = new FormatSnapshot
            {
                FontName = Convert.ToString(r.Font.Name), FontNameBi = Convert.ToString(r.Font.NameBi),
                FontSize = Convert.ToSingle(r.Font.Size), FontSizeBi = Convert.ToSingle(r.Font.SizeBi),
                Bold = Convert.ToInt32(r.Font.Bold), Italic = Convert.ToInt32(r.Font.Italic),
                Alignment = Convert.ToInt32(r.ParagraphFormat.Alignment), ReadingOrder = Convert.ToInt32(r.ParagraphFormat.ReadingOrder),
                SpaceAfter = Convert.ToSingle(r.ParagraphFormat.SpaceAfter), LineSpacingRule = Convert.ToInt32(r.ParagraphFormat.LineSpacingRule)
            };
            MessageBox.Show("تم حفظ التنسيق الحالي مؤقتًا.", "WordPro Suite");
        }

        internal static void ApplyFormatSnapshot()
        {
            if (Snapshot == null) throw new InvalidOperationException("احفظ تنسيقًا أولاً.");
            dynamic r = WordContext.TargetRange;
            r.Font.Name = Snapshot.FontName; r.Font.NameBi = Snapshot.FontNameBi;
            r.Font.Size = Snapshot.FontSize; r.Font.SizeBi = Snapshot.FontSizeBi;
            r.Font.Bold = Snapshot.Bold; r.Font.Italic = Snapshot.Italic;
            r.ParagraphFormat.Alignment = Snapshot.Alignment; r.ParagraphFormat.ReadingOrder = Snapshot.ReadingOrder;
            r.ParagraphFormat.SpaceAfter = Snapshot.SpaceAfter; r.ParagraphFormat.LineSpacingRule = Snapshot.LineSpacingRule;
        }

        // ---------------- Tables ----------------
        private static dynamic CurrentTable()
        {
            dynamic s = WordContext.Selection;
            if (s == null || s.Tables.Count == 0) throw new InvalidOperationException("ضع المؤشر داخل جدول أولاً.");
            return s.Tables[1];
        }

        private static dynamic CurrentCell()
        {
            dynamic s = WordContext.Selection;
            if (s == null || s.Cells.Count == 0) throw new InvalidOperationException("ضع المؤشر داخل خلية أولاً.");
            return s.Cells[1];
        }

        internal static void TableLtr() { dynamic t = CurrentTable(); t.Direction = 1; t.Range.ParagraphFormat.ReadingOrder = 1; }

        internal static void TableProfessional()
        {
            dynamic t = CurrentTable();
            t.Borders.Enable = 1; t.AutoFitBehavior(2);
            t.Range.Font.Name = "Arial"; t.Range.Font.NameBi = "Arial"; t.Range.Font.Size = 11; t.Range.Font.SizeBi = 11;
            t.Range.ParagraphFormat.SpaceAfter = 0;
            t.Rows[1].Range.Font.Bold = -1; t.Rows[1].Range.ParagraphFormat.Alignment = 1; t.Rows[1].Cells.VerticalAlignment = 1;
            t.Rows[1].Range.Shading.BackgroundPatternColor = 14277081;
            t.Rows[1].HeadingFormat = -1;
        }

        internal static void TableHeaderStyle()
        {
            dynamic row = CurrentTable().Rows[1];
            row.Range.Font.Bold = -1; row.Range.ParagraphFormat.Alignment = 1; row.Cells.VerticalAlignment = 1;
            row.Range.Shading.BackgroundPatternColor = 14277081; row.HeadingFormat = -1;
        }

        internal static void TableBandedRows()
        {
            dynamic t = CurrentTable();
            for (int i = 2; i <= t.Rows.Count; i++)
                t.Rows[i].Range.Shading.BackgroundPatternColor = (i % 2 == 0) ? 15921906 : 16777215;
        }

        internal static void TableTrimCells()
        {
            dynamic t = CurrentTable();
            foreach (dynamic cell in t.Range.Cells)
            {
                string text = WordContext.CleanCell(Convert.ToString(cell.Range.Text));
                cell.Range.Text = text;
            }
        }

        internal static void TableNumberFirstColumn()
        {
            dynamic t = CurrentTable();
            int start = t.Rows.Count > 1 ? 2 : 1;
            for (int i = start; i <= t.Rows.Count; i++) t.Cell(i, 1).Range.Text = (i - start + 1).ToString();
        }

        internal static void TableColumnAggregate(bool average)
        {
            dynamic t = CurrentTable();
            int col = Convert.ToInt32(CurrentCell().ColumnIndex);
            var values = new List<double>();
            for (int row = 1; row <= t.Rows.Count; row++)
            {
                double v;
                string raw = TextTransforms.Western(WordContext.CleanCell(Convert.ToString(t.Cell(row, col).Range.Text))).Replace(",", "");
                if (Double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out v)) values.Add(v);
            }
            if (values.Count == 0) throw new InvalidOperationException("لم يتم العثور على أرقام في العمود المحدد.");
            t.Rows.Add();
            int last = t.Rows.Count;
            t.Cell(last, 1).Range.Text = average ? "المتوسط" : "المجموع";
            t.Cell(last, col).Range.Text = (average ? values.Average() : values.Sum()).ToString("0.##", CultureInfo.InvariantCulture);
            t.Rows[last].Range.Font.Bold = -1;
        }

        internal static void TableRemoveDuplicateRows()
        {
            dynamic t = CurrentTable();
            var seen = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
            for (int row = t.Rows.Count; row >= 1; row--)
            {
                var sb = new StringBuilder();
                foreach (dynamic cell in t.Rows[row].Cells) sb.Append(WordContext.CleanCell(Convert.ToString(cell.Range.Text))).Append('|');
                string key = sb.ToString();
                if (!seen.Add(key)) t.Rows[row].Delete();
            }
        }

        internal static void TableSort(bool descending)
        {
            dynamic t = CurrentTable();
            int col = Convert.ToInt32(CurrentCell().ColumnIndex);
            t.Sort(ExcludeHeader: true, FieldNumber: "Column " + col, SortFieldType: 0, SortOrder: descending ? 1 : 0);
        }

        internal static void TableAddRowAbove()
        {
            dynamic t = CurrentTable(); int row = Convert.ToInt32(CurrentCell().RowIndex); t.Rows.Add(t.Rows[row]);
        }
        internal static void TableAddRowBelow() => CurrentTable().Rows.Add();
        internal static void TableAddColumnLeft()
        {
            dynamic t = CurrentTable(); int col = Convert.ToInt32(CurrentCell().ColumnIndex); t.Columns.Add(t.Columns[col]);
        }
        internal static void TableAddColumnRight() => CurrentTable().Columns.Add();
        internal static void TableDeleteRow()
        {
            dynamic t = CurrentTable(); int row = Convert.ToInt32(CurrentCell().RowIndex); t.Rows[row].Delete();
        }
        internal static void TableDeleteColumn()
        {
            dynamic t = CurrentTable(); int col = Convert.ToInt32(CurrentCell().ColumnIndex); t.Columns[col].Delete();
        }
        internal static void TableMergeCells() => WordContext.Selection.Cells.Merge();

        internal static void TableSplitCell()
        {
            string rowsRaw = Prompt.Show("تقسيم الخلية", "عدد الصفوف:", "2"); if (rowsRaw == null) return;
            string colsRaw = Prompt.Show("تقسيم الخلية", "عدد الأعمدة:", "2"); if (colsRaw == null) return;
            int rows, cols;
            if (!Int32.TryParse(rowsRaw, out rows) || !Int32.TryParse(colsRaw, out cols) || rows < 1 || cols < 1)
                throw new InvalidOperationException("أدخل أعدادًا صحيحة.");
            CurrentCell().Split(rows, cols);
        }

        internal static void TableBorders(bool enabled) => CurrentTable().Borders.Enable = enabled ? 1 : 0;
        internal static void TableVertical(int value) => CurrentTable().Range.Cells.VerticalAlignment = value;

        internal static void TextToTable()
        {
            dynamic r = WordContext.TargetRange;
            string delimiter = Prompt.Show("تحويل النص إلى جدول", "الفاصل: اكتب TAB أو COMMA أو PARAGRAPH", "TAB");
            if (delimiter == null) return;
            int sep = String.Equals(delimiter, "TAB", StringComparison.OrdinalIgnoreCase) ? 1 :
                      String.Equals(delimiter, "COMMA", StringComparison.OrdinalIgnoreCase) ? 2 :
                      String.Equals(delimiter, "PARAGRAPH", StringComparison.OrdinalIgnoreCase) ? 0 : 1;
            r.ConvertToTable(sep);
        }

        internal static void TableTranspose()
        {
            dynamic t = CurrentTable();
            int rows = t.Rows.Count, cols = t.Columns.Count;
            string[,] data = new string[rows, cols];
            for (int i = 1; i <= rows; i++) for (int j = 1; j <= cols; j++) data[i - 1, j - 1] = WordContext.CleanCell(Convert.ToString(t.Cell(i, j).Range.Text));
            int end = t.Range.End;
            dynamic r = WordContext.Document.Range(end, end);
            r.InsertParagraphAfter(); r.Collapse(0);
            dynamic nt = WordContext.Document.Tables.Add(r, cols, rows);
            for (int i = 1; i <= cols; i++) for (int j = 1; j <= rows; j++) nt.Cell(i, j).Range.Text = data[j - 1, i - 1];
            t.Delete(); nt.AutoFitBehavior(2); nt.Borders.Enable = 1;
        }

        // ---------------- Document and layout ----------------
        internal static void InsertPageBreak() => WordContext.Selection.InsertBreak(7);
        internal static void InsertSectionNext() => WordContext.Selection.InsertBreak(2);
        internal static void InsertSectionContinuous() => WordContext.Selection.InsertBreak(3);
        internal static void RemovePageBreaks() => WordContext.ReplaceAll(WordContext.Document.Content, "^m", "");
        internal static void RemoveSectionBreaks() => WordContext.ReplaceAll(WordContext.Document.Content, "^b", "");
        internal static void Columns(int count) => WordContext.Document.PageSetup.TextColumns.SetCount(count);
        internal static void LineNumbers(bool enabled) => WordContext.Document.PageSetup.LineNumbering.Active = enabled ? -1 : 0;
        internal static void SetPaper(int paperSize) => WordContext.Document.PageSetup.PaperSize = paperSize;

        internal static void SetHeaderText()
        {
            string text = Prompt.Show("رأس الصفحة", "اكتب نص رأس الصفحة:"); if (text == null) return;
            foreach (dynamic section in WordContext.Document.Sections) section.Headers[1].Range.Text = text;
        }

        internal static void SetFooterText()
        {
            string text = Prompt.Show("تذييل الصفحة", "اكتب نص تذييل الصفحة:"); if (text == null) return;
            foreach (dynamic section in WordContext.Document.Sections) section.Footers[1].Range.Text = text;
        }

        internal static void ClearHeadersFooters()
        {
            foreach (dynamic section in WordContext.Document.Sections)
            {
                foreach (dynamic h in section.Headers) h.Range.Text = "";
                foreach (dynamic f in section.Footers) f.Range.Text = "";
            }
        }

        internal static void InsertDateTime() => WordContext.Selection.TypeText(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        internal static void InsertFileNameField() => WordContext.Document.Fields.Add(WordContext.Selection.Range, -1, "FILENAME \\p", true);

        internal static void DocumentStatistics()
        {
            dynamic d = WordContext.Document;
            int words = d.ComputeStatistics(0); int chars = d.ComputeStatistics(3); int pages = d.ComputeStatistics(2); int paragraphs = d.ComputeStatistics(4);
            MessageBox.Show("الصفحات: " + pages + "\nالكلمات: " + words + "\nالأحرف: " + chars + "\nالفقرات: " + paragraphs + "\nالجداول: " + d.Tables.Count + "\nالصور: " + (d.InlineShapes.Count + d.Shapes.Count), "إحصاءات المستند");
        }

        internal static void ListStyleFonts()
        {
            var fonts = new SortedSet<string>(StringComparer.CurrentCultureIgnoreCase);
            foreach (dynamic style in WordContext.Document.Styles)
            {
                try { string n = Convert.ToString(style.Font.Name); if (!String.IsNullOrWhiteSpace(n)) fonts.Add(n); } catch { }
                try { string n = Convert.ToString(style.Font.NameBi); if (!String.IsNullOrWhiteSpace(n)) fonts.Add(n); } catch { }
            }
            MessageBox.Show(fonts.Count == 0 ? "لم يتم العثور على خطوط." : String.Join("\n", fonts), "خطوط الأنماط");
        }

        internal static void ReplaceFont()
        {
            string oldFont = Prompt.Show("استبدال الخط", "اسم الخط الحالي:", "Arial"); if (oldFont == null) return;
            string newFont = Prompt.Show("استبدال الخط", "اسم الخط الجديد:", "Arial"); if (newFont == null) return;
            dynamic find = WordContext.Document.Content.Find;
            find.ClearFormatting(); find.Font.Name = oldFont; find.Replacement.ClearFormatting(); find.Replacement.Font.Name = newFont;
            find.Text = ""; find.Replacement.Text = ""; find.Forward = true; find.Wrap = 1; find.Format = true; find.Execute(Replace: 2);
        }

        internal static void RemoveHiddenText()
        {
            dynamic find = WordContext.Document.Content.Find;
            find.ClearFormatting(); find.Font.Hidden = -1; find.Replacement.ClearFormatting(); find.Text = ""; find.Replacement.Text = ""; find.Format = true; find.Execute(Replace: 2);
        }

        internal static void UnlinkFields()
        {
            dynamic d = WordContext.Document;
            foreach (dynamic story in d.StoryRanges)
            {
                dynamic cur = story;
                while (cur != null)
                {
                    for (int i = cur.Fields.Count; i >= 1; i--) cur.Fields[i].Unlink();
                    cur = cur.NextStoryRange;
                }
            }
        }

        internal static void RemoveBookmarks()
        {
            dynamic d = WordContext.Document;
            while (d.Bookmarks.Count > 0) d.Bookmarks[1].Delete();
        }

        internal static void InsertCoverPage()
        {
            dynamic d = WordContext.Document;
            dynamic r = d.Range(0, 0);
            string title = Prompt.Show("صفحة غلاف", "عنوان المستند:", WordContext.BaseName()); if (title == null) return;
            string subtitle = Prompt.Show("صفحة غلاف", "العنوان الفرعي أو الجهة:", ""); if (subtitle == null) subtitle = "";
            r.Text = "\r\r\r\r" + title + "\r\r" + subtitle + "\r\r\r" + DateTime.Today.ToString("yyyy-MM-dd") + "\r";
            r.Font.Name = "Arial"; r.Font.NameBi = "Arial"; r.Font.Size = 20; r.Font.SizeBi = 20; r.Font.Bold = -1;
            r.ParagraphFormat.Alignment = 1; r.ParagraphFormat.ReadingOrder = 0;
            r.InsertBreak(7);
        }

        internal static void InsertSignatureBlock()
        {
            string name = Prompt.Show("كتلة توقيع", "الاسم:"); if (name == null) return;
            string title = Prompt.Show("كتلة توقيع", "الصفة:"); if (title == null) title = "";
            WordContext.Selection.TypeText("\r\rالاسم: " + name + "\rالصفة: " + title + "\rالتوقيع: ____________________\rالتاريخ: ____ / ____ / ______\r");
        }

        // ---------------- Review and security ----------------
        internal static void TrackChanges(bool enabled) => WordContext.Document.TrackRevisions = enabled;

        internal static void ProtectDocument(int type)
        {
            string password = Prompt.Show("حماية المستند", "كلمة المرور:", "", true); if (password == null) return;
            WordContext.Document.Protect(Type: type, NoReset: true, Password: password);
        }

        internal static void UnprotectDocument()
        {
            string password = Prompt.Show("إلغاء الحماية", "كلمة المرور:", "", true); if (password == null) return;
            WordContext.Document.Unprotect(password);
        }

        internal static void CleanProperties()
        {
            dynamic d = WordContext.Document;
            d.RemoveDocumentInformation(99);
            try
            {
                for (int i = d.CustomDocumentProperties.Count; i >= 1; i--) d.CustomDocumentProperties[i].Delete();
            }
            catch { }
        }

        internal static void RemoveExternalLinks()
        {
            dynamic d = WordContext.Document;
            object sources = null;
            try { sources = d.LinkSources(1); } catch { }
            if (sources == null) { MessageBox.Show("لا توجد روابط خارجية.", "WordPro Suite"); return; }
            foreach (object source in (Array)sources)
                try { d.BreakLink(Convert.ToString(source), 1); } catch { }
        }

        internal static void FinalShareWorkflow()
        {
            Commands.SaveBackup();
            Commands.AcceptRevisions();
            Commands.RemoveComments();
            Commands.UpdateFields();
            CleanProperties();
            RemoveHiddenText();
            MessageBox.Show("تم تنظيف المستند وتجهيزه للمشاركة. تم إنشاء نسخة احتياطية أولاً.", "WordPro Suite");
        }

        // ---------------- Files and PDF ----------------
        internal static void SaveVersionedCopy()
        {
            dynamic d = WordContext.Document;
            string source = ""; try { source = Convert.ToString(d.FullName); } catch { }
            string dir = !String.IsNullOrWhiteSpace(source) && File.Exists(source) ? Path.GetDirectoryName(source) : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string ext = Path.GetExtension(Convert.ToString(d.Name)); if (String.IsNullOrWhiteSpace(ext)) ext = ".docx";
            string output = Path.Combine(dir, WordContext.BaseName() + "_v" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ext);
            d.SaveCopyAs(output); MessageBox.Show("تم حفظ نسخة إصدار:\n" + output, "WordPro Suite");
        }

        internal static void SaveDocxCopy()
        {
            string output = WordContext.SavePath("حفظ نسخة DOCX", "Word Document (*.docx)|*.docx", WordContext.BaseName() + "_Copy.docx");
            if (String.IsNullOrWhiteSpace(output)) return;
            dynamic source = WordContext.Document;
            source.Content.Copy();
            dynamic temp = WordContext.Application.Documents.Add();
            temp.Content.PasteAndFormat(16); temp.SaveAs2(output, 16); temp.Close(false);
        }

        internal static void ExportSelectionPdf()
        {
            dynamic s = WordContext.Selection;
            if (s.Range.Start == s.Range.End) throw new InvalidOperationException("حدد نصًا أو محتوى أولاً.");
            string output = WordContext.SavePath("تصدير التحديد PDF", "PDF (*.pdf)|*.pdf", WordContext.BaseName() + "_Selection.pdf");
            if (String.IsNullOrWhiteSpace(output)) return;
            dynamic temp = WordContext.Application.Documents.Add();
            s.Range.Copy(); temp.Content.PasteAndFormat(16); temp.ExportAsFixedFormat(output, 17); temp.Close(false);
            MessageBox.Show("تم التصدير:\n" + output, "WordPro Suite");
        }

        internal static void ExportSectionsPdf()
        {
            dynamic d = WordContext.Document;
            string folder;
            using (var dlg = new FolderBrowserDialog { Description = "اختر مجلد حفظ أقسام PDF" })
                if (dlg.ShowDialog() != DialogResult.OK) return; else folder = dlg.SelectedPath;
            int index = 1;
            foreach (dynamic section in d.Sections)
            {
                dynamic temp = WordContext.Application.Documents.Add();
                section.Range.Copy(); temp.Content.PasteAndFormat(16);
                string path = Path.Combine(folder, WordContext.BaseName() + "_Section_" + index.ToString("00") + ".pdf");
                temp.ExportAsFixedFormat(path, 17); temp.Close(false); index++;
            }
            MessageBox.Show("تم تصدير " + (index - 1) + " قسمًا.", "WordPro Suite");
        }

        internal static void MergeDocuments()
        {
            string[] files;
            using (var dlg = new OpenFileDialog { Filter = "Word Documents|*.doc;*.docx;*.docm;*.rtf", Multiselect = true, Title = "اختر الملفات بالترتيب" })
                if (dlg.ShowDialog() != DialogResult.OK) return; else files = dlg.FileNames;
            dynamic r = WordContext.Selection.Range;
            foreach (string file in files)
            {
                r.InsertFile(file); r.Collapse(0); r.InsertBreak(7); r.Collapse(0);
            }
        }

        internal static void BatchPdf()
        {
            string folder;
            using (var dlg = new FolderBrowserDialog { Description = "اختر مجلد ملفات Word" })
                if (dlg.ShowDialog() != DialogResult.OK) return; else folder = dlg.SelectedPath;
            string outDir = Path.Combine(folder, "PDF_Output"); Directory.CreateDirectory(outDir);
            string[] files = Directory.GetFiles(folder, "*.doc*").Where(x => !x.StartsWith("~$")).ToArray();
            dynamic app = WordContext.Application; bool oldVisible = app.Visible;
            int ok = 0;
            foreach (string file in files)
            {
                dynamic doc = null;
                try
                {
                    doc = app.Documents.Open(file, ReadOnly: true, Visible: false);
                    string output = Path.Combine(outDir, Path.GetFileNameWithoutExtension(file) + ".pdf");
                    doc.ExportAsFixedFormat(output, 17); ok++;
                }
                catch (Exception ex) { Logger.Error("Batch PDF failed: " + file, ex); }
                finally { try { if (doc != null) doc.Close(false); } catch { } }
            }
            app.Visible = oldVisible;
            MessageBox.Show("تم تحويل " + ok + " من " + files.Length + " ملفًا.\n" + outDir, "تحويل دفعي PDF");
        }

        internal static void OpenDocumentFolder()
        {
            string path = Convert.ToString(WordContext.Document.FullName);
            if (!File.Exists(path)) throw new InvalidOperationException("احفظ المستند أولاً.");
            Process.Start("explorer.exe", "/select,\"" + path + "\"");
        }

        internal static void CopyDocumentPath()
        {
            string path = Convert.ToString(WordContext.Document.FullName);
            if (String.IsNullOrWhiteSpace(path)) throw new InvalidOperationException("احفظ المستند أولاً.");
            Clipboard.SetText(path); MessageBox.Show("تم نسخ مسار المستند.", "WordPro Suite");
        }

        internal static void SaveAllDocuments()
        {
            foreach (dynamic doc in WordContext.Application.Documents) if (!doc.Saved) doc.Save();
            MessageBox.Show("تم حفظ جميع المستندات المفتوحة.", "WordPro Suite");
        }

        internal static void ArchivePackage()
        {
            dynamic d = WordContext.Document;
            string full = Convert.ToString(d.FullName);
            if (!File.Exists(full)) throw new InvalidOperationException("احفظ المستند أولاً.");
            string folder = Path.Combine(Path.GetDirectoryName(full), WordContext.BaseName() + "_Package_" + DateTime.Now.ToString("yyyyMMdd_HHmm"));
            Directory.CreateDirectory(folder);
            string copy = Path.Combine(folder, Path.GetFileName(full)); d.SaveCopyAs(copy);
            string pdf = Path.Combine(folder, WordContext.BaseName() + ".pdf"); d.ExportAsFixedFormat(pdf, 17);
            File.WriteAllText(Path.Combine(folder, "PACKAGE_INFO.txt"), "Created: " + DateTime.Now + Environment.NewLine + "Source: " + full, Encoding.UTF8);
            Process.Start("explorer.exe", folder);
        }

        // ---------------- Templates ----------------
        private static readonly Dictionary<string, string> MoreTemplates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["memo"] = "مذكرة داخلية\rإلى:\rمن:\rالتاريخ:\rالموضوع:\r\rالخلفية:\r\rالإجراء المطلوب:\r\rالموعد النهائي:\r",
            ["quotation"] = "عرض سعر\rرقم العرض:\rالتاريخ:\rالعميل:\rمدة صلاحية العرض:\r\rالبند\tالوصف\tالكمية\tسعر الوحدة\tالإجمالي\r\rالإجمالي قبل الضريبة:\rالضريبة:\rالإجمالي النهائي:\r\rالشروط والملاحظات:\r",
            ["invoice"] = "فاتورة\rرقم الفاتورة:\rالتاريخ:\rالمورد:\rالعميل:\r\rالبند\tالكمية\tالسعر\tالإجمالي\r\rالمجموع:\rالضريبة:\rالإجمالي المستحق:\r",
            ["certificate"] = "شهادة تقدير\r\rتشهد ................................................\rبأن السيد/السيدة ................................................\rقد ........................................................................\r\rوذلك تقديرًا لجهوده/جهودها المميزة.\r\rالتاريخ:\rالتوقيع والختم:\r",
            ["attendance"] = "كشف حضور\rاسم الفعالية:\rالتاريخ:\rالمكان:\r\rم\tالاسم\tالجهة\tرقم الهاتف\tالتوقيع\r",
            ["tor"] = "الشروط المرجعية (ToR)\r\r1. الخلفية\r\r2. الهدف\r\r3. نطاق العمل\r\r4. المخرجات المطلوبة\r\r5. المدة والجدول الزمني\r\r6. المؤهلات والخبرات\r\r7. آلية الإشراف والتقارير\r\r8. معايير التقييم\r",
            ["concept"] = "مذكرة مفاهيمية\rاسم المبادرة:\rالجهة المنفذة:\rالموقع:\rالمدة:\rالميزانية التقديرية:\r\r1. المشكلة والسياق\r\r2. الهدف العام\r\r3. الأهداف المحددة\r\r4. الفئات المستهدفة\r\r5. الأنشطة الرئيسية\r\r6. النتائج المتوقعة\r\r7. المخاطر والاستدامة\r",
            ["donor"] = "تقرير للجهة المانحة\rاسم المشروع:\rالفترة المشمولة:\r\r1. الملخص التنفيذي\r\r2. التقدم مقابل المؤشرات\r\r3. الأنشطة المنفذة\r\r4. النتائج والأثر\r\r5. التحديات والإجراءات التصحيحية\r\r6. الوضع المالي\r\r7. خطة الفترة القادمة\r\r8. المرفقات والصور\r",
            ["sop"] = "إجراء تشغيلي قياسي (SOP)\rالرمز:\rالإصدار:\rتاريخ السريان:\rالجهة المسؤولة:\r\r1. الغرض\r\r2. النطاق\r\r3. التعريفات\r\r4. المسؤوليات\r\r5. خطوات الإجراء\r\r6. السلامة وإدارة المخاطر\r\r7. السجلات والنماذج\r\r8. المراجعة والتحديث\r",
            ["risk"] = "سجل المخاطر\r\rم\tالخطر\tالاحتمالية\tالأثر\tالتقييم\tإجراءات التخفيف\tالمسؤول\tالحالة\r",
            ["action"] = "خطة عمل\r\rم\tالنشاط\tالمخرج\tالمسؤول\tتاريخ البدء\tتاريخ الانتهاء\tالمؤشر\tالحالة\r",
            ["cover-letter"] = "التاريخ:\r\rالسادة/ ................................................ المحترمون\r\rالموضوع: طلب/تقديم ................................................\r\rتحية طيبة وبعد،\r\rأتقدم إليكم بـ ................................................................................\r\rشاكرًا لكم حسن تعاونكم، وتفضلوا بقبول الاحترام.\r\rالاسم:\rرقم الهاتف:\rالبريد الإلكتروني:\rالتوقيع:\r"
        };

        internal static void InsertMoreTemplate(string key)
        {
            string text;
            if (!MoreTemplates.TryGetValue(key, out text)) throw new InvalidOperationException("القالب غير موجود.");
            dynamic s = WordContext.Selection; s.TypeText(text);
            dynamic r = s.Range; r.ParagraphFormat.ReadingOrder = 0; r.ParagraphFormat.Alignment = 2;
            r.Font.Name = "Arial"; r.Font.NameBi = "Arial"; r.Font.Size = 14; r.Font.SizeBi = 14;
        }

        // ---------------- Utilities / licensing ----------------
        internal static void Calculator()
        {
            dynamic r = WordContext.TargetRange;
            string expr = TextTransforms.Western((Convert.ToString(r.Text) ?? "").Trim()).Replace("×", "*").Replace("÷", "/");
            if (!Regex.IsMatch(expr, @"^[0-9+\-*/().,%\s]+$") || String.IsNullOrWhiteSpace(expr))
                throw new InvalidOperationException("حدد تعبيرًا حسابيًا بسيطًا فقط.");
            object value = new DataTable().Compute(expr, "");
            r.Text = expr + " = " + Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        internal static void InsertGuid() => WordContext.Selection.TypeText(Guid.NewGuid().ToString());

        internal static void InsertArabicLorem()
        {
            string raw = Prompt.Show("نص عربي تجريبي", "عدد الفقرات:", "3"); if (raw == null) return;
            int count; if (!Int32.TryParse(raw, out count)) count = 3; count = Math.Max(1, Math.Min(20, count));
            string para = "هذا نص عربي تجريبي يُستخدم لمعاينة التنسيق وتوزيع الفقرات داخل المستند. يمكن استبداله لاحقًا بالمحتوى النهائي مع الحفاظ على الأنماط والمسافات والمحاذاة.";
            WordContext.Selection.TypeText(String.Join("\r\r", Enumerable.Repeat(para, count)));
        }

        internal static void InsertChecklist()
        {
            string text = Prompt.Show("قائمة مهام", "اكتب البنود مفصولة بفواصل أو أسطر:"); if (text == null) return;
            string[] items = Regex.Split(text, @"\r\n|\r|\n|,").Where(x => x.Trim().Length > 0).ToArray();
            WordContext.Selection.TypeText(String.Join("\r", items.Select(x => "☐ " + x.Trim())));
        }

        internal static void ShowMachineId()
        {
            Clipboard.SetText(LicenseManager.MachineId);
            MessageBox.Show("معرّف الجهاز:\n" + LicenseManager.MachineId + "\n\nتم نسخه إلى الحافظة.", "WordPro Suite");
        }

        internal static void ShowLicenseInfo()
        {
            var s = LicenseManager.Current;
            MessageBox.Show(LicenseManager.StatusText + "\n\nمعرّف الجهاز:\n" + LicenseManager.MachineId + "\n\nالإصدار: " + (s.Edition ?? ""), "حالة الترخيص");
        }

        internal static void Activate() => ActivationForm.ShowActivation();
        internal static void OpenLog() { if (File.Exists(Logger.CurrentLog)) Process.Start("notepad.exe", Logger.CurrentLog); else MessageBox.Show("لا يوجد سجل بعد."); }

        private sealed class FormatSnapshot
        {
            internal string FontName, FontNameBi;
            internal float FontSize, FontSizeBi, SpaceAfter;
            internal int Bold, Italic, Alignment, ReadingOrder, LineSpacingRule;
        }
    }
}
