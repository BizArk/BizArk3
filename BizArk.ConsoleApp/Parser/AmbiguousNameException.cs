using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BizArk.ConsoleApp.Parser
{
	[Serializable]
	internal class AmbiguousNameException : CmdLineException
	{

		/// <summary>
		/// Creates an instance of AmbiguousNameException.
		/// </summary>
		/// <param name="argName"></param>
		/// <param name="props"></param>
		public AmbiguousNameException(string argName, params CmdLineProperty[] props)
			: base(string.Format("The command-line argument name '{0}' matches the following command-line properties: {1}. You must disambiguate the argument name by using either the shortcut or include additional characters in the name.", argName, string.Join(", ", GetPropertyNames(props))))
		{
			ArgName = argName;
			AmbiguousProperties = props;
		}

		/// <summary>
		/// Gets the name of the invalid argument.
		/// </summary>
		public string ArgName { get; private set; }

		/// <summary>
		/// Gets the conflicting properties.
		/// </summary>
		public CmdLineProperty[] AmbiguousProperties { get; private set; }

		private static string[] GetPropertyNames(params CmdLineProperty[] props)
		{
			var names = new List<string>();
			foreach (var prop in props)
				names.Add(prop.Name);
			return names.ToArray();
		}

	}
}