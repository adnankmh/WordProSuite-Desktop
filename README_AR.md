# WordPro Suite Desktop Pro 2.1

إضافة COM مكتبية أصلية لبرنامج Microsoft Word، تحتوي على **259 أداة** مصنفة في **15 قسمًا**.

## الاستخدام النهائي

بعد نجاح GitHub Actions نزّل Artifact باسم:

`WordProSuite-Desktop-Pro-2.1-Windows`

ثم شغّل فقط:

`WordProSuite_Setup.exe`

لا يحتاج المستخدم إلى Payload أو MSI أو Node.js أو localhost.

## التثبيت والتفعيل

- **تثبيت تجريبي:** يثبت الإضافة ويشغل فترة التجربة.
- **تثبيت وتفعيل:** الصق Serial Number ثم اضغط الزر.
- **إصلاح:** يعيد نسخ DLL وتسجيل COM ومسح حالة تعطيل Word.
- **إزالة:** يحذف ملفات الإضافة وتسجيل COM من الحساب الحالي.

## الحماية والترخيص

يستخدم الترخيص توقيع RSA:
- المفتاح العام فقط داخل البرنامج.
- المفتاح الخاص موجود في حزمة Owner License Studio المنفصلة.
- يمكن إصدار ترخيص دائم أو مؤقت أو مرتبط بجهاز.

## البناء

```powershell
.\scripts\validate-source.ps1
.\scripts\build-release.ps1 -Configuration Release
```

## الملفات المهمة

- `catalog/commands_v2_1.csv`: قائمة جميع الأدوات.
- `V2_1_FEATURES_AR.md`: ملخص الميزات والأقسام.
- `scripts/validate-source.ps1`: اختبارات المصدر.
- `scripts/test-office2024.ps1`: اختبار تكامل فعلي على جهاز Office 2024.
