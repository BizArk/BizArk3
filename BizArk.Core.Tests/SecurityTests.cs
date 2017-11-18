using BizArk.Core.Extensions.StringExt;
using BizArk.Core.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizArk.Core.Tests
{

	///</summary>
	[TestClass]
	public class SecurityTests
	{

		[TestMethod]
		public void Sha256Test()
		{
			var actual = "Hello".SHA256("World");
			Assert.AreEqual(32, actual.Length);
			var actualStr = System.Convert.ToBase64String(actual);
			Assert.AreEqual("Pdp9lVozbpWY4zdxzGYPZfF5kDFwzOjeTZLhPhcEvmE=", actualStr);

			actual = "Hello".SHA256("World!");
			actualStr = System.Convert.ToBase64String(actual);
			Assert.AreEqual("lPLVG49QpmiXJRYSevbFro8LFsMqoQxQCgwL99vibBg=", actualStr);
		}

		[TestMethod]
		public void Sha512Test()
		{
			var actual = "Hello".SHA512("World");
			Assert.AreEqual(64, actual.Length);
			var actualStr = System.Convert.ToBase64String(actual);
			Assert.AreEqual("m2XbiSqK9PtSzt7qq0qnBbg8DVfTq9mF9lHz+X8rMvDX3uPmOzUfX5TSsq8nCtW/z0BhS+t2Eq1E7HRhdgmH/g==", actualStr);

			actual = "Hello".SHA512("World!");
			actualStr = System.Convert.ToBase64String(actual);
			Assert.AreEqual("UkGsz9f5MRk3As0Fvvick4Uy1T+OuT0hQHhQCeFvQVBuNIYOu7AELL+MPXoW8585QK863S0G4UO9evLB4uFeyA==", actualStr);
		}

		[TestMethod]
		public void GenerateSaltTest()
		{
			var salt1 = Security.GenerateSalt(1);
			Assert.AreEqual(1, salt1.Length);

			var salt2 = Security.GenerateSalt(1);
			Assert.That.EnumerablesAreNotEqual(salt1, salt2); // This is likely to fail randomly, but rarely.

			var salt3 = Security.GenerateSalt(20);
			Assert.AreEqual(20, salt3.Length);
		}

		[TestMethod]
		public void GenerateTokenTest()
		{
			var token1 = Security.GenerateToken(10);
			Assert.AreEqual(1, token1.Length);

			var token2 = Security.GenerateToken(10);
			Assert.AreNotEqual(token1, token2); // This is likely to fail randomly, but rarely.

			var token3 = Security.GenerateToken(20);
			Assert.AreEqual(20, token3.Length);

			// Ensures that we are using the characters we pass in.
			var token4 = Security.GenerateToken(5, "a");
			Assert.AreEqual("aaaaa", token4);
		}

	}
}

