using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapper_Framework.Enums
{
    /// <summary>
    /// Enum for postgres row lock types
    /// </summary>
    public enum LockType
    {
        [Description("FOR UPDATE")]
        ForUpdate,
        [Description("FOR NO KEY UPDATE")]
        ForNoKeyUpdate,
        [Description("FOR SHARE")]
        ForShare,
        [Description("FOR KEY SHARE")]
        ForKeyShare
    }
}
