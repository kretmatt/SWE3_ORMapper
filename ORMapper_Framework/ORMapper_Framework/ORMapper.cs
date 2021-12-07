using ORMapper_Framework.DBHelperClasses;
using ORMapper_Framework.MetaModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ORMapper_Framework
{
    public static class ORMapper
    {
        private static Dictionary<Type, __Entity> _Entities = new Dictionary<Type, __Entity>();
        public static IDbConnection Database { get; set; }

        public static bool RegisterNewEntity<T>() where T : AEntity
        {
            Type t = typeof(T);

            if(!_Entities.ContainsKey(t))
            {
                _Entities.Add(t, new __Entity(t));
                return true;
            }

            return false;
        }

        public static void EnsureCreated()
        {
            _Entities.Values.ToList().DecodeEntities();
        }
    }
}
