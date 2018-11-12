using BizArk.Data.SqlServer.SqlClientExt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;

namespace BizArk.Data.Tests
{
	[TestClass]
	public class SqlClientExtTests
	{

		[TestMethod]
		public void AddParametersTest()
		{
			var cmd = new SqlCommand("SELECT * FROM Person WHERE Id IN ({Id})");
			cmd.AddParameters("Id", new[] { 1, 2, 3 });
			Assert.AreEqual(3, cmd.Parameters.Count);
			Assert.AreEqual("@Id1", cmd.Parameters[0].ParameterName);
			Assert.AreEqual("SELECT * FROM Person WHERE Id IN (@Id1, @Id2, @Id3)", cmd.CommandText);
		}

	}
}
