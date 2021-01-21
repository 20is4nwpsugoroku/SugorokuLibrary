using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SugorokuLibrary;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SugorokuClientApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WaitOtherPlayerPage : ContentPage
	{
		public WaitOtherPlayerPage(Player myInfo)
		{
			InitializeComponent();

			GameStartButton.IsEnabled = myInfo.IsHost;
		}
	}
}