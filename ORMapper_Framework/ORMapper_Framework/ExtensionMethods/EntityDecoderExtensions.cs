using ORMapper_Framework.MetaModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapper_Framework.DBHelperClasses
{
    internal static class EntityDecoderExtensions
    {
        public static List<__Entity> DecodeEntities(this List<__Entity> entities)
        {
            List<__Entity> allEntities = new List<__Entity>();

            entities.ForEach(e => allEntities.AddRange(e.DecodeEntity()));

            return allEntities.GroupBy(e=>e.TableName).Select(g=>g.First()).ToList();
        }

        public static List<__Entity> DecodeEntity(this __Entity entity)
        {
            List<__Entity> entities = new List<__Entity>();
            
            if (entity.IsDerived)
            {
                __Entity ef = new __Entity(entity.Member.BaseType);
                entities.AddRange(ef.DecodeEntity());
            }


            for(int i = 0; i < entity.Fields.Length; i++)
            {
                if(entity.Fields[i].ColumnType.IsClass && !entities.Any(e=>e.Member == entity.Fields[i].ColumnType))
                {
                    if(typeof(AEntity).IsAssignableFrom(entity.Fields[i].ColumnType))
                        entities.AddRange(new __Entity(entity.Fields[i].ColumnType).DecodeEntity());
                    else if(entity.Fields[i].ColumnType.IsGenericType && entity.Fields[i].ColumnType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        Type listType = entity.Fields[i].ColumnType.GetGenericArguments()[0];
                        if(typeof(AEntity).IsAssignableFrom(listType))
                            entities.AddRange(new __Entity(listType).DecodeEntity());

                    }
                }      
            }

            entities.Add(entity);

            return entities;
        }
    }
}
