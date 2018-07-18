using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AORViewer.ViewModels
{
	public class ViewModelLocator
	{
		private static MainWindowViewModel _main;

		/// <summary>
		/// Initializes a new instance of the ViewModelLocator class.
		/// </summary>
		public ViewModelLocator()
		{
			_main = new MainWindowViewModel();
		}

		/// <summary>
		/// Gets the Main property which defines the main viewmodel.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic",
			Justification = "This non-static member is needed for data binding purposes.")]
		public MainWindowViewModel Main
		{
			get
			{
				return _main;
			}
		}
	}
}
