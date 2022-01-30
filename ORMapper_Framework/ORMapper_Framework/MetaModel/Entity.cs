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
    /// <summary>
    /// The entity class represents a table in the database
    /// </summary>
    internal class Entity
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="t">Type (e.g. class) for which an entity will be constructed</param>
        public Entity(Type t)
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
            List<Field> fields = new List<Field>();

            if (t.BaseType != null && t.BaseType.FullName != typeof(object).FullName && t.BaseType.IsClass && t.IsClass && t.BaseType.FullName != typeof(AEntity).FullName)
                IsDerived = true;

            foreach(PropertyInfo i in t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if ((IgnoreAttribute)i.GetCustomAttribute(typeof(IgnoreAttribute)) != null) continue;

                Field f = new Field(this);

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
                    f.DbType = cattr.ColumnDbType;

                    if (cattr is PkAttribute)
                    {
                        PrimaryKey = f;
                        f.IsPrimaryKey = true;
                    }
                    else if (cattr is FkAttribute attribute)
                    {
                        f.IsExternal = typeof(IEnumerable).IsAssignableFrom(i.PropertyType);
                        f.IsForeignKey = true;
                        f.AssignmentTable = attribute.AssignmentTable;
                        f.RemoteColumnName = attribute.RemoteColumnName;
                        f.DeleteConstraint = attribute.DeleteConstraint;
                        f.IsManyToMany = (!string.IsNullOrEmpty(f.AssignmentTable));
                        f.IsNullable = true;
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
        /// <summary>
        /// Original type of the entity
        /// </summary>
        public Type Member { get; private set; }
        /// <summary>
        /// Name of the database table
        /// </summary>
        public string TableName { get; private set; }
        /// <summary>
        /// Fields of the entity
        /// </summary>
        public Field[] Fields { get; private set; }
        /// <summary>
        /// External data associated with the entity (1:n, m:n)
        /// </summary>
        public Field[] Externals { get; private set; }
        /// <summary>
        /// Fields inside the database table
        /// </summary>
        public Field[] Internals { get; private set; }
        /// <summary>
        /// Primary key field
        /// </summary>
        public Field PrimaryKey { get; private set; }
        /// <summary>
        /// Bool that determines, whether the entity is derived
        /// </summary>
        public bool IsDerived { get; internal set; } = false;
        /// <summary>
        /// Returns a field with the specified name
        /// </summary>
        /// <param name="columnName">Column name of the field</param>
        /// <returns>Null, if no field with specified name can be found. Otherwise the associated field will be returned</returns>
        public Field GetFieldByName(string columnName)
        {
            columnName = columnName.ToUpper();
            return Internals.FirstOrDefault(i => i.ColumnName.ToUpper() == columnName);
        }
    }
}
