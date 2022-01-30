using ORMapper_Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapperDemo.Library
{
    public class Author : Person
    {
        public string AbbreviatedName { get; set; }

        [Fk(AssignmentTable = "AUTHOR_BOOK", ColumnName = "AUTHORID", RemoteColumnName = "BOOKID")]
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
