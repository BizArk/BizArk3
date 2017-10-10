using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizArk.Core.Extensions.AttributeExt;

namespace BizArk.ConsoleApp.Menu
{

	/// <summary>
	/// Base class for BaCon menu options.
	/// </summary>
	public abstract class BaConMenuOption
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates a new instance of BaConMenuOption.
		/// </summary>
		/// <param name="display"></param>
		public BaConMenuOption(string display)
		{
			Display = display;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets or sets the string to display to the user.
		/// </summary>
		public string Display { get; set; }

		/// <summary>
		/// Gets or sets the reason the option should be disabled in the menu. Setting this value will disable the menu option.
		/// </summary>
		public virtual string DisabledReason { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Override to provide functionality when the user selects this option.
		/// </summary>
		public abstract void OnSelected();

		#endregion

	}

	/// <summary>
	/// Implements BaCon menu options with an action handler that is executed if this option is selected.
	/// </summary>
	public class BaConActionMenuOption : BaConMenuOption
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates a new instance of BaConActionMenuOption.
		/// </summary>
		/// <param name="display"></param>
		/// <param name="onSelected"></param>
		public BaConActionMenuOption(string display, Action onSelected)
			: base(display)
		{
			OnSelectedHandler = onSelected;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets the handler used to execute the menu option.
		/// </summary>
		public Action OnSelectedHandler { get; private set; }

		#endregion

		#region Methods

		public override void OnSelected()
		{
			OnSelectedHandler();
		}

		#endregion

	}

	/// <summary>
	/// Implements BaCon menu options that takes no action.
	/// </summary>
	public class BaConNoActionMenuOption : BaConMenuOption
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates a new instance of BaConNoActionMenuOption.
		/// </summary>
		/// <param name="display"></param>
		/// <param name="onSelected"></param>
		public BaConNoActionMenuOption(string display)
			: base(display)
		{
		}

		#endregion

		#region Methods

		public override void OnSelected()
		{
			// Do nothing. Let external code handle this option.
		}

		#endregion

	}

	/// <summary>
	/// Implements BaCon menu options that takes no action, but provides an easy-to-use key to help identify the selected option.
	/// </summary>
	public class BaConKeyedMenuOption<T> : BaConNoActionMenuOption
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates a new instance of BaConNoActionMenuOption.
		/// </summary>
		/// <param name="display"></param>
		/// <param name="key">The key used to identify the menu option.</param>
		public BaConKeyedMenuOption(string display, T key)
			: base(display)
		{
			Key = key;
		}

		/// <summary>
		/// Creates a new instance of BaConNoActionMenuOption.
		/// </summary>
		/// <param name="key">The key used to identify the menu option. Gets the display value from the System.ComponentModel.DescriptionAttribute for the given key.</param>
		public BaConKeyedMenuOption(T key)
			: base(GetDisplay(key))
		{
		}

		private static string GetDisplay(T key)
		{
			var type = key.GetType();
			return type.GetDescription();
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets the key used to identify this menu option.
		/// </summary>
		public T Key { get; private set; }

		#endregion

	}

}
