using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORMapper_Framework.Enums;

namespace ORMapper_Framework.Attributes
{
    /// <summary>
    /// Marks a member as a foreign key in the database. 
    /// </summary>
    public class FkAttribute : ColumnAttribute
    {
        /// <summary>
        /// Name of the assignment table.
        /// </summary>
        /// <remarks>ColumnName = Near foreign key of assignment table, RemoteColumnName = far foreign key</remarks>
        public string AssignmentTable = null;
        /// <summary>
        /// Column name of other foreign key in the assignment table.
        /// </summary>
        public string RemoteColumnName = null;
        /// <summary>
        /// Delete constraint type
        /// </summary>
        public FkConstraintType DeleteConstraint = FkConstraintType.NoAction;
    }
}
