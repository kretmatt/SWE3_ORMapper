using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapper_Framework.Attributes
{
    /// <summary>
    /// ColumnAttribute explicitly marks a member of an entity class as a database column.
    /// </summary>
    public class ColumnAttribute:Attribute
    {
       /// <summary>
       /// Represents the database column name of the member
       /// </summary>
        public string ColumnName = null;
        /// <summary>
        /// Type of the database column in the program
        /// </summary>
        public Type ColumnType = null;
        /// <summary>
        /// Type of the database column in the actual database
        /// </summary>
        public string ColumnDbType = "";
        /// <summary>
        /// Nullable flag of the database column
        /// </summary>
        public bool Nullable = false;
    }
}
