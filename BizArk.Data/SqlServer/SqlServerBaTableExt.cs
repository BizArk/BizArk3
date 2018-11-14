using BizArk.Core.Extensions.ObjectExt;
using BizArk.Data.ExtractExt;
using BizArk.Data.SqlServer.CrudExt;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BizArk.Data.SqlServer.BaTableExt
{

	/// <summary>
	/// Extension methods for `IBaRepository` to work with `BaTableObject`.
	/// </summary>
	public static class SqlServerBaTableExt
	{

		#region Methods

		/// <summary>
		/// Saves the table object (INSERT and UPDATE). Does not handle validation.
		/// </summary>
		/// <param name="rep"></param>
		/// <param name="obj"></param>
		/// <param name="restricted"></param>
		public static void Save<T>(this IBaRepository rep, T obj, params string[] restricted) where T : BaTableObject
		{
			if (obj == null) return;

			// Check to see if there are any changes to save.
			var updates = obj.GetChanges(restricted);
			if (updates.Count == 0) return;

			if (obj.IsNew)
			{
				var values = rep.DB.Insert(obj.TableName, updates);
				obj.Fill((object)values);
			}
			else
			{
				var key = new Dictionary<string, object>();
				foreach (var fld in obj.GetKey())
					key.Add(fld.Name, fld.Value);
				rep.DB.Update(obj.TableName, key, updates);
			}
			obj.UpdateDefaults();
		}

		/// <summary>
		/// Saves the table object (INSERT and UPDATE). Does not handle validation.
		/// </summary>
		/// <param name="rep"></param>
		/// <param name="obj"></param>
		/// <param name="restricted"></param>
		public static async Task SaveAsync<T>(this IBaRepository rep, T obj, params string[] restricted) where T : BaTableObject, new()
		{
			if (obj == null) return;

			// Check to see if there are any changes to save.
			var updates = obj.GetChanges(restricted);
			if (updates.Count == 0) return;

			if (obj.IsNew)
			{
				var values = await rep.DB.InsertAsync(obj.TableName, updates).ConfigureAwait(false);
				obj.Fill((object)values);
			}
			else
			{
				var key = new Dictionary<string, object>();
				foreach (var fld in obj.GetKey())
					key.Add(fld.Name, fld.Value);
				await rep.DB.UpdateAsync(obj.TableName, key, updates).ConfigureAwait(false);
			}
			obj.UpdateDefaults();
		}

		/// <summary>
		/// Deletes the table object from the database.
		/// </summary>
		/// <param name="rep"></param>
		/// <param name="obj"></param>
		public static void Delete<T>(this IBaRepository rep, T obj) where T : BaTableObject
		{
			// Need to convert the key from a list of BaField to 
			// something that can be converted to a property bag.
			var keyFlds = obj.GetKey();
			var key = new Dictionary<string, object>();
			foreach (var keyFld in keyFlds)
				key.Add(keyFld.Name, keyFld.Value);

			rep.DB.Delete(obj.TableName, key);
		}

		/// <summary>
		/// Deletes the table object from the database.
		/// </summary>
		/// <param name="rep"></param>
		/// <param name="obj"></param>
		public static async Task DeleteAsync<T>(this IBaRepository rep, T obj) where T : BaTableObject
		{
			// Need to convert the key from a list of BaField to 
			// something that can be converted to a property bag.
			var keyFlds = obj.GetKey();
			var key = new Dictionary<string, object>();
			foreach (var keyFld in keyFlds)
				key.Add(keyFld.Name, keyFld.Value);

			await rep.DB.DeleteAsync(obj.TableName, key).ConfigureAwait(false);
		}

		/// <summary>
		/// Gets the requested object out of the database.
		/// </summary>
		/// <param name="rep"></param>
		/// <param name="key">The key to </param>
		/// <param name="flds">List of fields to get out of the database. Leave empty to retrieve all. Names are case sensitive.</param>
		/// <returns></returns>
		public static IEnumerable<T> Get<T>(this IBaRepository rep, object key, params string[] flds) where T : BaTableObject, new()
		{
			var obj = new T();
			var selCmd = new SelectCmdBuilder(obj.TableName);

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

			var cmd = selCmd.Build();
			return rep.DB.GetObjects<T>(cmd);
		}

		/// <summary>
		/// Gets the requested object out of the database.
		/// </summary>
		/// <param name="rep"></param>
		/// <param name="key">The key to </param>
		/// <param name="flds">List of fields to get out of the database. Leave empty to retrieve all. Names are case sensitive.</param>
		/// <returns></returns>
		public static async Task<IEnumerable<T>> GetAsync<T>(this IBaRepository rep, object key, params string[] flds) where T : BaTableObject, new()
		{
			var obj = new T();
			var selCmd = new SelectCmdBuilder(obj.TableName);

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

			var cmd = selCmd.Build();
			return await rep.DB.GetObjectsAsync<T>(cmd).ConfigureAwait(false);
		}

		#endregion

	}
}
