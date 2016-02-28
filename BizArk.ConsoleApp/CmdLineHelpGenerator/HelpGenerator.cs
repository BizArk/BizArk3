using BizArk.ConsoleApp.Parser;
using BizArk.Core;
using BizArk.Core.Extensions.ArrayExt;
using BizArk.Core.Extensions.FormatExt;
using BizArk.Core.Extensions.StringExt;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace BizArk.ConsoleApp.CmdLineHelpGenerator
{
	public class HelpGenerator
	{

		#region Initialization and Destruction

		public HelpGenerator(CmdLineParseResults results)
		{
			ParseResults = results;
		}

		#endregion

		#region Fields and Properties

		public CmdLineParseResults ParseResults { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Gets the usage line.
		/// </summary>
		/// <returns></returns>
		public string GetUsage()
		{
			var usage = new StringBuilder();
			var options = ParseResults.Options;

			usage.Append(ParseResults.ApplicationFileName);

			// Display default properties first.
			var dfltProps = ParseResults.Properties.DefaultProperties
				.Where(p => p.ShowInUsage);
			foreach (var prop in dfltProps)
			{
				var name = GetUsageName(prop);
				var fmt = prop.Required ? " <{0}|{1}>" : " [<{0}|{1}>]";
				usage.AppendFormat(fmt, name, prop.PropertyType.Name);
			}

			var props = ParseResults.Properties
				.Where(p => p.ShowInUsage && !dfltProps.Contains(p));
			foreach (var prop in props)
			{
				var name = GetUsageName(prop);
				string fmt = prop.Required ? " {0}{1}{2}<{3}>" : " [{0}{1}{2}<{3}>]";
				var usageType = GetPropertyTypeDisplay(prop);
				usage.AppendFormat(fmt, options.ArgumentPrefix, name, options.AssignmentDelimiter ?? " ", usageType);
			}

			return usage.ToString();
		}

		/// <summary>
		/// Gets the display value for the usage type.
		/// </summary>
		/// <param name="prop"></param>
		/// <returns></returns>
		private string GetPropertyTypeDisplay(CmdLineProperty prop)
		{
			if (prop.PropertyType.IsEnum)
			{
				var enumVals = string.Join("|", Enum.GetNames(prop.PropertyType));
				return enumVals;
			}
			else
				return prop.PropertyType.Name;
		}

		/// <summary>
		/// Gets the usage display name for the property.
		/// </summary>
		/// <param name="prop"></param>
		/// <returns></returns>
		private string GetUsageName(CmdLineProperty prop)
		{
			if (prop.Aliases.Length == 0) return prop.Name;
			return prop.Aliases[0];
		}

		/// <summary>
		/// Gets the help text for a single property.
		/// </summary>
		/// <param name="prop"></param>
		/// <returns></returns>
		public string GetPropertyHelp(CmdLineProperty prop)
		{
			var sb = new StringBuilder();

			sb.AppendFormat("{0}{1}", ParseResults.Options.ArgumentPrefix, prop.Name);
			if (prop.Aliases.Length > 0)
			{
				var aliases = string.Join(" | " + ParseResults.Options.ArgumentPrefix, prop.Aliases);
				sb.AppendFormat(" ({0}{1})", ParseResults.Options.ArgumentPrefix, aliases);
			}

			sb.AppendFormat(" <{0}>", GetPropertyTypeDisplay(prop));

			if (prop.Required)
				sb.Append(" REQUIRED");

			if (prop.Description.HasValue())
				sb.AppendFormat("\n\t{0}", prop.Description);

			object dflt = prop.DefaultValue;
			if (!ConvertEx.IsEmpty(dflt))
			{
				var arr = dflt as Array;
				if (arr != null)
				{
					var strs = arr.Convert<string>();
					if (dflt.GetType().GetElementType() == typeof(string))
						dflt = "[\"{0}\"]".Fmt(strs.Join("\", \""));
					else
						dflt = "[{0}]".Fmt(strs.Join(", "));
				}
				sb.AppendFormat("\n\tDefault: {0}", dflt);
			}

			foreach (var att in prop.Property.Attributes)
			{
				var validator = att as ValidationAttribute;
				if (validator == null) continue;

				// The RequiredAttribute is handled differently.
				if (validator.GetType() == typeof(RequiredAttribute)) continue;

				string message = validator.FormatErrorMessage(prop.Name);
				sb.AppendFormat("\n\t{0}", message);
			}

			return sb.ToString();
		}

		#endregion

	}
}
