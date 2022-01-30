using System;

namespace ORMapper_Framework.DBHelperClasses.DBTypeMapper
{
    /// <summary>
    /// Interface for DbTypeMapper classes.
    /// </summary>
    public interface IDbTypeMapper
    {
        /// <summary>
        /// Returns database column type for a specified type
        /// </summary>
        /// <param name="type">Type to be converted to database type</param>
        /// <returns>Empty string if type can't be mapped, otherwise appropriate database type gets returned</returns>
        string ConvertTypeToDbType(Type type);
    }
}
