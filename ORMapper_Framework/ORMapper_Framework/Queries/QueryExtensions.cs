using ORMapper_Framework.MetaModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapper_Framework.Queries
{
    /// <summary>
    /// Class for query extension methods. Extension methods are used for building the queries themselves.
    /// </summary>
    public static class QueryExtensions
    {
        /// <summary>
        /// Compares a database column with the specified value and checks if they are the same.
        /// </summary>
        /// <typeparam name="T">Target entity type</typeparam>
        /// <param name="query">Query object</param>
        /// <param name="columnName">Name of the database column</param>
        /// <param name="value">Value for comparison</param>
        /// <param name="not">Bool for "inversion" of the command (Equals -> NotEquals)</param>
        /// <returns>Updated query object</returns>
        public static Query<T> Equals<T>(this Query<T> query, string columnName, object value, bool not = false) where T : AEntity
        {

            if (not)
                return query.NotEquals(columnName, value);
    
            Field field = query.Entity.GetFieldByName(columnName);
            string sql = $" {field.ColumnName} = :p{query.ParameterCount}";
            query.QueryCommand.CommandText += sql;
            IDataParameter parameter = query.QueryCommand.CreateParameter();
            parameter.ParameterName = $":p{query.ParameterCount}";
            parameter.Value = field.ToColumnType(value);
            query.QueryCommand.Parameters.Add(parameter);
            query.IncreaseParameterCount();

            return query;
        }
        /// <summary>
        /// Compares a database column with the specified value and checks if they are not the same.
        /// </summary>
        /// <typeparam name="T">Target entity type</typeparam>
        /// <param name="query">Query object</param>
        /// <param name="columnName">Name of the database column</param>
        /// <param name="value">Value for comparison</param>
        /// <param name="not">"Inversion" parameter</param>
        /// <returns>Updated query object</returns>
        public static Query<T> NotEquals<T>(this Query<T> query, string columnName, object value, bool not = false) where T : AEntity
        {
            if (not)
                return query.Equals(columnName, value);

            Field field = query.Entity.GetFieldByName(columnName);
            string sql = $" {field.ColumnName} != :p{query.ParameterCount}";
            query.QueryCommand.CommandText += sql;
            IDataParameter parameter = query.QueryCommand.CreateParameter();
            parameter.ParameterName = $":p{query.ParameterCount}";
            parameter.Value = field.ToColumnType(value);
            query.QueryCommand.Parameters.Add(parameter);
            query.IncreaseParameterCount();

            return query;
        }
        /// <summary>
        /// Compares a database column with a value and checks if the row values are greater than the specified value.
        /// </summary>
        /// <typeparam name="T">Target entity type</typeparam>
        /// <param name="query">Query object</param>
        /// <param name="columnName">Name of the database column</param>
        /// <param name="value">Value for comparison</param>
        /// <param name="not">Inversion parameter</param>
        /// <returns>Updated query object</returns>
        public static Query<T> GreaterThan<T>(this Query<T> query, string columnName, object value, bool not = false) where T : AEntity
        {
            string cmpString = ">";

            if (not)
                cmpString = "<=";

            Field field = query.Entity.GetFieldByName(columnName);
            string sql = $" {field.ColumnName} {cmpString} :p{query.ParameterCount}";
            query.QueryCommand.CommandText += sql;
            IDataParameter parameter = query.QueryCommand.CreateParameter();
            parameter.ParameterName = $":p{query.ParameterCount}";
            parameter.Value = field.ToColumnType(value);
            query.QueryCommand.Parameters.Add(parameter);
            query.IncreaseParameterCount();

            return query;
        }
        /// <summary>
        /// Compares a database column with a value and checks if the row value is less than the specified value
        /// </summary>
        /// <typeparam name="T">Target entity type</typeparam>
        /// <param name="query">Query object</param>
        /// <param name="columnName">Name of the database column</param>
        /// <param name="value">Value for comparison</param>
        /// <param name="not">Inversion parameter</param>
        /// <returns>Updated query object</returns>
        public static Query<T> LessThan<T>(this Query<T> query, string columnName, object value, bool not = false) where T : AEntity
        {
            string cmpString = "<";

            if (not)
                cmpString = ">=";

            Field field = query.Entity.GetFieldByName(columnName);
            string sql = $" {field.ColumnName} {cmpString} :p{query.ParameterCount}";
            query.QueryCommand.CommandText += sql;
            IDataParameter parameter = query.QueryCommand.CreateParameter();
            parameter.ParameterName = $":p{query.ParameterCount}";
            parameter.Value = field.ToColumnType(value);
            query.QueryCommand.Parameters.Add(parameter);
            query.IncreaseParameterCount();

            return query;
        }
        /// <summary>
        /// Starts the WHERE part of the query
        /// </summary>
        /// <typeparam name="T">Target entity type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Updated query object</returns>
        public static Query<T> Where<T>(this Query<T> query) where T : AEntity
        {
            query.QueryCommand.CommandText += " WHERE";

            return query;
        }
        /// <summary>
        /// Inserts an AND into the query
        /// </summary>
        /// <typeparam name="T">Target entity type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Updated query object</returns>
        public static Query<T> And<T>(this Query<T> query) where T : AEntity
        {
            query.QueryCommand.CommandText += " AND";

            return query;
        }
        /// <summary>
        /// Inserts an OR into the query
        /// </summary>
        /// <typeparam name="T">Target entity type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Updated query object</returns>
        public static Query<T> Or<T>(this Query<T> query) where T : AEntity
        {
            query.QueryCommand.CommandText += " OR";

            return query;
        }
        /// <summary>
        /// Inserts a NOT into the query
        /// </summary>
        /// <typeparam name="T">Target entity type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Updated query object</returns>
        public static Query<T> Not<T>(this Query<T> query) where T : AEntity
        {
            query.QueryCommand.CommandText += " NOT";

            return query;
        }
        /// <summary>
        /// Begins a set (or group) -> Inserts a ( into the query.
        /// </summary>
        /// <typeparam name="T">Target entity type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Updated query object</returns>
        public static Query<T> BeginSet<T>(this Query<T> query) where T : AEntity
        {
            query.QueryCommand.CommandText += " (";

            return query;
        }
        /// <summary>
        /// Closes a set (or group) -> Inserts a ) into the query.
        /// </summary>
        /// <typeparam name="T">Target entity type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Updated query object</returns>
        public static Query<T> EndSet<T>(this Query<T> query) where T : AEntity
        {
            query.QueryCommand.CommandText += " )";

            return query;
        }
        /// <summary>
        /// Checks whether a certain database column is null
        /// </summary>
        /// <typeparam name="T">Target entity type</typeparam>
        /// <param name="query">Query object</param>
        /// <param name="columnName">Name of the database column</param>
        /// <param name="not">Inversion parameter</param>
        /// <returns>Updated query object</returns>
        public static Query<T> IsNull<T>(this Query<T> query, string columnName, bool not = false) where T : AEntity
        {
            string operatorString = not == true ? "IS NOT NULL" : "IS NULL";

            Field field = query.Entity.GetFieldByName(columnName);
            string sql = $" {field.ColumnName} {operatorString}";
            query.QueryCommand.CommandText += sql;

            return query;
        }
        /// <summary>
        /// Checks if values of database column is contained within the specified list of values
        /// </summary>
        /// <typeparam name="T">Target entity type</typeparam>
        /// <param name="query">Query object</param>
        /// <param name="columnName">Name of the database column</param>
        /// <param name="values">List with values used in IN statement</param>
        /// <param name="not">Inversion parameter</param>
        /// <returns>Updated query object</returns>
        public static Query<T> In<T>(this Query<T> query, string columnName, IEnumerable<object> values, bool not = false) where T : AEntity
        {
            string operatorString = not == true ? "NOT IN(" : "IN(";
            Field field = query.Entity.GetFieldByName(columnName);
            string sql = $" {field.ColumnName} {operatorString}";
            query.QueryCommand.CommandText += sql;

            int valCount = values.Count();

            foreach (var value in values)
            {
                valCount -= 1;
                IDataParameter parameter = query.QueryCommand.CreateParameter();
                parameter.ParameterName = $":p{query.ParameterCount}";
                query.QueryCommand.CommandText += $":p{query.ParameterCount}";
                if (valCount != 0)
                    query.QueryCommand.CommandText += ",";
                parameter.Value = field.ToColumnType(value);
                query.QueryCommand.Parameters.Add(parameter);
                query.IncreaseParameterCount();
            }

            query.QueryCommand.CommandText += ")";
            return query;
        }
        /// <summary>
        /// Checks if the values of a database column are between two specified values
        /// </summary>
        /// <typeparam name="T">Target entity type</typeparam>
        /// <param name="query">Query object</param>
        /// <param name="columnName">Name of the database column</param>
        /// <param name="value1">Low value</param>
        /// <param name="value2">High value</param>
        /// <param name="not">Inversion parameter</param>
        /// <returns>Updated query object</returns>
        public static Query<T> Between<T>(this Query<T> query, string columnName, object value1, object value2, bool not = false) where T : AEntity
        {
            string operationString = "BETWEEN";

            if (not)
                operationString = "NOT BETWEEN";

            Field field = query.Entity.GetFieldByName(columnName);

            string sql = $" {field.ColumnName} {operationString} :p{query.ParameterCount}";

            query.QueryCommand.CommandText += sql;
            IDataParameter parameter = query.QueryCommand.CreateParameter();
            parameter.ParameterName = $":p{query.ParameterCount}";
            parameter.Value = field.ToColumnType(value1);
            query.QueryCommand.Parameters.Add(parameter);
            query.IncreaseParameterCount();

            query.QueryCommand.CommandText += $" AND :p{query.ParameterCount}";
            parameter = query.QueryCommand.CreateParameter();
            parameter.ParameterName = $":p{query.ParameterCount}";
            parameter.Value = field.ToColumnType(value2);
            query.QueryCommand.Parameters.Add(parameter);
            query.IncreaseParameterCount();

            return query;
        }
        /// <summary>
        /// Checks if values of a database column fulfill a specified string pattern
        /// </summary>
        /// <typeparam name="T">Target entity type</typeparam>
        /// <param name="query">Query object</param>
        /// <param name="columnName">Name of the database column</param>
        /// <param name="pattern">String pattern</param>
        /// <param name="caseSensitive">Parameter for activating case sensitivity</param>
        /// <param name="not">Inversion parameter</param>
        /// <returns>Updated query object</returns>
        public static Query<T> Like<T>(this Query<T> query, string columnName, string pattern, bool caseSensitive = false, bool not = false) where T : AEntity
        {
            string operationString = "LIKE";

            if (not)
                operationString = "NOT LIKE";

            Field field = query.Entity.GetFieldByName(columnName);

            var sql = caseSensitive
                ? $" {field.ColumnName} {operationString} :p{query.ParameterCount}"
                : $" LOWER({field.ColumnName}) {operationString} LOWER(:p{query.ParameterCount})";

            query.QueryCommand.CommandText += sql;
            IDataParameter parameter = query.QueryCommand.CreateParameter();
            parameter.ParameterName = $":p{query.ParameterCount}";
            parameter.Value = field.ToColumnType(pattern);
            query.QueryCommand.Parameters.Add(parameter);
            query.IncreaseParameterCount();

            return query;
        }
    }
}
