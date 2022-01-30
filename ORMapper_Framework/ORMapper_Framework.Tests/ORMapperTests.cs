using System;
using System.Data;
using System.Linq;
using Moq;
using NUnit.Framework;
using ORMapperDemo.Library;

namespace ORMapper_Framework.Tests
{
    /// <summary>
    /// In this test class some methods of the actual ORMapper class get tested
    /// </summary>
    [TestFixture]
    public class ORMapperTests
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
        /// Special customer test data
        /// </summary>
        private SpecialCustomer specialCustomer;

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
            mockCommand.Setup(m => m.ExecuteNonQuery());

            mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(m => m.CreateCommand()).Returns(mockCommand.Object);

            OrMapper.Database = mockConnection.Object;
            OrMapper.RegisterNewEntity<SpecialCustomer>();

            specialCustomer = new SpecialCustomer();
            specialCustomer.Password = "123";
            specialCustomer.BirthDate = DateTime.Today;
            specialCustomer.Gender = EGender.Male;
            specialCustomer.ID = 1;
            specialCustomer.Name = "Fritz";
            specialCustomer.SurName = "Fantom";
            specialCustomer.RegisteredSince = DateTime.MinValue;
        }


        /// <summary>
        /// In this test the SpecialCustomer amount and types of registered entities get verified.
        /// The expected registered entities are Customer, Person, Author, Genre, Book and SpecialCustomer.
        /// </summary>
        [Test]
        public void RegisterNewEntity_SpecialCustomer_6EntitiesRegistered()
        {
            //arrange & act
            //assert

            Assert.AreEqual(6, OrMapper.GetEntitiesData().Count);
            // Check if SpecialCustomer, Customer, Person, Author, Genre and Book classes are registered
            Assert.IsTrue(OrMapper.GetEntitiesData().Any(e=>e.Member == typeof(SpecialCustomer)));
            Assert.IsTrue(OrMapper.GetEntitiesData().Any(e => e.Member == typeof(Customer)));
            Assert.IsTrue(OrMapper.GetEntitiesData().Any(e => e.Member == typeof(Person)));
            Assert.IsTrue(OrMapper.GetEntitiesData().Any(e => e.Member == typeof(Author)));
            Assert.IsTrue(OrMapper.GetEntitiesData().Any(e => e.Member == typeof(Genre)));
            Assert.IsTrue(OrMapper.GetEntitiesData().Any(e => e.Member == typeof(Book)));
        }


        /// <summary>
        /// In this test case, commands for dropping and creating tables get executed.
        /// The amount of calls to mockConnection and mockCommand gets verified.
        /// </summary>
        [Test]
        public void EnsureCreatedEnsureDeleted_ForRegisteredEntities_CorrectAmountOfCalls()
        {
            //arrange & act

            OrMapper.EnsureCreated();
            OrMapper.EnsureDeleted();

            //assert - Verify that the correct (amount) of calls were made

            mockConnection.Verify(m => m.CreateCommand(), Times.Exactly(18));
            mockCommand.Verify(m => m.ExecuteNonQuery(), Times.Exactly(18));
        }
        /// <summary>
        /// In this test case, a special customer gets saved.
        /// The amount of calls to mockConnection and mockCommand gets verified
        /// </summary>
        [Test]
        public void Save_SpecialCustomer_CorrectAmountOfCalls()
        {
            //arrange & act
            
            OrMapper.Save(specialCustomer);

            //assert - Verify that the correct (amount) of calls were made (5 commands created because references also get updated)
            mockConnection.Verify(m=>m.CreateCommand(), Times.Exactly(5));
            mockCommand.Verify(m=>m.Parameters.Add(It.IsAny<IDbDataParameter>()), Times.Exactly(11));
            mockCommand.Verify(m=>m.ExecuteNonQuery(), Times.Exactly(5));
        }

        /// <summary>
        /// In this test case, a special customer gets deleted.
        /// The amount of calls to mockConnection and mockCommand gets verified.
        /// </summary>
        [Test]
        public void Delete_SpecialCustomer_CorrectAmountOfCalls()
        {
            //arrange & act

            OrMapper.Delete(specialCustomer);

            //assert - Verify that the correct (amount) of calls were made (5 commands created because references also get updated)
            mockConnection.Verify(m => m.CreateCommand(), Times.Exactly(1));
            mockCommand.Verify(m => m.Parameters.Add(It.IsAny<IDbDataParameter>()), Times.Exactly(1));
            mockCommand.Verify(m => m.ExecuteNonQuery(), Times.Exactly(1));
        }
    }
}