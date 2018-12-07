using BizArk.Data.Examples.MyRepository;
using BizArk.Data.SprocExt;

namespace BizArk.Data.Examples
{
	public class RepositoryExamples
	{

		/// <summary>
		/// Shows how to use a BizArk style repository.
		/// </summary>
		public void ExtensionRepository()
		{
			using (var db = BaDatabase.Create("MyDatabase"))
			{
				// Cast BaDatabase to an IBaRepository so that when we use it, we don't have a 
				// bunch of BaDatabase methods cluttering the interface.
				var rep = (IBaRepository)db;
				rep.GetPerson(1002);
			}
		}

	}

	// Created with it's own namespace so we can import it like you normally would in a real project.
	namespace MyRepository
	{

		/// <summary>
		/// Create the repository as a set of extension methods. This allows us to share methods if 
		/// we want to (though not really recommended). Also, since extension methods are static, it
		/// discourages us from trying to maintain state. Repositories should be dumb in that they 
		/// should not contain any business logic, such as validation, calculations, etc. They 
		/// should only contain the most basic code necessary to interact with the database.
		/// </summary>
		public static class MyRepository
		{
			public static Person GetPerson(this IBaRepository rep, int Id)
			{
				return rep.DB.GetObject<Person>("GetPersonSproc", new { Id });
			}
		}

	}
}
