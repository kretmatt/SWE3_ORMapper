using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapper_Framework.DBHelperClasses.DBTypeMapper
{
    /// <summary>
    /// DBTypeMapper is used for mapping C# types to postgresql types (table creation process). Primarily write mapping
    /// https://www.npgsql.org/doc/types/basic.html was a reference for compatability between types
    /// </summary>
    internal class DbTypeMapper
    {
        /// <summary>
        /// Dictionary for type to database type mapping
        /// </summary>
        private static readonly Dictionary<Type, string> DatabaseTypeMap = new Dictionary<Type, string>()
        {
            { typeof(bool), "BOOLEAN"},
            { typeof(short), "SMALLINT"},
            { typeof(byte), "SMALLINT"},
            { typeof(sbyte), "SMALLINT"},
            { typeof(int), "INTEGER"},
            { typeof(long), "BIGINT"},
            { typeof(float), "REAL"},
            { typeof(double), "DOUBLE PRECISION"},
            { typeof(decimal), "NUMERIC"},
            { typeof(string), "VARCHAR"},
            { typeof(char[]), "VARCHAR"},
            { typeof(char), "VARCHAR"},
            { typeof(Guid), "UUID"},
            { typeof(byte[]), "BYTEA"},
            //Local datetime
            { typeof(DateTime), "TIMESTAMP WITHOUT TIME ZONE"},
            //datetime with offset
            { typeof(DateTimeOffset), "TIMESTAMP"}
        };
        /// <summary>
        /// Returns database column type for a specified type
        /// </summary>
        /// <param name="type">Type to be converted to database type</param>
        /// <returns>Empty string if type can't be mapped, otherwise appropriate database type gets returned</returns>
        public static string MapType(Type type)
        {
            if (type.IsEnum)
                return "INTEGER";
            return DatabaseTypeMap.ContainsKey(type) ? DatabaseTypeMap[type] : "";
        }
    }
}
