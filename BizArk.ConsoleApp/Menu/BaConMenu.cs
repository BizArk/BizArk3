using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizArk.ConsoleApp.Menu
{

	/// <summary>
	/// Represents a list of options to determine what actions a user can take.
	/// </summary>
	public class BaConMenu
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates a new instance of BaConMenu.
		/// </summary>
		/// <param name="title">The title of the menu. Displayed before showing the menu.</param>
		public BaConMenu(string title)
		{
			Title = title;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets or sets the title of the menu.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets the list of menu options available to the user.
		/// </summary>
		public List<BaConMenuOption> Options { get; } = new List<BaConMenuOption>();

		#endregion

	}
}
