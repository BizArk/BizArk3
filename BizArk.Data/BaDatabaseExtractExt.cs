using BizArk.Core;
using BizArk.Core.Extensions.StringExt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Threading.Tasks;

namespace BizArk.Data.ExtractExt
{

	/// <summary>
	/// Extension methods for `BaDatabase` to pull data out in either strongly-typed or dynamic objects.
	/// </summary>
	public static class BaDatabaseExtractExt
	{

		#region Typed Object Methods

		/// <summary>
		/// Instantiates the object and sets properties based on the field name. Only returns the first row.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="db"></param>
		/// <param name="cmd"></param>
		/// <param name="load">A method that will create an object and fill it. If null, the object will be instantiated based on its type using the ClassFactory (must have a default ctor).</param>
		/// <returns></returns>
		public static T GetObject<T>(this BaDatabase db, DbCommand cmd, Func<IDataReader, T> load = null) where T : class
		{
			T obj = null;

			db.ExecuteReader(cmd, (row) =>
			{
				if (load != null)
				{
					obj = load(row);
					return false;
				}

				// Load doesn't have a value, so use the default loader.
				obj = ClassFactory.CreateObject<T>();
				var props = TypeDescriptor.GetProperties(typeof(T));
				FillObject(row, obj, props);

				return false;
			});

			return obj;
		}

		/// <summary>
		/// Instantiates the object and sets properties based on the field name. Only returns the first row.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="db"></param>
		/// <param name="cmd"></param>
		/// <param name="load">A method that will create an object and fill it. If null, the object will be instantiated based on its type using the ClassFactory (must have a default ctor).</param>
		/// <returns></returns>
		public async static Task<T> GetObjectAsync<T>(this BaDatabase db, DbCommand cmd, Func<IDataReader, Task<T>> load = null) where T : class
		{
			T obj = null;

			await db.ExecuteReaderAsync(cmd, async (row) =>
			{
				if (load != null)
				{
					obj = await load(row).ConfigureAwait(false);
					return false;
				}

				// Load doesn't have a value, so use the default loader.
				obj = ClassFactory.CreateObject<T>();
				var props = TypeDescriptor.GetProperties(typeof(T));
				FillObject(row, obj, props);

				return false;
			}).ConfigureAwait(false);

			return obj;
		}

		/// <summary>
		/// Instantiates the object and sets properties based on the field name. Only returns the first row.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="db"></param>
		/// <param name="cmd"></param>
		/// <param name="load">A method that will create an object and fill it. If null, the object will be instantiated based on its type using the ClassFactory (must have a default ctor). If this returns null, it will not be added to the results.</param>
		/// <returns></returns>
		public static IEnumerable<T> GetObjects<T>(this BaDatabase db, DbCommand cmd, Func<DbDataReader, T> load = null) where T : class
		{
			var results = new List<T>();

			// If load doesn't have a value, use the default loader.
			if (load == null)
			{
				var props = TypeDescriptor.GetProperties(typeof(T));
				load = (row) =>
				{
					var obj = ClassFactory.CreateObject<T>();
					FillObject(row, obj, props);
					return obj;
				};
			}

			db.ExecuteReader(cmd, (row) =>
			{
				var result = load(row);
				if (result != null)
					results.Add(result);

				return true;
			});

			return results;
		}

		/// <summary>
		/// Instantiates the object and sets properties based on the field name. Only returns the first row.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="db"></param>
		/// <param name="cmd"></param>
		/// <param name="load">A method that will create an object and fill it. If null, the object will be instantiated based on its type using the ClassFactory (must have a default ctor). If this returns null, it will not be added to the results.</param>
		/// <returns></returns>
		public async static Task<IEnumerable<T>> GetObjectsAsync<T>(this BaDatabase db, DbCommand cmd, Func<DbDataReader, Task<T>> load = null) where T : class
		{
			var results = new List<T>();

			// If load doesn't have a value, use the default loader.
			if (load == null)
			{
				var props = TypeDescriptor.GetProperties(typeof(T));
				load = async (row) =>
				{
					var obj = ClassFactory.CreateObject<T>();
					FillObject(row, obj, props);
					return await Task.FromResult(obj).ConfigureAwait(false);
				};
			}

			await db.ExecuteReaderAsync(cmd, async (row) =>
			{
				var result = await load(row).ConfigureAwait(false);
				if (result != null)
					results.Add(result);

				return true;
			}).ConfigureAwait(false);

			return results;
		}

		/// <summary>
		/// Fills the object and sets properties based on the field name. Assumes that the DataReader is on the correct row.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="obj"></param>
		/// <param name="props"></param>
		/// <returns>True if the object was filled, false if the data reader didn't contain any data.</returns>
		private static void FillObject(DbDataReader row, object obj, PropertyDescriptorCollection props)
		{
			for (var i = 0; i < row.FieldCount; i++)
			{
				var name = row.GetName(i);
				if (name.IsEmpty()) continue;
				var prop = props.Find(name, false);
				if (prop == null) continue;
				var value = ConvertEx.To(row[i], prop.PropertyType);
				prop.SetValue(obj, value);
			}
		}

		#endregion

		#region Dynamic Methods

		/// <summary>
		/// Gets the first row as a dynamic object.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public static dynamic GetDynamic(this BaDatabase db, DbCommand cmd)
		{
			dynamic result = null;

			db.ExecuteReader(cmd, (row) =>
			{
				result = DbDataReaderToDynamic(row);
				return false;
			});

			return result;
		}

		/// <summary>
		/// Gets the first row as a dynamic object.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public async static Task<dynamic> GetDynamicAsync(this BaDatabase db, DbCommand cmd)
		{
			dynamic result = null;

			await db.ExecuteReaderAsync(cmd, async (row) =>
			{
				result = DbDataReaderToDynamic(row);
				return await Task.FromResult(false).ConfigureAwait(false);
			}).ConfigureAwait(false);

			return result;
		}

		/// <summary>
		/// Returns the results of the SQL command as a list of dynamic objects.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public static IEnumerable<dynamic> GetDynamics(this BaDatabase db, DbCommand cmd)
		{
			var results = new List<dynamic>();

			db.ExecuteReader(cmd, (row) =>
			{
				var result = DbDataReaderToDynamic(row);
				results.Add(result);
				return true;
			});

			return results;
		}

		/// <summary>
		/// Returns the results of the SQL command as a list of dynamic objects.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public async static Task<IEnumerable<dynamic>> GetDynamicsAsync(this BaDatabase db, DbCommand cmd)
		{
			var results = new List<dynamic>();

			await db.ExecuteReaderAsync(cmd, async (row) =>
			{
				var result = DbDataReaderToDynamic(row);
				results.Add(result);
				return await Task.FromResult(true).ConfigureAwait(false);
			}).ConfigureAwait(false);

			return results;
		}

		private static dynamic DbDataReaderToDynamic(DbDataReader row)
		{
			var result = new ExpandoObject() as IDictionary<string, object>;

			for (var i = 0; i < row.FieldCount; i++)
			{
				var value = row[i];
				if (value == DBNull.Value) value = null;
				var name = row.GetName(i);
				if (result.ContainsKey(name))
					result[name] = value;
				else
					result.Add(name, value);
			}

			return result;
		}

		#endregion

	}

}
