using ORMapper_Framework;
using ORMapperDemo.School;
using System;

namespace ORMapperDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ORMapper.RegisterNewEntity<Course>();
            ORMapper.EnsureCreated();


            Console.WriteLine("Hello World!");
        }
    }
}
