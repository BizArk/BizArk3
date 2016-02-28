using System;
using System.IO;
using BizArk.DB;
using BizArk.DB.Adapters;
using NUnit.Framework;

namespace BizArk.Core.Tests
{

	[TestFixture,Ignore]
	public class DbAdapterTests
	{

        private const string cNorthwindConnStr = "Data Source=NorthwindTmp.sdf;Persist Security Info=False;";

        [TestFixtureSetUp]
        public void Setup()
        {
            // Make a copy of the test DB so we don't mess with the original.
            File.Copy(@"Data\Northwind.sdf", "NorthwindTmp.sdf", true);

            // The Northwind sample DB is used in multiple tests. Register it so it's easier to use.
            var adapter = new SqlCeDbAdapter(cNorthwindConnStr);
            Database.RegisterAdapter("nw", adapter);
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            // Unregister the adapter (the DB won't exist after Teardown).
            Database.RemoveAdapter("nw");

            // The temporary DB is no longer needed.
            File.Delete("NorthwindTmp.sdf");
        }

        [Test]
        public void RegisterAdapterTest()
        {
            var adapter = new SqlDbAdapter(cNorthwindConnStr);
            Database.RegisterAdapter("test", adapter);
            var tmp = Database.GetAdapter("test");
            Assert.AreSame(adapter, tmp);
            Database.RemoveAdapter("test");
            tmp = Database.GetAdapter("test");
            Assert.IsNull(tmp);
        }

        [Test]
        public void AccessDBTest()
        {
            using (var db = Database.CreateDB("nw"))
            {
                var dt = db.ExecuteScalar<DateTime>("SELECT GETDATE()");
                AssertEx.AreClose(DateTime.Now, dt, TimeSpan.FromSeconds(1));
            }
        }

        [Test]
        public void NowTest()
        {
            using (var db = Database.CreateDB("nw"))
            {
                var dt = db.Now;
                AssertEx.AreClose(DateTime.Now, dt, TimeSpan.FromSeconds(1));
                dt = db.NowUtc;
                AssertEx.AreClose(DateTime.UtcNow, dt, TimeSpan.FromSeconds(1));
            }
        }

    }
}
