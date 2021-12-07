using ORMapper_Framework;
using ORMapper_Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapperDemo.School
{
    [Entity(TableName = "COURSES")]
    public class Course : AEntity
    {
        [PK]
        public string ID { get; set; }

        [FK(ColumnName = "KTEACHER")]
        public Teacher Teacher { get; set; }

        [FK(AssignmentTable ="STUDENT_COURSES", ColumnName ="KCOURSE", RemoteColumnName ="KSTUDENT")]
        public List<Student> Students { get; set; } = new List<Student>();
    }
}
