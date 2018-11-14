using BizArk.Data.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizArk.Data.Tests
{
	[TestClass]
	public class SelectCmdBuilderTests
	{

		[TestMethod]
		public void SimpleSelectTest()
		{
			var bldr = new SelectCmdBuilder("Person p");

			var cmd = bldr.Build();
			AreEqual("SELECT *\n\tFROM Person p\n", cmd.CommandText);
		}

		[TestMethod]
		public void SimpleCountTest()
		{
			var bldr = new SelectCmdBuilder("Person p");

			var cmd = bldr.BuildCounter();
			AreEqual("SELECT COUNT(*)\n\tFROM Person p\n", cmd.CommandText);
		}

		[TestMethod]
		public void SingleFieldTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.Fields.Add("p.Name");

			var cmd = bldr.Build();
			AreEqual("SELECT p.Name\n\tFROM Person p\n", cmd.CommandText);
		}

		[TestMethod]
		public void MultipleFieldTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.Fields.Add("p.Name");
			bldr.Fields.Add("p.PersonTypeID");

			var cmd = bldr.Build();
			var expected =
@"SELECT p.Name, p.PersonTypeID
	FROM Person p
";
			AreEqual(expected, cmd.CommandText);
		}

		[TestMethod]
		public void FiveFieldTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.Fields.Add("p.ID");
			bldr.Fields.Add("p.Name");
			bldr.Fields.Add("p.PersonTypeID");
			bldr.Fields.Add("p.AddressID");
			bldr.Fields.Add("p.Nickname");

			var cmd = bldr.Build();
			var expected =
@"SELECT p.ID,
		p.Name,
		p.PersonTypeID,
		p.AddressID,
		p.Nickname
	FROM Person p
";
			AreEqual(expected, cmd.CommandText);
		}

		[TestMethod]
		public void SingleJoinTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.Joins.Add("JOIN Address a ON (p.AddressID = a.AddressID)");
			bldr.Fields.Add("*");

			var cmd = bldr.Build();
			var expected =
@"SELECT *
	FROM Person p
		JOIN Address a ON (p.AddressID = a.AddressID)
";

			AreEqual(expected, cmd.CommandText);
		}

		[TestMethod]
		public void MultipleJoinTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.Joins.Add("JOIN Address a ON (p.AddressID = a.AddressID)");
			bldr.Joins.Add("JOIN PersonType pt ON (p.PersonTypeID = pt.PersonTypeID)");
			bldr.Fields.Add("*");

			var cmd = bldr.Build();
			var expected =
@"SELECT *
	FROM Person p
		JOIN Address a ON (p.AddressID = a.AddressID)
		JOIN PersonType pt ON (p.PersonTypeID = pt.PersonTypeID)
";
			AreEqual(expected, cmd.CommandText);
		}

		[TestMethod]
		public void SingleCriteriaTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.Criteria.Add("p.Name = 'Jill'");

			var cmd = bldr.Build();
			var expected =
@"SELECT *
	FROM Person p
	WHERE p.Name = 'Jill'
";
			AreEqual(expected, cmd.CommandText);
		}

		[TestMethod]
		public void MultipleCriteriaTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.Criteria.Add("p.Name = 'Jill'");
			bldr.Criteria.Add("p.PersonTypeID = 1");

			var cmd = bldr.Build();
			var expected =
@"SELECT *
	FROM Person p
	WHERE p.Name = 'Jill'
		AND p.PersonTypeID = 1
";
			AreEqual(expected, cmd.CommandText);
		}

		[TestMethod]
		public void SingleOrderByTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.OrderBy.Add("p.Name");

			var cmd = bldr.Build();
			var expected =
@"SELECT *
	FROM Person p
	ORDER BY p.Name
";
			AreEqual(expected, cmd.CommandText);
		}

		[TestMethod]
		public void MultipleOrderByTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.OrderBy.Add("p.Name");
			bldr.OrderBy.Add("p.PersonTypeID");

			var cmd = bldr.Build();
			var expected =
@"SELECT *
	FROM Person p
	ORDER BY p.Name, p.PersonTypeID
";
			AreEqual(expected, cmd.CommandText);
		}

		[TestMethod]
		public void PagedTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.OrderBy.Add("p.Name");

			var cmd = bldr.Build(0, 10);
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

		[TestMethod]
		public void SingleParameterTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.Criteria.Add("p.Name = @Name");
			var param = bldr.Parameters.AddWithValue("Name", "Jill");

			var cmd = bldr.Build();
			var expected =
@"SELECT *
	FROM Person p
	WHERE p.Name = @Name
";
			AreEqual(expected, cmd.CommandText);
			Assert.AreEqual(1, cmd.Parameters.Count);
			Assert.AreSame(param, cmd.Parameters[0]);
		}

		[TestMethod]
		public void MultipleParameterTest()
		{
			var bldr = new SelectCmdBuilder("Person p");
			bldr.Criteria.Add("p.Name = @Name");
			bldr.Criteria.Add("p.PersonTypeID = @PersonTypeID");
			var param = bldr.Parameters.AddValues(new { Name = "Jill", PersonTypeID = 1 });

			var cmd = bldr.Build();
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

		[TestMethod]
		public void ComplexSelectTest()
		{
			var bldr = new SelectCmdBuilder("Person p");

			bldr.Fields.Add("p.ID");
			bldr.Fields.Add("p.Name");
			bldr.Fields.Add("pt.PersonTypeID");
			bldr.Fields.Add("pt.PersonTypeName");
			bldr.Fields.Add("a.AddressID");
			bldr.Fields.Add("a.City");

			bldr.Joins.Add("JOIN Address a ON (p.AddressID = a.AddressID)");
			bldr.Joins.Add("JOIN PersonType pt ON (p.PersonTypeID = pt.PersonTypeID)");

			bldr.Criteria.Add("p.Name = @Name");
			bldr.Criteria.Add("p.PersonTypeID = @PersonTypeID");
			var param = bldr.Parameters.AddValues(new { Name = "Jill", PersonTypeID = 1 });

			bldr.OrderBy.Add("p.Name");
			bldr.OrderBy.Add("a.City");

			var cmd = bldr.Build();
			var expected =
@"SELECT p.ID,
		p.Name,
		pt.PersonTypeID,
		pt.PersonTypeName,
		a.AddressID,
		a.City
	FROM Person p
		JOIN Address a ON (p.AddressID = a.AddressID)
		JOIN PersonType pt ON (p.PersonTypeID = pt.PersonTypeID)
	WHERE p.Name = @Name
		AND p.PersonTypeID = @PersonTypeID
	ORDER BY p.Name, a.City
";

			AreEqual(expected, cmd.CommandText);
			Assert.AreEqual(2, cmd.Parameters.Count);
		}

		private void AreEqual(string expected, string actual)
		{
			Assert.AreEqual(expected.Replace("\r", ""), actual.Replace("\r", ""));
		}

	}
}
