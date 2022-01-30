using System;
using System.Data;
using NUnit.Framework;
using ORMapper_Framework.DBHelperClasses;
using ORMapperDemo.Library;
using Moq;
using ORMapper_Framework.Enums;

namespace ORMapper_Framework.Tests
{
    /// <summary>
    /// In this test class some methods for building sql statements get tested.
    /// </summary>
    [TestFixture]
    public class SqlBuilderTests
    {
        /// <summary>
        /// Mock database connection
        /// </summary>
        private Mock<IDbConnection> mockConnection;
        /// <summary>
        /// Mock database command
        /// </summary>
        private Mock<IDbCommand> mockCommand;
        /// <summary>
        /// Mock database command parameter
        /// </summary>
        private Mock<IDbDataParameter> mockParameter;

        /// <summary>
        /// SetUp gets executed before the unit tests and sets up some shared test data
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            mockParameter = new Mock<IDbDataParameter>();

            mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(m => m.CreateParameter()).Returns(mockParameter.Object);
            mockCommand.Setup(m => m.Parameters.Add(It.IsAny<IDbDataParameter>()));

            mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(m => m.CreateCommand()).Returns(mockCommand.Object);

            OrMapper.Database = mockConnection.Object;
        }


        /// <summary>
        /// In this test create table statements for the entities will be generated. Although there will only be 8 distinct tables,
        /// 9 statements will be returned because the m:n relation between author and books is defined on both sides.
        /// The test ist successful if the amount of generated statements equals 9.
        /// </summary>
        [Test]
        public void Create_SpecialCustomerRegistered_Returns9Statements()
        {
            //arrange
            OrMapper.RegisterNewEntity<SpecialCustomer>();
            //act
            var statements = SqlBuilder.Create();
            //assert
            Assert.AreEqual(9, statements.Count);
        }
        /// <summary>
        /// In this test drop table statements for the entities will be generated. Although there are only 8 distinct tables,
        /// 9 statements will be returned because the m:n relation between author and books is defined on both sides.
        /// The test ist successful if the amount of generated statements equals 9.
        /// </summary>
        [Test]
        public void Drops_SpecialCustomerRegistered_Returns9Statements()
        {
            //arrange
            OrMapper.RegisterNewEntity<SpecialCustomer>();
            //act
            var statements = SqlBuilder.Drops();
            //assert
            Assert.AreEqual(9, statements.Count);
            mockConnection.Verify(m=>m.CreateCommand(), Times.Exactly(9));
        }
        /// <summary>
        /// In this test lock statements for a special customer will get created. Because the SpecialCustomer
        /// inherits from other classes, lock commands for other tables will also get generated.
        /// In this particular test case, 3 database commands should be generated.
        /// </summary>
        [Test]
        public void LockObject_SpecialCustomer_Returns3Commands()
        {
            //arrange
            SpecialCustomer specialCustomer = new SpecialCustomer();
            specialCustomer.Password = "123";
            specialCustomer.BirthDate = DateTime.Today;
            specialCustomer.Gender = EGender.Male;
            specialCustomer.ID = 1;
            specialCustomer.Name = "Fritz";
            specialCustomer.SurName = "Fantom";
            specialCustomer.RegisteredSince = DateTime.MinValue;
            //act
            var commands =
                SqlBuilder.LockObject(specialCustomer, typeof(SpecialCustomer).GetEntity(), LockType.ForShare);

            //assert

            Assert.AreEqual(3, commands.Count);
            mockCommand.Verify(m=>m.CreateParameter(), Times.Exactly(3));
            mockConnection.Verify(m=>m.CreateCommand(), Times.Exactly(3));
        }

    }
}