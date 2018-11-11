using BizArk.Core.Extensions.StringExt;
using BizArk.Data.SqlServer.SqlClientExt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace BizArk.Data.Tests
{

	[TestClass]
	public class DataExtTests
	{

		[TestMethod]
		public void DebugSqlTest()
		{
			var cmd = new SqlCommand("SELECT * FROM MyTable WHERE MyField = @MyField");
			cmd.Parameters.AddWithValue("@MyField", "SomeValue");
			var sql = cmd.DebugText();
			Debug.WriteLine(sql);
			Assert.IsTrue(sql.Contains("DECLARE @MyField AS NVARCHAR(9) = N'SomeValue'"));
			Assert.IsTrue(sql.Contains(cmd.CommandText));
		}

		[TestMethod]
		public void DebugSqlWithJsonTest()
		{
			var cmd = new SqlCommand("SELECT * FROM MyTable WHERE MyField = @MyField");
			cmd.Parameters.AddWithValue("@MyField", "{SomeField:\"SomeValue\"}");
			var sql = cmd.DebugText();
			Debug.WriteLine(sql);
			Assert.IsTrue(sql.Contains("DECLARE @MyField AS NVARCHAR(23) = N'{SomeField:\"SomeValue\"}'"));
			Assert.IsTrue(sql.Contains(cmd.CommandText));
		}

		[TestMethod]
		public void DebugSqlWithoutVarDesignatorTest()
		{
			var cmd = new SqlCommand("SELECT * FROM MyTable WHERE MyField = @MyField");
			cmd.Parameters.AddWithValue("MyField", "SomeValue"); // This is legal syntax.
			var sql = cmd.DebugText();
			Debug.WriteLine(sql);
			Assert.IsTrue(sql.Contains("DECLARE @MyField AS NVARCHAR(9) = N'SomeValue'"));
			Assert.IsTrue(sql.Contains(cmd.CommandText));
		}

		[TestMethod]
		public void DebugSqlWithByteArrayTest()
		{
			var cmd = new SqlCommand("UPDATE MyTable SET MyField = @SomeValue WHERE MyTableID = @SomeID");
			cmd.Parameters.AddWithValue("@MyField", new byte[] { 1, 255, 0, 1 });
			cmd.Parameters.AddWithValue("@SomeID", 5);
			var sql = cmd.DebugText();
			Debug.WriteLine(sql);
			var lines = sql.Lines();
			Assert.IsTrue(lines.Count() > 0);
			Assert.AreEqual("DECLARE @MyField AS VARBINARY(4) = 0x01FF0001", lines.First());
			Assert.IsTrue(sql.Contains(cmd.CommandText));
		}

		[TestMethod]
		public void DebugSqlWithSprocTest()
		{
			var cmd = new SqlCommand("MySproc");
			cmd.CommandType = System.Data.CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("MyField", "SomeValue");
			cmd.Parameters.AddWithValue("AnotherField", "AnotherValue");
			var sql = cmd.DebugText();
			Debug.WriteLine(sql);
			Assert.IsTrue(sql.Contains("EXEC MySproc @MyField, @AnotherField"));
		}

	}
}
