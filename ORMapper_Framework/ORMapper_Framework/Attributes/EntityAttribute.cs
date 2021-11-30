using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapper_Framework.Attributes
{
    /// <summary>
    /// Marks a class as a database entity. Can only be defined for classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityAttribute:Attribute
    {
        /// <summary>
        /// Name of the database table
        /// </summary>
        public string TableName;
    }
}
