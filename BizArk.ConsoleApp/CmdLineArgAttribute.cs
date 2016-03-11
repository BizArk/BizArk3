using BizArk.Core;
using System;

namespace BizArk.ConsoleApp
{

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class CmdLineArgAttribute : Attribute
	{

		public CmdLineArgAttribute()
		{
			ShowInHelp = true;
		}

		public CmdLineArgAttribute(string alias)
			: this()
		{
			Aliases = new string[] { alias };
		}

		public string[] Aliases { get; set; }
		public bool ShowInHelp { get; set; }

		// Attributes do not support null properties, so we need to make the field visible internally.
		internal bool? mShowDefaultValue = null;
		public bool ShowDefaultValue
		{
			get { return mShowDefaultValue ?? false; }
			set { mShowDefaultValue = value; }
		}

		// Attributes do not support null properties, so we need to make the field visible internally.
		internal bool? mShowInUsage = null;
		public bool ShowInUsage
		{
			get { return mShowInUsage ?? false; }
			set { mShowInUsage = value; }
		}
	}

}
