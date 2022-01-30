using ORMapper_Framework.Cache;
using ORMapper_Framework.DBHelperClasses;
using ORMapper_Framework.DBHelperClasses.DBTypeMapper;
using ORMapper_Framework.MetaModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ORMapper_Framework.Enums;

namespace ORMapper_Framework
{
    /// <summary>
    /// ORMapper is the class that manages database access, methods for accessing data and transactions.
    /// </summary>
    public static class OrMapper
    {
        /// <summary>
        /// Dictionary of all registered entities
        /// </summary>
        private static readonly Dictionary<Type, Entity> Entities = new Dictionary<Type, Entity>();
        /// <summary>
        /// Connection to the database
        /// </summary>
        public static IDbConnection Database { get; set; }
        /// <summary>
        /// Cache of the ORMapper class
        /// </summary>
        internal static ICache OrMapperCache { get; set; } = new OrCache();
        /// <summary>
        /// Determines whether ORMapper currently is in a transaction or not
        /// </summary>
        public static bool InTransaction { get; set; } = false;

        // =================== Transactions and Locking ===================

        /// <summary>
        /// Starts the transaction
        /// </summary>
        public static void StartTransaction()
        {
            try
            {
                IDbCommand command = SqlBuilder.StartTransaction();
                command.ExecuteNonQuery();
                command.Dispose();
                InTransaction = true;
            }
            catch (Exception e)
            {
                throw new OrMapperDatabaseException("Transaction could not be started", e);
            }
        }
        /// <summary>
        /// Commit the commands of the transaction
        /// </summary>
        public static void CommitTransaction()
        {
            try
            {
                IDbCommand command = SqlBuilder.CommitTransaction();
                command.ExecuteNonQuery();
                command.Dispose();
                InTransaction = false;
                OrMapper.ClearCache();
            }
            catch (Exception e)
            {
                throw new OrMapperDatabaseException("Changes could not be committed", e);
            }

        }
        /// <summary>
        /// Rolls back all changes of current transaction
        /// </summary>
        public static void RollbackTransaction()
        {
            try
            {
                IDbCommand command = SqlBuilder.RollbackTransaction();
                command.ExecuteNonQuery();
                command.Dispose();
                InTransaction = false;
                OrMapper.ClearCache();
            }
            catch (Exception e)
            {
                throw new OrMapperDatabaseException("Changes could not be rolled back", e);
            }

        }
        /// <summary>
        /// Locks an object in the database (row level lock)
        /// </summary>
        /// <param name="obj">Object to be locked</param>
        /// <param name="lockType">Lock type (FOR SHARE, FOR UPDATE, FOR KEY SHARE, FOR NO KEY UPDATE)</param>
        public static void LockObject(object obj, LockType lockType = LockType.ForUpdate)
        {
            Entity entity = obj.GetEntity();
            SqlBuilder.LockObject(obj, entity, lockType).ForEach(command =>
            {
                try
                {
                    command.ExecuteNonQuery();
                    command.Dispose();
                }
                catch (Exception e)
                {
                    throw new OrMapperDatabaseException($"Object could not be locked ({lockType})", e);
                }

            });
        }

        // =================== Extension methods ===================

        /// <summary>
        /// Checks if a type is already registered as an entity (and if it is derived from the abstract AEntity class)
        /// </summary>
        /// <param name="type">Type for which the check is conducted</param>
        /// <returns>True if type is registered and derived from AEntity. Otherwise it returns false.</returns>
        internal static bool IsEntity(this Type type)
        {
            return typeof(AEntity).IsAssignableFrom(type) && Entities.ContainsKey(type);
        }

        /// <summary>
        /// Returns the database type for a specific field
        /// </summary>
        /// <param name="field">Field for which the database type is needed</param>
        /// <returns>Database type string. Empty string if type can not be mapped</returns>
        internal static string GetDbTypeString(this Field field)
        {
            if (field.ColumnType.IsEntity())
                return field.ColumnType.GetEntityDbTypeString();
            if (!field.ColumnType.IsListType())
                return string.IsNullOrEmpty(field.DbType) ? DbTypeMapper.MapType(field.ColumnType) : field.DbType;
            return "";
        }

