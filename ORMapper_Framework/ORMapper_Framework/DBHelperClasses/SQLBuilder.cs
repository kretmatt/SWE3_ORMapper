using ORMapper_Framework.MetaModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ORMapper_Framework.Enums;

namespace ORMapper_Framework.DBHelperClasses
{
    /// <summary>
    /// Class with methods for building sql statements and database commands
    /// </summary>
    internal static class SqlBuilder
    {
        /// <summary>
        /// Creates database structure
        /// </summary>
        /// <returns>List of sql create statement string</returns>
        public static List<string> Create()
        {
            List<string> createStatements = new List<string>();
            const string emptyText = "";
            const string notNullable = "NOT NULL";
            const string primaryKey = "PRIMARY KEY";
            const string unique = "UNIQUE";
            string columnType = "";
            Type colType;
            StringBuilder sb = new StringBuilder();

            OrMapper.GetEntitiesData().ForEach(e =>
            {
                sb.Append($"CREATE TABLE IF NOT EXISTS {e.TableName} ( \n");
                
                foreach(Field i in e.Internals)
                {
                    // Skip inherited fields except id
                    if (e.InheritedField(i.ColumnName))
                    {
                        continue;
                    }
                    // Get database type string 
                    columnType = i.GetDbTypeString();
                    // Avoid lists - Check already done in above line (GetDBTypeString)
                    if (!string.IsNullOrEmpty(columnType))
                    {
                        sb.Append($"{i.ColumnName} {columnType} {(i.IsPrimaryKey ? primaryKey : emptyText)} {(!i.IsNullable ? notNullable : emptyText)} {(i.IsUnique ? unique : emptyText)},\n");
                    }
                    columnType = "";
                }

                if (e.IsDerived)
                {
                    // Set primary key constraint to parent table. ON DELETE CASCADE to remove children if parent gets deleted
                    Type baseType = e.Member.BaseType;
                    sb.Append($"CONSTRAINT fk_{e.TableName}_{baseType.GetEntityPkName()} FOREIGN KEY({e.PrimaryKey.ColumnName}) REFERENCES {baseType.GetEntityTableName()}({baseType.GetEntityPkName()}) ON DELETE CASCADE,\n");
                }

                foreach (Field i in e.Internals.Where(intObj => intObj.ColumnType.IsEntity() && !e.InheritedField(intObj.ColumnName)).ToList())
                {
                    colType = i.ColumnType;
                    sb.Append($"CONSTRAINT fk_{e.TableName}_{i.ColumnName} FOREIGN KEY({i.ColumnName}) REFERENCES {colType.GetEntityTableName()}({colType.GetEntityPkName()}) ON DELETE {i.DeleteConstraint.GetDescription()},\n");
                }

                //Remove ,\n from last row
                sb.Length-=2;
                sb.Append("\n);");
                createStatements.Add(sb.ToString());
                sb.Clear();
            });

            OrMapper.GetEntitiesData().ForEach(e =>
            {
                foreach (Field i in e.Externals)
                {
                    colType = i.ColumnType;

                    if (!i.IsManyToMany || e.InheritedField(i.ColumnName)) continue;

                    Type remColType = i.ColumnType;

                    if (i.ColumnType.IsListType())
                        remColType = i.ColumnType.GetGenericArguments()[0];

                    sb.Append($"CREATE TABLE IF NOT EXISTS {i.AssignmentTable}(\n");
                    sb.Append($"{i.ColumnName} {e.PrimaryKey.GetDbTypeString()} NOT NULL,\n");
                    sb.Append($"{i.RemoteColumnName} {remColType.GetEntityDbTypeString()} NOT NULL,\n");
                    sb.Append($"PRIMARY KEY({i.ColumnName}, {i.RemoteColumnName}),\n");
                    sb.Append($"CONSTRAINT fk_{i.AssignmentTable}_{e.TableName}_assignment FOREIGN KEY({i.ColumnName}) REFERENCES {e.TableName}({e.PrimaryKey.ColumnName}) ON DELETE CASCADE,\n");
                    sb.Append($"CONSTRAINT fk_{i.AssignmentTable}_{remColType.GetEntityTableName()}_assignment FOREIGN KEY({i.RemoteColumnName}) REFERENCES {remColType.GetEntityTableName()}({remColType.GetEntityPkName()}) ON DELETE CASCADE\n");
                    sb.Append(");");
                    createStatements.Add(sb.ToString());
                    sb.Clear();
                }
            });

            return createStatements;
        }
        /// <summary>
        /// Drops database tables. Every entity table that needs to be dropped has to be registered beforehand.
        /// </summary>
        /// <returns>List of database commands for dropping the registered tables</returns>
        public static List<IDbCommand> Drops()
        {
            List<IDbCommand> commands = new List<IDbCommand>();

            OrMapper.GetEntitiesData().ForEach(e =>
            {
                foreach (Field i in e.Externals)
                {

                    if (!i.IsManyToMany || e.InheritedField(i.ColumnName)) continue;

                    IDbCommand dropAssignmentTable = OrMapper.Database.CreateCommand();
                    dropAssignmentTable.CommandText = $"DROP TABLE IF EXISTS {i.AssignmentTable};";
                    commands.Add(dropAssignmentTable);
                }

            });

            var entities = OrMapper.GetEntitiesData();
            entities.Reverse();

            entities.ForEach(e =>
            {
                IDbCommand dropTable = OrMapper.Database.CreateCommand();
                dropTable.CommandText = $"DROP TABLE IF EXISTS {e.TableName};";
                commands.Add(dropTable);
            });

            return commands;
        }


