using System;
using NUnit.Framework;
using ORMapper_Framework.MetaModel;
using ORMapperDemo.Library;

namespace ORMapper_Framework.Tests
{
    /// <summary>
    /// In this test class methods of the Entity class get tested
    /// </summary>
    [TestFixture]
    public class EntityTests
    {

        /// <summary>
        /// In this test a new entity for the type SpecialCustomer gets created.
        /// Various fields / properties are compared to expected values.
        /// </summary>
        [Test]
        public void Entity_ConstructorWithSpecialCustomer_CorrectEntityDefinition()
        {
            //arrange
            string expectedTableName = "SPECIALCUSTOMER";
            bool expectedDerivedStatus = true;
            string expectedPKName = "ID";
            int expectedAmountOfFields = 8;
            Type expectedType = typeof(SpecialCustomer);

            //act

            Entity entity = new Entity(typeof(SpecialCustomer));

            //assert
            Assert.AreEqual(expectedDerivedStatus, entity.IsDerived);
            Assert.AreEqual(expectedPKName, entity.PrimaryKey.ColumnName);
            Assert.AreEqual(expectedTableName, entity.TableName);
            Assert.AreEqual(expectedAmountOfFields, entity.Fields.Length);
            Assert.AreEqual(expectedType, entity.Member);
        }
        /// <summary>
        /// In this test a field with a specific name gets retrieved.
        /// Afterwards the names are compared. The names should be equal.
        /// </summary>
        [Test]
        public void EntityGetFieldByName_FieldWithNameExists_ReturnsField()
        {
            //arrange
            string nameOfField = "Password";
            Entity entity = new Entity(typeof(SpecialCustomer));

            //act
            Field field = entity.GetFieldByName(nameOfField);

            //assert
            Assert.AreEqual(nameOfField, field.ColumnName);
        }
        /// <summary>
        /// In this test a field with a specific name gets retrieved.
        /// Because a field with the specified name does not exist null gets returned.
        /// </summary>
        [Test]
        public void EntityGetFieldByName_FieldWithNameDoesNotExist_ReturnsNull()
        {
            //arrange
            string nameOfField = "CreditCardCode";
            Entity entity = new Entity(typeof(SpecialCustomer));

            //act
            Field field = entity.GetFieldByName(nameOfField);

            //assert
            Assert.IsNull(field);
        }
    }
}