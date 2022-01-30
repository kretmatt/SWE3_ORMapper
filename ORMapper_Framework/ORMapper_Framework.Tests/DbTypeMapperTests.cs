using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using ORMapperDemo.Library;
using ORMapper_Framework.DBHelperClasses.DBTypeMapper;

namespace ORMapper_Framework.Tests
{
    /// <summary>
    /// In this test class all supported types get converted to database types and compared to expected results.
    /// </summary>
    [TestFixture]
    public class DbTypeMapperTests
    {
        /// <summary>
        /// Test case data for MapTypeTests method.
        /// </summary>
        private static IEnumerable<TestCaseData> MapTestCases
        {
            get
            {
                yield return new TestCaseData(typeof(bool), "BOOLEAN");
                yield return new TestCaseData(typeof(short), "SMALLINT");
                yield return new TestCaseData(typeof(byte), "SMALLINT");
                yield return new TestCaseData(typeof(sbyte), "SMALLINT");
                yield return new TestCaseData(typeof(int), "INTEGER");
                yield return new TestCaseData(typeof(long), "BIGINT");
                yield return new TestCaseData(typeof(float), "REAL");
                yield return new TestCaseData(typeof(double), "DOUBLE PRECISION");
                yield return new TestCaseData(typeof(decimal), "NUMERIC");
                yield return new TestCaseData(typeof(string), "VARCHAR");
                yield return new TestCaseData(typeof(char[]), "VARCHAR");
                yield return new TestCaseData(typeof(char), "VARCHAR");
                yield return new TestCaseData(typeof(Guid), "UUID");
                yield return new TestCaseData(typeof(byte[]), "BYTEA");
                yield return new TestCaseData(typeof(DateTime), "TIMESTAMP WITHOUT TIME ZONE");
                yield return new TestCaseData(typeof(DateTimeOffset), "TIMESTAMP");
                yield return new TestCaseData(typeof(EGender), "INTEGER");
                yield return new TestCaseData(typeof(Book), "");
            }
        }
        /// <summary>
        /// In this test a type gets converted to the corresponding database type and compared to an expected result.
        /// </summary>
        /// <param name="type">Type to be converted</param>
        /// <param name="expectedResult">Expected database type</param>
        [Test]
        [TestCaseSource(nameof(MapTestCases))]
        public void MapTypeTests(Type type, string expectedResult)
        {
            //arrange - Test case data is already arranged in MapTestCases

            //act
            string actualResult = DbTypeMapper.MapType(type);
            
            //assert
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}