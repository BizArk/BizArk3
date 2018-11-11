using BizArk.Data.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BizArk.Data.Tests
{
	[TestClass]
	public class BaDatabaseTests
	{

		[TestMethod]
		public void RegisterDbFactoryTest()
		{
			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				BaDatabase.Create("test");
			});

			BaDatabase.Register("test", () =>
			{
				return new SqlServerDatabase("test");
			});

			var db = BaDatabase.Create("test");
			Assert.IsNotNull(db);
			Assert.IsInstanceOfType(db, typeof(SqlServerDatabase));

			BaDatabase.Unregister("test");
			Assert.ThrowsException<ArgumentException>(() =>
			{
				BaDatabase.Create("test");
			});
		}

		[TestMethod]
		public void RegisterSqlServerDbFactoryTest()
		{
			var connStr1 = "db=test1";
			var connStr2 = "db=test2";

			var factory = new SqlServerDbFactory(connStr1);
			BaDatabase.Register("test1", factory.Create);

			factory = new SqlServerDbFactory(connStr2);
			BaDatabase.Register("test2", factory.Create);

			var db = BaDatabase.Create("test1") as SqlServerDatabase;
			Assert.IsNotNull(db);
			Assert.AreEqual(connStr1, db.ConnectionString);

			db = BaDatabase.Create("test2") as SqlServerDatabase;
			Assert.IsNotNull(db);
			Assert.AreEqual(connStr2, db.ConnectionString);

			BaDatabase.Unregister("test1");
			BaDatabase.Unregister("test2");
		}

	}
}
