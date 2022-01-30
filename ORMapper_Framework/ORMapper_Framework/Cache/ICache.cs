using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORMapper_Framework.MetaModel;

namespace ORMapper_Framework.Cache
{
    /// <summary>
    /// Interface for the cache used in the ORMapper
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Puts / saves the object in the cache
        /// </summary>
        /// <param name="obj">Object, which should be put in the cache</param>
        /// <param name="type">Type of object</param>
        void Put(object obj, Type type);
        /// <summary>
        /// Clears the cache
        /// </summary>
        void ClearCache();
        /// <summary>
        /// Retrieves the object with the specified data from the cache
        /// </summary>
        /// <param name="t">Type of the object</param>
        /// <param name="primaryKey">Primary key of the object</param>
        /// <returns>Null if an object with the type and primary key does not exist in the cache. Otherwise, the object with the specified parameters gets returned</returns>
        object Get(Type t, object primaryKey);
        /// <summary>
        /// Deletes the specified object from the cache
        /// </summary>
        /// <param name="obj">Object to be deleted from the cache</param>
        void Delete(object obj);
        /// <summary>
        /// Checks whether an object with the specified type and primary key exists in the cache.
        /// </summary>
        /// <param name="t">Type of the object</param>
        /// <param name="primaryKey">Primary key of the object</param>
        /// <returns>False if object with specified data does not exist in cache. True if object with specified data exists in the cache</returns>
        bool Contains(Type t, object primaryKey);
        /// <summary>
        /// Checks whether the object is contained by the cache.
        /// </summary>
        /// <param name="obj">Object, for which the contain check is conducted.</param>
        /// <returns>True if the cache contains the specified object</returns>
        bool Contains(object obj);

        /// <summary>
        /// Checks whether the object changed in the cache (compares hashes)
        /// </summary>
        /// <param name="obj">Object, for which the change check is conducted</param>
        /// <param name="type">Type of object</param>
        /// <returns>True if object changed or is not in the cache. False if object did not change.</returns>
        bool ObjectChanged(object obj, Type type);
        /// <summary>
        /// Puts an object into the temporary cache (for reads / queries)
        /// </summary>
        /// <param name="obj">Object for the cache</param>
        void PutTempCache(object obj);

        /// <summary>
        /// Gets an object with the specified primary key from the temporary cache
        /// </summary>
        /// <param name="type">Type of the wanted object</param>
        /// <param name="pk">Primary key of the wanted object</param>
        /// <returns>Null if object with specified primary key does not exist</returns>
        object GetTempCache(Type type, object pk);
        /// <summary>
        /// Clears temporary cache after queries / reads
        /// </summary>
        void ClearTempCache();
    }
}
