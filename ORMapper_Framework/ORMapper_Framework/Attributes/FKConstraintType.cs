using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapper_Framework.Attributes
{
    public enum FKConstraintType
    {
        [Description("CASCADE")]
        CASCADE,
        [Description("NO ACTION")]
        NOACTION,
        [Description("SET NULL")]
        SETNULL
    }
}
