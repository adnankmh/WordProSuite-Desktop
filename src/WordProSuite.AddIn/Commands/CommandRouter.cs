using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WordProSuite.Desktop.Infrastructure;
using WordProSuite.Desktop.Licensing;
using WordProSuite.Desktop.UI;

namespace WordProSuite.Desktop.Commands
{
    internal sealed class CommandDescriptor
    {
        internal CommandDescriptor(string id, string category, string title, string description, string keywords, Action execute, bool requiresLicense)
        {
            Id = id; Category = category; Title = title; Description = description; Keywords = keywords ?? "";
            Execute = execute; RequiresLicense = requiresLicense;
        }
        internal string Id { get; }
        internal string Category { get; }
        internal string Title { get; }
        internal string Description { get; }
        internal string Keywords { get; }
        internal Action Execute { get; }
        internal bool RequiresLicense { get; }
    }

    internal static class CommandRouter
    {
        private static readonly Dictionary<string, CommandDescriptor> Map =
            new Dictionary<string, CommandDescriptor>(StringComparer.OrdinalIgnoreCase);

        static CommandRouter()
        {
            // العربية والاتجاه
            A("rtl", "العربية والاتجاه", "اتجاه RTL", "تطبيق اتجاه الكتابة من اليمين إلى اليسار.", "عربي يمين", Commands.Rtl);
            A("ltr", "العربية والاتجاه", "اتجاه LTR", "تطبيق اتجاه الكتابة من اليسار إلى اليمين.", "انجليزي يسار", Commands.Ltr);
            A("auto-direction", "العربية والاتجاه", "اكتشاف الاتجاه تلقائيًا", "اختيار RTL أو LTR حسب النص المحدد.", "تلقائي لغة", AdvancedCommands.AutoDirection);
            A("remove-diacritics", "العربية والاتجاه", "حذف التشكيل", "حذف الحركات والعلامات العربية.", "حركات", Commands.RemoveDiacritics);
            A("remove-tatweel", "العربية والاتجاه", "حذف التطويل", "حذف الكشيدة ـ من النص.", "كشيدة", Commands.RemoveTatweel);
            A("normalize-arabic", "العربية والاتجاه", "توحيد الحروف العربية", "توحيد أشكال الألف والياء والهمزات.", "الف همزة ياء", Commands.NormalizeArabic);
            A("normalize-arabic-punctuation", "العربية والاتجاه", "ترقيم عربي ذكي", "تحويل الفاصلة والفاصلة المنقوطة وعلامة السؤال إلى العربية وضبط المسافات.", "فاصلة ترقيم", AdvancedCommands.NormalizeArabicPunctuation);
            A("arabic-quotes", "العربية والاتجاه", "علامات اقتباس عربية", "تحويل علامات الاقتباس إلى « ».", "اقتباس", AdvancedCommands.ArabicQuotes);
            A("remove-zero-width", "العربية والاتجاه", "حذف المحارف المخفية", "حذف محارف zero-width وBOM من النص.", "مخفي", AdvancedCommands.RemoveZeroWidth);
            A("digits-eastern", "العربية والاتجاه", "أرقام عربية", "تحويل الأرقام الغربية إلى ٠١٢٣.", "ارقام", Commands.ToEastern);
            A("digits-western", "العربية والاتجاه", "أرقام غربية", "تحويل الأرقام العربية والفارسية إلى 0123.", "ارقام", Commands.ToWestern);
            A("keyboard-ar-to-en", "العربية والاتجاه", "تصحيح كتابة عربية إلى إنجليزية", "إصلاح نص كُتب بلوحة مفاتيح عربية بدل الإنجليزية.", "كيبورد لوحة", AdvancedCommands.KeyboardArabicToEnglish);
            A("keyboard-en-to-ar", "العربية والاتجاه", "تصحيح كتابة إنجليزية إلى عربية", "إصلاح نص كُتب بلوحة مفاتيح إنجليزية بدل العربية.", "كيبورد لوحة", AdvancedCommands.KeyboardEnglishToArabic);
            A("insert-arabic-date", "العربية والاتجاه", "إدراج التاريخ بالعربية", "إدراج التاريخ الميلادي بأسماء الأشهر العربية.", "تاريخ", AdvancedCommands.InsertArabicDate);
            A("insert-hijri-date", "العربية والاتجاه", "إدراج التاريخ الهجري", "إدراج تاريخ أم القرى الحالي.", "هجري تاريخ", AdvancedCommands.InsertHijriDate);
            A("reverse-text", "العربية والاتجاه", "عكس النص", "عكس ترتيب الأحرف في النص المحدد.", "عكس", AdvancedCommands.ReverseText);
            A("number-arabic-words", "العربية والاتجاه", "رقم إلى كلمات عربية", "تحويل الرقم المحدد إلى كلمات عربية.", "تفقيط أرقام", AdvancedCommands.NumberToArabicWords);
            A("arabic-clean-all", "العربية والاتجاه", "تنظيف عربي شامل", "حذف التشكيل والتطويل وضبط RTL وArial 14.", "تنظيف شامل", Commands.ArabicCleanAll);

            // النص والتحرير
            A("collapse-spaces", "النص والتحرير", "تنظيف المسافات", "حذف المسافات المتكررة.", "مسافة", Commands.CollapseSpaces);
            A("trim-lines", "النص والتحرير", "قص حواف الأسطر", "حذف المسافات من بداية ونهاية كل سطر.", "trim", AdvancedCommands.TrimLines);
            A("remove-empty-paragraphs", "النص والتحرير", "حذف الفقرات الفارغة", "اختصار الفقرات الفارغة المتكررة.", "فراغ", Commands.RemoveEmptyParagraphs);
            A("join-paragraphs", "النص والتحرير", "دمج الفقرات", "تحويل الفقرات المحددة إلى فقرة واحدة.", "دمج", AdvancedCommands.JoinParagraphs);
            A("tabs-to-spaces", "النص والتحرير", "Tabs إلى مسافات", "استبدال علامات الجدولة بمسافات.", "tab", Commands.TabsToSpaces);
            A("linebreaks-to-paragraphs", "النص والتحرير", "فواصل الأسطر إلى فقرات", "تحويل line breaks إلى paragraph breaks.", "سطر فقرة", Commands.LineBreaksToParagraphs);
            A("remove-duplicate-lines", "النص والتحرير", "حذف الأسطر المكررة", "الاحتفاظ بأول ظهور لكل سطر.", "مكرر", Commands.RemoveDuplicateLines);
            A("remove-duplicate-paragraphs", "النص والتحرير", "حذف الفقرات المكررة", "حذف الفقرات المتكررة مع تجاهل اختلاف المسافات.", "تكرار", AdvancedCommands.RemoveDuplicateParagraphs);
            A("remove-duplicate-words", "النص والتحرير", "حذف الكلمات المتتابعة المكررة", "تحويل كلمة كلمة إلى كلمة واحدة.", "مكرر كلمات", AdvancedCommands.RemoveDuplicateConsecutiveWords);
            A("sort-lines-asc", "النص والتحرير", "فرز تصاعدي", "فرز الأسطر تصاعديًا.", "ترتيب", () => Commands.SortLines(false));
            A("sort-lines-desc", "النص والتحرير", "فرز تنازلي", "فرز الأسطر تنازليًا.", "ترتيب", () => Commands.SortLines(true));
            A("prefix-lines", "النص والتحرير", "إضافة بادئة للأسطر", "إضافة نص قبل كل سطر.", "بادئة", AdvancedCommands.PrefixLines);
            A("suffix-lines", "النص والتحرير", "إضافة لاحقة للأسطر", "إضافة نص بعد كل سطر.", "لاحقة", AdvancedCommands.SuffixLines);
            A("split-by-delimiter", "النص والتحرير", "تقسيم حسب فاصل", "استبدال فاصل محدد بفواصل فقرات.", "تقسيم", AdvancedCommands.SplitByDelimiter);
            A("paragraphs-bullets", "النص والتحرير", "تحويل إلى نقاط", "تحويل الفقرات إلى قائمة نقطية.", "قائمة", AdvancedCommands.ParagraphsToBullets);
            A("paragraphs-numbers", "النص والتحرير", "تحويل إلى ترقيم", "تحويل الفقرات إلى قائمة مرقمة.", "قائمة أرقام", AdvancedCommands.ParagraphsToNumbers);
            A("clear-list", "النص والتحرير", "إزالة تنسيق القائمة", "إزالة الترقيم أو التعداد من الفقرات.", "قائمة", AdvancedCommands.ClearList);
            A("uppercase", "النص والتحرير", "UPPERCASE", "تحويل الأحرف الإنجليزية إلى كبيرة.", "case", Commands.Uppercase);
            A("lowercase", "النص والتحرير", "lowercase", "تحويل الأحرف الإنجليزية إلى صغيرة.", "case", Commands.Lowercase);
            A("titlecase", "النص والتحرير", "Title Case", "تكبير بدايات الكلمات الإنجليزية.", "case", Commands.TitleCase);
            A("sentence-case", "النص والتحرير", "Sentence case", "تكبير بداية كل جملة.", "case", AdvancedCommands.SentenceCase);
            A("invert-case", "النص والتحرير", "Invert Case", "عكس حالة الأحرف الكبيرة والصغيرة.", "case", AdvancedCommands.InvertCase);
            A("smart-quotes", "النص والتحرير", "اقتباس ذكي", "تحويل الاقتباسات المستقيمة إلى ذكية.", "quotes", AdvancedCommands.SmartQuotes);
            A("normalize-dashes", "النص والتحرير", "توحيد الشرطات", "توحيد hyphen وen dash وem dash.", "شرطة", AdvancedCommands.NormalizeDashes);
            A("remove-nonprinting", "النص والتحرير", "حذف المحارف غير المطبوعة", "حذف المحارف التحكمية غير المرئية.", "control", AdvancedCommands.RemoveNonPrinting);
            A("remove-hyperlinks", "النص والتحرير", "إزالة الروابط", "حذف الارتباطات مع إبقاء النص.", "روابط", Commands.RemoveHyperlinks);
            A("extract-emails", "النص والتحرير", "استخراج البريد الإلكتروني", "استخراج عناوين البريد من النص.", "email", AdvancedCommands.ExtractEmails);
            A("extract-urls", "النص والتحرير", "استخراج الروابط", "استخراج الروابط من النص.", "url", AdvancedCommands.ExtractUrls);
            A("extract-phones", "النص والتحرير", "استخراج أرقام الهواتف", "استخراج أرقام الهواتف المحتملة.", "phone", AdvancedCommands.ExtractPhones);
            A("text-statistics", "النص والتحرير", "إحصاءات النص", "عرض الكلمات والأحرف والفقرات والجمل.", "عدد احصاء", AdvancedCommands.TextStatistics);
            A("paste-plain", "النص والتحرير", "لصق كنص فقط", "لصق محتوى الحافظة دون تنسيق.", "لصق", AdvancedCommands.PastePlainText);

            // التنسيق والأنماط
            A("font-arial-14", "التنسيق والأنماط", "Arial 14", "تطبيق Arial 14 عربي وإنجليزي.", "خط", Commands.Arial14);
            A("font-tahoma-12", "التنسيق والأنماط", "Tahoma 12", "تطبيق Tahoma 12.", "خط", () => AdvancedCommands.FontPreset("Tahoma", 12));
            A("font-times-12", "التنسيق والأنماط", "Times New Roman 12", "تطبيق Times New Roman 12.", "خط", () => AdvancedCommands.FontPreset("Times New Roman", 12));
            A("font-calibri-11", "التنسيق والأنماط", "Calibri 11", "تطبيق Calibri 11.", "خط", () => AdvancedCommands.FontPreset("Calibri", 11));
            A("clear-formatting", "التنسيق والأنماط", "إزالة التنسيق المباشر", "إرجاع النص إلى تنسيق النمط.", "تنسيق", Commands.ClearFormatting);
            A("align-right", "التنسيق والأنماط", "محاذاة يمين", "محاذاة الفقرات إلى اليمين.", "محاذاة", () => Commands.Align(2));
            A("align-left", "التنسيق والأنماط", "محاذاة يسار", "محاذاة الفقرات إلى اليسار.", "محاذاة", () => Commands.Align(0));
            A("align-center", "التنسيق والأنماط", "توسيط", "توسيط الفقرات.", "محاذاة", () => Commands.Align(1));
            A("align-justify", "التنسيق والأنماط", "ضبط كامل", "ضبط الفقرات من الطرفين.", "محاذاة", () => Commands.Align(3));
            A("line-spacing-1", "التنسيق والأنماط", "تباعد مفرد", "تباعد أسطر مفرد.", "تباعد", () => AdvancedCommands.LineSpacingRule(0));
            A("line-spacing-115", "التنسيق والأنماط", "تباعد 1.15", "تباعد أسطر 1.15.", "تباعد", () => Commands.LineSpacing(13.8f));
            A("line-spacing-15", "التنسيق والأنماط", "تباعد 1.5", "تباعد أسطر 1.5.", "تباعد", () => Commands.LineSpacing(18f));
            A("line-spacing-2", "التنسيق والأنماط", "تباعد مزدوج", "تباعد أسطر مزدوج.", "تباعد", () => AdvancedCommands.LineSpacingRule(2));
            A("space-after-0", "التنسيق والأنماط", "مسافة بعد 0", "إلغاء المسافة بعد الفقرة.", "فقرة", () => AdvancedCommands.SpaceAfter(0));
            A("space-after-6", "التنسيق والأنماط", "مسافة بعد 6", "تعيين 6 نقاط بعد الفقرة.", "فقرة", () => AdvancedCommands.SpaceAfter(6));
            A("space-after-12", "التنسيق والأنماط", "مسافة بعد 12", "تعيين 12 نقطة بعد الفقرة.", "فقرة", () => AdvancedCommands.SpaceAfter(12));
            A("indent-first-line", "التنسيق والأنماط", "مسافة بادئة للسطر الأول", "تطبيق مسافة بادئة 0.75 سم.", "indent", () => AdvancedCommands.FirstLineIndent(21.26f));
            A("indent-remove", "التنسيق والأنماط", "إزالة المسافات البادئة", "إزالة إزاحات الفقرة.", "indent", AdvancedCommands.RemoveIndents);
            A("keep-with-next", "التنسيق والأنماط", "إبقاء مع التالي", "منع فصل العنوان عن الفقرة التالية.", "صفحة", AdvancedCommands.KeepWithNext);
            A("page-break-before", "التنسيق والأنماط", "بدء الفقرة في صفحة جديدة", "تطبيق Page break before.", "صفحة", AdvancedCommands.PageBreakBefore);
            A("highlight-yellow", "التنسيق والأنماط", "تمييز أصفر", "تمييز النص باللون الأصفر.", "لون", AdvancedCommands.HighlightYellow);
            A("highlight-clear", "التنسيق والأنماط", "إزالة التمييز", "إزالة لون التمييز.", "لون", AdvancedCommands.ClearHighlight);
            A("border-box", "التنسيق والأنماط", "إطار حول النص", "إضافة حدود حول الفقرات المحددة.", "حدود", AdvancedCommands.BorderBox);
            A("border-clear", "التنسيق والأنماط", "إزالة الحدود", "إزالة حدود الفقرات.", "حدود", AdvancedCommands.ClearBorders);
            A("heading-1", "التنسيق والأنماط", "عنوان 1", "تطبيق Heading 1.", "نمط", () => Commands.Style("Heading 1"));
            A("heading-2", "التنسيق والأنماط", "عنوان 2", "تطبيق Heading 2.", "نمط", () => Commands.Style("Heading 2"));
            A("heading-3", "التنسيق والأنماط", "عنوان 3", "تطبيق Heading 3.", "نمط", () => Commands.Style("Heading 3"));
            A("style-normal", "التنسيق والأنماط", "نمط عادي", "تطبيق Normal.", "نمط", () => Commands.Style("Normal"));
            A("format-arabic-report", "التنسيق والأنماط", "تقرير عربي", "تنسيق تقرير عربي احترافي.", "تقرير", Commands.ArabicReport);
            A("format-official-letter", "التنسيق والأنماط", "خطاب رسمي", "ضبط الصفحة والخط والفقرات لخطاب عربي رسمي.", "خطاب", AdvancedCommands.OfficialLetterFormat);
            A("format-academic", "التنسيق والأنماط", "تنسيق أكاديمي", "ضبط الصفحة والخطوط والتباعد لبحث أكاديمي.", "بحث", AdvancedCommands.AcademicFormat);
            A("format-copy", "التنسيق والأنماط", "حفظ لقطة تنسيق", "حفظ تنسيق النص الحالي داخل الجلسة.", "نسخ تنسيق", AdvancedCommands.CopyFormatSnapshot);
            A("format-apply", "التنسيق والأنماط", "تطبيق لقطة التنسيق", "تطبيق آخر تنسيق محفوظ.", "لصق تنسيق", AdvancedCommands.ApplyFormatSnapshot);

            // الجداول
            A("table-rtl", "الجداول", "جدول RTL", "اتجاه الجدول من اليمين إلى اليسار.", "جدول", Commands.TableRtl);
            A("table-ltr", "الجداول", "جدول LTR", "اتجاه الجدول من اليسار إلى اليمين.", "جدول", AdvancedCommands.TableLtr);
            A("table-professional", "الجداول", "تنسيق جدول احترافي", "حدود وعنوان وتوسيط وملاءمة الصفحة.", "فخم جدول", AdvancedCommands.TableProfessional);
            A("table-header-style", "الجداول", "تنسيق صف العنوان", "تظليل وتغليظ وتكرار الصف الأول.", "عنوان", AdvancedCommands.TableHeaderStyle);
            A("table-banded-rows", "الجداول", "صفوف متبادلة", "تظليل صفوف الجدول بالتناوب.", "تظليل", AdvancedCommands.TableBandedRows);
            A("table-autofit-window", "الجداول", "ملاءمة الصفحة", "ملاءمة عرض الجدول للصفحة.", "عرض", () => Commands.TableAutoFit(2));
            A("table-autofit-content", "الجداول", "ملاءمة المحتوى", "ملاءمة عرض الأعمدة للمحتوى.", "عرض", () => Commands.TableAutoFit(1));
            A("table-distribute-columns", "الجداول", "توزيع الأعمدة", "توزيع الأعمدة بالتساوي.", "أعمدة", Commands.TableDistributeColumns);
            A("table-distribute-rows", "الجداول", "توزيع الصفوف", "توزيع الصفوف بالتساوي.", "صفوف", Commands.TableDistributeRows);
            A("table-repeat-header", "الجداول", "تكرار صف العنوان", "تكرار الصف الأول في كل صفحة.", "عنوان", Commands.TableRepeatHeader);
            A("table-remove-empty-rows", "الجداول", "حذف الصفوف الفارغة", "حذف الصفوف التي لا تحتوي بيانات.", "صفوف", Commands.TableRemoveEmptyRows);
            A("table-remove-duplicate-rows", "الجداول", "حذف الصفوف المكررة", "حذف الصفوف المتطابقة.", "مكرر", AdvancedCommands.TableRemoveDuplicateRows);
            A("table-trim-cells", "الجداول", "تنظيف الخلايا", "حذف المسافات الزائدة داخل الخلايا.", "تنظيف", AdvancedCommands.TableTrimCells);
            A("table-center-cells", "الجداول", "توسيط الخلايا", "توسيط أفقي وعمودي.", "توسيط", Commands.TableCenterCells);
            A("table-number-first-column", "الجداول", "ترقيم العمود الأول", "إضافة تسلسل رقمي للصفوف.", "ترقيم", AdvancedCommands.TableNumberFirstColumn);
            A("table-sum-column", "الجداول", "مجموع العمود", "إضافة صف مجموع للعمود الحالي.", "حساب", () => AdvancedCommands.TableColumnAggregate(false));
            A("table-average-column", "الجداول", "متوسط العمود", "إضافة صف متوسط للعمود الحالي.", "حساب", () => AdvancedCommands.TableColumnAggregate(true));
            A("table-sort-asc", "الجداول", "فرز الجدول تصاعديًا", "فرز الصفوف حسب العمود الحالي.", "ترتيب", () => AdvancedCommands.TableSort(false));
            A("table-sort-desc", "الجداول", "فرز الجدول تنازليًا", "فرز الصفوف تنازليًا حسب العمود الحالي.", "ترتيب", () => AdvancedCommands.TableSort(true));
            A("table-row-above", "الجداول", "صف أعلى", "إضافة صف فوق الخلية الحالية.", "إضافة صف", AdvancedCommands.TableAddRowAbove);
            A("table-row-below", "الجداول", "صف أسفل", "إضافة صف في نهاية الجدول.", "إضافة صف", AdvancedCommands.TableAddRowBelow);
            A("table-col-left", "الجداول", "عمود يسار", "إضافة عمود قبل العمود الحالي.", "إضافة عمود", AdvancedCommands.TableAddColumnLeft);
            A("table-col-right", "الجداول", "عمود يمين", "إضافة عمود في نهاية الجدول.", "إضافة عمود", AdvancedCommands.TableAddColumnRight);
            A("table-delete-row", "الجداول", "حذف الصف", "حذف الصف الحالي.", "حذف", AdvancedCommands.TableDeleteRow);
            A("table-delete-column", "الجداول", "حذف العمود", "حذف العمود الحالي.", "حذف", AdvancedCommands.TableDeleteColumn);
            A("table-merge-cells", "الجداول", "دمج الخلايا", "دمج الخلايا المحددة.", "دمج", AdvancedCommands.TableMergeCells);
            A("table-split-cell", "الجداول", "تقسيم الخلية", "تقسيم الخلية إلى صفوف وأعمدة.", "تقسيم", AdvancedCommands.TableSplitCell);
            A("table-borders-all", "الجداول", "كل الحدود", "إظهار جميع حدود الجدول.", "حدود", () => AdvancedCommands.TableBorders(true));
            A("table-borders-none", "الجداول", "بدون حدود", "إخفاء حدود الجدول.", "حدود", () => AdvancedCommands.TableBorders(false));
            A("table-vertical-top", "الجداول", "محاذاة عمودية أعلى", "محاذاة محتوى الخلايا للأعلى.", "عمودي", () => AdvancedCommands.TableVertical(0));
            A("table-vertical-middle", "الجداول", "محاذاة عمودية وسط", "محاذاة محتوى الخلايا للوسط.", "عمودي", () => AdvancedCommands.TableVertical(1));
            A("table-vertical-bottom", "الجداول", "محاذاة عمودية أسفل", "محاذاة محتوى الخلايا للأسفل.", "عمودي", () => AdvancedCommands.TableVertical(3));
            A("text-to-table", "الجداول", "تحويل نص إلى جدول", "تحويل نص مفصول بعلامات إلى جدول.", "تحويل", AdvancedCommands.TextToTable);
            A("table-to-text", "الجداول", "تحويل جدول إلى نص", "تحويل الجدول إلى نص مفصول.", "تحويل", Commands.TableToText);
            A("table-transpose", "الجداول", "تبديل الصفوف والأعمدة", "Transpose للجدول الحالي.", "تبديل", AdvancedCommands.TableTranspose);

            // المستند والصفحات
            A("update-fields", "المستند والصفحات", "تحديث جميع الحقول", "تحديث الحقول والفهارس.", "حقول", Commands.UpdateFields);
            A("insert-toc", "المستند والصفحات", "إضافة فهرس محتويات", "إدراج فهرس من العناوين 1 إلى 3.", "فهرس", Commands.InsertToc);
            A("page-numbers-add", "المستند والصفحات", "إضافة أرقام الصفحات", "إضافة الترقيم إلى التذييل.", "ترقيم صفحة", Commands.AddPageNumbers);
            A("page-numbers-remove", "المستند والصفحات", "حذف أرقام الصفحات", "حذف حقول PAGE من التذييلات.", "ترقيم صفحة", Commands.RemovePageNumbers);
            A("insert-page-break", "المستند والصفحات", "فاصل صفحة", "إدراج فاصل صفحة يدوي.", "فاصل", AdvancedCommands.InsertPageBreak);
            A("insert-section-next", "المستند والصفحات", "مقطع صفحة جديدة", "إدراج Section Break Next Page.", "مقطع", AdvancedCommands.InsertSectionNext);
            A("insert-section-continuous", "المستند والصفحات", "مقطع مستمر", "إدراج Section Break Continuous.", "مقطع", AdvancedCommands.InsertSectionContinuous);
            A("remove-page-breaks", "المستند والصفحات", "حذف فواصل الصفحات", "حذف جميع فواصل الصفحات اليدوية.", "فاصل", AdvancedCommands.RemovePageBreaks);
            A("remove-section-breaks", "المستند والصفحات", "حذف فواصل المقاطع", "حذف جميع فواصل المقاطع.", "مقطع", AdvancedCommands.RemoveSectionBreaks);
            A("page-a4", "المستند والصفحات", "حجم A4", "ضبط الورق على A4.", "ورق", Commands.SetA4);
            A("page-a3", "المستند والصفحات", "حجم A3", "ضبط الورق على A3.", "ورق", () => AdvancedCommands.SetPaper(6));
            A("page-letter", "المستند والصفحات", "حجم Letter", "ضبط الورق على Letter.", "ورق", () => AdvancedCommands.SetPaper(2));
            A("page-portrait", "المستند والصفحات", "اتجاه عمودي", "جعل الصفحة عمودية.", "اتجاه", () => Commands.Orientation(0));
            A("page-landscape", "المستند والصفحات", "اتجاه أفقي", "جعل الصفحة أفقية.", "اتجاه", () => Commands.Orientation(1));
            A("margins-normal", "المستند والصفحات", "هوامش عادية", "ضبط الهوامش إلى 2.54 سم.", "هوامش", () => Commands.Margins(72f));
            A("margins-narrow", "المستند والصفحات", "هوامش ضيقة", "ضبط الهوامش إلى 1.27 سم.", "هوامش", () => Commands.Margins(36f));
            A("columns-1", "المستند والصفحات", "عمود واحد", "إرجاع النص إلى عمود واحد.", "أعمدة", () => AdvancedCommands.Columns(1));
            A("columns-2", "المستند والصفحات", "عمودان", "تقسيم الصفحة إلى عمودين.", "أعمدة", () => AdvancedCommands.Columns(2));
            A("columns-3", "المستند والصفحات", "ثلاثة أعمدة", "تقسيم الصفحة إلى ثلاثة أعمدة.", "أعمدة", () => AdvancedCommands.Columns(3));
            A("line-numbers-on", "المستند والصفحات", "تشغيل أرقام الأسطر", "إظهار ترقيم الأسطر.", "أسطر", () => AdvancedCommands.LineNumbers(true));
            A("line-numbers-off", "المستند والصفحات", "إيقاف أرقام الأسطر", "إخفاء ترقيم الأسطر.", "أسطر", () => AdvancedCommands.LineNumbers(false));
            A("header-text", "المستند والصفحات", "نص رأس الصفحة", "كتابة رأس موحد في جميع المقاطع.", "هيدر", AdvancedCommands.SetHeaderText);
            A("footer-text", "المستند والصفحات", "نص تذييل الصفحة", "كتابة تذييل موحد في جميع المقاطع.", "فوتر", AdvancedCommands.SetFooterText);
            A("clear-headers-footers", "المستند والصفحات", "مسح الرؤوس والتذييلات", "حذف نصوص الرؤوس والتذييلات.", "هيدر فوتر", AdvancedCommands.ClearHeadersFooters);
            A("insert-date-time", "المستند والصفحات", "إدراج تاريخ ووقت", "إدراج التاريخ والوقت الحاليين.", "تاريخ", AdvancedCommands.InsertDateTime);
            A("insert-file-name", "المستند والصفحات", "إدراج اسم ومسار الملف", "إدراج حقل FILENAME مع المسار.", "مسار", AdvancedCommands.InsertFileNameField);
            A("document-statistics", "المستند والصفحات", "إحصاءات المستند", "عرض الصفحات والكلمات والجداول والصور.", "احصاء", AdvancedCommands.DocumentStatistics);
            A("list-fonts", "المستند والصفحات", "قائمة خطوط الأنماط", "عرض الخطوط المستخدمة في أنماط المستند.", "خطوط", AdvancedCommands.ListStyleFonts);
            A("replace-font", "المستند والصفحات", "استبدال خط", "استبدال خط بآخر في المستند.", "خط", AdvancedCommands.ReplaceFont);
            A("remove-hidden-text", "المستند والصفحات", "حذف النص المخفي", "حذف كل النصوص ذات خاصية Hidden.", "مخفي", AdvancedCommands.RemoveHiddenText);
            A("unlink-fields", "المستند والصفحات", "تحويل الحقول إلى نص", "إلغاء ارتباط الحقول مع إبقاء النتائج.", "حقول", AdvancedCommands.UnlinkFields);
            A("remove-bookmarks", "المستند والصفحات", "حذف الإشارات المرجعية", "حذف جميع Bookmarks.", "bookmark", AdvancedCommands.RemoveBookmarks);
            A("insert-cover", "المستند والصفحات", "إنشاء صفحة غلاف", "إضافة غلاف منسق في بداية المستند.", "غلاف", AdvancedCommands.InsertCoverPage);
            A("insert-signature", "المستند والصفحات", "كتلة توقيع", "إدراج اسم وصفة وتوقيع وتاريخ.", "توقيع", AdvancedCommands.InsertSignatureBlock);

            // المراجعة والأمان
            A("track-on", "المراجعة والأمان", "تشغيل تعقب التغييرات", "تشغيل Track Changes.", "تعقب", () => AdvancedCommands.TrackChanges(true));
            A("track-off", "المراجعة والأمان", "إيقاف تعقب التغييرات", "إيقاف Track Changes.", "تعقب", () => AdvancedCommands.TrackChanges(false));
            A("remove-comments", "المراجعة والأمان", "حذف التعليقات", "حذف جميع التعليقات.", "تعليقات", Commands.RemoveComments);
            A("accept-revisions", "المراجعة والأمان", "قبول كل التعديلات", "قبول جميع التغييرات المتعقبة.", "تعديلات", Commands.AcceptRevisions);
            A("reject-revisions", "المراجعة والأمان", "رفض كل التعديلات", "رفض جميع التغييرات المتعقبة.", "تعديلات", Commands.RejectRevisions);
            A("protect-readonly", "المراجعة والأمان", "حماية للقراءة فقط", "حماية المستند بكلمة مرور ضد التحرير.", "حماية", () => AdvancedCommands.ProtectDocument(3));
            A("protect-forms", "المراجعة والأمان", "حماية تعبئة النماذج", "السماح بتعبئة النماذج فقط.", "حماية", () => AdvancedCommands.ProtectDocument(2));
            A("unprotect", "المراجعة والأمان", "إلغاء الحماية", "إلغاء حماية المستند بكلمة المرور.", "حماية", AdvancedCommands.UnprotectDocument);
            A("remove-metadata", "المراجعة والأمان", "إزالة البيانات الشخصية", "حذف معلومات المستند الشخصية.", "خصوصية", Commands.RemoveMetadata);
            A("clean-properties", "المراجعة والأمان", "تنظيف الخصائص المخصصة", "حذف البيانات الشخصية والخصائص المخصصة.", "خصوصية", AdvancedCommands.CleanProperties);
            A("remove-external-links", "المراجعة والأمان", "فصل الروابط الخارجية", "فصل روابط البيانات الخارجية.", "روابط", AdvancedCommands.RemoveExternalLinks);
            A("workflow-final-delivery", "المراجعة والأمان", "تجهيز نسخة نهائية", "نسخة احتياطية ثم قبول التعديلات وحذف التعليقات وتحديث الحقول.", "نهائي", Commands.FinalDelivery);
            A("workflow-final-share", "المراجعة والأمان", "تنظيف شامل قبل المشاركة", "نسخة احتياطية وتنظيف مراجعة وخصوصية ونص مخفي.", "مشاركة", AdvancedCommands.FinalShareWorkflow);

            // الملفات وPDF
            A("save-backup", "الملفات وPDF", "نسخة احتياطية مؤرخة", "حفظ نسخة احتياطية دون تغيير المستند المفتوح.", "حفظ", Commands.SaveBackup);
            A("save-version", "الملفات وPDF", "حفظ إصدار جديد", "حفظ نسخة باسم يحتوي التاريخ والوقت.", "نسخة", AdvancedCommands.SaveVersionedCopy);
            A("save-docx-copy", "الملفات وPDF", "نسخة DOCX", "إنشاء نسخة DOCX مستقلة.", "docx", AdvancedCommands.SaveDocxCopy);
            A("save-all", "الملفات وPDF", "حفظ كل المستندات", "حفظ جميع مستندات Word المفتوحة.", "حفظ", AdvancedCommands.SaveAllDocuments);
            A("export-pdf", "الملفات وPDF", "تصدير PDF", "تصدير المستند إلى PDF.", "pdf", () => Commands.ExportPdf(false));
            A("export-pdfa", "الملفات وPDF", "تصدير PDF/A", "تصدير أرشيفي PDF/A.", "pdfa", () => Commands.ExportPdf(true));
            A("export-selection-pdf", "الملفات وPDF", "تصدير التحديد PDF", "تصدير الجزء المحدد فقط إلى PDF.", "pdf تحديد", AdvancedCommands.ExportSelectionPdf);
            A("export-sections-pdf", "الملفات وPDF", "كل مقطع PDF مستقل", "تصدير كل Section إلى ملف PDF منفصل.", "pdf مقاطع", AdvancedCommands.ExportSectionsPdf);
            A("batch-pdf", "الملفات وPDF", "تحويل مجلد إلى PDF", "تحويل ملفات Word داخل مجلد إلى PDF دفعة واحدة.", "دفعي", AdvancedCommands.BatchPdf);
            A("merge-documents", "الملفات وPDF", "دمج مستندات", "إدراج عدة مستندات في المستند الحالي.", "دمج ملفات", AdvancedCommands.MergeDocuments);
            A("open-document-folder", "الملفات وPDF", "فتح موقع المستند", "فتح Explorer وتحديد الملف الحالي.", "مجلد", AdvancedCommands.OpenDocumentFolder);
            A("copy-document-path", "الملفات وPDF", "نسخ مسار المستند", "نسخ المسار الكامل إلى الحافظة.", "مسار", AdvancedCommands.CopyDocumentPath);
            A("archive-package", "الملفات وPDF", "حزمة أرشيف", "إنشاء مجلد يحتوي نسخة Word وPDF ومعلومات الحزمة.", "أرشيف", AdvancedCommands.ArchivePackage);

            // القوالب
            A("template-agenda", "القوالب", "جدول أعمال", "إدراج قالب جدول أعمال اجتماع.", "اجتماع", () => Commands.InsertTemplate("agenda"));
            A("template-minutes", "القوالب", "محضر اجتماع", "إدراج قالب محضر اجتماع.", "اجتماع", () => Commands.InsertTemplate("minutes"));
            A("template-official-letter", "القوالب", "خطاب رسمي", "إدراج قالب خطاب رسمي.", "خطاب", () => Commands.InsertTemplate("letter"));
            A("template-medical-report", "القوالب", "تقرير طبي", "إدراج قالب تقرير طبي.", "طبي", () => Commands.InsertTemplate("medical"));
            A("template-project-report", "القوالب", "تقرير مشروع", "إدراج قالب تقرير مشروع.", "مشروع", () => Commands.InsertTemplate("project"));
            A("template-handover", "القوالب", "تسليم واستلام", "إدراج محضر تسليم واستلام.", "تسليم", () => Commands.InsertTemplate("handover"));
            A("template-memo", "القوالب", "مذكرة داخلية", "إدراج قالب مذكرة.", "مذكرة", () => AdvancedCommands.InsertMoreTemplate("memo"));
            A("template-quotation", "القوالب", "عرض سعر", "إدراج قالب عرض سعر.", "سعر", () => AdvancedCommands.InsertMoreTemplate("quotation"));
            A("template-invoice", "القوالب", "فاتورة", "إدراج قالب فاتورة.", "فاتورة", () => AdvancedCommands.InsertMoreTemplate("invoice"));
            A("template-certificate", "القوالب", "شهادة تقدير", "إدراج قالب شهادة.", "شهادة", () => AdvancedCommands.InsertMoreTemplate("certificate"));
            A("template-attendance", "القوالب", "كشف حضور", "إدراج كشف حضور.", "حضور", () => AdvancedCommands.InsertMoreTemplate("attendance"));
            A("template-tor", "القوالب", "شروط مرجعية ToR", "إدراج هيكل شروط مرجعية.", "tor", () => AdvancedCommands.InsertMoreTemplate("tor"));
            A("template-concept-note", "القوالب", "مذكرة مفاهيمية", "إدراج هيكل Concept Note.", "مشروع", () => AdvancedCommands.InsertMoreTemplate("concept"));
            A("template-donor-report", "القوالب", "تقرير جهة مانحة", "إدراج هيكل تقرير مانحين.", "مانح", () => AdvancedCommands.InsertMoreTemplate("donor"));
            A("template-sop", "القوالب", "إجراء تشغيلي SOP", "إدراج هيكل SOP.", "اجراء", () => AdvancedCommands.InsertMoreTemplate("sop"));
            A("template-risk-register", "القوالب", "سجل مخاطر", "إدراج جدول سجل مخاطر.", "خطر", () => AdvancedCommands.InsertMoreTemplate("risk"));
            A("template-action-plan", "القوالب", "خطة عمل", "إدراج جدول خطة عمل.", "خطة", () => AdvancedCommands.InsertMoreTemplate("action"));
            A("template-cover-letter", "القوالب", "خطاب تغطية", "إدراج قالب Cover Letter عربي.", "خطاب", () => AdvancedCommands.InsertMoreTemplate("cover-letter"));

            // الأدوات الذكية
            A("calculator", "الأدوات الذكية", "حاسبة التحديد", "حساب تعبير رياضي محدد وإدراج النتيجة.", "حساب", AdvancedCommands.Calculator);
            A("insert-guid", "الأدوات الذكية", "إدراج UUID", "إدراج معرف فريد عالمي.", "guid uuid", AdvancedCommands.InsertGuid);
            A("insert-lorem-arabic", "الأدوات الذكية", "نص عربي تجريبي", "إدراج فقرات عربية لتجربة التصميم.", "lorem", AdvancedCommands.InsertArabicLorem);
            A("insert-checklist", "الأدوات الذكية", "قائمة مهام بمربعات", "تحويل بنود إلى قائمة ☐.", "checklist", AdvancedCommands.InsertChecklist);
            A("open-log", "الأدوات الذكية", "فتح سجل الإضافة", "فتح أحدث ملف Log.", "سجل", AdvancedCommands.OpenLog, false);


            // الحزمة الاحترافية — تنظيف النص والتحرير المتقدم
            A("normalize-unicode-spaces", "النص الاحترافي", "توحيد المسافات Unicode", "تحويل المسافات غير القياسية إلى مسافات عادية.", "unicode nbsp مسافات", EnterpriseCommands.NormalizeUnicodeSpaces);
            A("remove-leading-spaces", "النص الاحترافي", "حذف مسافات بداية الأسطر", "تنظيف بداية كل سطر.", "بداية trim", EnterpriseCommands.RemoveLeadingSpaces);
            A("remove-trailing-spaces", "النص الاحترافي", "حذف مسافات نهاية الأسطر", "تنظيف نهاية كل سطر.", "نهاية trim", EnterpriseCommands.RemoveTrailingSpaces);
            A("remove-all-blank-paragraphs", "النص الاحترافي", "حذف كل الفقرات الفارغة", "إزالة جميع الفقرات الخالية من التحديد.", "فراغ فقرات", EnterpriseCommands.RemoveAllBlankParagraphs);
            A("sentences-to-paragraphs", "النص الاحترافي", "الجمل إلى فقرات", "وضع كل جملة في فقرة مستقلة.", "جمل تقسيم", EnterpriseCommands.SentencesToParagraphs);
            A("paragraphs-semicolon-list", "النص الاحترافي", "الفقرات إلى قائمة منقوطة", "دمج الفقرات بفواصل منقوطة عربية.", "قائمة منقوطة", EnterpriseCommands.ParagraphsToSemicolonList);
            A("wrap-arabic-quotes", "النص الاحترافي", "إحاطة باقتباس عربي", "إحاطة التحديد بعلامتي « ».", "اقتباس", EnterpriseCommands.WrapArabicQuotes);
            A("wrap-parentheses", "النص الاحترافي", "إحاطة بأقواس", "إحاطة التحديد بقوسين.", "أقواس", EnterpriseCommands.WrapParentheses);
            A("wrap-brackets", "النص الاحترافي", "إحاطة بأقواس مربعة", "إحاطة التحديد بـ [ ].", "brackets", EnterpriseCommands.WrapBrackets);
            A("strip-html-tags", "النص الاحترافي", "حذف وسوم HTML", "إزالة وسوم HTML وفك الكيانات النصية.", "html تنظيف", EnterpriseCommands.StripHtmlTags);
            A("extract-numbers", "النص الاحترافي", "استخراج الأرقام", "إنشاء مستند جديد بالأرقام الموجودة.", "أرقام استخراج", EnterpriseCommands.ExtractNumbers);
            A("extract-hashtags", "النص الاحترافي", "استخراج الوسوم", "استخراج Hashtags إلى مستند جديد.", "هاشتاق hashtag", EnterpriseCommands.ExtractHashtags);
            A("extract-mentions", "النص الاحترافي", "استخراج الإشارات", "استخراج @mentions إلى مستند جديد.", "mentions", EnterpriseCommands.ExtractMentions);
            A("word-frequency-top", "التحليل والإحصاء", "أكثر الكلمات تكرارًا", "إنشاء تقرير بأعلى الكلمات تكرارًا.", "frequency تكرار", EnterpriseCommands.WordFrequencyTop);
            A("find-replace-prompt", "النص الاحترافي", "بحث واستبدال سريع", "بحث واستبدال داخل التحديد أو المستند.", "بحث استبدال", EnterpriseCommands.FindReplacePrompt);
            A("highlight-term", "النص الاحترافي", "تمييز كلمة أو عبارة", "تمييز جميع مرات ظهور النص.", "highlight تمييز", EnterpriseCommands.HighlightTerm);
            A("clear-highlight-pro", "النص الاحترافي", "إزالة التمييز", "إزالة لون التمييز من التحديد.", "highlight", EnterpriseCommands.ClearHighlight);
            A("duplicate-selection", "النص الاحترافي", "تكرار التحديد", "نسخ النص المحدد مباشرة بعده.", "duplicate", EnterpriseCommands.DuplicateSelection);
            A("sort-paragraphs-length", "النص الاحترافي", "فرز الفقرات حسب الطول", "ترتيب الفقرات من الأقصر إلى الأطول.", "فرز طول", EnterpriseCommands.SortParagraphsByLength);
            A("remove-short-paragraphs", "النص الاحترافي", "حذف الفقرات القصيرة", "حذف الفقرات الأقل من عدد أحرف تختاره.", "قصيرة", EnterpriseCommands.RemoveShortParagraphs);
            A("unique-words-only", "النص الاحترافي", "الكلمات الفريدة فقط", "الاحتفاظ بأول ظهور لكل كلمة.", "unique كلمات", EnterpriseCommands.UniqueWordsOnly);
            A("comma-list-bullets", "النص الاحترافي", "قائمة مفصولة إلى نقاط", "تحويل العناصر المفصولة بفواصل إلى قائمة نقطية.", "فواصل نقاط", EnterpriseCommands.CommaListToBullets);

            // إدراجات وحقول احترافية
            A("insert-current-time", "الإدراج والحقول", "إدراج الوقت الحالي", "إدراج الوقت بصيغة HH:mm.", "وقت", EnterpriseCommands.InsertCurrentTime);
            A("insert-iso-date", "الإدراج والحقول", "إدراج تاريخ ISO", "إدراج التاريخ بصيغة yyyy-MM-dd.", "iso تاريخ", EnterpriseCommands.InsertIsoDate);
            A("insert-timestamp", "الإدراج والحقول", "إدراج طابع زمني", "إدراج التاريخ والوقت الكامل.", "timestamp", EnterpriseCommands.InsertTimestamp);
            A("insert-page-x-of-y", "الإدراج والحقول", "صفحة X من Y", "إضافة ترقيم صفحة من إجمالي الصفحات في التذييل.", "ترقيم صفحات", EnterpriseCommands.InsertPageXOfY);
            A("insert-checkbox-symbols", "الإدراج والحقول", "مربعات اختيار جاهزة", "إدراج عدد من مربعات الاختيار.", "checkbox", EnterpriseCommands.InsertCheckboxSymbols);
            A("insert-document-info", "الإدراج والحقول", "كتلة معلومات المستند", "إدراج العنوان والمؤلف واسم الملف والتاريخ.", "خصائص معلومات", EnterpriseCommands.InsertDocumentInfoBlock);

            // أطر العمل والجداول المؤسسية
            A("insert-decision-log", "أطر العمل المؤسسية", "سجل القرارات", "إدراج جدول احترافي لتوثيق القرارات.", "قرار log", EnterpriseCommands.InsertDecisionLog);
            A("insert-raci-matrix", "أطر العمل المؤسسية", "مصفوفة RACI", "إدراج جدول توزيع المسؤوليات RACI.", "raci مسؤوليات", EnterpriseCommands.InsertRaciMatrix);
            A("insert-swot-matrix", "أطر العمل المؤسسية", "تحليل SWOT", "إدراج جدول نقاط القوة والضعف والفرص والتهديدات.", "swot", EnterpriseCommands.InsertSwotMatrix);
            A("insert-kpi-table", "أطر العمل المؤسسية", "جدول مؤشرات KPI", "إدراج إطار مؤشرات أداء ونتائج.", "kpi مؤشرات", EnterpriseCommands.InsertKpiTable);
            A("insert-budget-table", "أطر العمل المؤسسية", "جدول ميزانية", "إدراج جدول ميزانية تفصيلي.", "budget ميزانية", EnterpriseCommands.InsertBudgetTable);
            A("insert-timeline-table", "أطر العمل المؤسسية", "جدول زمني", "إدراج جدول أنشطة ومواعيد ومسؤوليات.", "timeline", EnterpriseCommands.InsertTimelineTable);
            A("insert-yes-no-table", "أطر العمل المؤسسية", "قائمة تحقق نعم/لا", "إدراج جدول Checklist مؤسسي.", "checklist تحقق", EnterpriseCommands.InsertYesNoTable);

            // الجداول المتقدمة
            A("table-currency-format", "الجداول المتقدمة", "تنسيق خلايا كعملة", "تنسيق القيم الرقمية برمز عملة تختاره.", "عملة currency", EnterpriseCommands.TableCurrencyFormat);
            A("table-percentage-format", "الجداول المتقدمة", "تنسيق خلايا كنسبة", "تنسيق الأرقام كنسب مئوية.", "percentage نسبة", EnterpriseCommands.TablePercentageFormat);
            A("table-digits-eastern-pro", "الجداول المتقدمة", "أرقام الجدول عربية", "تحويل أرقام جميع خلايا الجدول إلى ٠١٢٣.", "جدول أرقام", EnterpriseCommands.TableDigitsEastern);
            A("table-digits-western-pro", "الجداول المتقدمة", "أرقام الجدول غربية", "تحويل أرقام جميع خلايا الجدول إلى 0123.", "جدول أرقام", EnterpriseCommands.TableDigitsWestern);
            A("table-remove-cell-breaks", "الجداول المتقدمة", "حذف فواصل الخلايا", "إزالة فواصل الأسطر من داخل الخلايا.", "تنظيف خلايا", EnterpriseCommands.TableRemoveCellBreaks);
            A("table-bold-first-row", "الجداول المتقدمة", "صف أول بارز", "تغليظ الصف الأول وتكراره كرأس.", "header عنوان", EnterpriseCommands.TableBoldFirstRow);
            A("table-auto-row-height", "الجداول المتقدمة", "ارتفاع صفوف تلقائي", "إعادة ارتفاع الصفوف إلى الوضع التلقائي.", "ارتفاع", EnterpriseCommands.TableAutoRowHeight);
            A("table-column-width-prompt", "الجداول المتقدمة", "عرض أعمدة مخصص", "ضبط عرض الأعمدة بالسنتيمتر.", "عرض عمود", EnterpriseCommands.TableColumnWidthPrompt);

            // التحليل والتصدير
            A("document-dashboard", "التحليل والإحصاء", "لوحة المستند", "عرض الصفحات والكلمات والجداول والصور والروابط والمراجعات.", "dashboard احصاء", EnterpriseCommands.DocumentDashboard);
            A("list-document-headings", "التحليل والإحصاء", "قائمة العناوين", "إنشاء مستند جديد بجميع العناوين وأنماطها.", "headings عناوين", EnterpriseCommands.ListDocumentHeadings);
            A("list-bookmarks", "التحليل والإحصاء", "قائمة الإشارات المرجعية", "إنشاء قائمة بأسماء Bookmarks.", "bookmarks", EnterpriseCommands.ListBookmarks);
            A("export-plain-text", "الملفات وPDF", "تصدير المستند TXT", "تصدير محتوى المستند كنص UTF-8.", "txt text", EnterpriseCommands.ExportPlainText);
            A("export-selection-text", "الملفات وPDF", "تصدير التحديد TXT", "حفظ النص المحدد في ملف TXT.", "selection txt", EnterpriseCommands.ExportSelectionText);
            A("copy-as-markdown", "الملفات وPDF", "نسخ كـ Markdown", "نسخ التحديد كنص Markdown بسيط.", "markdown", EnterpriseCommands.CopyAsMarkdown);
            A("unlink-selection-fields", "المراجعة والأمان", "فصل حقول التحديد", "تحويل الحقول الموجودة في التحديد إلى نص ثابت.", "fields unlink", EnterpriseCommands.UnlinkSelectionFields);
            A("remove-footnotes", "المراجعة والأمان", "حذف الحواشي السفلية", "حذف جميع Footnotes من المستند.", "footnotes", EnterpriseCommands.RemoveFootnotes);
            A("remove-endnotes", "المراجعة والأمان", "حذف الحواشي الختامية", "حذف جميع Endnotes من المستند.", "endnotes", EnterpriseCommands.RemoveEndnotes);
            A("move-selection-new-document", "الملفات وPDF", "التحديد إلى مستند جديد", "نسخ التحديد مع تنسيقه إلى مستند مستقل.", "مستند جديد", EnterpriseCommands.MoveSelectionToNewDocument);



            // حزمة الاحتراف والجودة 2.2
            A("persian-to-arabic-letters", "العربية والاتجاه", "تصحيح الحروف الفارسية", "تحويل ي وك الفارسية إلى الحروف العربية القياسية.", "فارسي ياء كاف", ProfessionalCommands.PersianToArabicLetters);
            A("digits-persian", "العربية والاتجاه", "أرقام فارسية", "تحويل الأرقام إلى ۰۱۲۳.", "فارسي أرقام", ProfessionalCommands.ToPersianDigits);
            A("remove-repeated-punctuation", "الجودة والتحرير", "حذف الترقيم المكرر", "اختصار علامات الترقيم المتتابعة المكررة.", "ترقيم مكرر", ProfessionalCommands.RemoveRepeatedPunctuation);
            A("wrap-arabic-brackets", "العربية والاتجاه", "إحاطة بقوسين عربيين", "إحاطة النص المحدد بعلامتي ﴿ ﴾.", "اقواس", ProfessionalCommands.WrapArabicBrackets);
            A("reverse-paragraph-order", "الجودة والتحرير", "عكس ترتيب الفقرات", "عكس ترتيب الأسطر والفقرات المحددة.", "عكس فقرات", ProfessionalCommands.ReverseParagraphOrder);
            A("unique-sorted-lines", "الجودة والتحرير", "أسطر فريدة مرتبة", "حذف التكرار وفرز الأسطر تصاعديًا.", "فريد فرز", ProfessionalCommands.UniqueSortedLines);
            A("extract-numbers-pro", "الاستخراج والتحليل", "استخراج الأرقام", "استخراج القيم الرقمية الفريدة من النص.", "ارقام استخراج", ProfessionalCommands.ExtractNumbers);
            A("long-paragraph-report", "الاستخراج والتحليل", "تقرير الفقرات الطويلة", "إنشاء تقرير بالفقرات التي تتجاوز حد الكلمات.", "تقرير فقرات طويلة", ProfessionalCommands.LongParagraphReport);
            A("word-frequency-report", "الاستخراج والتحليل", "تحليل تكرار الكلمات", "إنشاء تقرير بأكثر 50 كلمة تكرارًا.", "تكرار كلمات تحليل", ProfessionalCommands.WordFrequencyReport);

            A("insert-current-datetime", "إدارة المستند", "إدراج التاريخ والوقت", "إدراج التاريخ والوقت الحاليين بصيغة موحدة.", "وقت تاريخ", ProfessionalCommands.InsertCurrentDateTime);
            A("insert-document-name", "إدارة المستند", "إدراج اسم المستند", "إدراج اسم الملف الحالي عند المؤشر.", "اسم ملف", ProfessionalCommands.InsertDocumentName);
            A("insert-document-path", "إدارة المستند", "إدراج مسار المستند", "إدراج المسار الكامل للملف الحالي.", "مسار ملف", ProfessionalCommands.InsertDocumentPath);
            A("insert-page-break-pro", "إدارة المستند", "إدراج فاصل صفحة", "إدراج Page Break عند المؤشر.", "فاصل صفحة", ProfessionalCommands.InsertPageBreak);
            A("insert-section-next-page", "إدارة المستند", "مقطع في الصفحة التالية", "إدراج Section Break من نوع Next Page.", "مقطع section", ProfessionalCommands.InsertSectionBreakNextPage);
            A("lock-all-fields", "الحقول والأتمتة", "قفل جميع الحقول", "منع تحديث جميع حقول المستند مؤقتًا.", "fields lock", ProfessionalCommands.LockAllFields);
            A("unlock-all-fields", "الحقول والأتمتة", "فتح جميع الحقول", "إتاحة تحديث جميع حقول المستند.", "fields unlock", ProfessionalCommands.UnlockAllFields);
            A("toggle-field-codes", "الحقول والأتمتة", "إظهار/إخفاء أكواد الحقول", "التبديل بين نتائج الحقول وأكوادها.", "field codes", ProfessionalCommands.ToggleFieldCodes);
            A("remove-all-bookmarks-pro", "الحقول والأتمتة", "حذف كل الإشارات المرجعية", "حذف جميع Bookmarks من المستند.", "bookmark حذف", ProfessionalCommands.RemoveAllBookmarks);

            A("proofing-language-arabic", "التنسيق والأنماط", "لغة التدقيق العربية", "تعيين لغة التدقيق للنص إلى العربية.", "لغة تدقيق", ProfessionalCommands.SetArabicProofingLanguage);
            A("proofing-language-english", "التنسيق والأنماط", "لغة التدقيق الإنجليزية", "تعيين لغة التدقيق للنص إلى الإنجليزية.", "لغة تدقيق", ProfessionalCommands.SetEnglishProofingLanguage);
            A("keep-lines-together", "التنسيق والأنماط", "إبقاء أسطر الفقرة معًا", "منع تقسيم أسطر الفقرة بين الصفحات.", "فقرة صفحة", ProfessionalCommands.KeepLinesTogether);
            A("widow-control-on", "التنسيق والأنماط", "منع الأرامل واليتامى", "تفعيل Widow/Orphan Control للفقرات.", "widow orphan", ProfessionalCommands.WidowControlOn);
            A("hanging-indent", "التنسيق والأنماط", "مسافة بادئة معلّقة", "تطبيق Hanging Indent قياسي.", "indent hanging", ProfessionalCommands.HangingIndent);
            A("highlight-green", "التنسيق والأنماط", "تمييز أخضر", "تمييز النص باللون الأخضر.", "highlight", ProfessionalCommands.HighlightGreen);
            A("highlight-red", "التنسيق والأنماط", "تمييز أحمر", "تمييز النص باللون الأحمر.", "highlight", ProfessionalCommands.HighlightRed);
            A("font-color-automatic", "التنسيق والأنماط", "لون خط تلقائي", "إعادة لون الخط إلى اللون التلقائي.", "لون خط", ProfessionalCommands.FontColorAutomatic);

            A("insert-risk-register-pro", "أطر العمل المؤسسية", "سجل مخاطر احترافي", "إدراج سجل مخاطر متكامل بثمانية أعمدة.", "risk مخاطر", ProfessionalCommands.InsertRiskRegister);
            A("insert-action-tracker-pro", "أطر العمل المؤسسية", "متابعة الإجراءات", "إدراج جدول متابعة إجراءات ومسؤوليات ومواعيد.", "actions متابعة", ProfessionalCommands.InsertActionTracker);
            A("insert-issue-log-pro", "أطر العمل المؤسسية", "سجل المشكلات", "إدراج جدول لتتبع القضايا والمشكلات حتى الإغلاق.", "issue log", ProfessionalCommands.InsertIssueLog);
            A("insert-contact-directory-pro", "أطر العمل المؤسسية", "دليل جهات اتصال", "إدراج دليل جهات اتصال منظم.", "contacts دليل", ProfessionalCommands.InsertContactDirectory);
            A("insert-inventory-table-pro", "أطر العمل المؤسسية", "سجل مخزون", "إدراج جدول مخزون وكميات ومواقع وحالة.", "inventory مخزون", ProfessionalCommands.InsertInventoryTable);
            A("paragraphs-checkboxes", "الجودة والتحرير", "تحويل إلى قائمة تحقق", "إضافة مربع ☐ في بداية كل فقرة.", "checkbox تحقق", ProfessionalCommands.ParagraphsToCheckboxes);
            A("paragraphs-checked", "الجودة والتحرير", "تحويل إلى قائمة منجزة", "إضافة علامة ☑ في بداية كل فقرة.", "checked منجز", ProfessionalCommands.ParagraphsToCheckedBoxes);
            A("strip-list-markers", "الجودة والتحرير", "حذف بادئات القوائم", "إزالة الأرقام والنقاط والشرطات اليدوية من بدايات الفقرات.", "قائمة تنظيف", ProfessionalCommands.StripLeadingListMarkers);
            A("insert-confidential-banner", "إدارة المستند", "شريط سري", "إدراج شريط تصنيف سري أعلى المستند.", "confidential سري", ProfessionalCommands.InsertConfidentialBanner);
            A("insert-document-control", "أطر العمل المؤسسية", "بطاقة ضبط المستند", "إدراج بطاقة رقم الإصدار والمالك والمراجعة والاعتماد.", "document control", ProfessionalCommands.InsertDocumentControlBlock);
            A("insert-approval-table-pro", "أطر العمل المؤسسية", "جدول الاعتماد", "إدراج جدول إعداد ومراجعة واعتماد وتواريخ.", "approval اعتماد", ProfessionalCommands.InsertApprovalTable);
            A("insert-distribution-list-pro", "أطر العمل المؤسسية", "قائمة التوزيع", "إدراج جدول الجهات المستلمة ووسيلة الإرسال والحالة.", "distribution توزيع", ProfessionalCommands.InsertDistributionList);
            A("insert-executive-summary", "القوالب والمحتوى", "هيكل ملخص تنفيذي", "إدراج هيكل احترافي للملخص التنفيذي والنتائج والتوصيات.", "executive summary", ProfessionalCommands.InsertExecutiveSummarySkeleton);
            A("comments-report-pro", "المراجعة والأمان", "تقرير التعليقات", "إنشاء مستند مستقل يحتوي جميع التعليقات وسياقها.", "comments تقرير", ProfessionalCommands.CommentsReport);
            A("revisions-summary-pro", "المراجعة والأمان", "ملخص التعديلات", "إنشاء ملخص عددي للتعديلات المتعقبة حسب النوع.", "revisions ملخص", ProfessionalCommands.RevisionsSummary);

            // النظام والترخيص
            A("command-center", "النظام والترخيص", "مركز الأدوات الاحترافي", "بحث وفئات ومفضلة وآخر استخدام لكل الأدوات.", "مركز", CommandCenterForm.ShowCenter, false);
            A("activate", "النظام والترخيص", "تفعيل البرنامج", "إدخال Serial Number وتفعيل النسخة.", "ترخيص", AdvancedCommands.Activate, false);
            A("license-info", "النظام والترخيص", "حالة الترخيص", "عرض حالة الترخيص ومعرّف الجهاز.", "ترخيص", AdvancedCommands.ShowLicenseInfo, false);
            A("machine-id", "النظام والترخيص", "نسخ معرّف الجهاز", "نسخ Machine ID لإرساله إلى مالك البرنامج.", "ترخيص", AdvancedCommands.ShowMachineId, false);
            A("health-check", "النظام والترخيص", "فحص الإضافة", "عرض حالة الاتصال وإصدار Word والسجل.", "فحص", SystemCommands.HealthCheck, false);
            A("about", "النظام والترخيص", "حول WordPro Suite", "معلومات الإصدار والتقنية.", "حول", SystemCommands.About, false);
        }

        internal static IEnumerable<CommandDescriptor> All => Map.Values;

        private static void A(string id, string category, string title, string description, string keywords, Action action, bool requiresLicense = true)
        {
            Map.Add(id, new CommandDescriptor(id, category, title, description, keywords, action, requiresLicense));
        }

        internal static void Execute(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) return;
            CommandDescriptor command;
            if (!Map.TryGetValue(id, out command))
            {
                MessageBox.Show("الأداة غير مسجلة: " + id, "WordPro Suite");
                return;
            }

            if (command.RequiresLicense && !LicenseManager.EnsureLicensedWithUi())
            {
                MessageBox.Show("يلزم تفعيل البرنامج أو وجود فترة تجريبية فعالة.", "WordPro Suite", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Logger.Info("Start " + id);
                command.Execute();
                CommandUsageStore.RecordRun(id);
                Logger.Info("Done " + id);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed " + id, ex);
                MessageBox.Show(ex.Message, "WordPro Suite", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
