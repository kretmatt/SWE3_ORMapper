using ORMapper_Framework.MetaModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapper_Framework
{
    public static class ORMapper
    {
        private static Dictionary<Type, __Entity> _Entities = new Dictionary<Type, __Entity>();

        public static IDbConnection Database { get; set; }

        public static bool RegisterNewEntity<T>() where T : class
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

        }


    }
}
