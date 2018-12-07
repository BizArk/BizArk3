using BizArk.Data.SqlServer.BaTableExt;
using System;
using System.Linq;

namespace BizArk.Data.Examples
{
	public static class BaTableObjectExamples
	{

		/// <summary>
		/// Shows how to use a strongly typed BaTableObject, including validation and saving it to 
		/// the database.
		/// </summary>
		public static void CreatePerson()
		{
			var p = new Person();
			p.FirstName = "Bartlet";
			p.LastName = "Simpson";
			p.BirthDate = DateTime.Now.AddYears(-10);

			// Validate the data before saving it.
			var errs = p.Validate();
			if (errs.Count() > 0)
			{
				Console.WriteLine("ERROR!!!");
				foreach (var err in errs)
					Console.WriteLine(err.ErrorMessage);
			}
			else
			{
				using (var db = BaDatabase.Create("MyDatabase"))
				using (var trans = db.BeginTransaction())
				{
					// NOTE: Save is an extension method from BizArk.Data.SqlServer.BaTableExt.
					db.Save(p);

					// When Save is called on a new record, the object is updated with the values 
					// from the database after inserting. So the object will have the identity 
					// field (and any other defaults that are set on INSERT).

					var p2 = db.Get<Person>(new { p.Id }).FirstOrDefault();
					Console.WriteLine($"[{p2.Id}] {p2.FirstName} {p2.LastName}, born {p2.BirthDate:D}.");

					trans.Commit();
				}
			}
		}

		/// <summary>
		/// Shows how to simply update a single value without bothering to fetch data from the 
		/// database first.
		/// </summary>
		public static void UpdateBirthDate()
		{
			var p = new Person();
			p.Id = 1002;
			p.BirthDate = DateTime.Now.AddYears(-10);

			// Validate the data before saving it (only validates the fields that are set).
			var errs = p.Validate();
			if (errs.Count() > 0)
			{
				Console.WriteLine("ERROR!!!");
				foreach (var err in errs)
					Console.WriteLine(err.ErrorMessage);
			}
			else
			{
				using (var db = BaDatabase.Create("MyDatabase"))
				{
					db.Save(p);

					var p2 = db.Get<Person>(new { p.Id }).FirstOrDefault();
					Console.WriteLine($"[{p2.Id}] {p2.FirstName} {p2.LastName}, born {p2.BirthDate:D}.");
				}
			}

		}

	}
}
