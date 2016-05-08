using NUnit.Framework;

namespace BizArk.Data.SqlServer.Tests
{
	[TestFixture]
	public class SelectCmdBuilderTests
	{

		[Test]
		public void SimpleSelectTest()
		{
			var bldr = new SelectCmdBuilder("Person p");

			var cmd = bldr.CreateCmd();
			AreEqual("SELECT *\n\tFROM Person p\n", cmd.CommandText);
		}

		[Test]
		public void SimpleCountTest()
		{
			var bldr = new SelectCmdBuilder("Person p");

			var cmd = bldr.CreateCountCmd();
			AreEqual("SELECT COUNT(*)\n\tFROM Person p\n", cmd.CommandText);
		}

		[Test]
		public void SingleJoinTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.Joins.Add("JOIN Address a ON (p.AddressID = a.AddressID)");
			bldr.Fields.Add("*");

			var cmd = bldr.CreateCmd();
			var expected =
@"SELECT *
	FROM Person p
		JOIN Address a ON (p.AddressID = a.AddressID)
";

			AreEqual(expected, cmd.CommandText);
		}

		[Test]
		public void MultipleJoinTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.Joins.Add("JOIN Address a ON (p.AddressID = a.AddressID)");
			bldr.Joins.Add("JOIN PersonType pt ON (p.PersonTypeID = pt.PersonTypeID)");
			bldr.Fields.Add("*");

			var cmd = bldr.CreateCmd();
			var expected =
@"SELECT *
	FROM Person p
		JOIN Address a ON (p.AddressID = a.AddressID)
		JOIN PersonType pt ON (p.PersonTypeID = pt.PersonTypeID)
";
			AreEqual(expected, cmd.CommandText);
		}

		[Test]
		public void SingleCriteriaTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.Criteria.Add("p.Name = 'Jill'");

			var cmd = bldr.CreateCmd();
			var expected =
@"SELECT *
	FROM Person p
	WHERE p.Name = 'Jill'
";
			AreEqual(expected, cmd.CommandText);
		}

		[Test]
		public void MultipleCriteriaTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.Criteria.Add("p.Name = 'Jill'");
			bldr.Criteria.Add("p.PersonTypeID = 1");

			var cmd = bldr.CreateCmd();
			var expected =
@"SELECT *
	FROM Person p
	WHERE p.Name = 'Jill'
		AND p.PersonTypeID = 1
";
			AreEqual(expected, cmd.CommandText);
		}

		[Test]
		public void SingleOrderByTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.OrderBy.Add("p.Name");

			var cmd = bldr.CreateCmd();
			var expected =
@"SELECT *
	FROM Person p
	ORDER BY p.Name
";
			AreEqual(expected, cmd.CommandText);
		}

		[Test]
		public void MultipleOrderByTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.OrderBy.Add("p.Name");
			bldr.OrderBy.Add("p.PersonTypeID");

			var cmd = bldr.CreateCmd();
			var expected =
@"SELECT *
	FROM Person p
	ORDER BY p.Name, p.PersonTypeID
";
			AreEqual(expected, cmd.CommandText);
		}

		[Test]
		public void PagedTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.OrderBy.Add("p.Name");

			var cmd = bldr.CreateCmd(0, 10);
			var expected =
@"WITH qry AS
(
SELECT ROW_NUMBER() OVER(ORDER BY p.Name) AS RowNbr,
		COUNT(*) OVER() AS TotalRows,
		*
	FROM Person p
)
SELECT *
	FROM qry
	WHERE RowNbr BETWEEN 0 AND 10
	ORDER BY RowNbr
";
			AreEqual(expected, cmd.CommandText);
		}

		[Test]
		public void SingleParameterTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.Criteria.Add("p.Name = @Name");
			var param = bldr.Parameters.AddWithValue("Name", "Jill");

			var cmd = bldr.CreateCmd();
			var expected =
@"SELECT *
	FROM Person p
	WHERE p.Name = @Name
";
			AreEqual(expected, cmd.CommandText);
			Assert.AreEqual(1, cmd.Parameters.Count);
			Assert.AreSame(param, cmd.Parameters[0]);
		}

		[Test]
		public void MultipleParameterTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.Criteria.Add("p.Name = @Name");
			bldr.Criteria.Add("p.PersonTypeID = @PersonTypeID");
			var param = bldr.Parameters.AddValues(new { Name = "Jill", PersonTypeID = 1 });

			var cmd = bldr.CreateCmd();
			var expected =
@"SELECT *
	FROM Person p
	WHERE p.Name = @Name
		AND p.PersonTypeID = @PersonTypeID
";
			AreEqual(expected, cmd.CommandText);
			Assert.AreEqual(2, cmd.Parameters.Count);
			Assert.AreEqual("Name", cmd.Parameters[0].ParameterName);
			Assert.AreEqual("Jill", cmd.Parameters[0].Value);
			Assert.AreEqual("PersonTypeID", cmd.Parameters[1].ParameterName);
			Assert.AreEqual(1, cmd.Parameters[1].Value);
		}

		private void AreEqual(string expected, string actual)
		{
			Assert.AreEqual(expected.Replace("\r", ""), actual.Replace("\r", ""));
		}

	}
}
