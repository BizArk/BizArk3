using BizArk.Core.DataAnnotations;
using System;
using System.Data;

namespace BizArk.Data.Examples
{
public class Person : BaTableObject
{

	public Person()
		: base(GetSchema(), true)
	{
		var fld = base.Fields[nameof(FirstName)];
		var validator = new CustomAttribute(value =>
		{
			var valstr = value as string;
			if (valstr == "Bart") return false;
			return true;
		});
		validator.ErrorMessage = "Name cannot be \"Bart\"";
		fld.Validators.Add(validator);
	}

	private static DataTable sSchema;
	private static DataTable GetSchema()
	{
		if (sSchema == null)
		{
			// Use a short-lived database connection so that it doesn't get caught up in any transactions.
			// Since sSchema is static, it should only need to happen once.
			using (var db = BaDatabase.Create("MyDatabase"))
			{
				sSchema = db.GetSchema("Person");
			}
		}

		return sSchema;
	}

	public int? Id
	{
		get { return (int?)base[nameof(Id)]; }
		set { base[nameof(Id)] = value; }
	}

	public string FirstName
	{
		get { return (string)base[nameof(FirstName)]; }
		set { base[nameof(FirstName)] = value; }
	}

	public string LastName
	{
		get { return (string)base[nameof(LastName)]; }
		set { base[nameof(LastName)] = value; }
	}

	public DateTime? BirthDate
	{
		get { return (DateTime?)base[nameof(BirthDate)]; }
		set { base[nameof(BirthDate)] = value; }
	}

}
}
