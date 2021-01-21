using System;
using SugorokuLibrary;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SugorokuClientApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlayPage
	{
		private Player _player;

		public PlayPage(Player player)
		{
			InitializeComponent();

			_player = player;
			FieldImage.Source = ImageSource.FromResource("SugorokuClientApp.ImageResource.sugorokuField.png");
		}

		private void FieldImageZoomButtonClicked(object sender, EventArgs e)
		{
		}
	}
}