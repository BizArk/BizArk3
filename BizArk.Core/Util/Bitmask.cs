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
		/// <param name="max">The maximum number of bits that are allowed to be set. Cannot be larger than 64.</param>
		protected Bitmask(long value, int max = cMaxBits)
		{
			Value = value;

			if (max > cMaxBits)
				throw new ArgumentException($"The argument max cannot be larger than {cMaxBits}.", "max");
			MaxBits = max;
		}

		#endregion

		#region Fields and Properties

		private const short cMaxBits = sizeof(long) * 8;

		/// <summary>
		/// Gets the bitmask value.
		/// </summary>
		public long Value { get; private set; }

		/// <summary>
		/// Gets the number of bits that can be set. Cannot be larger than 64.
		/// </summary>
		public int MaxBits { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Returns the value of the specified bit in the bitmask.
		/// </summary>
		/// <param name="bitNum">A value between 1 and MaxBits that represents the position of the bit in the bitmask.</param>
		/// <returns>True if the bit is 1, false if the bit is 0.</returns>
		protected bool IsSet(int bitNum)
		{
			if (bitNum < 1 || bitNum > MaxBits) throw new ArgumentException($"bitNum must be between 1 and {MaxBits}, inclusive.", "bitNum");
			return (Value & (1L << bitNum - 1)) != 0;
		}

		/// <summary>
		/// Sets the specified bit in the bitmask.
		/// </summary>
		/// <param name="bitNum">A value between 1 and MaxBits that represents the position of the bit in the bitmask.</param>
		/// <param name="val">True to set the bit to 1, false to set the bit to 0.</param>
		/// <returns>The updated bitmask.</returns>
		protected void SetBit(int bitNum, bool val)
		{
			if (bitNum < 1L || bitNum > MaxBits) throw new ArgumentException($"bitNum must be between 1 and {MaxBits}, inclusive.", "bitNum");

			if (val)
				Value |= (1L << bitNum - 1);
			else
				Value &= ~(1L << bitNum - 1);
		}

		/// <summary>
		/// Gets a string that represents the bitmask. Intended for debugging purposes.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.Append('[');
			for (var i = MaxBits; i >= 1; i--)
			{
				if (((i - 1) % 4) == 3 && i != MaxBits) sb.Append("][");
				if (IsSet(i))
					sb.Append('1');
				else
					sb.Append('0');
			}
			sb.Append(']');

			return sb.ToString();
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			var bm = obj as Bitmask;
			if (bm != null)
				return Value.Equals(bm.Value);

			var lval = obj as long?;
			if (lval.HasValue)
				return Value.Equals(lval.Value);

			var ival = obj as int?;
			if (ival.HasValue)
				return Value.Equals(ival.Value);

			var sval = obj as short?;
			if (sval.HasValue)
				return Value.Equals(sval.Value);

			return base.Equals(obj);
		}

		/// <summary>
		/// Gets the hashcode for the object.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		#endregion

	}

}
