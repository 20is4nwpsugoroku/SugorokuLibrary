using System.Collections.Generic;
using SugorokuLibrary;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SugorokuClientApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultPage : ContentPage
    {
        public ResultPage(IEnumerable<int> ranking, int myId, IEnumerable<Player> players)
        {
            InitializeComponent();
        }
    }
}