using System;
using System.IO;
using System.Reflection;
using BizArk.Core.Extensions.AttributeExt;
using BizArk.Core.Extensions.StringExt;
using BizArk.Core.Util;

namespace BizArk.Core
{
	/// <summary>
	/// Provides easy access to information about the currently running application.
	/// </summary>
	public static class Application
	{
		static Application()
		{
			var asm = Assembly.GetEntryAssembly();

			if (asm == null) return;

			var titleAtt = asm.GetAttribute<AssemblyTitleAttribute>();
			if (titleAtt != null)
				Title = titleAtt.Title;

			var descAtt = asm.GetAttribute<AssemblyDescriptionAttribute>();
			if (descAtt != null)
				Description = descAtt.Description;

			var companyAtt = asm.GetAttribute<AssemblyCompanyAttribute>();
			if (companyAtt != null)
				Company = companyAtt.Company;

			var productAtt = asm.GetAttribute<AssemblyProductAttribute>();
			if (productAtt != null)
				Product = productAtt.Product;

			var copyrightAtt = asm.GetAttribute<AssemblyCopyrightAttribute>();
			if (copyrightAtt != null)
				Copyright = copyrightAtt.Copyright;

			var trademarkAtt = asm.GetAttribute<AssemblyTrademarkAttribute>();
			if (trademarkAtt != null)
				Trademark = trademarkAtt.Trademark;

			Version = asm.GetName().Version;

			Uri uri = new Uri(asm.EscapedCodeBase);
			if (uri.Scheme == "file")
				ExePath = uri.LocalPath + uri.Fragment;
			else
				ExePath = uri.ToString();

			ExeName = System.IO.Path.GetFileName(ExePath);
		}

		/// <summary>
		/// Gets the title of the executing assembly from AssemblyTitleAttribute.
		/// </summary>
		public static string Title { get; private set; }

		/// <summary>
		/// Gets the version of the executing assembly.
		/// </summary>
		public static Version Version { get; private set; }

		/// <summary>
		/// Gets the description of the executing assembly from AssemblyDescriptionAttribute.
		/// </summary>
		public static string Description { get; private set; }

		/// <summary>
		/// Gets the company name of the executing assembly from AssemblyCompanyAttribute.
		/// </summary>
		public static string Company { get; private set; }

		/// <summary>
		/// Gets the product name of the executing assembly from AssemblyProductAttribute.
		/// </summary>
		public static string Product { get; private set; }

		/// <summary>
		/// Gets the copyright of the executing assembly from AssemblyCopyrightAttribute.
		/// </summary>
		public static string Copyright { get; private set; }

		/// <summary>
		/// Gets the trademark of the executing assembly from AssemblyTrademarkAttribute.
		/// </summary>
		public static string Trademark { get; private set; }

		/// <summary>
		/// Gets the path the the executing assembly.
		/// </summary>
		public static string ExePath { get; private set; }

		/// <summary>
		/// Gets the just the name of the exe (without the extension).
		/// </summary>
		public static string ExeName { get; private set; }

		/// <summary>
		/// Returns an absolute path relative to the ExePath.
		/// </summary>
		/// <param name="relativePath"></param>
		/// <returns></returns>
		public static string GetPath(string relativePath)
		{
			var dirPath = System.IO.Path.GetDirectoryName(ExePath);
			var path = System.IO.Path.Combine(dirPath, relativePath);
			return Path.GetFullPath(path);
		}

		/// <summary>
		/// Gets the path to the temporary directory for this application. This is a subdirectory off of the system temp directory.
		/// </summary>
		/// <returns></returns>
		public static string GetTempPath()
		{
			string tempPath = Path.GetTempPath();
			if (!ExeName.IsEmpty())
				tempPath = Path.Combine(tempPath, ExeName);

			if (!Directory.Exists(tempPath))
				Directory.CreateDirectory(tempPath);

			return tempPath;
		}

		/// <summary>
		/// Removes the temp directory for this application.
		/// </summary>
		public static void CleanTempDirectory()
		{
			var tempPath = GetTempPath();
			if (!Directory.Exists(tempPath)) return;
			FileEx.DeleteDirectory(tempPath);
		}

		/// <summary>
		/// Gets a value that determines if the application was deployed via ClickOnce.
		/// </summary>
		public static bool ClickOnceDeployed
		{
			get
			{
				if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments == null) return false;
				if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData == null) return false;
				return true;
			}
		}

		/// <summary>
		/// Gets the URL used for click-once deployed apps.
		/// </summary>
		public static string ClickOnceUrl
		{
			get
			{
				if (AppDomain.CurrentDomain == null) return "";
				if (AppDomain.CurrentDomain.SetupInformation == null) return "";
				if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments == null) return "";
				if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData == null) return "";
				if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData.Length == 0) return "";
				return AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData[0];
			}
		}

	}
}
