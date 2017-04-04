using System;
using System.Text;

namespace BizArk.Core.Util
{

	/// <summary>
	/// Provides a simple way to work with a bitmask.
	/// </summary>
	public abstract class Bitmask
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of Bitmask.
		/// </summary>
		/// <param name="value"></param>
		protected Bitmask(int value)
		{
			Value = value;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets the bitmask value.
		/// </summary>
		public int Value { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Returns the value of the specified bit in the bitmask.
		/// </summary>
		/// <param name="bitNum">A value between 1 and 32 that represents the position of the bit in the bitmask.</param>
		/// <returns>True if the bit is 1, false if the bit is 0.</returns>
		protected bool GetBit(int bitNum)
		{
			if (bitNum < 1 || bitNum > 32) throw new ArgumentException("bitNum must be between 1 and 32, inclusive.", "bitNum");
			return (Value & (1 << bitNum - 1)) != 0;
		}

		/// <summary>
		/// Sets the specified bit in the bitmask.
		/// </summary>
		/// <param name="bitNum">A value between 1 and 32 that represents the position of the bit in the bitmask.</param>
		/// <param name="val">True to set the bit to 1, false to set the bit to 0.</param>
		/// <returns>The updated bitmask.</returns>
		protected void SetBit(int bitNum, bool val)
		{
			if (bitNum < 1 || bitNum > 32) throw new ArgumentException("bitNum must be between 1 and 32, inclusive.", "bitNum");

			if (val)
				Value |= (1 << bitNum - 1);
			else
				Value &= ~(1 << bitNum - 1);
		}

		/// <summary>
		/// Gets a string that represents the bitmask. Intended for debugging purposes.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.Append('[');
			for (int i = 32; i >= 1; i--)
			{
				if (((i - 1) % 4) == 3 && i != 32) sb.Append("][");
				if (GetBit(i))
					sb.Append('1');
				else
					sb.Append('0');
			}
			sb.Append(']');

			return sb.ToString();
		}

		#endregion

	}

}
