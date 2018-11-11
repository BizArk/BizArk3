using BizArk.Core.Extensions.ObjectExt;
using BizArk.Data.Crud;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BizArk.Data
{

	/// <summary>
	/// Provides repository methods for BaTableObjects.
	/// </summary>
	public class BaTableRepository<T> : IBaRepository where T : BaTableObject, new()
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates a new instance of BaTableRepository.
		/// </summary>
		/// <param name="db">The database to use for the repository. The database will not be disposed with the repository.</param>
		public BaTableRepository(ISupportBaDatabase db)
		{
			DB = db.DB;
			InitTableName();
		}

		private void InitTableName()
		{
			// Instantiate an object just so we can get the table name.
			var obj = new T();
			TableName = obj.TableName;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets the name of the table in the database.
		/// </summary>
		public string TableName { get; private set; }

		/// <summary>
		/// Gets the database associated with this repository.
		/// </summary>
		public BaDatabase DB { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Saves the table object (INSERT and UPDATE). Does not handle validation.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="restricted"></param>
		public virtual void Save(T obj, params string[] restricted)
		{
			if (obj == null) return;

			// Check to see if there are any changes to save.
			var updates = obj.GetChanges(restricted);
			if (updates.Count == 0) return;

			if (obj.IsNew)
			{
				var values = DB.Insert(obj.TableName, updates);
				obj.Fill((object)values);
			}
			else
			{
				var key = new Dictionary<string, object>();
				foreach (var fld in obj.GetKey())
					key.Add(fld.Name, fld.Value);
				DB.Update(obj.TableName, key, updates);
			}
			obj.UpdateDefaults();
		}

		/// <summary>
		/// Saves the table object (INSERT and UPDATE). Does not handle validation.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="restricted"></param>
		public async virtual Task SaveAsync(T obj, params string[] restricted)
		{
			if (obj == null) return;

			// Check to see if there are any changes to save.
			var updates = obj.GetChanges(restricted);
			if (updates.Count == 0) return;

			if (obj.IsNew)
			{
				var values = await DB.InsertAsync(obj.TableName, updates).ConfigureAwait(false);
				obj.Fill((object)values);
			}
			else
			{
				var key = new Dictionary<string, object>();
				foreach (var fld in obj.GetKey())
					key.Add(fld.Name, fld.Value);
				await DB.UpdateAsync(obj.TableName, key, updates).ConfigureAwait(false);
			}
			obj.UpdateDefaults();
		}

		/// <summary>
		/// Deletes the table object from the database.
		/// </summary>
		/// <param name="obj"></param>
		public virtual void Delete(T obj)
		{
			// Need to convert the key from a list of BaField to 
			// something that can be converted to a property bag.
			var keyFlds = obj.GetKey();
			var key = new Dictionary<string, object>();
			foreach (var keyFld in keyFlds)
				key.Add(keyFld.Name, keyFld.Value);

			DB.Delete(obj.TableName, key);
		}

		/// <summary>
		/// Deletes the table object from the database.
		/// </summary>
		/// <param name="obj"></param>
		public async virtual Task DeleteAsync(T obj)
		{
			// Need to convert the key from a list of BaField to 
			// something that can be converted to a property bag.
			var keyFlds = obj.GetKey();
			var key = new Dictionary<string, object>();
			foreach (var keyFld in keyFlds)
				key.Add(keyFld.Name, keyFld.Value);

			await DB.DeleteAsync(obj.TableName, key).ConfigureAwait(false);
		}

		/// <summary>
		/// Gets the requested object out of the database.
		/// </summary>
		/// <param name="key">The key to </param>
		/// <param name="flds">List of fields to get out of the database. Leave empty to retrieve all. Names are case sensitive.</param>
		/// <returns></returns>
		public virtual IEnumerable<T> Get(object key, params string[] flds)
		{
			var obj = new T();
			var selCmd = new SelectCmdBuilder(TableName);

			// Add specified fields to the select command.
			foreach (var fldName in flds)
			{
				var fld = obj.Fields[fldName];
				if (fld != null)
					selCmd.Fields.Add(fld.Name);
			}

			var propBag = key.ToPropertyBag();
			foreach (var prop in propBag)
			{
				selCmd.Criteria.Add($"{prop.Key} = @{prop.Key}");
				selCmd.Parameters.AddWithValue(prop.Key, prop.Value);
			}

			var cmd = selCmd.CreateCmd();
			return DB.GetObjects<T>(cmd);
		}

		/// <summary>
		/// Gets the requested object out of the database.
		/// </summary>
		/// <param name="key">The key to </param>
		/// <param name="flds">List of fields to get out of the database. Leave empty to retrieve all. Names are case sensitive.</param>
		/// <returns></returns>
		public async virtual Task<IEnumerable<T>> GetAsync(object key, params string[] flds)
		{
			var obj = new T();
			var selCmd = new SelectCmdBuilder(TableName);

			// Add specified fields to the select command.
			foreach (var fldName in flds)
			{
				var fld = obj.Fields[fldName];
				if (fld != null)
					selCmd.Fields.Add(fld.Name);
			}

			var propBag = key.ToPropertyBag();
			foreach (var prop in propBag)
			{
				selCmd.Criteria.Add($"{prop.Key} = @{prop.Key}");
				selCmd.Parameters.AddWithValue(prop.Key, prop.Value);
			}

			var cmd = selCmd.CreateCmd();
			return await DB.GetObjectsAsync<T>(cmd).ConfigureAwait(false);
		}

		#endregion

	}
}
