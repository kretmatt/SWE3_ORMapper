using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapper_Framework.Attributes
{
    /// <summary>
    /// Marks a property as ignored. The property won't get saved in the database
    /// </summary>
    public class IgnoreAttribute : Attribute
    {
    }
}
