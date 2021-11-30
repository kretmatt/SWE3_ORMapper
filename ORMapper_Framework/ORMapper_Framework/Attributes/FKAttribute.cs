using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapper_Framework.Attributes
{
    /// <summary>
    /// Marks
    /// </summary>
    public class FKAttribute : ColumnAttribute
    {
        /// <summary>
        /// Name of the assignment table.
        /// </summary>
        /// <remarks>ColumnName = Near foreign key of assignment table, RemoteColumnName = far foreign key</remarks>
        public string AssignmentTable = null;
        /// <summary>
        /// Far side foreign key in assignment table
        /// </summary>
        public string RemoteColumnName = null;
        /// <summary>
        /// Delete constraint type
        /// </summary>
        public FKConstraintType DeleteConstraint = FKConstraintType.NOACTION;
    }
}
