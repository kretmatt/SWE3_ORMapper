using ORMapper_Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ORMapper_Framework.DBHelperClasses;
using ORMapper_Framework.Enums;

namespace ORMapper_Framework.MetaModel
{
    /// <summary>
    /// The Field class represents a column in the database
    /// </summary>
    internal class Field
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entity">Parent entity</param>
        public Field(Entity entity)
        {
            Entity = entity;
        }
        /// <summary>
        /// Parent entity
        /// </summary>
        public Entity Entity { get; private set; }
        /// <summary>
        /// Memberinfo of type
        /// </summary>
        public MemberInfo Member { get; internal set; }
        /// <summary>
        /// Type of field in program
        /// </summary>
        public Type Type {
            get
            {
                if(Member is PropertyInfo info) 
                    return info.PropertyType;
                throw new NotSupportedException("Member type not supported");
            } 
        }
        /// <summary>
        /// Name of the column in the database
        /// </summary>
        public string ColumnName { get; internal set; }
        /// <summary>
        /// Type of the field
        /// </summary>
        public Type ColumnType { get; internal set; }
        /// <summary>
        /// Type of the column in the database
        /// </summary>
        public string DbType { get; internal set; } = "";
        /// <summary>
        /// Bool that determines whether the field is a primary key
        /// </summary>
        public bool IsPrimaryKey { get; internal set; } = false;
        /// <summary>
        /// Bool that determines whether the field is a foreign key
        /// </summary>
        public bool IsForeignKey { get; internal set; } = false;
        /// <summary>
        /// Name of the assignment table (m:n relationship)
        /// </summary>
        public string AssignmentTable { get; internal set; }
        /// <summary>
        /// Name of the other column in the assignment table
        /// </summary>
        public string RemoteColumnName { get; internal set; }
        /// <summary>
        /// Bool that determines whether the field represents a m:n relationship
        /// </summary>
        public bool IsManyToMany { get; internal set; }
        /// <summary>
        /// Bool that determines whether the field can be null
        /// </summary>
        public bool IsNullable { get; internal set; } = true;
        /// <summary>
        /// Bool that determines whether the field is external
        /// </summary>
        public bool IsExternal { get; internal set; } = false;
        /// <summary>
        /// Bool that determines whether the field should be tagged as unique in the table creation process
        /// </summary>
        public bool IsUnique { get; internal set; } = false;
        /// <summary>
        /// Delete constraint used for foreign key column
        /// </summary>
        public FkConstraintType DeleteConstraint { get; internal set; } = FkConstraintType.Cascade;
        /// <summary>
        /// Sets the value of the field
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="val">New value of field</param>
        public void SetFieldValue(object obj, object val)
        {
            if (Member is not PropertyInfo info) 
                throw new ArgumentException("Not supported");
            info.SetValue(obj, val);
        }
        /// <summary>
        /// Returns value of a specific field
        /// </summary>
        /// <param name="obj">Field for which the value is needed</param>
        /// <returns>Value of the field</returns>
        public object GetFieldValue(object obj)
        {
            return Member is not PropertyInfo info
                ? throw new ArgumentException("Not supported")
                : info.GetValue(obj);
        }
        /// <summary>
        /// Converts a value to a suitable database type
        /// </summary>
        /// <param name="value"></param>
        /// 
        /// <returns></returns>
        public object ToColumnType(object value)
        {
            if (IsForeignKey)
            {
                return value == null ? null : Type.GetEntity().PrimaryKey.ToColumnType(Type.GetEntity().PrimaryKey.GetFieldValue(value));
            }
            if (value is Enum)
            {
                return Convert.ToInt32(value);
            }

            if (Type == ColumnType) { return value; }

            if(value is bool b)
            {
                if (ColumnType == typeof(int)) { return (b ? 1 : 0); }
                if (ColumnType == typeof(short)) { return (short)(b ? 1 : 0); }
                if (ColumnType == typeof(long)) { return (long)(b ? 1 : 0); }
            }

            return value;
        }
        /// <summary>
        /// Checks whether an array is null or empty. Used for converting database types to program types
        /// </summary>
        /// <typeparam name="T">Generic type of the array</typeparam>
        /// <param name="array">Array</param>
        /// <returns>True if array is null or empty. False otherwise</returns>
        public static bool IsNullOrEmpty<T>(T[] array) => array == null || array.Length == 0;
        /// <summary>
        /// Converts a database type to a suitable "program" type
        /// </summary>
        /// <param name="value">Database value</param>
        /// <returns>Value in a suitable type for the program.</returns>
        public object ToProgramType(object value)
        {
            if (IsForeignKey)
            {
                return value == null || IsNullOrEmpty((value.ToString() ?? string.Empty).ToArray())
                    ? null
                    : OrMapper.ReadWithPrimaryKey(Type, value);
            }

            if (Type == typeof(bool))
            {
                switch (value)
                {
                    case int i:
                        return (i != 0);
                    case short s:
                        return (s != 0);
                    case long l:
                        return (l != 0);
                }
            }

            if (Type == typeof(short)) return Convert.ToInt16(value);
            if (Type == typeof(int))  return Convert.ToInt32(value); 
            if (Type == typeof(long))  return Convert.ToInt64(value); 

            return Type.IsEnum ? Enum.ToObject(Type, value) : value;
        }
        /// <summary>
        /// Fills a field with data (primarily list fields)
        /// </summary>
        /// <param name="list">List to be filled</param>
        /// <param name="obj">Parent object of field</param>
        /// <returns>Filled list</returns>
        public object Fill(object list, object obj)
        {
            OrMapper.FillList(Type.GetGenericArguments()[0], list, this.ForeignKeyQuery(obj)); 
            return list;
        }
    }
}
