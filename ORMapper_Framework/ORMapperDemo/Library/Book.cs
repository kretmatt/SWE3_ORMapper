using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORMapper_Framework;
using ORMapper_Framework.Attributes;

namespace ORMapperDemo.Library
{
    public class Book : AEntity
    {
        [Pk]
        public string BookID { get; set; }

        public string Title { get; set; }

        [Fk(ColumnName = "GID")]
        public Genre Genre { get; set; }


        [Fk(AssignmentTable = "AUTHOR_BOOK", ColumnName = "BOOKID", RemoteColumnName = "AUTHORID")]
        public List<Author> Authors { get; set; } = new List<Author>();


    }
}