        /// <summary>
        /// Returns the database type for an entity class (PK)
        /// </summary>
        /// <param name="type">Type for which the database type is needed</param>
        /// <returns>Database type string. Empty string if type can not be mapped</returns>
        internal static string GetEntityDbTypeString(this Type type)
        {
            return Entities.ContainsKey(type)
                ? Entities[type].PrimaryKey.GetDbTypeString()
                : "";
        }

        /// <summary>
        /// Returns true or false depending on whether the specified field is inherited
        /// </summary>
        /// <param name="entity">Entity which needs to be checked</param>
        /// <param name="fieldName">Field of entity that needs to be checked if it is inherited</param>
        /// <returns></returns>
        internal static bool InheritedField(this Entity entity, string fieldName)
        {
            return entity?.Member.BaseType != null && entity.IsDerived &&
                   Entities[entity.Member.BaseType].Fields.Any(f => f.ColumnName == fieldName) &&
                   entity.PrimaryKey.ColumnName != fieldName;
        }

        /// <summary>
        /// Checks whether the specified type is a list
        /// </summary>
        /// <param name="type">Type that needs to be checked</param>
        /// <returns>True if type is a list, false otherwise</returns>
        internal static bool IsListType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        /// <summary>
        /// Returns the table name of an entity
        /// </summary>
        /// <param name="type">Type for which the table name is needed</param>
        /// <returns>Table name. Empty string if type is not registered as entity</returns>
        internal static string GetEntityTableName(this Type type)
        {
            return Entities.ContainsKey(type)
                ? Entities[type].TableName
                : "";
        }

        /// <summary>
        /// Returns the name of the primary key of a type
        /// </summary>
        /// <param name="type">Type for which the primary key name is needed</param>
        /// <returns>Primary key name. Empty string if type is not registered as entity</returns>
        internal static string GetEntityPkName(this Type type)
        {
            return Entities.ContainsKey(type)
                ? Entities[type].PrimaryKey.ColumnName
                : "";
        }

        /// <summary>
        /// Returns the entity definition for an object
        /// </summary>
        /// <param name="o">Object for which the entity definition is needed</param>
        /// <returns>Entity definition for the specified object</returns>
        internal static Entity GetEntity(this object o)
        {
            Type t = ((o is Type type) ? type : o.GetType());

            if (!Entities.ContainsKey(t))
            {
                RegisterNewEntity(t);
            }

            return Entities[t];
        }

        // =================== Public & internal methods ===================

        public static void ClearCache()
        {
            OrMapperCache.ClearCache();
            OrMapperCache.ClearTempCache();
        }

        /// <summary>
        /// Returns all registered entity objects
        /// </summary>
        /// <returns>All registered entity objects</returns>
        internal static List<Entity> GetEntitiesData()
        {
            return Entities.Values.ToList();
        }

        /// <summary>
        /// Registers a new entity in the ORMapper
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        public static void RegisterNewEntity<T>() where T : AEntity
        {
            Type t = typeof(T);
            List<Entity> entities = new Entity(t).UniqueDecodeEntities();
            foreach (var e in entities.Where(e => !Entities.ContainsKey(e.Member)))
            {
                Entities.Add(e.Member, e);
            }
        }
        /// <summary>
        /// Registers a new entity in the ORMapper
        /// </summary>
        /// <param name="t"></param>
        public static void RegisterNewEntity(Type t)
        {
            List<Entity> entities = new Entity(t).UniqueDecodeEntities();
            foreach (var e in entities.Where(e => !Entities.ContainsKey(e.Member)))
            {
                Entities.Add(e.Member, e);
            }
        }
        /// <summary>
        /// Create all tables in the database if they do not already exist
        /// </summary>
        public static void EnsureCreated()
        {
            // Generate create sql statements for every registered entity
            List<string> order = SqlBuilder.Create();
            SqlBuilder.Create().ForEach(s =>
            {
                try
                {
                    IDbCommand createCommand = Database.CreateCommand();
                    createCommand.CommandText = s;
                    createCommand.ExecuteNonQuery();
                    createCommand.Dispose();
                }
                catch (Exception e)
                {
                    throw new OrMapperDatabaseException($"Table creation failed. Quitting table creation process", e);
                }

            });
        }

