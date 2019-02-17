using System;

namespace Alexw.JsonHierarchy
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class SubTypeAttribute : Attribute
    {
        public Type NewType { get; }
        public string PropertyName { get; }
        public string Value { get; }

        public SubTypeAttribute(Type original, string property, string value)
        {
            NewType = original ?? throw new ArgumentNullException(nameof(original));
            PropertyName = property ?? throw new ArgumentNullException(nameof(property));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}