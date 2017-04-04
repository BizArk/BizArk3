using System.IO;
using System.Text;

namespace BizArk.Core.Util
{

	/// <summary>
	/// Implements a System.IO.TextWriter for writing information to a string. The 
	/// information is stored in an underlying System.Text.StringBuilder.
	/// </summary>
	public class StringWriterEx
		: StringWriter
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of StringWriterEx.
		/// </summary>
		/// <param name="encoding"></param>
		public StringWriterEx(Encoding encoding)
		{
			mEncoding = encoding;
		}

		/// <summary>
		/// Creates an instance of StringWriterEx.
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="encoding"></param>
		public StringWriterEx(StringBuilder sb, Encoding encoding)
			: base(sb)
		{
			mEncoding = encoding;
		}

		#endregion

		#region Fields and Properties

		private Encoding mEncoding;
		/// <summary>
		/// Gets the System.Text.Encoding in which the output is written.
		/// </summary>
		public override Encoding Encoding
		{
			get { return mEncoding; }
		}

		#endregion

	}
}