        public static void EnsureDeleted()
        {
            SqlBuilder.Drops().ForEach(command =>
            {
                try
                {
                    command.ExecuteNonQuery();
                    command.Dispose();
                }
                catch (Exception e)
                {
                    throw new OrMapperDatabaseException($"Table drop failed. Quitting table drop process", e);
                }
            });
        }

        /// <summary>
        /// Saves an object to the database (Recursive for inheritance)
        /// </summary>
        /// <param name="obj">Object to be saved to the database</param>
        /// <param name="type">Type of the object</param>
        public static void Save(object obj, Type type = null)
        {
            if (obj == null) return;

            Entity entity = type == null ? obj.GetEntity() : type.GetEntity();

            if (entity.IsDerived)
            {
                Type baseType = entity.Member.BaseType;

                if (baseType.IsEntity() && baseType != typeof(AEntity))
                {
                    Save(obj, baseType);
                }
                        
            }

            SaveEntityData(obj, entity);
        }

        /// <summary>
        /// Saves an object to the database (actually executes the commands)
        /// </summary>
        /// <param name="obj">Object to be saved to the database</param>
        /// <param name="entity">Entity definition for the object</param>
        internal static void SaveEntityData(object obj, Entity entity)
        {
            if (OrMapperCache != null)
                if (!OrMapperCache.ObjectChanged(obj, entity.Member))
                    return;

            IDbCommand command = SqlBuilder.Save(Database.CreateCommand(), obj, entity);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new OrMapperDatabaseException(
                    $"Object of type {entity.Member.ToString()} could not be saved to the database", e);
            }

            
            foreach(Field e in entity.Externals)
            {
                List<IDbCommand> commands = e.BuildUpdateReferenceCommands(obj);
                commands.ForEach(c =>
                {
                    try
                    {
                        c.ExecuteNonQuery();
                        c.Dispose();
                    }
                    catch (Exception e)
                    {
                        throw new OrMapperDatabaseException("References of object could not be updated", e);
                    }

                });
            }
            command.Dispose();

            OrMapperCache?.Put(obj, entity.Member);
        }

        /// <summary>
        /// Deletes an object in the database
        /// </summary>
        /// <param name="obj">Object to be deleted in the database</param>
        public static void Delete(object obj)
        {
            Entity entity = obj.GetEntity();
            IDbCommand command = SqlBuilder.Delete(obj, entity);

            try
            {
                command.ExecuteNonQuery();
                command.Dispose();
            }
            catch (Exception e)
            {
                throw new OrMapperDatabaseException($"Object of type {entity.Member.ToString()} could not be deleted",
                    e);
            }


            OrMapperCache?.Delete(obj);
        }

        /// <summary>
        /// Retrieves an object from the database
        /// </summary>
        /// <typeparam name="T">Type of the wanted object</typeparam>
        /// <param name="primaryKey">Primary key of the object</param>
        /// <returns>Retrieved object. Null if the corresponding object could not be found</returns>
        public static T Read<T>(object primaryKey)
        {
            T result = (T) ReadWithPrimaryKey(typeof(T), primaryKey);
            OrMapperCache.ClearTempCache();
            return result;
        }
        /// <summary>
        /// Retrieves an object through its primary key
        /// </summary>
        /// <param name="t">Type of the object</param>
        /// <param name="primaryKey">Primary key of the wanted object</param>
        /// <returns>Null if no corresponding could be found. On success, the wanted object will be returned</returns>
        internal static object ReadWithPrimaryKey(Type t, object primaryKey)
        {

            object result = null;

            IDbCommand command = Database.CreateCommand();

            Entity entity = t.GetEntity();
            command.CommandText = $"{entity.GetBaseQuery()} WHERE {entity.PrimaryKey.ColumnName} = :pk";

            IDataParameter par = command.CreateParameter();
            par.ParameterName = ":pk";
            par.Value = primaryKey;
            command.Parameters.Add(par);

            try
            {
                IDataReader reader = command.ExecuteReader();
                Dictionary<string, object> queryValues = DataReaderToDictionary(reader, entity);
                reader.Close();
                command.Dispose();
                result = ReadFromDictionary(t, queryValues);
            }
            catch (Exception e)
            {
                throw new OrMapperDatabaseException($"Object with PK {primaryKey} could not be read", e);
            }

