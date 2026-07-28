# بوابات إصدار Ultimate 4.0

لا يعد الإصدار نهائيًا قبل نجاح:

1. فحص 600 Command IDs فريدة.
2. فحص ثلاثة تبويبات Ribbon وربط كل tag بأمر مسجل.
3. فحص CS0103 الخاص بـ Prompt namespace.
4. فحص CS1977 الخاص بـ dynamic/lambda.
5. Restore وبناء الحل مع warnings-as-errors.
6. بناء WordProSuite_Setup.exe مع DLL مضمّن.
7. التحقق من اسم المورد داخل Setup بالـReflection.
8. التحقق من SHA-256.
9. Install / Repair / Uninstall.
10. إنشاء COM object فعليًا قبل نجاح Setup.
11. ظهور الثلاثة تبويبات داخل Word.
12. تنفيذ عينات من العربية والجداول وPDF والقوالب والتحليل.
13. إغلاق Word بلا Crash.
14. اختبار Office 2024 على self-hosted runner.
15. توقيع Setup بشهادة Code Signing قبل التوزيع التجاري العام.
