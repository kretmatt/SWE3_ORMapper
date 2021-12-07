using ORMapper_Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapperDemo.School
{
    [Entity(TableName ="TEACHERS")]
    public class Teacher:Person
    {
        public int Salary { get; set; }
    }
}
