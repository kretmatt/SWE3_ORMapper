using ORMapper_Framework.DBHelperClasses;
using ORMapper_Framework.MetaModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapper_Framework.Queries
{
    /// <summary>
    /// Generic class for creating queries for different entities
    /// </summary>
    /// <typeparam name="T">Target entity</typeparam>
    public class Query<T> where T : AEntity
    {
        /// <summary>
        /// Command behind the query.
        /// </summary>
        public IDbCommand QueryCommand { get; private set; }
        /// <summary>
        /// Type of the query "target"
        /// </summary>
        private readonly Type _type;
        /// <summary>
        /// Entity for the query "target" type
        /// </summary>
        internal Entity Entity;
        /// <summary>
        /// Current amount of parameters in the query
        /// </summary>
        public int ParameterCount { get; private set; }
        /// <summary>
        /// Results of the query
        /// </summary>
        private readonly List<T> _results;
        /// <summary>
        /// Constructor for initializing the query
        /// </summary>
        public Query()
        {
            _type = typeof(T);
            Entity = _type.GetEntity();
            QueryCommand = OrMapper.Database.CreateCommand();
            QueryCommand.CommandText = $"{Entity.GetBaseQuery()}";
            _results = new List<T>();
        }
        /// <summary>
        /// Method for increasing parameter count
        /// </summary>
        public void IncreaseParameterCount() => ParameterCount += 1;
        /// <summary>
        /// Executes the query and its core IDbCommand object
        /// </summary>
        /// <param name="redo">Parameter for ignoring cache</param>
        /// <returns>Results of the query</returns>
        public List<T> Execute(bool redo = false)
        {
            if (redo == false && _results.Count != 0) return _results;
            OrMapper.FillList(_type, _results, QueryCommand);
            OrMapper.OrMapperCache.ClearTempCache();
            return _results;
        }
    }

}
