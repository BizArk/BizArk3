using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BizArk.Core.Extensions.StringExt;

namespace BizArk.Core.Util
{

	/// <summary>
	/// Provides some convenient methods for handling security using industry best practices.
	/// </summary>
	public static class Security
	{

		/// <summary>
		/// Generates a cryptographically secure random salt. Uses RNGCryptoServiceProvider for security.
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static byte[] GenerateSalt(int length = 20)
		{
			using (var crypto = new RNGCryptoServiceProvider())
			{
				var salt = new byte[length];
				crypto.GetBytes(salt);
				return salt;
			}
		}

		/// <summary>
		/// Generates a cryptographically secure random string of characters. Uses RNGCryptoServiceProvider for security.
		/// </summary>
		/// <param name="length">The length of the token to generate.</param>
		/// <param name="chars">The list of valid characters to use for the token.</param>
		/// <returns></returns>
		public static string GenerateToken(int length = 20, string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890")
		{
			var data = GenerateSalt(length); // Salt is a cryptographically secure sequence of random bytes.
			var token = new StringBuilder(length);
			foreach (byte b in data)
				token.Append(chars[b % (chars.Length)]);
			return token.ToString();
		}

		/// <summary>
		/// Generates a hash based on SHA256.
		/// </summary>
		/// <param name="str">The value to hash.</param>
		/// <param name="salt">An optional salt.</param>
		/// <returns></returns>
		public static byte[] SHA256(string str, string salt = null)
		{
			if (str == null) throw new ArgumentNullException("str");

			using (var hasher = System.Security.Cryptography.SHA256.Create())
				return HashIt(hasher, str, salt);
		}

		/// <summary>
		/// Generates a hash based on SHA256.
		/// </summary>
		/// <param name="str">The value to hash.</param>
		/// <param name="salt">An optional salt.</param>
		/// <returns></returns>
		public static byte[] SHA256(string str, byte[] salt = null)
		{
			if (str == null) throw new ArgumentNullException("str");

			var value = str.ToBytes(Encoding.UTF32);
			using (var hasher = System.Security.Cryptography.SHA256.Create())
				return HashIt(hasher, value, salt);
		}

		/// <summary>
		/// Generates a hash based on SHA256.
		/// </summary>
		/// <param name="value">The value to hash.</param>
		/// <param name="salt">An optional salt.</param>
		/// <returns></returns>
		public static byte[] SHA256(byte[] value, byte[] salt = null)
		{
			if (value == null) throw new ArgumentNullException("value");

			using (var hasher = System.Security.Cryptography.SHA256.Create())
				return HashIt(hasher, value, salt);
		}

		/// <summary>
		/// Generates a hash based on SHA512.
		/// </summary>
		/// <param name="str">The value to hash.</param>
		/// <param name="salt">An optional salt.</param>
		/// <returns></returns>
		public static byte[] SHA512(string str, string salt = null)
		{
			if (str == null) throw new ArgumentNullException("str");

			using (var hasher = System.Security.Cryptography.SHA512.Create())
				return HashIt(hasher, str, salt);
		}

		/// <summary>
		/// Generates a hash based on SHA512.
		/// </summary>
		/// <param name="str">The value to hash.</param>
		/// <param name="salt">An optional salt.</param>
		/// <returns></returns>
		public static byte[] SHA512(string str, byte[] salt = null)
		{
			if (str == null) throw new ArgumentNullException("str");

			var value = str.ToBytes(Encoding.UTF32);
			using (var hasher = System.Security.Cryptography.SHA512.Create())
				return HashIt(hasher, value, salt);
		}

		/// <summary>
		/// Generates a hash based on SHA512.
		/// </summary>
		/// <param name="value">The value to hash.</param>
		/// <param name="salt">An optional salt.</param>
		/// <returns></returns>
		public static byte[] SHA512(byte[] value, byte[] salt = null)
		{
			if (value == null) throw new ArgumentNullException("value");

			using (var hasher = System.Security.Cryptography.SHA256.Create())
				return HashIt(hasher, value, salt);
		}

		private static byte[] HashIt(HashAlgorithm hasher, string str, string salt)
		{
			// Concatenate the string and salt, then convert it to a stream using UTF32.
			// By using this encoding every time, we can assure that we can get the 
			// correct hash every time, even if the default encoding changes.

			var value = (str + salt ?? "").ToBytes(Encoding.UTF32);
			return HashIt(hasher, value, null);
		}

		private static byte[] HashIt(HashAlgorithm hasher, byte[] value, byte[] salt)
		{
			var saltedValue = salt == null ? value : value.Concat(salt).ToArray();
			using (var s = new MemoryStream(saltedValue))
				return hasher.ComputeHash(s);
		}

	}
}
