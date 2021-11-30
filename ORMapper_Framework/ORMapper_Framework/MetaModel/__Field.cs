using ORMapper_Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ORMapper_Framework.MetaModel
{
    internal class __Field
    {
        public __Field(__Entity entity)
        {
            Entity = entity;
        }
        public __Entity Entity { get; private set; }
        public MemberInfo Member { get; internal set; }
        public Type Type {
            get
            {
                if(Member is PropertyInfo) { return ((PropertyInfo)Member).PropertyType; }
                throw new NotSupportedException("Member type not supported");
            } 
        }
        public string ColumnName { get; internal set; }
        public Type ColumnType { get; internal set; }
        public bool IsPrimaryKey { get; internal set; } = false;
        public bool IsForeignKey { get; internal set; } = false;
        public string AssignmentTable { get; internal set; }
        public string RemoteColumnName { get; internal set; }
        public bool IsManyToMany { get; internal set; }
        public bool IsNullable { get; internal set; } = false;
        public bool IsExternal { get; internal set; } = false;
        public bool IsUnique { get; internal set; } = false;
        public FKConstraintType DeleteConstraint { get; internal set; } = FKConstraintType.NOACTION;
    }
}
