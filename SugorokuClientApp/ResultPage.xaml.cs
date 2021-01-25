using System;
using System.Collections.Generic;
using System.Linq;
using SugorokuLibrary;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SugorokuClientApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultPage
    {
        public ResultPage(IEnumerable<int> ranking, IReadOnlyCollection<Player> players)
        {
            InitializeComponent();
            EndButton.Source = ImageSource.FromResource("SugorokuClientApp.ImageResource.endButton.png");

            var resultViewModels = new List<ResultPageViewModel>();

            foreach (var (rank, id) in ranking.Select((playerId, i) => (i, playerId)))
            {
                var player = players.First(p => p.PlayerID == id);
                var playerViewModel = new ResultPageViewModel
                {
                    PlayerName = player.PlayerName, RankText = $"{rank + 1}位",
                    ImageSource = ImageSource.FromResource($"SugorokuClientApp.ImageResource.koma_{id}.png")
                };
                resultViewModels.Add(playerViewModel);
            }

            ResultView.ItemsSource = resultViewModels;
        }

        private void GameFinishButtonClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () => await Navigation.PopToRootAsync(false));
        }
    }
}