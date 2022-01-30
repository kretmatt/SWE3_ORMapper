using ORMapper_Framework;
using System;
using Npgsql;
using ORMapper_Framework.Queries;
using System.Collections.Generic;
using ORMapper_Framework.Enums;
using ORMapperDemo.Library;

namespace ORMapperDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // INSERT DATABASE CONNECTION STRING HERE
            // For example: Server=localhost;Port=5432;Database=swe3;User Id=user;Password=example;
            OrMapper.Database = new NpgsqlConnection("");
            
            OrMapper.Database.Open();
            Showcase.CreateTables();
            Showcase.InsertData();
            Showcase.RollbackExample();
            Showcase.UpdateData();
            Showcase.DeleteData();
            Showcase.OneToMany();
            Showcase.ManyToMany();
            Showcase.Queries();

            OrMapper.Database.Close();
        }
    }
}
