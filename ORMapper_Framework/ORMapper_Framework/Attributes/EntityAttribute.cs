using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapper_Framework.Attributes
{
    /// <summary>
    /// Explicitly marks a class as an entity and database table. Specifically designed for classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class EntityAttribute:Attribute
    {
        /// <summary>
        /// Name of the database table
        /// </summary>
        public string TableName;
    }
}
