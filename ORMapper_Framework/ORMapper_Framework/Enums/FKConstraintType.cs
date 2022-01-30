using System.ComponentModel;

namespace ORMapper_Framework.Enums
{
    /// <summary>
    /// Enum for database delete constraints
    /// </summary>
    public enum FkConstraintType
    {
        [Description("CASCADE")]
        Cascade,
        [Description("NO ACTION")]
        NoAction,
        [Description("SET NULL")]
        SetNull
    }
}
