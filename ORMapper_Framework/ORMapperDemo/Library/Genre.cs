using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORMapper_Framework;
using ORMapper_Framework.Attributes;

namespace ORMapperDemo.Library
{

    public class Genre : AEntity
    {
        [Pk]
        public int Id { get; set; }

        [Unique]
        [Column(ColumnDbType = "TEXT", ColumnName = "GNAME")]
        public string GenreName { get; set; }

        [Fk(ColumnName = "GID")] 
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
