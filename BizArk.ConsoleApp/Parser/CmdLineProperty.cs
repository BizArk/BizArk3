using BizArk.Core;
using BizArk.Core.Extensions.FormatExt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BizArk.ConsoleApp.Parser
{
	/// <summary>
	/// Represents a property that can be set via the command-line.
	/// </summary>
	public class CmdLineProperty
	{
		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of a CmdLineProperty.
		/// </summary>
		public CmdLineProperty(PropertyDescriptor prop, object obj)
		{
			Property = prop;
			CmdLineObj = obj;
			ShowInHelp = true;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets the PropertyInfo object associated with this CmdLine property.
		/// </summary>
		public PropertyDescriptor Property { get; private set; }

		/// <summary>
		/// Gets the instance of the CmdLine object that we are populating.
		/// </summary>
		public object CmdLineObj { get; private set; }

		/// <summary>
		/// The name of the command-line property.
		/// </summary>
		public string Name { get { return Property.Name; } }

		/// <summary>
		/// Gets the aliases associated with this property.
		/// </summary>
		public string[] Aliases { get; internal set; }

		/// <summary>
		/// Gets the type of the property.
		/// </summary>
		public Type PropertyType { get { return Property.PropertyType; } }

		/// <summary>
		/// Gets or sets the current value for this property.
		/// </summary>
		public object Value
		{
			get
			{
				return Property.GetValue(CmdLineObj);
			}
			set
			{
				Property.SetValue(CmdLineObj, value);
				ValueSet = true;
			}
		}

		/// <summary>
		/// Gets a value that determines if this property was set or not.
		/// </summary>
		public bool ValueSet { get; private set; }

		/// <summary>
		/// Gets the description associated with the property.
		/// </summary>
		public string Description { get { return Property.Description; } }

		/// <summary>
		/// Gets the default value for this property. Used in the command-line help description. Will show any non-null value except for empty strings.
		/// </summary>
		public object DefaultValue { get; internal set; }

		/// <summary>
		/// Gets a value that determines if the property should be shown in the usage. Defaults to true for required properties.
		/// </summary>
		public bool ShowInUsage { get; internal set; }

		/// <summary>
		/// Gets a value that determines if the property should be shown in the help. Defaults to true if there is a description for the property (DescriptionAttribute).
		/// </summary>
		public bool ShowInHelp { get; internal set; }

		/// <summary>
		/// Gets a value that determins if the property is required. Value comes from System.ComponentModel.DataAnnotations.RequiredAttribute applied to property.
		/// </summary>
		public bool Required { get; internal set; }

		#endregion

		#region Methods

		/// <summary>
		/// Gets the textual representation of this command-line object. Useful for debugging.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var value = new StringBuilder();
			var vals = Value as IEnumerable;
			if (vals != null)
			{
				var first = true;
				foreach (var val in vals)
				{
					if (!first) value.Append(", ");
					first = false;
					value.Append(ConvertEx.ToString(val));
				}
			}
			else
				value.Append(ConvertEx.ToString(Value));
			return "{0}=[{1}]".Fmt(Name, value);
		}

		#endregion
	}

	/// <summary>
	/// Collection of CmdLineProperty objects for a command-line object.
	/// </summary>
	public class CmdLinePropertyBag : IEnumerable<CmdLineProperty>
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of CmdLinePropertyBag.
		/// </summary>
		/// <param name="options"></param>
		public CmdLinePropertyBag(CmdLineOptions options)
		{
			Options = options;
		}

		#endregion

		#region Fields and Properties

		private List<CmdLineProperty> mProperties = new List<CmdLineProperty>();

		/// <summary>
		/// Gets the options associated with the command-line parser.
		/// </summary>
		public CmdLineOptions Options { get; private set; }

		/// <summary>
		/// Gets the command-line property that matches the argName. Searches aliases and partial matches.
		/// </summary>
		/// <param name="argName"></param>
		/// <returns></returns>
		public CmdLineProperty this[string argName]
		{
			get
			{
				if (string.IsNullOrEmpty(argName)) return null;

				// Search for a command-line property that starts with this name.
				var foundProps = new List<CmdLineProperty>();
				foreach (var prop in mProperties)
				{
					// Exact match, just return it.
					if (prop.Name.Equals(argName, Options.Comparer))
						return prop;

					// Check if the name matches the name of the property.
					if (prop.Name.StartsWith(argName, Options.Comparer))
					{
						foundProps.Add(prop);
						continue; // Once added, go to the next property.
					}

					// Check if the name matches any of the aliases.
					foreach (var alias in prop.Aliases)
					{
						// Exact match, just return it.
						if (alias.Equals(argName, Options.Comparer))
							return prop;

						if (alias.StartsWith(argName, Options.Comparer))
						{
							foundProps.Add(prop);
							continue; // Once added, go to the next property.
						}
					}
				}

				if (foundProps.Count == 0) return null;
				if (foundProps.Count == 1) return foundProps[0];
				// Multiple properties were found. We cannot process this argument.
				throw new AmbiguousNameException(argName, foundProps.ToArray());
			}
		}

		/// <summary>
		/// Gets the number of properties in this collection.
		/// </summary>
		public int Count
		{
			get
			{
				return mProperties.Count;
			}
		}

		/// <summary>
		/// Gets the properties that will be set if the first arguments do not have a name.
		/// </summary>
		public CmdLineProperty[] DefaultProperties { get; internal set; }

		#endregion

		#region Methods

		/// <summary>
		/// Adds a command-line property to the list.
		/// </summary>
		/// <param name="prop"></param>
		public void Add(CmdLineProperty prop)
		{
			mProperties.Add(prop);
		}

		/// <summary>
		///     Gets the enumerator for the list.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<CmdLineProperty> GetEnumerator()
		{
			return mProperties.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

	}

}
