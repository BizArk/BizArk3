using BizArk.Core;
using System;

namespace BizArk.ConsoleApp
{

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class CmdLineArgAttribute : Attribute
	{

		public CmdLineArgAttribute()
		{
			ShowDefaultValue = DefaultBoolean.Default;
			ShowInUsage = DefaultBoolean.Default;
			ShowInHelp = true;
		}

		public CmdLineArgAttribute(string alias)
			: this()
		{
			Aliases = new string[] { alias };
		}

		public string[] Aliases { get; set; }
		public DefaultBoolean ShowDefaultValue { get; set; }
		public bool ShowInHelp { get; set; }
		public DefaultBoolean ShowInUsage { get; set; }
	}

}
