using ORMapper_Framework.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ORMapper_Framework.MetaModel
{
    public class __Entity
    {
        public __Entity(Type t)
        {
            EntityAttribute eAttr = (EntityAttribute)t.GetCustomAttribute(typeof(EntityAttribute));
            
            if ((eAttr == null) || (string.IsNullOrWhiteSpace(eAttr.TableName)))
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
                {
                    TableName = t.GetGenericArguments()[0].Name.ToUpper();
                    t = t.GetGenericArguments()[0];
                }
                    
                else
                    TableName = t.Name.ToUpper();
            }
            else
                TableName = eAttr.TableName;

            Member = t;
            List<__Field> fields = new List<__Field>();

            if (t.BaseType.FullName != typeof(object).FullName && t.BaseType.IsClass && t.IsClass && t.BaseType.FullName != typeof(AEntity).FullName)
                IsDerived = true;

            foreach(PropertyInfo i in t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if ((IgnoreAttribute)i.GetCustomAttribute(typeof(IgnoreAttribute)) != null) continue;

                __Field f = new __Field(this);

                if ((UniqueAttribute)i.GetCustomAttribute(typeof(UniqueAttribute)) != null)
                {
                    f.IsUnique = true;
                }

                ColumnAttribute cattr = (ColumnAttribute)i.GetCustomAttribute(typeof(ColumnAttribute));

                if(cattr != null)
                {
                    f.ColumnName = cattr.ColumnName ?? i.Name;
                    f.ColumnType = cattr.ColumnType ?? i.PropertyType; 
                    f.IsNullable = cattr.Nullable;

                    if(cattr is PKAttribute)
                    {
                        PrimaryKey = f;
                        f.IsPrimaryKey = true;
                    }
                    if(cattr is FKAttribute)
                    {
                        f.IsExternal = typeof(IEnumerable).IsAssignableFrom(i.PropertyType);
                        f.IsForeignKey = true;
                        f.AssignmentTable = ((FKAttribute)cattr).AssignmentTable;
                        f.RemoteColumnName = ((FKAttribute)cattr).RemoteColumnName;
                        f.DeleteConstraint = ((FKAttribute)cattr).DeleteConstraint;
                        f.IsManyToMany = (!string.IsNullOrWhiteSpace(f.AssignmentTable));
                    }
                }
                else
                {
                    if ((i.GetGetMethod() == null) || (!i.GetGetMethod().IsPublic)) continue;

                    f.ColumnName = i.Name;
                    f.ColumnType = i.PropertyType;
                }

                f.Member = i;
                fields.Add(f);
            }

            Fields = fields.ToArray();
            Internals = fields.Where(m => (!m.IsExternal)).ToArray();
            Externals = fields.Where(m => m.IsExternal).ToArray();
        }

        public Type Member { get; private set; }
        public string TableName { get; private set; }
        public __Field[] Fields { get; private set; }
        public __Field[] Externals { get; private set; }
        public __Field[] Internals { get; private set; }
        public __Field PrimaryKey { get; private set; }
        public bool IsDerived { get; internal set; } = false;

    }
}
