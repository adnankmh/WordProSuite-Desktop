using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WordProSuite.Desktop.Infrastructure;

namespace WordProSuite.Desktop.Commands
{
    internal static class Commands
    {
        internal static void Rtl()
        {
            dynamic r=WordContext.TargetRange;
            r.ParagraphFormat.ReadingOrder=0;
            r.ParagraphFormat.Alignment=2;
        }
        internal static void Ltr()
        {
            dynamic r=WordContext.TargetRange;
            r.ParagraphFormat.ReadingOrder=1;
            r.ParagraphFormat.Alignment=0;
        }
        internal static void RemoveDiacritics(){dynamic r=WordContext.TargetRange;r.Text=TextTransforms.RemoveDiacritics((string)r.Text);}
        internal static void RemoveTatweel(){dynamic r=WordContext.TargetRange;r.Text=((string)r.Text).Replace("ـ","");}
        internal static void NormalizeArabic(){dynamic r=WordContext.TargetRange;r.Text=TextTransforms.NormalizeArabic((string)r.Text);}
        internal static void ToEastern(){dynamic r=WordContext.TargetRange;r.Text=TextTransforms.Eastern((string)r.Text);}
        internal static void ToWestern(){dynamic r=WordContext.TargetRange;r.Text=TextTransforms.Western((string)r.Text);}
        internal static void ArabicCleanAll()
        {
            dynamic r=WordContext.TargetRange;
            string t=TextTransforms.CollapseSpaces(TextTransforms.RemoveDiacritics((string)r.Text).Replace("ـ",""));
            r.Text=t;r.ParagraphFormat.ReadingOrder=0;r.ParagraphFormat.Alignment=3;
            r.Font.Name="Arial";r.Font.NameBi="Arial";r.Font.Size=14;r.Font.SizeBi=14;
        }

        internal static void CollapseSpaces(){dynamic r=WordContext.TargetRange;r.Text=TextTransforms.CollapseSpaces((string)r.Text);}
        internal static void RemoveEmptyParagraphs(){dynamic r=WordContext.TargetRange;for(int i=0;i<8;i++)WordContext.ReplaceAll(r,"^p^p","^p");}
        internal static void TabsToSpaces()=>WordContext.ReplaceAll(WordContext.TargetRange,"^t"," ");
        internal static void LineBreaksToParagraphs()=>WordContext.ReplaceAll(WordContext.TargetRange,"^l","^p");
        internal static void RemoveDuplicateLines()
        {
            dynamic r=WordContext.TargetRange;
            var seen=new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
            var output=new List<string>();
            foreach(string line in TextTransforms.Lines((string)r.Text))
            {
                string key=line.Trim();
                if(key.Length==0||seen.Add(key))output.Add(line.TrimEnd());
            }
            r.Text=String.Join("\r",output);
        }
        internal static void SortLines(bool desc)
        {
            dynamic r=WordContext.TargetRange;
            var lines=TextTransforms.Lines((string)r.Text).Where(x=>x.Trim().Length>0).Select(x=>x.Trim()).ToList();
            lines.Sort(StringComparer.CurrentCultureIgnoreCase);if(desc)lines.Reverse();r.Text=String.Join("\r",lines);
        }
        internal static void Uppercase(){dynamic r=WordContext.TargetRange;r.Text=((string)r.Text).ToUpper(CultureInfo.CurrentCulture);}
        internal static void Lowercase(){dynamic r=WordContext.TargetRange;r.Text=((string)r.Text).ToLower(CultureInfo.CurrentCulture);}
        internal static void TitleCase(){dynamic r=WordContext.TargetRange;string t=((string)r.Text).ToLower(CultureInfo.CurrentCulture);r.Text=CultureInfo.CurrentCulture.TextInfo.ToTitleCase(t);}
        internal static void RemoveHyperlinks(){dynamic d=WordContext.Document;while(d.Hyperlinks.Count>0)d.Hyperlinks[1].Delete();}

        internal static void Arial14(){dynamic r=WordContext.TargetRange;r.Font.Name="Arial";r.Font.NameBi="Arial";r.Font.Size=14;r.Font.SizeBi=14;}
        internal static void ClearFormatting()=>WordContext.TargetRange.ClearFormatting();
        internal static void Align(int value)=>WordContext.TargetRange.ParagraphFormat.Alignment=value;
        internal static void LineSpacing(float points){dynamic p=WordContext.TargetRange.ParagraphFormat;p.LineSpacingRule=4;p.LineSpacing=points;}
        internal static void Style(string name)=>WordContext.TargetRange.set_Style(name);
        internal static void ArabicReport()
        {
            Arial14();dynamic r=WordContext.TargetRange;r.ParagraphFormat.ReadingOrder=0;
            r.ParagraphFormat.Alignment=3;r.ParagraphFormat.SpaceAfter=6;r.ParagraphFormat.LineSpacingRule=0;
        }

