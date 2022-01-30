using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORMapper_Framework.Attributes;

namespace ORMapperDemo.Library
{
    [Entity(TableName = "LIBCUSTOMER")]
    public class Customer : Person
    {
        public DateTime RegisteredSince { get; set; }

        [Fk(AssignmentTable = "CUSTOMER_BOOK", ColumnName = "CUSTOMERID", RemoteColumnName = "BOOKID")]
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
