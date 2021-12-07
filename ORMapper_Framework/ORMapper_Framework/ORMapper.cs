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
            bool result = true;
            List<__Entity> entities = new List<__Entity>() { new __Entity(t) };
            entities = entities.DecodeEntities();
            foreach (__Entity e in entities)
            {
                if (!_Entities.ContainsKey(e.Member))
                {
                    _Entities.Add(e.Member, e);
                }
                else
                    result = false;
            }
            return result;
        }

        public static void EnsureCreated()
        {
            _Entities.Values.ToList().ForEach(e => Console.WriteLine(e.TableName+" "+e.IsDerived));
        }
    }
}
