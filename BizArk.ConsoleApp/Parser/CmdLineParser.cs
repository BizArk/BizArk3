using BizArk.Core;
using BizArk.Core.Extensions.ArrayExt;
using BizArk.Core.Extensions.AttributeExt;
using BizArk.Core.Extensions.FormatExt;
using BizArk.Core.Extensions.ObjectExt;
using BizArk.Core.Extensions.StringExt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace BizArk.ConsoleApp.Parser
{

	/// <summary>
	/// Constructs a new command-line object from the command-line arguments.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class CmdLineParser<T> where T : new()
	{

		#region Construction and Destruction

		/// <summary>
		/// Creates an instance of CmdLineParser.
		/// </summary>
		/// <param name="options"></param>
		public CmdLineParser(CmdLineOptions options = null)
		{
			if (options == null)
			{
				// Check to see if we can get options from the class attribute.
				var att = typeof(T).GetAttribute<CmdLineOptionsAttribute>(false);
				if (att != null)
					options = att.CmdLineOptions;
			}
			Options = options ?? new CmdLineOptions(); // We must have options for parsing.
		}

		#endregion

		#region Properties and Fields

		/// <summary>
		/// Gets the options being used for this instance of the parser.
		/// </summary>
		public CmdLineOptions Options { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Parses the command-line arguments into a new command-line object.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public CmdLineParseResults<T> Parse(IEnumerable<string> args)
		{
			var obj = CreateObject();
			return Fill(obj, args);
		}

		/// <summary>
		/// Fills an existing command-line object from the command-line arguments.
		/// </summary>
		/// <param name="obj">The instance to fill.</param>
		/// <param name="args"></param>
		/// <returns></returns>
		public CmdLineParseResults<T> Fill(T obj, IEnumerable<string> args)
		{
			VerifyCmdLineOptions();

			var results = new CmdLineParseResults<T>(Options);
			results.CmdLineObj = obj;
			var props = results.Properties = GetCmdLineProperties(obj);
			var errors = new List<string>();
			CmdLineProperty prop = null;
			List<string> arrayVals = null;

			// To keep the parsing logic as simple as possible, 
			// normalize the argument list.
			// Normalize for assignment first so other transforms 
			// can work with the same type of list.
			args = TransformAssignedArgs(args);
			args = TransformDefaultArgs(args);

			foreach (var arg in args)
			{
				try
				{
					if (arg.StartsWith(Options.ArgumentPrefix)) // Named value.
					{
						// We might be setting a flag or array value.
						SetPropertyValue(prop, null, arrayVals);
						arrayVals = null;

						var name = arg.Substring(Options.ArgumentPrefix.Length);
						prop = props[name];
					}
					else if (prop != null)
					{
						// Either setting a value or adding to an array.
						if (prop.PropertyType.IsArray)
						{
							if (Options.ArraySeparator != null)
							{
								var values = Regex.Split(arg, Regex.Escape(Options.ArraySeparator));
								SetPropertyValue(prop, null, values);
								prop = null;
							}
							else
							{
								if (arrayVals == null)
									arrayVals = new List<string>();
								arrayVals.Add(arg);
							}
						}
						else
						{
							SetPropertyValue(prop, arg, null);
							prop = null;
						}
					}
				}
				catch (Exception ex)
				{
					if (prop == null || prop.Name.IsEmpty())
						errors.Add("ERR: " + ex.Message);
					else if (arrayVals == null)
						errors.Add("Unable to set {0}. Value <{1}> is not valid.".Fmt(prop.Name, arg));
					else
						errors.Add("Unable to set {0}. Invalid value.".Fmt(prop.Name, arg));
				}
			}
			try
			{
				SetPropertyValue(prop, null, arrayVals);
			}
			catch (Exception ex)
			{
				if (prop == null || prop.Name.IsEmpty())
					errors.Add("ERR: " + ex.Message);
				else
					errors.Add("Unable to set {0}. Invalid value.".Fmt(prop.Name));
			}

			// Populate header-level information, such as the title.
			FillCmdLineObjInfo(results);

			// Validate the object using validation attributes.
			errors.AddRange(ValidateCmdLineObject(obj));
			results.Errors = errors.ToArray();

			return results;
		}

		private void FillCmdLineObjInfo(CmdLineParseResults<T> results)
		{
			// Get the app title.
			if (Application.Title.IsEmpty())
				results.Title = "Command-line options.";
			else if (Application.Version == null)
				results.Title = Application.Title;
			else
				results.Title = string.Format("{0} ver. {1}", Application.Title, Application.Version);

			// Get the app description.
			var att = this.GetAttribute<DescriptionAttribute>(true);
			if (att != null)
				results.Description = att.Description;
			else
				results.Description = Application.Description;

			// Get the file name.
			results.ApplicationFileName = Application.ExeName;
		}

		/// <summary>
		/// Check to make sure the CmdLineOptions are setup correctly.
		/// </summary>
		private void VerifyCmdLineOptions()
		{

			if (Options.ArgumentPrefix.IsEmpty())
				throw new InvalidOperationException("CmdLineOptions.ArgumentPrefix is required.");

			if (Options.AssignmentDelimiter.HasValue() && Options.ArraySeparator.IsEmpty())
				throw new InvalidOperationException("CmdLineOptions.ArraySeparator must be defined when CmdLineOptions.AssignmentDelimiter is set.");

		}

		/// <summary>
		/// If an assignment delimiter is specified, normalize the args array so we can process it like normal.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		internal IEnumerable<string> TransformAssignedArgs(IEnumerable<string> args)
		{
			// No assignment delimiter, just return the args as-is.
			if (Options.AssignmentDelimiter.IsEmpty())
				return args;

			var newArgs = new List<string>();

			foreach (var arg in args)
			{
				var idx = arg.IndexOf(Options.AssignmentDelimiter);
				if (!arg.StartsWith(Options.ArgumentPrefix))
				{
					// Might be a default value or something.
					newArgs.Add(arg);
				}
				else if (idx >= 0)
				{
					var name = arg.Substring(0, idx);
					var value = arg.Substring(idx + Options.AssignmentDelimiter.Length);
					newArgs.Add(name);
					newArgs.Add(value);
				}
			}

			return newArgs;
		}

		/// <summary>
		/// If there are default args specified, normalize the args array so they are named.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private IEnumerable<string> TransformDefaultArgs(IEnumerable<string> args)
		{
			// No defaults, just return the args as-is.
			if (Options.DefaultArgNames == null || Options.DefaultArgNames.Length == 0)
				return args;

			// Loop through the args and normalize the default 
			// arguments so they have a name then value.
			var newArgs = new List<string>();
			var idx = 0;
			foreach (var arg in args)
			{
				if (arg.StartsWith(Options.ArgumentPrefix) || Options.DefaultArgNames.Length < idx)
				{
					// No more default arguments. Exit loop.
					newArgs.AddRange(args.Skip(idx));
					break;
				}

				// Add the name then value args.
				newArgs.Add(Options.ArgumentPrefix + Options.DefaultArgNames[idx]);
				newArgs.Add(arg);
				idx++;
			}

			return newArgs;
		}

		private string[] ValidateCmdLineObject(T obj)
		{
			var errors = new List<string>();
			var results = ObjectExt.Validate(obj);
			foreach (var result in results)
			{
				if (result.ErrorMessage.HasValue())
					errors.Add(result.ErrorMessage);
			}
			return errors.ToArray();
		}

		private void SetPropertyValue(CmdLineProperty prop, string val, IEnumerable<string> arrayVals)
		{
			if (prop == null) return;

			object value = null;

			// Array conversion.
			if (arrayVals != null)
				value = arrayVals.ToArray().Convert(prop.PropertyType.GetElementType());

			// Standard type conversion.
			else if (val != null)
				value = ConvertEx.To(val, prop.PropertyType);

			// Flag with no additional values is true.
			else if (prop.PropertyType == typeof(bool))
				value = true;

			// Name defined, but no value. Set to default for type.
			else if (prop.PropertyType.IsValueType)
				value = Activator.CreateInstance(prop.PropertyType);

			prop.Value = value;
		}

		internal CmdLinePropertyBag GetCmdLineProperties(T obj)
		{
			var props = new CmdLinePropertyBag(Options);

			foreach (PropertyDescriptor propDesc in TypeDescriptor.GetProperties(obj))
			{
				var prop = GetCmdLineProperty(propDesc, obj);
				if (prop != null)
					props.Add(prop);
			}

			var defaultProps = new List<CmdLineProperty>();
			if (Options.DefaultArgNames != null)
			{
				foreach (var dfltName in Options.DefaultArgNames)
				{
					var prop = props[dfltName];
					if (prop == null)
						throw new InvalidOperationException("Unable to find the property for default property '{0}'.".Fmt(dfltName));
					defaultProps.Add(prop);
				}
			}
			props.DefaultProperties = defaultProps.ToArray();

			return props;
		}

		private CmdLineProperty GetCmdLineProperty(PropertyDescriptor propDesc, T obj)
		{
			var prop = new CmdLineProperty(propDesc, obj);
			var claAtt = propDesc.GetAttribute<CmdLineArgAttribute>();
			var showUsage = DefaultBoolean.Default;
			var showDflt = DefaultBoolean.Default;
			if (claAtt != null)
			{
				prop.Aliases = claAtt.Aliases;
				showDflt = claAtt.ShowDefaultValue;
				showUsage = claAtt.ShowInUsage;
				prop.ShowInHelp = claAtt.ShowInHelp;
			}

			prop.Required = propDesc.GetAttribute<RequiredAttribute>() != null;

			// Determine if we should show this in the usage or not.
			if (showUsage == DefaultBoolean.True)
				prop.ShowInUsage = true;
			else if (showUsage == DefaultBoolean.Default && prop.Required)
				prop.ShowInUsage = true;
			else
				prop.ShowInUsage = false;

			prop.DefaultValue = prop.Value;

			// Make sure aliases has a valid value so we don't have to check for nulls everywhere.
			if (prop.Aliases == null)
				prop.Aliases = new string[] { };

			return prop;
		}

		private T CreateObject()
		{
			return Activator.CreateInstance<T>();
		}

		private T FillObject(T cmdLineObj, Dictionary<string, string> args)
		{
			return cmdLineObj;
		}

		#endregion

	}

	/// <summary>
	/// Base class for command-line parser results.
	/// </summary>
	public class CmdLineParseResults
	{

		/// <summary>
		/// Creates a new instance of CmdLineParseResults.
		/// </summary>
		/// <param name="options"></param>
		public CmdLineParseResults(CmdLineOptions options)
		{
			Options = options;
		}

		/// <summary>
		/// Gets the options used to generate the results.
		/// </summary>
		public CmdLineOptions Options { get; private set; }

		/// <summary>
		/// Gets the properties that are part of the command-line object.
		/// </summary>
		public CmdLinePropertyBag Properties { get; internal set; }

		/// <summary>
		/// Gets the parse errors.
		/// </summary>
		public string[] Errors { get; internal set; }

		/// <summary>
		/// Gets the title of the console application.
		/// </summary>
		public string Title { get; internal set; }

		/// <summary>
		/// Gets the description of the console application.
		/// </summary>
		public string Description { get; internal set; }

		/// <summary>
		/// Gets the file name of the console application.
		/// </summary>
		public string ApplicationFileName { get; internal set; }
	}

	/// <summary>
	/// Typed command-line parser results.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CmdLineParseResults<T> : CmdLineParseResults
	{

		/// <summary>
		/// Creates a new instance of CmdLineParseResults.
		/// </summary>
		/// <param name="options"></param>
		public CmdLineParseResults(CmdLineOptions options)
			: base(options)
		{
		}

		/// <summary>
		/// Gets the command-line object that contains the results of the parser.
		/// </summary>
		public T CmdLineObj { get; internal set; }
	}
}
