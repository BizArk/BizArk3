using BizArk.Data.SqlServer;
using System;

namespace BizArk.Data.Examples
{
	class Program
	{
		static void Main(string[] args)
		{
			// Registering databases is optional. You can create a SqlServerDatabase directly from 
			// the connection string if you prefer.
			var factory = new SqlServerDbFactory("Server=localhost\\SqlExpress;Database=BizArkTest;Trusted_Connection=Yes;");
			BaDatabase.Register("MyDatabase", factory.Create);

			//NOTE: If you want to run this project, the database is located in the BizArk.Data.Tests project (Data directory).

			BaTableObjectExamples.CreatePerson();
			//BaTableObjectExamples.UpdateBirthDate();

			Console.WriteLine("Press any key to exit.");
			Console.ReadKey(true);
		}
	}
}
