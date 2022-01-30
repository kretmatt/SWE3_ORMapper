using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ORMapper_Framework.MetaModel;

namespace ORMapper_Framework.Cache
{
    /// <summary>
    /// Concrete implementation of the ICache interface
    /// </summary>
    internal class OrCache : ICache
    {
        /// <summary>
        /// Member that administers the objects themselves
        /// </summary>
        private readonly Dictionary<Type, Dictionary<object, object>> _cacheRegistry = new Dictionary<Type, Dictionary<object, object>>();
        
        /// <summary>
        /// Member that administers the hashes of the objects in the cache
        /// </summary>
        private readonly Dictionary<Type, Dictionary<object, string>> _hashRegistry = new Dictionary<Type, Dictionary<object, string>>();

        private ICollection<object> _tempCache = new List<object>();

        /// <summary>
        /// Retrieve the cache dictionary for a specific type
        /// </summary>
        /// <param name="type">Type of the wanted cache dictionary</param>
        /// <returns>(Sub-)Cache dictionary</returns>
        private Dictionary<object,object> GetSubCache(Type type)
        {
            if (_cacheRegistry.ContainsKey(type))
                return _cacheRegistry[type];

            Dictionary<object, object> newSubCache = new Dictionary<object, object>();
            _cacheRegistry.Add(type, newSubCache);
            return newSubCache;
        }

        /// <summary>
        /// Retrieve the hash dictionary for a specific type
        /// </summary>
        /// <param name="type">Type of the wanted hash dictionary</param>
        /// <returns>(Sub-)Hash dictionary</returns>
        private Dictionary<object, string> GetSubHash(Type type)
        {
            if (_hashRegistry.ContainsKey(type))
                return _hashRegistry[type];

            Dictionary<object, string> newSubHash = new Dictionary<object, string>();
            _hashRegistry.Add(type, newSubHash);
            return newSubHash;
        }
        /// <summary>
        /// Checks whether an object with the specified type and primary key exists in the cache.
        /// </summary>
        /// <param name="t">Type of the object</param>
        /// <param name="primaryKey">Primary key of the object</param>
        /// <returns>False if object with specified data does not exist in cache. True if object with specified data exists in the cache</returns>
        public bool Contains(Type t, object primaryKey)
        {
            return GetSubCache(t).ContainsKey(primaryKey);
        }
        /// <summary>
        /// Checks whether the object is contained by the cache.
        /// </summary>
        /// <param name="obj">Object, for which the contain check is conducted.</param>
        /// <returns>True if the cache contains the specified object</returns>
        public bool Contains(object obj)
        {
            return GetSubCache(obj.GetType()).ContainsKey(obj.GetEntity().PrimaryKey.GetFieldValue(obj));
        }
        /// <summary>
        /// Deletes the specified object from the cache
        /// </summary>
        /// <param name="obj">Object to be deleted from the cache</param>
        public void Delete(object obj)
        {
            GetSubCache(obj.GetType()).Remove(obj.GetEntity().PrimaryKey.GetFieldValue(obj));
            GetSubHash(obj.GetType()).Remove(obj.GetEntity().PrimaryKey.GetFieldValue(obj));
        }
        /// <summary>
        /// Retrieves the object with the specified data from the cache
        /// </summary>
        /// <param name="t">Type of the object</param>
        /// <param name="primaryKey">Primary key of the object</param>
        /// <returns>Null if an object with the type and primary key does not exist in the cache. Otherwise, the object with the specified parameters gets returned</returns>
        public object Get(Type t, object primaryKey)
        {
            Dictionary<object, object> subCache = GetSubCache(t);
            return subCache.ContainsKey(primaryKey)
                ? subCache[primaryKey]
                : null;
        }

        /// <summary>
        /// Checks whether the object changed in the cache (compares hashes)
        /// </summary>
        /// <param name="obj">Object, for which the change check is conducted</param>
        /// <param name="type">Type of object</param>
        /// <returns>True if object changed or is not in the cache. False if object did not change.</returns>
        public bool ObjectChanged(object obj, Type type)
        {
            Dictionary<object, string> subHash = GetSubHash(type);
            object pk = type.GetEntity().PrimaryKey.GetFieldValue(obj);

            if (subHash.ContainsKey(pk))
                return subHash[pk] != GenerateHash(obj, type);

            return true;
        }

        /// <summary>
        /// Puts / saves the object in the cache
        /// </summary>
        /// <param name="obj">Object, which should be put in the cache</param>
        /// <param name="type">Type of object</param>
        public void Put(object obj, Type type)
        {
            if (obj == null) return;

            object primaryKey = type.GetEntity().PrimaryKey.GetFieldValue(obj);
            GetSubCache(type)[primaryKey] = obj;
            GetSubHash(type)[primaryKey] = GenerateHash(obj, type);
        }

        /// <summary>
        /// Generate the hash of an (entity)object
        /// </summary>
        /// <param name="obj">Object, for which the hash needs to be generated</param>
        /// <param name="type">Type of object</param>
        /// <returns>Hash of the object</returns>
        private string GenerateHash(object obj, Type type)
        {
            string objHash = "";
            
            foreach(Field i in type.GetEntity().Internals)
            {
                object internalValue = i.GetFieldValue(obj);

                if (internalValue == null) continue;

                objHash += i.IsForeignKey
                    ? i.ColumnName + "=" +
                      internalValue.GetEntity().PrimaryKey.GetFieldValue(internalValue).ToString() + "|"
                    : i.ColumnName + "=" + internalValue.ToString() + "|";
            }

            foreach (Field e in type.GetEntity().Externals)
            {
                IEnumerable externalValues = (IEnumerable)e.GetFieldValue(obj);

                if (externalValues == null) continue;

                objHash += (e.ColumnName + "=");
                objHash = externalValues.Cast<object>().Aggregate(objHash, (current, externalValue) => current + (externalValue.GetEntity().PrimaryKey.GetFieldValue(externalValue).ToString() + ","));
                objHash += "|";
            }

            return Encoding.UTF8.GetString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(objHash)));
        }
        /// <summary>
        /// Puts an object into the temporary cache (for reads / queries)
        /// </summary>
        /// <param name="obj">Object for the cache</param>
        public void PutTempCache(object obj)
        {
            _tempCache.Add(obj);
        }

        /// <summary>
        /// Gets an object with the specified primary key from the temporary cache
        /// </summary>
        /// <param name="type">Type of the wanted object</param>
        /// <param name="pk">Primary key of the wanted object</param>
        /// <returns>Null if object with specified primary key does not exist</returns>
        public object GetTempCache(Type type, object pk)
        {
            return _tempCache.FirstOrDefault(t =>
            {
                Entity entity = t.GetEntity();
                return entity.PrimaryKey.GetFieldValue(t).Equals(pk) && type == entity.Member;
            });
        }
        /// <summary>
        /// Clears temporary cache after queries / reads
        /// </summary>
        public void ClearTempCache()
        {
            _tempCache.Clear();
        }
        /// <summary>
        /// Clears the cache
        /// </summary>
        public void ClearCache()
        {
            _cacheRegistry.Clear();
            _hashRegistry.Clear();
        }
    }
}
