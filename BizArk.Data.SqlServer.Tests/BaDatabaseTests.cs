using BizArk.Data.SqlServer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizArk.Core.Extensions.DataExt;
using System.Diagnostics;
using System.Dynamic;
using System.Data.SqlClient;

namespace BizArk.Data.SqlServer.Tests
{
	[TestFixture]
	public class BaDatabaseTests
	{
		[Test]
		public void UpdateCmdTest()
		{
			using (var db = new BaDatabase("myconnectionstring"))
			{
				var dt = DateTime.Parse("3/15/2016");
				var cmd = db.PrepareUpdateCmd("MyTable", new { MyID = 123 }, new { MyField = "SomeValue", SomeDate = dt });
				Debug.WriteLine(cmd.DebugText());
				Assert.IsTrue(cmd.CommandText.Contains("UPDATE MyTable SET"));
				Assert.IsTrue(cmd.CommandText.Contains("MyField = @MyField"));
				Assert.IsTrue(cmd.CommandText.Contains("SomeDate = @SomeDate"));
				Assert.IsTrue(cmd.CommandText.Contains("MyID = @MyID"));
				Assert.AreEqual(123, cmd.Parameters["MyID"].Value);
				Assert.AreEqual("SomeValue", cmd.Parameters["MyField"].Value);
				Assert.AreEqual(dt, cmd.Parameters["SomeDate"].Value);
			}
		}

		[Test]
		public void UpdateCmdWithMultCriteriaTest()
		{
			using (var db = new BaDatabase("myconnectionstring"))
			{
				var dt = DateTime.Parse("3/15/2016");
				var cmd = db.PrepareUpdateCmd("MyTable", new { MyID = 123, AnotherID = 654 }, new { MyField = "SomeValue", SomeDate = dt });
				Debug.WriteLine(cmd.DebugText());
				Assert.IsTrue(cmd.CommandText.Contains("UPDATE MyTable SET"));
				Assert.IsTrue(cmd.CommandText.Contains("MyField = @MyField"));
				Assert.IsTrue(cmd.CommandText.Contains("SomeDate = @SomeDate"));
				Assert.IsTrue(cmd.CommandText.Contains("WHERE MyID = @MyID"));
				Assert.IsTrue(cmd.CommandText.Contains("AND AnotherID = @AnotherID"));
				Assert.AreEqual(123, cmd.Parameters["MyID"].Value);
				Assert.AreEqual("SomeValue", cmd.Parameters["MyField"].Value);
				Assert.AreEqual(dt, cmd.Parameters["SomeDate"].Value);
			}
		}

		[Test]
		public void InsertCmdTest()
		{
			using (var db = new BaDatabase("myconnectionstring"))
			{
				var dt = DateTime.Parse("3/15/2016");
				var cmd = db.PrepareInsertCmd("MyTable", new { MyField = "SomeValue", SomeDate = dt });
				Debug.WriteLine(cmd.DebugText());
				Assert.IsTrue(cmd.CommandText.Contains("INSERT INTO MyTable (MyField, SomeDate)"));
				Assert.IsTrue(cmd.CommandText.Contains("VALUES (@MyField, @SomeDate)"));
				Assert.AreEqual("SomeValue", cmd.Parameters["MyField"].Value);
				Assert.AreEqual(dt, cmd.Parameters["SomeDate"].Value);
			}
		}

		[Test]
		public void InsertDynamicCmdTest()
		{
			using (var db = new BaDatabase("myconnectionstring"))
			{
				var dt = DateTime.Parse("3/15/2016");
				dynamic obj = new ExpandoObject();
				obj.MyField = "SomeValue";
				obj.SomeDate = dt;
				var cmd = (SqlCommand)db.PrepareInsertCmd("MyTable", obj);
				Debug.WriteLine(cmd.DebugText());
				Assert.IsTrue(cmd.CommandText.Contains("INSERT INTO MyTable (MyField, SomeDate)"));
				Assert.IsTrue(cmd.CommandText.Contains("VALUES (@MyField, @SomeDate)"));
				Assert.AreEqual("SomeValue", cmd.Parameters["MyField"].Value);
				Assert.AreEqual(dt, cmd.Parameters["SomeDate"].Value);
			}
		}

		[Test]
		public void DeleteCmdTest()
		{
			using (var db = new BaDatabase("myconnectionstring"))
			{
				var dt = DateTime.Parse("3/15/2016");
				var cmd = db.PrepareDeleteCmd("MyTable", new { MyField = "SomeValue", SomeDate = dt });
				Debug.WriteLine(cmd.DebugText());
				Assert.IsTrue(cmd.CommandText.Contains("DELETE FROM MyTable"));
				Assert.IsTrue(cmd.CommandText.Contains("WHERE MyField = @MyField"));
				Assert.IsTrue(cmd.CommandText.Contains("AND SomeDate = @SomeDate"));				
				Assert.AreEqual("SomeValue", cmd.Parameters["MyField"].Value);
				Assert.AreEqual(dt, cmd.Parameters["SomeDate"].Value);
			}
		}

		[Test]
		public void SprocCmdTest()
		{
			using (var db = new BaDatabase("myconnectionstring"))
			{
				var dt = DateTime.Parse("3/15/2016");
				var cmd = db.PrepareSprocCmd("MySproc", new { MyField = "SomeValue", SomeDate = dt });
				Debug.WriteLine(cmd.DebugText());
				Assert.AreEqual("MySproc", cmd.CommandText);
				Assert.AreEqual("SomeValue", cmd.Parameters["MyField"].Value);
				Assert.AreEqual(dt, cmd.Parameters["SomeDate"].Value);
			}
		}

		[Test]
		public void ConnStrFromConfig()
		{
			using (var db = BaDatabase.Create("test"))
			{
				Assert.AreEqual("Wazzup!", db.mConnectionString);
			}
		}

	}
}
