using NUnit.Framework;
using BizArk.Core.Extensions.DataExt;
using BizArk.Core.Extensions.StringExt;
using System.Data.SqlClient;
using System.Diagnostics;
using BizArk.Core.Util;

namespace BizArk.Core.Tests
{

	[TestFixture]
	public class DataExtTests
	{

		[Test]
		public void DebugSqlTest()
		{
			var cmd = new SqlCommand("SELECT * FROM MyTable WHERE MyField = @MyField");
			cmd.Parameters.AddWithValue("@MyField", "SomeValue");
			var sql = cmd.DebugText();
			Debug.WriteLine(sql);
			Assert.IsTrue(sql.Contains("DECLARE @MyField AS NVARCHAR(9) = N'SomeValue'"));
			Assert.IsTrue(sql.Contains(cmd.CommandText));
		}

		[Test]
		public void DebugSqlWithoutVarDesignatorTest()
		{
			var cmd = new SqlCommand("SELECT * FROM MyTable WHERE MyField = @MyField");
			cmd.Parameters.AddWithValue("MyField", "SomeValue"); // This is legal syntax.
			var sql = cmd.DebugText();
			Debug.WriteLine(sql);
			Assert.IsTrue(sql.Contains("DECLARE @MyField AS NVARCHAR(9) = N'SomeValue'"));
			Assert.IsTrue(sql.Contains(cmd.CommandText));
		}

		[Test]
		public void DebugSqlWithByteArrayTest()
		{
			var cmd = new SqlCommand("UPDATE MyTable SET MyField = @SomeValue WHERE MyTableID = @SomeID");
			cmd.Parameters.AddWithValue("@MyField", new byte[] { 1, 255, 0, 1 });
			cmd.Parameters.AddWithValue("@SomeID", 5);
			var sql = cmd.DebugText();
			Debug.WriteLine(sql);
			var lines = sql.Lines();
			Assert.IsTrue(lines.Length > 0);
			Assert.AreEqual("DECLARE @MyField AS VARBINARY(4) = 0x01FF0001", lines[0]);
			Assert.IsTrue(sql.Contains(cmd.CommandText));
		}

	}
}
