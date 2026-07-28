# اختبار Office 2024 الحقيقي

1. جهّز Windows self-hosted runner على جهاز يحتوي Microsoft Office 2024.
2. أضف labels: `windows` و`office2024`.
3. ثبّت Visual Studio Build Tools و.NET Framework 4.8 Developer Pack.
4. شغّل Workflow: `Test Ultimate 4.0 on Office 2024`.
5. يبني Workflow Setup، ويثبته بصمت، ويفتح Word عبر COM، ويفعّل الإضافة، ويتحقق من `OnConnection`، ثم يزيلها.
6. راجع Artifact الخاص بالسجلات عند أي فشل.
