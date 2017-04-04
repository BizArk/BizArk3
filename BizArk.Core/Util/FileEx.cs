using System;
using System.Diagnostics;
using System.IO;
using BizArk.Core.Extensions.StringExt;

namespace BizArk.Core.Util
{

	/// <summary>
	/// Provides methods that are useful when working with files and directories.
	/// </summary>
	public static class FileEx
	{

		/// <summary>
		/// Creates the specified directory if it doesn't already exist.
		/// </summary>
		/// <param name="dirPath"></param>
		public static void EnsureDirectory(string dirPath)
		{
			// Performance tests suggest that using Directory.Exists is slightly faster
			// than just calling Directory.CreateDirectory() every time, but just 
			// a small % difference. If the common use-case is that the directory
			// rarely exists before calling this method, then just calling
			// Directory.CreateDirectory() would be faster.

			if (Directory.Exists(dirPath)) return;
			Directory.CreateDirectory(dirPath);
		}

		/// <summary>
		/// Removes a directory as best as it can, this includes all files and subdirectories. Errors are ignored.
		/// </summary>
		/// <param name="dirPath"></param>
		/// <param name="deleteEmpties">If true, deletes any empty directory in the path after removing the specified directory.</param>
		public static bool DeleteDirectory(string dirPath, bool deleteEmpties = false)
		{
			var success = DeleteDirectoryHelper(dirPath);

			if (success && deleteEmpties)
				DeleteEmptyDirectories(dirPath);

			return success;
		}

		private static bool DeleteDirectoryHelper(string dirPath)
		{
			var success = true;

			// We want to test for the existance of the directory, but calling
			// Directory.Exists is fairly slow. So we'll just make a call and
			// handle the exception if it comes up.
			string[] childDirs = new string[] { };
			try
			{
				childDirs = Directory.GetDirectories(dirPath);
			}
			catch (DirectoryNotFoundException ex)
			{
				// The directory doesn't exist. Exit.
				Debug.WriteLine($"{ex.Message} Directory doesn't exist, so we should be good to ignore and continue.");
				return true;
			}

			// We don't care if we were able to successfully delete the child
			// directories. In the end, if we are able to delete this directory
			// then any errors in child directories don't matter.
			foreach (string childDirPath in childDirs)
				DeleteDirectory(childDirPath);

			foreach (string filePath in Directory.GetFiles(dirPath))
			{
				try
				{
					DeleteFile(filePath);
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Unable to delete " + filePath + ": " + ex.Message);
					success = false;
				}
			}

			try
			{
				Directory.Delete(dirPath);
				success = true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Unable to delete " + dirPath + ": " + ex.Message);
				success = false;
			}

			return success;
		}

		/// <summary>
		/// Removes all empty directories in the path.
		/// </summary>
		/// <param name="dirPath"></param>
		public static void DeleteEmptyDirectories(string dirPath)
		{
			var curDir = dirPath;
			try
			{
				var root = Path.GetPathRoot(dirPath);
				while (true)
				{
					Directory.Delete(curDir);
					curDir = Path.GetDirectoryName(curDir);

					// Cannot delete the root.
					if (curDir == root) return;
				}
			}
			catch (Exception ex)
			{
				// Ignore errors. This isn't a critical function and we use
				// exceptions to determine if the folder is empty or not.
				// This performs better than checking if the directory 
				// contains any files.
				Debug.WriteLine($"Unable to delete {curDir}: {ex.Message}");
			}
		}

		/// <summary>
		/// Gets a directory structure based on a number. For example, if the number passed in is 12345, 00/00/00/01/23 is passed back. Useful if you have a large number of files stored on disk.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static string GetIDDir(int id)
		{
			var dir = id.ToString();
			dir = ("000000000000" + dir).Right(12).Left(10); // get a string with 10 chars in it with the number being at the end (remove the last two chars).
			return string.Format("{0}/{1}/{2}/{3}/{4}", dir.Substring(0, 2), dir.Substring(2, 2), dir.Substring(4, 2), dir.Substring(6, 2), dir.Substring(8, 2));
		}

		/// <summary>
		/// Strips illegal characters from a potential file name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns>Legal file name.</returns>
		public static string GetLegalFileName(string name)
		{
			var newName = name;
			foreach (char ch in Path.GetInvalidFileNameChars())
				newName = newName.Replace(ch.ToString(), "");
			return newName;
		}

		/// <summary>
		/// Creates a unique file name in the given directory. 
		/// </summary>
		/// <param name="dir">The path to the directory.</param>
		/// <param name="template">The template for the file name. Place a {0} where the counter should go (ex, MyPicture{0}.jpg).</param>
		/// <returns>The full path to the unique file name.</returns>
		public static string GetUniqueFileName(string dir, string template)
		{
			string path;
			dir = dir ?? Application.GetTempPath();
			path = Path.Combine(dir, string.Format(template, ""));
			var i = 2; // this will be the second version of the file name, the first version doesn't include a number but is effectively 1.
			while (File.Exists(path))
			{
				path = Path.Combine(dir, string.Format(template, i));
				i++;
			}
			return path;
		}

		/// <summary>
		/// Creates a unique file name in the temp directory.
		/// </summary>
		/// <param name="ext">The extension for the file.</param>
		/// <returns>The full path to the unique file name.</returns>
		public static string GetUniqueFileName(string ext = "tmp")
		{
			var tempDir = Application.GetTempPath();
			string path;
			do
			{
				var id = Guid.NewGuid();
				path = Path.Combine(tempDir, string.Format("{0}.{1}", id, ext));
			} while (File.Exists(path));
			return path;
		}

		/// <summary>
		/// Does everything possible to delete a file, including changing the file attributes.
		/// </summary>
		/// <param name="path"></param>
		public static void DeleteFile(string path)
		{
			// File existance checked later as an exception for performance reasons.
			//if (!File.Exists(path)) return;

			try
			{
				File.Delete(path);
			}
			catch (DirectoryNotFoundException ex)
			{
				// Want the file gone and it doesn't exist. It is OK to ignore this error and continue.
				Debug.WriteLine($"{ex.Message} File doesn't exist, so we should be good to ignore and continue.");
			}
			catch (UnauthorizedAccessException ex)
			{
				Debug.WriteLine($"UnauthorizedAccessException detected for file {path} ({ex.Message}). Changing file attributes and trying again.");
				var fi = new FileInfo(path);
				fi.Attributes = FileAttributes.Normal; // do what we can to delete the file.
				File.Delete(path);
			}
		}
	}
}
