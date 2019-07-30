using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DERMSApp.ViewModels
{
	/// <summary>
	/// This class contains static references to all the view models in the
	/// application and provides an entry point for the bindings.
	/// </summary>
	public class ViewModelLocator
	{
		private static MainWindowViewModel _main;

		/// <summary>
		/// Initializes a new instance of the ViewModelLocator class.
		/// </summary>
		public ViewModelLocator()
		{
			try
			{
				_main = new MainWindowViewModel();
			}
			catch (Exception e)
			{
				Trace.Write("Stupid exception in constructor" + e.StackTrace);
				throw e;
			}
		  
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
