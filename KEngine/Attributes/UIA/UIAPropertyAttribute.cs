using KEngine.Enums;
using System;

namespace KEngine.Attributes.UIA
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UIAPropertyAttribute : Attribute
    {
        public EUIAEditorType EditorType { get; set; } = EUIAEditorType.TextEdit;
        public string Label { get; set; } = string.Empty;
        public Type TypeOfValues { get; set; } = null;
    }
}
