using System;
using System.Collections.Generic;
using System.Linq;
using ORMapper_Framework.MetaModel;

namespace ORMapper_Framework.DBHelperClasses
{
    /// <summary>
    /// Class with extension methods for retrieving related entities 
    /// </summary>
    internal static class EntityDecoderExtensions
    {
        /// <summary>
        /// Retrieves related entities for a specified entity
        /// </summary>
        /// <param name="entity">Entity, for which related entities will be searched for</param>
        /// <returns>List of detected entities (grouped by tablename to prevent duplicates)</returns>
        public static List<Entity> UniqueDecodeEntities(this Entity entity)
        {

            List<Entity> allEntities = new List<Entity>();

            allEntities.AddRange(entity.DecodeEntity());

            return allEntities.GroupBy(e=>e.TableName).Select(g=>g.First()).ToList();
        }

        /// <summary>
        /// Recursive method for decoding entities into several entities
        /// </summary>
        /// <param name="entity">Current entity for which associated types will be searched for</param>
        /// <param name="foundTypes">List of already detected types</param>
        /// <returns>List of detected entities (related types / entities)</returns>
        public static List<Entity> DecodeEntity(this Entity entity, List<Type> foundTypes = null)
        {
            List<Entity> entities = new List<Entity>();

            foundTypes ??= new List<Type>();
            // Add current type to found types
            foundTypes.Add(entity.Member);
            
            // Detect inheritance relationships
            if (entity.IsDerived && foundTypes.All(t => t != entity.Member.BaseType))
            {
                Entity ef = new Entity(entity.Member.BaseType);
                entities.AddRange(ef.DecodeEntity(foundTypes));
            }


            // Detect other entities contained within the entity (n:1)
            for(int i = 0; i < entity.Internals.Length; i++)
            {
                if (!entity.Internals[i].ColumnType.IsClass || foundTypes.Any(e => e == entity.Internals[i].ColumnType)) continue;

                if(typeof(AEntity).IsAssignableFrom(entity.Internals[i].ColumnType))
                    entities.AddRange(new Entity(entity.Internals[i].ColumnType).DecodeEntity(foundTypes));
            }


            // Detect external related entities (1:n, m:n)
            for (int i = 0; i < entity.Externals.Length; i++)
            {
                if (!entity.Externals[i].ColumnType.IsClass ||
                    foundTypes.Any(e => e == entity.Externals[i].ColumnType)) continue;
                {
                    if (!entity.Externals[i].ColumnType.IsListType()) continue;
                    Type listType = entity.Externals[i].ColumnType.GetGenericArguments()[0];
                    if (typeof(AEntity).IsAssignableFrom(listType) && foundTypes.All(e => e != listType))
                        entities.AddRange(new Entity(listType).DecodeEntity(foundTypes));
                }
            }

            // Add entity to list after all relations were decoded
            entities.Add(entity);

            return entities;
        }
    }
}
