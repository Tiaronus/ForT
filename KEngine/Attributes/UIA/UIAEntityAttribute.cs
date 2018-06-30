using System;

namespace KEngine.Attributes.UIA
{
    public class UIAEntityAttribute : Attribute
    {
        public bool bAddMenuRecord { get; set; } = true;

        public string MenuRecordName { get; set; } = "Entity";

        public Type EntityType { get; set; } = null;
    }
}
