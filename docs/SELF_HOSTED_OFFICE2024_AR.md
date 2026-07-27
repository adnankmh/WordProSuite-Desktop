# اختبار Office 2024 الحقيقي

GitHub-hosted runner يبني الملفات، لكنه لا يمثل جهاز Office النهائي.

للاختبار الفعلي:
1. أنشئ مستودعًا مستقلًا باسم `WordProSuite-Desktop`.
2. أضف Windows self-hosted runner من إعدادات GitHub Actions.
3. شغّل الـrunner على جهاز يحتوي Office 2024.
4. أضف label باسم `office2024`.
5. ثبّت Visual Studio Build Tools و.NET Framework 4.8 Developer Pack وWiX 3.14.1.
6. شغّل Workflow: `Test on Office 2024`.
7. اختر x86 أو x64 حسب Office المثبت.