        /// <summary>
        /// Inserts / updates an object in the database
        /// </summary>
        /// <param name="command">Insert/Update command</param>
        /// <param name="obj">Object to be inserted / updated</param>
        /// <param name="entity">Associated entity</param>
        /// <returns>Command for inserting / updating an object</returns>
        public static IDbCommand Save(IDbCommand command, object obj, Entity entity)
        {
            string update = "";
            string insert = "";
       
            command.CommandText = ("INSERT INTO " + entity.TableName + "(");

            update = $"ON CONFLICT ({entity.PrimaryKey.ColumnName}) DO UPDATE SET ";

            int fieldCount = entity.Internals.Count(i=>!entity.InheritedField(i.ColumnName));
            int updateFieldCount = fieldCount - 1;
            foreach (Field i in entity.Internals)
            {
                
                // Skip inherited fields except id
                if (entity.InheritedField(i.ColumnName))
                {
                    continue;
                }
                fieldCount -= 1;
                command.CommandText += i.ColumnName;

                insert += (":i" + i.ColumnName);

                IDataParameter dataParameter = command.CreateParameter();
                dataParameter.ParameterName = (":i" + i.ColumnName);
                dataParameter.Value = i.ToColumnType(i.GetFieldValue(obj));
                command.Parameters.Add(dataParameter);

                if(!i.IsPrimaryKey)
                {
                    // Update string - Excluded table is the row that was not inserted due to conflict.
                    update += $"{i.ColumnName}=EXCLUDED.{i.ColumnName}";
                    updateFieldCount -= 1;
                    if(updateFieldCount>0)
                        update += ",";
                }

                if (fieldCount <= 0) continue;

                command.CommandText += ",";
                insert += ",";

            }

            update += ";";
            // If there is only a primary key, only insert
            if (!(entity.Internals.Length == 1 && entity.Internals[0].IsPrimaryKey))
                command.CommandText += $") VALUES ({insert}) {update}";
            else
                command.CommandText += $") VALUES ({insert});"; // if there is only an id, it will only be inserted

            return command;
        }
        /// <summary>
        /// Updates the references of an object (field)
        /// </summary>
        /// <param name="field">Field for which the references need to be updated</param>
        /// <param name="obj">Object for which the references need to be updated</param>
        /// <returns>Commands for updating the references of an object</returns>
        public static List<IDbCommand> BuildUpdateReferenceCommands(this Field field, object obj)
        {
            List<IDbCommand> commands = new List<IDbCommand>();

            if (!field.IsExternal || field.GetFieldValue(obj) == null)
                return commands;

            Type listType = field.Type.GetGenericArguments()[0];
            Entity innerEntity = listType.GetEntity();
            object primaryKey = field.Entity.PrimaryKey.ToColumnType(field.Entity.PrimaryKey.GetFieldValue(obj));
            IDbCommand command;
            IDataParameter par;

            if (field.IsManyToMany)
            {
                command = OrMapper.Database.CreateCommand();
                command.CommandText = $"DELETE FROM {field.AssignmentTable} WHERE {field.ColumnName} = :pk";
                par = command.CreateParameter();
                par.ParameterName = ":pk";
                par.Value = primaryKey;
                command.Parameters.Add(par);

                commands.Add(command);

                foreach(object o in (IEnumerable)field.GetFieldValue(obj))
                {
                    command = OrMapper.Database.CreateCommand();
                    command.CommandText = $"INSERT INTO {field.AssignmentTable}({field.ColumnName},{field.RemoteColumnName}) VALUES (:pk,:fk);";
                    par = command.CreateParameter();
                    par.ParameterName = ":pk";
                    par.Value = primaryKey;
                    command.Parameters.Add(par);

                    par = command.CreateParameter();
                    par.ParameterName = ":fk";
                    par.Value = innerEntity.PrimaryKey.ToColumnType(innerEntity.PrimaryKey.GetFieldValue(o));
                    command.Parameters.Add(par);

                    commands.Add(command);
                }
            }
            else
            {
                Field remoteField = innerEntity.GetFieldByName(field.ColumnName);


                if (remoteField == null) return commands;

                

                foreach(object o in (IEnumerable)field.GetFieldValue(obj))
                {
                    if (remoteField.IsNullable)
                    {
                        command = OrMapper.Database.CreateCommand();
                        command.CommandText = $"UPDATE {innerEntity.TableName} SET {field.ColumnName} = NULL WHERE {innerEntity.PrimaryKey.ColumnName} = :pk;";
                        par = command.CreateParameter();
                        par.ParameterName = ":pk";
                        par.Value = innerEntity.PrimaryKey.ToColumnType(innerEntity.PrimaryKey.GetFieldValue(o));
                        command.Parameters.Add(par);
                        commands.Add(command);
                    }

                    remoteField.SetFieldValue(o, obj);

                    command = OrMapper.Database.CreateCommand();
                    command.CommandText = $"UPDATE {innerEntity.TableName} SET {field.ColumnName} = :fk WHERE {innerEntity.PrimaryKey.ColumnName} = :pk;";

                    par = command.CreateParameter();
                    par.ParameterName = ":fk";
                    par.Value = primaryKey;
                    command.Parameters.Add(par);

                    IDataParameter par2 = command.CreateParameter();
                    par2.ParameterName = ":pk";
                    par2.Value = innerEntity.PrimaryKey.ToColumnType(innerEntity.PrimaryKey.GetFieldValue(o));
                    command.Parameters.Add(par2);

                    commands.Add(command);
                }
            }


            return commands;
        }
        /// <summary>
        /// Deletes an object in the database
        /// </summary>
        /// <param name="obj">Object to be deleted</param>
        /// <param name="entity">Entity of the object</param>
        /// <returns>Command for deleting an object in the database</returns>
        public static IDbCommand Delete(object obj, Entity entity)
        {
            IDbCommand command = OrMapper.Database.CreateCommand();
            command.CommandText = $"DELETE FROM {entity.TableName} WHERE {entity.PrimaryKey.ColumnName} = :pk;";
            IDataParameter par = command.CreateParameter();
            par.ParameterName = ":pk";
            par.Value = entity.PrimaryKey.ToColumnType(entity.PrimaryKey.GetFieldValue(obj));
            command.Parameters.Add(par);
            return command;
        }
        /// <summary>
        /// Starts the transaction
        /// </summary>
        /// <returns>Command for starting the transaction</returns>
        public static IDbCommand StartTransaction()
        {
            IDbCommand command = OrMapper.Database.CreateCommand();
            command.CommandText = "BEGIN;";
            return command;
        }
        /// <summary>
        /// Commits the transaction
        /// </summary>
        /// <returns>Command for committing the transaction</returns>
        public static IDbCommand CommitTransaction()
        {
            IDbCommand command = OrMapper.Database.CreateCommand();
            command.CommandText = "COMMIT;";
            return command;
        }
        /// <summary>
        /// Rolls back the transaction
        /// </summary>
        /// <returns>Command for rolling back the transaction</returns>
        public static IDbCommand RollbackTransaction()
        {
            IDbCommand command = OrMapper.Database.CreateCommand();
            command.CommandText = "ROLLBACK;";
            return command;
        }
        /// <summary>
        /// Locks object in postgres database
        /// </summary>
        /// <param name="obj">Object that should get locked</param>
        /// <param name="entity"></param>
        /// <param name="lockType">Lock type</param>
        /// <returns>Database commands for locking row associated with object (+ rows in parent tables)</returns>
        public static List<IDbCommand> LockObject(object obj, Entity entity, LockType lockType = LockType.ForUpdate)
        {
            List<IDbCommand> commands = new List<IDbCommand>();

            if (entity.IsDerived)
            {
                commands.AddRange(LockObject(obj, entity.Member.BaseType.GetEntity(), lockType));
            }

            IDbCommand command = OrMapper.Database.CreateCommand();
            command.CommandText = $"SELECT * FROM {entity.TableName} WHERE {entity.PrimaryKey.ColumnName} = :pk {lockType.GetDescription()};";
            IDataParameter par = command.CreateParameter();
            par.ParameterName = ":pk";
            par.Value = entity.PrimaryKey.ToColumnType(entity.PrimaryKey.GetFieldValue(obj));
            command.Parameters.Add(par);
            commands.Add(command);
            return commands;
        }
        /// <summary>
        /// Builds the base sql statement for an entity
        /// </summary>
        /// <param name="entity">Entity, for which the base sql statement needs to be created</param>
        /// <returns>SQL string with base query of entity</returns>
        public static string GetBaseQuery(this Entity entity)
        {

            StringBuilder selectBuilder = new StringBuilder();
            selectBuilder.Append("SELECT ");

            selectBuilder.Append(entity.PrimaryKey.ColumnName + ",");

            foreach (Field f in entity.Internals)
                if (!f.IsPrimaryKey)
                    selectBuilder.Append($"{f.ColumnName},");

            selectBuilder.Length -= 1;

            selectBuilder.Append(" FROM " + entity.TableName);

            Entity currentEntity = entity;

            // Select data from parent-tables
            while (currentEntity.IsDerived)
            {
                currentEntity = currentEntity.Member.BaseType.GetEntity();
                selectBuilder.Append($" INNER JOIN {currentEntity.TableName} USING ({currentEntity.PrimaryKey.ColumnName}) ");
            }

            return selectBuilder.ToString();
        }
        /// <summary>
        /// Builds foreign key sql
        /// </summary>
        /// <param name="field">Field for which the foreign key sql statement gets created</param>
        /// <param name="obj">Object</param>
        /// <returns>Command with foreign key sql statement</returns>
        public static IDbCommand ForeignKeyQuery(this Field field, object obj)
        {
            if (!field.IsForeignKey) return null;

            Entity entity = field.Type.GenericTypeArguments[0].GetEntity();

            IDbCommand command = OrMapper.Database.CreateCommand();
            command.CommandText = field.IsManyToMany
                ? $"{entity.GetBaseQuery()} WHERE {entity.PrimaryKey.ColumnName} IN (SELECT {field.RemoteColumnName} FROM {field.AssignmentTable} WHERE {field.ColumnName} = :fk)"
                : $"{entity.GetBaseQuery()} WHERE {field.ColumnName} = :fk";
            IDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = ":fk";
            parameter.Value = field.Entity.PrimaryKey.GetFieldValue(obj);
            command.Parameters.Add(parameter);

            return command;
        }
    }
}