        private static dynamic Table()
        {
            dynamic s=WordContext.Selection;
            if(s==null||s.Tables.Count==0)throw new InvalidOperationException("ضع المؤشر داخل جدول أولاً.");
            return s.Tables[1];
        }
        internal static void TableRtl(){dynamic t=Table();t.Direction=0;t.Range.ParagraphFormat.ReadingOrder=0;t.Range.ParagraphFormat.Alignment=2;}
        internal static void TableAutoFit(int behavior)=>Table().AutoFitBehavior(behavior);
        internal static void TableDistributeColumns()=>Table().Columns.DistributeWidth();
        internal static void TableDistributeRows()=>Table().Rows.DistributeHeight();
        internal static void TableRepeatHeader()=>Table().Rows[1].HeadingFormat=-1;
        internal static void TableRemoveEmptyRows()
        {
            dynamic t=Table();
            for(int row=t.Rows.Count;row>=1;row--)
            {
                string all="";
                for(int col=1;col<=t.Rows[row].Cells.Count;col++)all+=WordContext.CleanCell((string)t.Rows[row].Cells[col].Range.Text);
                if(String.IsNullOrWhiteSpace(all))t.Rows[row].Delete();
            }
        }
        internal static void TableCenterCells(){dynamic t=Table();t.Range.ParagraphFormat.Alignment=1;t.Range.Cells.VerticalAlignment=1;}
        internal static void TableToText()=>Table().ConvertToText(1);

        internal static void UpdateFields()
        {
            dynamic d=WordContext.Document;
            foreach(dynamic story in d.StoryRanges)
            {
                dynamic current=story;
                while(current!=null){current.Fields.Update();current=current.NextStoryRange;}
            }
            foreach(dynamic toc in d.TablesOfContents)toc.Update();
        }
        internal static void InsertToc()
        {
            dynamic d=WordContext.Document;dynamic r=WordContext.Selection.Range;
            d.TablesOfContents.Add(r,true,1,3,false,"",true,true,"",true,true,true);
        }
        internal static void AddPageNumbers()
        {
            dynamic d=WordContext.Document;
            foreach(dynamic section in d.Sections){dynamic footer=section.Footers[1];if(footer.PageNumbers.Count==0)footer.PageNumbers.Add(1,true);}
        }
        internal static void RemovePageNumbers()
        {
            dynamic d=WordContext.Document;
            foreach(dynamic section in d.Sections)
                foreach(dynamic footer in section.Footers)
                    for(int i=footer.Range.Fields.Count;i>=1;i--){dynamic f=footer.Range.Fields[i];if((int)f.Type==33)f.Delete();}
        }
        internal static void SetA4()=>WordContext.Document.PageSetup.PaperSize=7;
        internal static void Orientation(int value)=>WordContext.Document.PageSetup.Orientation=value;
        internal static void Margins(float points){dynamic p=WordContext.Document.PageSetup;p.TopMargin=points;p.BottomMargin=points;p.LeftMargin=points;p.RightMargin=points;}
        internal static void RemoveComments(){dynamic d=WordContext.Document;while(d.Comments.Count>0)d.Comments[1].Delete();}
        internal static void AcceptRevisions()=>WordContext.Document.AcceptAllRevisions();
        internal static void RejectRevisions()=>WordContext.Document.RejectAllRevisions();
        internal static void RemoveMetadata()=>WordContext.Document.RemoveDocumentInformation(99);

        internal static void SaveBackup()
        {
            dynamic d=WordContext.Document;string source="";try{source=(string)d.FullName;}catch{}
            string dir=!String.IsNullOrWhiteSpace(source)&&File.Exists(source)?Path.GetDirectoryName(source):Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string ext=".docx";try{ext=Path.GetExtension((string)d.Name);}catch{}
            string output=Path.Combine(dir,WordContext.BaseName()+"_Backup_"+DateTime.Now.ToString("yyyyMMdd_HHmmss")+ext);
            d.SaveCopyAs(output);MessageBox.Show("تم إنشاء النسخة الاحتياطية:\n"+output,"WordPro Suite");
        }
        internal static void ExportPdf(bool pdfA)
        {
            dynamic d=WordContext.Document;
            string output=WordContext.SavePath(pdfA?"تصدير PDF/A":"تصدير PDF","PDF (*.pdf)|*.pdf",WordContext.BaseName()+(pdfA?"_PDFA.pdf":".pdf"));
            if(String.IsNullOrWhiteSpace(output))return;
            d.ExportAsFixedFormat(output,17,false,0,0,1,1,0,true,true,0,true,true,pdfA,0);
            MessageBox.Show("تم التصدير:\n"+output,"WordPro Suite");
        }