            OrMapperCache?.Put(result, entity.Member);

            return result;
        }
        /// <summary>
        /// Converts a data results to a dictionary (to prevent multiple readers being executed at the same time)
        /// </summary>
        /// <param name="dataReader">Data results that needs to be converted</param>
        /// <param name="entity">Entity definition of the results</param>
        /// <returns>Dictionary object with results</returns>
        internal static Dictionary<string, object> DataReaderToDictionary(IDataReader dataReader, Entity entity)
        {
            Dictionary<string, object> columnValuePairs = new();
            
            if (!dataReader.Read()) return columnValuePairs;

            foreach (Field modelField in entity.Internals)
            {
                columnValuePairs.Add(modelField.ColumnName, dataReader.GetValue(dataReader.GetOrdinal(modelField.ColumnName)));
            }
            return columnValuePairs;
        }
        /// <summary>
        /// Returns the object inside the specified dictionary
        /// </summary>
        /// <param name="t">Type of the object</param>
        /// <param name="results">Results dictionary from a query</param>
        /// <returns>Object inside the dictionary or from the cache</returns>
        internal static object ReadFromDictionary(Type t, Dictionary<string, object> results)
        {

            Entity entity = t.GetEntity();
            object result = SearchCache(t, entity.PrimaryKey.ToProgramType(results[entity.PrimaryKey.ColumnName]));

            if (result != null) return result;

            OrMapperCache.PutTempCache(result = Activator.CreateInstance(t));

            entity.PrimaryKey.SetFieldValue(result, entity.PrimaryKey.ToProgramType(results[entity.PrimaryKey.ColumnName]));

            foreach (Field i in entity.Internals.Where(ei => !ei.IsPrimaryKey).ToList())
            {
                i.SetFieldValue(result, i.ToProgramType(results[i.ColumnName]));
            }

            foreach (Field e in entity.Externals)
            {
                e.SetFieldValue(result, e.Fill(Activator.CreateInstance(e.Type), result));
            }
            OrMapperCache.Put(result, entity.Member);

            return result;
        }

        /// <summary>
        /// Fills a list of objects with the data of a list of dictionaries
        /// </summary>
        /// <param name="t">Type of the objects</param>
        /// <param name="list">List to be filled</param>
        /// <param name="resultsList">Dictionary list with the data</param>
        internal static void FillListFromDictionaries(Type t, object list, List<Dictionary<string, object>> resultsList)
        {
            foreach(var dict in resultsList)
            {
                list.GetType().GetMethod("Add")?.Invoke(list, new object[] { ReadFromDictionary(t, dict)});
            }
        }

        /// <summary>
        /// Fills a list with the results of a command
        /// </summary>
        /// <param name="t">Type of the objects</param>
        /// <param name="list">List to be filled</param>
        /// <param name="command">Database command</param>
        internal static void FillList(Type t, object list, IDbCommand command)
        {

            Entity entity = t.GetEntity();
            IDataReader reader = null;

            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception e)
            {
                throw new OrMapperDatabaseException("Query for list objects was not successful", e);
            }

            List<Dictionary<string, object>> results = new();
            Dictionary<string, object> row = null;

            do
            {
                row = DataReaderToDictionary(reader, entity);
                if (row.Count > 0)
                    results.Add(row);
            } while (row.Count > 0);
            reader.Close();
            FillListFromDictionaries(t, list, results);
            reader.Dispose();
            command.Dispose();
        }

        /// <summary>
        /// Searches the ORMapper cache for an object with a specific primary key.
        /// </summary>
        /// <param name="t">Type of the object</param>
        /// <param name="primaryKey">Primary key of the object</param>
        /// <returns>Null if no object with specified primary key can be found. Otherwise the wanted object will be returned.</returns>
        internal static object SearchCache(Type t, object primaryKey)
        {
            if ((OrMapperCache != null) && OrMapperCache.Contains(t, primaryKey))
                return OrMapperCache.Get(t, primaryKey);

            return OrMapperCache?.GetTempCache(t, primaryKey);
        }
    }
}
