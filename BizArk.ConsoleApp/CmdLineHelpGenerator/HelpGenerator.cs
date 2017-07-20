using BizArk.ConsoleApp.Parser;
using BizArk.Core;
using BizArk.Core.Extensions.ArrayExt;
using BizArk.Core.Extensions.FormatExt;
using BizArk.Core.Extensions.StringExt;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using BizArk.Core.Extensions.AttributeExt;
using System.Collections.Generic;

namespace BizArk.ConsoleApp.CmdLineHelpGenerator
{

	/// <summary>
	/// Generates help text for a command-line object.
	/// </summary>
	public class HelpGenerator
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of HelpGenerator.
		/// </summary>
		/// <param name="results"></param>
		public HelpGenerator(CmdLineParseResults results)
		{
			ParseResults = results;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets the command-line parser results that are used to generate the help.
		/// </summary>
		public CmdLineParseResults ParseResults { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Gets the usage line.
		/// </summary>
		/// <returns></returns>
		public virtual string GetUsage()
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
		protected string GetPropertyTypeDisplay(CmdLineProperty prop)
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
		protected string GetUsageName(CmdLineProperty prop)
		{
			if (prop.Aliases.Length == 0) return prop.Name;
			return prop.Aliases[0];
		}

		/// <summary>
		/// Gets the help text for a single property.
		/// </summary>
		/// <param name="prop"></param>
		/// <returns></returns>
		public virtual string GetPropertyHelp(CmdLineProperty prop)
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

			if (prop.ShowDefaultValue)
			{
				object dflt = prop.DefaultValue;
				if (!ConvertEx.IsEmpty(dflt) || prop.PropertyType.IsEnum)
				{
					if (dflt is Array arr)
					{
						var strs = arr.Convert<string>();
						if (dflt.GetType().GetElementType() == typeof(string))
							dflt = "[\"{0}\"]".Fmt(strs.Join("\", \""));
						else
							dflt = "[{0}]".Fmt(strs.Join(", "));
					}
					sb.AppendFormat("\n\tDefault: {0}", dflt);
				}
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

		/// <summary>
		/// Gets the help text for exit codes, if defined.
		/// </summary>
		/// <returns></returns>
		public virtual string GetExitCodesDisplay()
		{
			var exitCodes = ParseResults.Options.ExitCodes;
			if (exitCodes == null) return "";

			var values = new List<(string nbr, string desc)>();

			var max = 0;
			foreach (Enum val in Enum.GetValues(exitCodes))
			{
				var desc = val.GetDescription();
				var nbr = ConvertEx.ToInt(val).ToString();
				max = Math.Max(max, nbr.Length);
				values.Add((nbr, desc));
			}

			var sb = new StringBuilder();
			foreach (var val in values)
			{
				sb.AppendLine($"{val.nbr.PadLeft(max, ' ')} = {val.desc}");
			}
			return sb.ToString();
		}

		public virtual void WriteHelp(CmdLineParseResults results)
		{

			if (results.Errors.Length > 0)
			{
				if (BaCon.ErrorMessageTitle.HasValue())
				{
					using (var clr = new BaConColor(BaCon.Theme.ErrorTitleText, BaCon.Theme.ErrorTitleBackground))
						BaCon.WriteLine(BaCon.ErrorMessageTitle);
				}
				foreach (var err in results.Errors)
					BaCon.WriteLine(err, BaCon.Theme.ErrorColor, " > ", "\t");
				BaCon.WriteLine();
			}

			var usage = GetUsage();
			if (usage.HasValue())
			{
				BaCon.WriteLine();
				BaCon.WriteLine("[USAGE]");
				BaCon.WriteLine();
				BaCon.WriteLine(usage, BaCon.Theme.UsageColor, "", "\t");
				BaCon.WriteLine();
			}

			var showSectionTitle = true;
			foreach (var prop in results.Properties.Where(p => p.ShowInHelp))
			{
				var propHelp = GetPropertyHelp(prop);
				if (propHelp.IsEmpty()) continue;

				if (showSectionTitle)
				{
					// We only want to display the section title if we 
					// are displaying help text for properties.
					BaCon.WriteLine();
					BaCon.WriteLine("[LIST OF VALID ARGUMENTS]");
					showSectionTitle = false;
				}

				BaCon.WriteLine(); // Place a line break between properties.

				var clr = prop.Required ? BaCon.Theme.RequiredArgColor : BaCon.Theme.StandardArgColor;
				BaCon.WriteLine(propHelp, clr, "", "\t");
				BaCon.WriteLine();
			}

			var exitCodes = GetExitCodesDisplay();
			if (exitCodes.HasValue())
			{
				BaCon.WriteLine();
				BaCon.WriteLine("[EXIT CODES]");
				BaCon.WriteLine();
				BaCon.WriteLine(exitCodes);
				BaCon.WriteLine();
			}
		}

		#endregion

	}
}
