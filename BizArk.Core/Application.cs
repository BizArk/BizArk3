using System;
using System.Diagnostics;
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

		#region Initialization and Destruction

		static Application()
		{
			var process = Process.GetCurrentProcess();
			if (process != null)
			{
				ProcessID = process.Id;
				ProcessName = process.ProcessName;
			}

			var asm = Assembly.GetEntryAssembly();
			if (asm == null) return;

			Title = asm.GetAttribute<AssemblyTitleAttribute>()?.Title;
			Description = asm.GetAttribute<AssemblyDescriptionAttribute>()?.Description;
			Company = asm.GetAttribute<AssemblyCompanyAttribute>()?.Company;
			Product = asm.GetAttribute<AssemblyProductAttribute>()?.Product;
			Copyright = asm.GetAttribute<AssemblyCopyrightAttribute>()?.Copyright;
			Trademark = asm.GetAttribute<AssemblyTrademarkAttribute>()?.Trademark;

			Version = asm.GetName().Version;

			Uri uri = new Uri(asm.EscapedCodeBase);
			if (uri.Scheme == "file")
				ExePath = uri.LocalPath + uri.Fragment;
			else
				ExePath = uri.ToString();

			ExeName = System.IO.Path.GetFileName(ExePath);
		}

		#endregion

		#region Fields and Propertiers

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
		/// Gets the ID of the process.
		/// </summary>
		public static int ProcessID { get; private set; }

		/// <summary>
		/// Gets the name of the process.
		/// </summary>
		public static string ProcessName { get; private set; }

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

		#endregion

		#region Methods

		/// <summary>
		/// Sets the current directory. Place in using statement to return the directory to it's original directory.
		/// </summary>
		/// <param name="dir"></param>
		/// <returns></returns>
		public static Cleanup SetCurrentDirectory(string dir)
		{
			var origDir = Environment.CurrentDirectory;
			Environment.CurrentDirectory = dir;
			return new Cleanup(() =>
			{
				Environment.CurrentDirectory = origDir;
			});
		}

		#endregion

	}
}
