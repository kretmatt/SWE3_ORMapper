using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapper_Framework
{
    /// <summary>
    /// ORMapper Database exception. Gets thrown if database commands could not be executed
    /// </summary>
    public class OrMapperDatabaseException : ApplicationException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Exception from the database</param>
        public OrMapperDatabaseException(string message, Exception innerException):base(message, innerException){}
    }
}
