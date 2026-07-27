# إصلاح C# Runtime Binder

تمت إضافة مرجع `Microsoft.CSharp` إلى مشروع إضافة Word لحل الخطأ:

`CS0656: Missing compiler required member Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create`

هذا المرجع مطلوب لأن المشروع يستخدم النوع `dynamic` للتعامل مع واجهات Word COM.