        private static readonly Dictionary<string,string> Templates=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase)
        {
            ["agenda"]="جدول أعمال الاجتماع\rالتاريخ:\rالوقت:\rالمكان:\rالحضور:\r\r1. افتتاح الاجتماع\r2. اعتماد جدول الأعمال\r3. متابعة القرارات السابقة\r4. البنود الجديدة\r5. ما يستجد من أعمال\r\rالقرارات والمسؤوليات:\r",
            ["minutes"]="محضر اجتماع\rالتاريخ:\rالوقت:\rالمكان:\rرئيس الاجتماع:\rمقرر الاجتماع:\rالحضور:\r\rأولاً: ملخص المناقشات\r\rثانياً: القرارات\r\rثالثاً: المهام والمسؤوليات\r\rموعد الاجتماع القادم:\r",
            ["letter"]="التاريخ:\rالرقم المرجعي:\r\rالسادة/ ........................................ المحترمون\rتحية طيبة وبعد،\r\rالموضوع: ........................................\r\rنص الخطاب:\r\rوتفضلوا بقبول فائق الاحترام،\r\rالاسم:\rالصفة:\rالتوقيع:\r",
            ["medical"]="تقرير طبي\rاسم المريض:\rرقم الملف:\rالتاريخ:\rالمنشأة الصحية:\rالطبيب المعالج:\rالتخصص:\r\rالشكوى الرئيسية:\r\rالتاريخ المرضي:\r\rالفحص السريري:\r\rالفحوصات:\r\rالتشخيص:\r\rالخطة العلاجية:\r\rالتوصيات:\r\rاسم الطبيب وتوقيعه:\r",
            ["project"]="تقرير مشروع\rاسم المشروع:\rالفترة:\rالجهة المنفذة:\rالموقع:\r\r1. الملخص التنفيذي\r\r2. الأهداف\r\r3. الأنشطة المنفذة\r\r4. مؤشرات الإنجاز\r\r5. التحديات والمخاطر\r\r6. الإجراءات التصحيحية\r\r7. الخطة القادمة\r\r8. المرفقات\r",
            ["handover"]="محضر تسليم واستلام\rالتاريخ:\rالمكان:\r\rالطرف المسلم:\rالطرف المستلم:\r\rوصف المواد أو الأعمال:\r\rالكمية والحالة:\r\rالملاحظات:\r\rالاسم والتوقيع:\r"
        };
        internal static void InsertTemplate(string key)
        {
            if(!Templates.TryGetValue(key,out string text))throw new InvalidOperationException("القالب غير موجود.");
            dynamic s=WordContext.Selection;s.TypeText(text);dynamic r=s.Range;r.ParagraphFormat.ReadingOrder=0;r.ParagraphFormat.Alignment=2;
            r.Font.Name="Arial";r.Font.NameBi="Arial";r.Font.Size=14;r.Font.SizeBi=14;
        }
        internal static void FinalDelivery(){AcceptRevisions();RemoveComments();UpdateFields();RemoveMetadata();}
        internal static void About()=>MessageBox.Show("WordPro Suite Desktop\nVersion 1.0.0\n\nلا يستخدم Node.js أو localhost أو VBA.","حول WordPro Suite");
        internal static void HealthCheck()
        {
            var b=new StringBuilder();
            b.AppendLine("WordPro Suite Desktop Health Check");
            b.AppendLine("Version: 1.0.0");
            b.AppendLine("Process: "+(Environment.Is64BitProcess?"64-bit":"32-bit"));
            b.AppendLine("OS: "+(Environment.Is64BitOperatingSystem?"64-bit":"32-bit"));
            b.AppendLine("Connected: "+(WordContext.Application!=null));
            b.AppendLine("Log: "+Logger.CurrentLog);
            try{b.AppendLine("Word version: "+(string)WordContext.Application.Version);}catch(Exception ex){b.AppendLine(ex.Message);}
            MessageBox.Show(b.ToString(),"فحص WordPro Suite");
        }
    }
}
