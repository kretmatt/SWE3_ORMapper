using System;
using System.ComponentModel;
using System.Reflection;

namespace ORMapper_Framework.Enums
{
    /// <summary>
    /// Extensions for enums
    /// </summary>
    internal static class EnumExtensions
    {
        /// <summary>
        /// Enum extension method for retrieving enum description attributes (used for associating "sql text" with enums)
        /// </summary>
        /// <param name="value">Specified enum</param>
        /// <returns>Null if no description exists. Otherwise the description will be returned</returns>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);

            if (name == null) return null;

            FieldInfo field = type.GetField(name);

            if (field == null) return null;

            DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attr?.Description;
        }
    }
}
