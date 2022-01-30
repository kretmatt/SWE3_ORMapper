using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapper_Framework
{
    /// <summary>
    /// Abstract base class for marking classes as entities. Important for table creation (it stops the recursive DecodeEntity method from advancing to object etc.)
    /// </summary>
    public abstract class AEntity
    {
    }
}
