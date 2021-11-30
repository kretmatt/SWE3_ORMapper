using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapper_Framework.Attributes
{
    /// <summary>
    /// Marks a member of a class as a database column.
    /// </summary>
    public class ColumnAttribute:Attribute
    {
       /// <summary>
       /// Name of the database column.
       /// </summary>
        public string ColumnName = null;
        /// <summary>
        /// Type of the database column.
        /// </summary>
        public Type ColumnType = null;
        /// <summary>
        /// Nullable flag of the database column
        /// </summary>
        public bool Nullable = false;
    }
}
