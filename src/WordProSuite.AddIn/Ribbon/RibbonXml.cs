namespace WordProSuite.Desktop.Ribbon
{
    internal static class RibbonXml
    {
        internal const string Value = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<customUI xmlns=""http://schemas.microsoft.com/office/2009/07/customui"" onLoad=""RibbonOnLoad"">
  <ribbon>
    <tabs>
      <tab id=""tabWordProDesktop"" label=""WordPro Suite Desktop"">
        <group id=""grpMain"" label=""الرئيسية"">
          <button id=""btnCenter"" tag=""command-center"" label=""مركز الأدوات"" size=""large"" imageMso=""AddInManager"" onAction=""RibbonOnAction""/>
          <button id=""btnHealth"" tag=""health-check"" label=""فحص الإضافة"" size=""large"" imageMso=""HappyFace"" onAction=""RibbonOnAction""/>
          <button id=""btnAbout"" tag=""about"" label=""حول"" imageMso=""Info"" onAction=""RibbonOnAction""/>
        </group>
        <group id=""grpArabic"" label=""العربية"">
          <button id=""btnRtl"" tag=""rtl"" label=""RTL"" size=""large"" imageMso=""ParagraphRightToLeft"" onAction=""RibbonOnAction""/>
          <button id=""btnLtr"" tag=""ltr"" label=""LTR"" size=""large"" imageMso=""ParagraphLeftToRight"" onAction=""RibbonOnAction""/>
          <button id=""btnArabicClean"" tag=""arabic-clean-all"" label=""تنظيف عربي"" size=""large"" imageMso=""ReviewDeleteAllMarkupInDocument"" onAction=""RibbonOnAction""/>
          <menu id=""menuArabic"" label=""المزيد"" imageMso=""TextDirectionOptions"">
            <button id=""btnDiac"" tag=""remove-diacritics"" label=""حذف التشكيل"" onAction=""RibbonOnAction""/>
            <button id=""btnTatweel"" tag=""remove-tatweel"" label=""حذف التطويل"" onAction=""RibbonOnAction""/>
            <button id=""btnNormAr"" tag=""normalize-arabic"" label=""توحيد الحروف"" onAction=""RibbonOnAction""/>
            <button id=""btnEDigits"" tag=""digits-eastern"" label=""أرقام عربية"" onAction=""RibbonOnAction""/>
            <button id=""btnWDigits"" tag=""digits-western"" label=""أرقام غربية"" onAction=""RibbonOnAction""/>
          </menu>
        </group>
        <group id=""grpText"" label=""النص"">
          <button id=""btnSpaces"" tag=""collapse-spaces"" label=""المسافات"" size=""large"" imageMso=""ClearFormatting"" onAction=""RibbonOnAction""/>
          <button id=""btnEmpty"" tag=""remove-empty-paragraphs"" label=""الفقرات الفارغة"" size=""large"" imageMso=""ParagraphSpacingAfter"" onAction=""RibbonOnAction""/>
          <menu id=""menuText"" label=""أدوات النص"" imageMso=""TextBoxInsert"">
            <button id=""btnTabs"" tag=""tabs-to-spaces"" label=""Tabs إلى مسافات"" onAction=""RibbonOnAction""/>
            <button id=""btnBreaks"" tag=""linebreaks-to-paragraphs"" label=""الأسطر إلى فقرات"" onAction=""RibbonOnAction""/>
            <button id=""btnDup"" tag=""remove-duplicate-lines"" label=""حذف المكرر"" onAction=""RibbonOnAction""/>
            <button id=""btnSortA"" tag=""sort-lines-asc"" label=""فرز تصاعدي"" onAction=""RibbonOnAction""/>
            <button id=""btnSortD"" tag=""sort-lines-desc"" label=""فرز تنازلي"" onAction=""RibbonOnAction""/>
            <button id=""btnUpper"" tag=""uppercase"" label=""UPPERCASE"" onAction=""RibbonOnAction""/>
            <button id=""btnLower"" tag=""lowercase"" label=""lowercase"" onAction=""RibbonOnAction""/>
            <button id=""btnTitle"" tag=""titlecase"" label=""Title Case"" onAction=""RibbonOnAction""/>
          </menu>
        </group>
        <group id=""grpFormat"" label=""التنسيق"">
          <button id=""btnArial"" tag=""font-arial-14"" label=""Arial 14"" size=""large"" imageMso=""FontDialog"" onAction=""RibbonOnAction""/>
          <button id=""btnReport"" tag=""format-arabic-report"" label=""تقرير عربي"" size=""large"" imageMso=""StylesPane"" onAction=""RibbonOnAction""/>
          <menu id=""menuFormat"" label=""الفقرات والأنماط"" imageMso=""ParagraphDialog"">
            <button id=""btnRight"" tag=""align-right"" label=""يمين"" onAction=""RibbonOnAction""/>
            <button id=""btnLeft"" tag=""align-left"" label=""يسار"" onAction=""RibbonOnAction""/>
            <button id=""btnCenter2"" tag=""align-center"" label=""توسيط"" onAction=""RibbonOnAction""/>
            <button id=""btnJustify"" tag=""align-justify"" label=""ضبط"" onAction=""RibbonOnAction""/>
            <button id=""btnLine115"" tag=""line-spacing-115"" label=""تباعد 1.15"" onAction=""RibbonOnAction""/>
            <button id=""btnLine15"" tag=""line-spacing-15"" label=""تباعد 1.5"" onAction=""RibbonOnAction""/>
            <button id=""btnH1"" tag=""heading-1"" label=""عنوان 1"" onAction=""RibbonOnAction""/>
            <button id=""btnH2"" tag=""heading-2"" label=""عنوان 2"" onAction=""RibbonOnAction""/>
            <button id=""btnH3"" tag=""heading-3"" label=""عنوان 3"" onAction=""RibbonOnAction""/>
            <button id=""btnNormal"" tag=""style-normal"" label=""عادي"" onAction=""RibbonOnAction""/>
            <button id=""btnClearFmt"" tag=""clear-formatting"" label=""إزالة التنسيق"" onAction=""RibbonOnAction""/>
          </menu>
        </group>
        <group id=""grpTables"" label=""الجداول"">
          <button id=""btnTableRtl"" tag=""table-rtl"" label=""جدول RTL"" size=""large"" imageMso=""TableInsertDialogWord"" onAction=""RibbonOnAction""/>
          <button id=""btnTableFit"" tag=""table-autofit-window"" label=""ملاءمة الصفحة"" size=""large"" imageMso=""TableAutoFitWindow"" onAction=""RibbonOnAction""/>
          <menu id=""menuTables"" label=""المزيد"" imageMso=""TablePropertiesDialog"">
            <button id=""btnFitContent"" tag=""table-autofit-content"" label=""ملاءمة المحتوى"" onAction=""RibbonOnAction""/>
            <button id=""btnCols"" tag=""table-distribute-columns"" label=""توزيع الأعمدة"" onAction=""RibbonOnAction""/>
            <button id=""btnRows"" tag=""table-distribute-rows"" label=""توزيع الصفوف"" onAction=""RibbonOnAction""/>
            <button id=""btnHeader"" tag=""table-repeat-header"" label=""تكرار صف العنوان"" onAction=""RibbonOnAction""/>
            <button id=""btnEmptyRows"" tag=""table-remove-empty-rows"" label=""حذف الصفوف الفارغة"" onAction=""RibbonOnAction""/>
            <button id=""btnCellCenter"" tag=""table-center-cells"" label=""توسيط الخلايا"" onAction=""RibbonOnAction""/>
            <button id=""btnToText"" tag=""table-to-text"" label=""تحويل إلى نص"" onAction=""RibbonOnAction""/>
          </menu>
        </group>
        <group id=""grpDocument"" label=""المستند"">
          <button id=""btnFields"" tag=""update-fields"" label=""تحديث الحقول"" size=""large"" imageMso=""RefreshAll"" onAction=""RibbonOnAction""/>
          <button id=""btnToc"" tag=""insert-toc"" label=""إضافة فهرس"" size=""large"" imageMso=""TableOfContentsAddTextGallery"" onAction=""RibbonOnAction""/>
          <menu id=""menuDocument"" label=""الصفحات والمراجعة"" imageMso=""PageSetupDialog"">
            <button id=""btnPageNum"" tag=""page-numbers-add"" label=""إضافة أرقام الصفحات"" onAction=""RibbonOnAction""/>
            <button id=""btnPageRemove"" tag=""page-numbers-remove"" label=""حذف أرقام الصفحات"" onAction=""RibbonOnAction""/>
            <button id=""btnA4"" tag=""page-a4"" label=""A4"" onAction=""RibbonOnAction""/>
            <button id=""btnPortrait"" tag=""page-portrait"" label=""عمودي"" onAction=""RibbonOnAction""/>
            <button id=""btnLandscape"" tag=""page-landscape"" label=""أفقي"" onAction=""RibbonOnAction""/>
            <button id=""btnMargins"" tag=""margins-normal"" label=""هوامش عادية"" onAction=""RibbonOnAction""/>
            <button id=""btnNarrow"" tag=""margins-narrow"" label=""هوامش ضيقة"" onAction=""RibbonOnAction""/>
            <button id=""btnComments"" tag=""remove-comments"" label=""حذف التعليقات"" onAction=""RibbonOnAction""/>
            <button id=""btnAccept"" tag=""accept-revisions"" label=""قبول التعديلات"" onAction=""RibbonOnAction""/>
            <button id=""btnReject"" tag=""reject-revisions"" label=""رفض التعديلات"" onAction=""RibbonOnAction""/>
            <button id=""btnLinks"" tag=""remove-hyperlinks"" label=""إزالة الروابط"" onAction=""RibbonOnAction""/>
            <button id=""btnMeta"" tag=""remove-metadata"" label=""إزالة البيانات الشخصية"" onAction=""RibbonOnAction""/>
          </menu>
        </group>
        <group id=""grpFiles"" label=""الملفات و PDF"">
          <button id=""btnBackup"" tag=""save-backup"" label=""نسخة احتياطية"" size=""large"" imageMso=""FileSaveAs"" onAction=""RibbonOnAction""/>
          <button id=""btnPdf"" tag=""export-pdf"" label=""PDF"" size=""large"" imageMso=""FileSaveAsPdfOrXps"" onAction=""RibbonOnAction""/>
          <button id=""btnPdfA"" tag=""export-pdfa"" label=""PDF/A"" imageMso=""FileSaveAsPdfOrXps"" onAction=""RibbonOnAction""/>
        </group>
        <group id=""grpTemplates"" label=""القوالب والأتمتة"">
          <menu id=""menuTemplates"" label=""إدراج قالب"" size=""large"" imageMso=""NewDocumentOrTemplate"">
            <button id=""btnAgenda"" tag=""template-agenda"" label=""جدول أعمال"" onAction=""RibbonOnAction""/>
            <button id=""btnMinutes"" tag=""template-minutes"" label=""محضر اجتماع"" onAction=""RibbonOnAction""/>
            <button id=""btnLetter"" tag=""template-official-letter"" label=""خطاب رسمي"" onAction=""RibbonOnAction""/>
            <button id=""btnMedical"" tag=""template-medical-report"" label=""تقرير طبي"" onAction=""RibbonOnAction""/>
            <button id=""btnProject"" tag=""template-project-report"" label=""تقرير مشروع"" onAction=""RibbonOnAction""/>
            <button id=""btnHandover"" tag=""template-handover"" label=""تسليم واستلام"" onAction=""RibbonOnAction""/>
          </menu>
          <button id=""btnFinal"" tag=""workflow-final-delivery"" label=""نسخة نهائية"" size=""large"" imageMso=""FileCheckIn"" onAction=""RibbonOnAction""/>
        </group>
      </tab>
    </tabs>
  </ribbon>
</customUI>";
    }
}
