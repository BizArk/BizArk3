using BizArk.Core.Extensions.DataExt;
using BizArk.Data.DataExt;
using BizArk.Data.ExtractExt;
using BizArk.Data.SprocExt;
using BizArk.Data.SqlServer;
using BizArk.Data.SqlServer.SqlClientExt;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BizArk.Data.Examples
{
	public static class BaDatabaseExamples
	{

		/// <summary>
		/// Shows how to use BeginTransaction.
		/// </summary>
		public static void BeginTransaction()
		{
			using (var db = BaDatabase.Create("MyDatabase"))
			using (var trans = db.BeginTransaction())
			{
				// Some complex database updates

				// NOTE: Must call commit, but rollback is called automatically if you don't call 
				// Commit before the transaction is disposed.
				trans.Commit();
			}
		}

		/// <summary>
		/// Shows how to use TryTransaction.
		/// </summary>
		public static void TryTransaction()
		{
			using (var db = BaDatabase.Create("MyDatabase"))
			{
				db.TryTransaction(() =>
				{
					// Some complex database updates
					// NOTE: There is no need to call commit or rollback. It's handled for you.
				});
			}
		}

		/// <summary>
		/// Shows how to use ExecuteReader to extract data. 
		/// </summary>
		/// <remarks>
		/// This is the most basic way to extract data. Typically you would use one of the other 
		/// extraction methods, such as GetObjects().
		/// </remarks>
		public static void ExtractDataUsingExecuteReader()
		{
			using (var db = BaDatabase.Create("MyDatabase"))
			{
				var cmd = new SqlCommand("SELECT * FROM Person");
				var people = new List<Person>();
				db.ExecuteReader(cmd, (rdr) =>
				{
					var p = new Person()
					{
						Id = rdr.GetInt32(0),
						FirstName = rdr.GetString(1),
						LastName = rdr.GetString(2),
						BirthDate = rdr.GetDateTime(3)
					};
					people.Add(p);

					return true;
				});
			}
		}

		/// <summary>
		/// Shows how to use GetObjects to extract data.
		/// </summary>
		/// <remarks>
		/// This is the recommended way to extract data into strongly typed objects.
		/// </remarks>
		public static void ExtractStronglyTypedData()
		{
			using (var db = BaDatabase.Create("MyDatabase"))
			{
				var cmd = new SqlCommand("SELECT * FROM Person");
				// NOTE: GetObjects is an extension method from BizArk.Data.ExtractExt.
				var people = db.GetObjects<Person>(cmd, rdr =>
				{
					return new Person()
					{
						Id = rdr.GetInt("Id"),
						FirstName = rdr.GetString("FirstName"),
						LastName = rdr.GetString("LastName"),
						BirthDate = rdr.GetDateTime("BirthDate")
					};
				});
			}
		}

		/// <summary>
		/// Shows how to use GetDynamics to extract data.
		/// </summary>
		/// <remarks>
		/// This is the recommended way to extract data into dynamic objects. Dynamic objects are
		/// particularly useful when you are simply taking data from the database and stuffing it
		/// into a response (such as a JSON blob in an API request or using it in a CSHTML file).
		/// </remarks>
		public static void ExtractDynamicData()
		{
			using (var db = BaDatabase.Create("MyDatabase"))
			{
				var cmd = new SqlCommand("SELECT * FROM Person");
				// NOTE: GetDynamics is an extension method from BizArk.Data.ExtractExt.
				var people = db.GetDynamics(cmd);
			}
		}

		/// <summary>
		/// Shows how to use GetDynamic for stored procedures.
		/// </summary>
		public static void ExtractDataFromSproc()
		{
			using (var db = BaDatabase.Create("MyDatabase"))
			{
				// NOTE: GetObjects is an extension method from BizArk.Data.SprocExt.
				var bart = db.GetDynamic("MyPeopleSproc", new { Id = 1003 });
			}
		}

		/// <summary>
		/// Shows how to save data.
		/// </summary>
		public static void SaveData()
		{
			using (var db = BaDatabase.Create("MyDatabase"))
			{
				var cmd = new SqlCommand("UPDATE Person SET BirthDate = @BirthDate WHERE PersonID = @PersonID");
				cmd.AddParameters(new { BirthDate = DateTime.Now.AddYears(-10) });
				db.ExecuteNonQuery(cmd);
			}
		}

	}
}
