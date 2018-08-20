using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DERMSApp.ViewModels;

namespace DERMSApp.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class LoginWindow : UserControl
	{
		public LoginWindow()
		{
			InitializeComponent();
		}

		private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (this.DataContext != null)
				((dynamic)this.DataContext).SecurePassword = ((PasswordBox)sender).SecurePassword;
		}

		//private void Button_Click(object sender, RoutedEventArgs e) // bolo
		//{
		//	LoginViewModel vm = this.DataContext as LoginViewModel;
		//	vm.ButtonLoginOnClickExecute();

		//	if (vm.IsUserAuthenticated == true)
		//	{
		//		Window main = this.Parent as Window;
		//		// umesto new setovati onaj tamo
		//		(main.DataContext as MainWindowViewModel).CurrentViewModel = new EntireNetworkViewModel();
		//	}

		//}
	}
}