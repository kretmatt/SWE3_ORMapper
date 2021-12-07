using ORMapper_Framework;
using ORMapper_Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapperDemo.School
{
    public abstract class Person : AEntity
    {
        [PK]
        public string ID { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }
        
        [Column(ColumnName = "BDATE")]
        public DateTime BirthDate { get; set; }

    }
}
