using BizArk.Data.ExtractExt;
using BizArk.Data.SqlServer;
using BizArk.Data.SqlServer.CrudExt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BizArk.Data.Tests
{

	[TestClass]
	[Ignore] // These tests require a database to be setup first.
	public class SqlServerDatabaseTests
	{

		public static BizArkDataConfiguration Config { get; private set; }

		[ClassInitialize]
		public static void Init(TestContext ctx)
		{
			Config = TestHelper.GetApplicationConfiguration(Directory.GetCurrentDirectory());
		}

		[TestMethod]
		public void GetSchemaTest()
		{
			Assert.IsNotNull(Config);
			Assert.IsNotNull(Config.ConnectionString);

			var connstr = Config.ConnectionString;
			using (var db = new SqlServerDatabase(connstr))
			{
				var conn = db.Connection;
				Assert.IsNotNull(conn);
				Assert.AreEqual(connstr, conn.ConnectionString);
				var schema = db.GetSchema("Person");
				Assert.IsNotNull(schema);

				var col = schema.Columns["Id"];
				Assert.IsNotNull(col);
				Assert.AreEqual(typeof(int), col.DataType);
				Assert.IsFalse(col.AllowDBNull);
				Assert.AreEqual(1, schema.PrimaryKey.Length);
				Assert.AreSame(col, schema.PrimaryKey[0]);

				col = schema.Columns["FirstName"];
				Assert.IsNotNull(col);
				Assert.AreEqual(typeof(string), col.DataType);
				Assert.IsTrue(col.AllowDBNull);
				Assert.AreNotSame(col, schema.PrimaryKey[0]);
			}
		}

		[TestMethod]
		public void ExtractTest()
		{
			using (var db = new SqlServerDatabase(Config.ConnectionString))
			{
				var cmd = new SqlCommand("SELECT * FROM Person WHERE Id = 1");
				var john = db.GetDynamic(cmd);
				Assert.IsNotNull(john);
				Assert.AreEqual("John", john.FirstName);

				cmd = new SqlCommand("SELECT * FROM Person");
				var people = db.GetDynamics(cmd);
				Assert.IsNotNull(people);
				Assert.IsTrue(people.Count() > 0);

				cmd = new SqlCommand("SELECT * FROM Person WHERE Id = 2");
				var jane = db.GetObject<Person>(cmd);
				Assert.IsNotNull(jane);
				Assert.AreEqual("Jane", jane.FirstName);
			}
		}

		[TestMethod]
		public void ExecuteScalarTest()
		{
			using (var db = new SqlServerDatabase(Config.ConnectionString))
			{
				var cmd = new SqlCommand("SELECT COUNT(*) FROM Person");
				var val = db.ExecuteScalar<int>(cmd);
				Assert.IsTrue(val >= 3);
			}
		}

		[TestMethod]
		public void TransactionTest()
		{
			using (var db = new SqlServerDatabase(Config.ConnectionString))
			{
				var countCmd = new SqlCommand("SELECT COUNT(*) FROM Person");
				var count = db.ExecuteScalar<int>(countCmd);

				using (db.BeginTransaction())
				{
					var nope = db.Insert("Person", new { FirstName = "Nope" });
					Assert.IsNotNull(nope);
					Assert.AreEqual("Nope", nope.FirstName);
					Assert.IsNull(nope.LastName);

					db.Update("Person", new { Id = (int)nope.Id }, new { LastName = "McSurly" });

					var cmd = new SqlCommand("SELECT * FROM Person WHERE Id = @Id");
					cmd.Parameters.AddWithValue("Id", (int)nope.Id);
					var nope2 = db.GetDynamic(cmd);
					Assert.IsNotNull(nope2);
					Assert.AreEqual("McSurly", nope2.LastName);

					// There is no commit, so this transaction will automatically get rolled back.
				}

				var newcount = db.ExecuteScalar<int>(countCmd);
				Assert.AreEqual(count, newcount); // We rolled the transaction back, so we should still have the same number of rows.
			}
		}

		private class Person
		{
			public int Id { get; set; }
			public string FirstName { get; set; }
			public string LastName { get; set; }
			public DateTime? BirthDate { get; set; }
		}

	}
}
