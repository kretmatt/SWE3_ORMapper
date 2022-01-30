using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORMapper_Framework;
using ORMapper_Framework.Attributes;

namespace ORMapperDemo.Library
{
    [Entity(TableName = "BASEPERSON")]
    public class Person : AEntity
    {
        [Pk]
        public int ID { get; set; }

        public string Name { get; set; }
        [Column(ColumnDbType = "TEXT", ColumnName = "SNAME", Nullable = true)]
        public string SurName { get; set; }

        public DateTime BirthDate { get; set; }

        public EGender Gender { get; set; }
    }
}
