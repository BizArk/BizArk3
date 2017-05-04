namespace BizArk.ConsoleApp.Logging
{

	/// <summary>
	/// Log level supported by BaCon.
	/// </summary>
	public enum BaConLogLevel
	{

		/// <summary>Most verbose logging level.</summary>
		Trace,
		/// <summary>Debug messages useful for identifying basically what is happening at a somewhat verbose level.</summary>
		Debug,
		/// <summary>Basic information. Good for important events that should be logged.</summary>
		Info,
		/// <summary>Just a warning to the user.</summary>
		Warn,
		/// <summary>An error has occurred, but the application is able to continue.</summary>
		Error,
		/// <summary>A fatal error has occurred and execution of the application is ending.</summary>
		Fatal

	}
}