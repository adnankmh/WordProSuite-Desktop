إصلاح فشل تحميل إضافة Word

السبب:
تم تعريف IDTExtensibility2 وIRibbonExtensibility كواجهات IUnknown، بينما Office يستدعيهما كواجهات Automation/IDispatch.
هذا يسبب تخطيط v-table غير صحيح وقد يجعل Word يعرض رسالة أن الإضافة سببت مشكلة خطيرة.

ما تم إصلاحه:
1) استخدام InterfaceIsIDispatch.
2) إضافة DispId الصحيحة.
3) إضافة MarshalAs المناسبة.
4) منع أي استثناء من الخروج من constructor أو OnConnection أو GetCustomUI إلى Word.

طريقة الاستخدام:
- انسخ مجلد src فوق مجلد src في المستودع.
- Commit ثم Push.
- نزّل Artifact الجديد.
- أزل الإصدار القديم من Windows.
- أعد تشغيل الجهاز أو أغلق WINWORD.EXE بالكامل من Task Manager.
- ثبّت MSI المطابق لبنية Office كمسؤول.
