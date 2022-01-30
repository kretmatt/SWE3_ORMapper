using System;
using NUnit.Framework;
using ORMapper_Framework.Cache;
using ORMapperDemo.Library;

namespace ORMapper_Framework.Tests
{
    /// <summary>
    /// In this test class, some cache methods get verified through tests.
    /// </summary>
    [TestFixture]
    public class CacheTests
    {
        /// <summary>
        /// ORMapper cache object
        /// </summary>
        private ICache ormCache;
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
            ormCache = new OrCache();
            specialCustomer = new SpecialCustomer();
            specialCustomer.Password = "123";
            specialCustomer.BirthDate = DateTime.Today;
            specialCustomer.Gender = EGender.Male;
            specialCustomer.ID = 1;
            specialCustomer.Name = "Fritz";
            specialCustomer.SurName = "Fantom";
            specialCustomer.RegisteredSince = DateTime.MinValue;

            ormCache.Put(specialCustomer, typeof(SpecialCustomer));
        }

        /// <summary>
        /// In this test case a special customer gets changed and compared to its "state" in the cache. 
        /// The expected result is true.
        /// </summary>
        [Test]
        public void ObjectChanged_SpecialCustomerChanged_ReturnsTrue()
        {
            //arrange
            specialCustomer.Password = "Tom Turbo";
            //act
            bool changed = ormCache.ObjectChanged(specialCustomer, typeof(SpecialCustomer));
            //assert
            Assert.IsTrue(changed);
        }

        /// <summary>
        /// In this test case a special customer does not get changed and gets compared to its "state" in the cache. 
        /// The expected result is false.
        /// </summary>
        [Test]
        public void ObjectChanged_SpecialCustomerNotChanged_ReturnsFalse()
        {
            //arrange
            //act
            bool changed = ormCache.ObjectChanged(specialCustomer, typeof(SpecialCustomer));
            //assert
            Assert.IsFalse(changed);
        }

        /// <summary>
        /// In this test it gets verified that an object of type SpecialCustomer with primary key 1 exists in the cache.
        /// The expected result is true.
        /// </summary>
        [Test]
        public void Contains_ExistingSpecialCustomer_ReturnsTrue()
        {
            //arrange
            //act
            bool contains = ormCache.Contains(typeof(SpecialCustomer), 1);
            //assert
            Assert.IsTrue(contains);
        }

        /// <summary>
        /// In this test it gets tested whether an object of type SpecialCustomer and primary key 123456 exists in the cache.
        /// The expected result is false.
        /// </summary>
        [Test]
        public void Contains_NotExistingId_ReturnsFalse()
        {
            //arrange
            //act
            bool contains = ormCache.Contains(typeof(SpecialCustomer), 123456);
            //assert
            Assert.IsFalse(contains);
        }

        /// <summary>
        /// In this test the SpecialCustomer object with primary key 1 inside the cache gets deleted.
        /// Afterwards it gets tested whether the object still exists inside the cache.
        /// The expected result is false.
        /// </summary>
        [Test]
        public void Delete_ExistingSpecialCustomer_Successful()
        {
            //arrange
            var objInCache = ormCache.Get(typeof(SpecialCustomer), 1);
            //act
            ormCache.Delete(objInCache);
            //assert
            Assert.IsFalse(ormCache.Contains(objInCache));
        }
    }
}